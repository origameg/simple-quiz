using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;
using NUnit.Framework;
using SimpleQuiz.Backend.DataClient;
using SimpleQuiz.Backend.DataClient.FileClient;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.UnitTests.DataClient.FileClient
{
    [TestFixture]
    internal class JsonFileClientTest
    {
        private const int AvailableQuestionCount = 15;
        private const int AnswersPerQuestion = 4;
        private const string FakeQuestionFile = "FakeQuestionFile.json";

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            JsonQuestion[] fakeQuestions = new JsonQuestion[AvailableQuestionCount];
            for (int i = 0; i < AvailableQuestionCount; i++)
                fakeQuestions[i] = new JsonQuestion();
            string json =JsonSerializer.Serialize(fakeQuestions);
            File.WriteAllText(FakeQuestionFile, json);
        }

        [Test]
        public void Initialization_Gets_FilePath_FromConfig()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();

            // Act
            CreateUnderTest(mocks);

            // Assert
            mocks.Configuration.Verify(c => c[JsonFileClient.ConfigFileKey]);
        }

        [Test]
        public void Initialization_Converts_Questions()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();

            // Act
            CreateUnderTest(mocks);

            // Assert
            mocks.QuestionConverter.Verify(c => c.Convert(It.IsAny<JsonQuestion>()), Times.Exactly(AvailableQuestionCount));
        }

        [Test]
        public void GetAvailableQuestionCount_Returns_CorrectNumber()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            int actual = underTest.GetAvailableQuestionCount();

            // Assert
            Assert.That(actual, Is.EqualTo(AvailableQuestionCount));
        }

        [Theory]
        public void GetQuestions_NotEnoughQuestions_Throws_Exception(bool randomSelection)
        {
            // Arrange
            int tooHighCount = AvailableQuestionCount + 1;

            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                underTest.GetQuestions(tooHighCount, randomSelection));
        }

        [Test]
        public void GetQuestions_Returns_CorrectNumberOfQuestions(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual = underTest.GetQuestions(questionCount, randomSelection);

            // Assert
            Assert.That(actual.Count, Is.EqualTo(questionCount));
        }

        [Test]
        public void GetQuestions_Returns_UniqueQuestions(
            [Values(2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual = underTest.GetQuestions(questionCount, randomSelection);

            // Assert
            Assert.That(actual.Select(x => x.QuestionText), Is.Unique);
        }

        [Test]
        public void GetQuestions_Returns_UniqueQuestionIds(
            [Values(2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual = underTest.GetQuestions(questionCount, randomSelection);

            // Assert
            Assert.That(actual.Select(x => x.Id), Is.Unique);
        }

        [Test]
        public void GetQuestions_Returns_UniqueAnswerIds_PerQuestion(
            [Values(2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual = underTest.GetQuestions(questionCount, randomSelection);

            // Assert
            foreach (QuizQuestion question in actual)
                Assert.That(question.Answers.Select(x => x.Id), Is.Unique);
        }

        [Test]
        public void GetQuestions_RandomSelection_False_Returns_SameQuestions(
            [Values(1, 2, 10)] int questionCount)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual1 = underTest.GetQuestions(questionCount, false);
            IEnumerable<QuizQuestion> actual2 = underTest.GetQuestions(questionCount, false);

            // Assert
            Assert.That(actual1.Select(x => x.Id), Is.EquivalentTo(actual2.Select(x => x.Id)));
        }

        [Test]
        public void GetQuestions_RandomSelection_True_Returns_DifferentQuestions(
            [Values(1, 2, 10)] int questionCount)
        {
            // Arrange
            const int seed = 11235;
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = new JsonFileClient(mocks.QuestionConverter.Object, mocks.Configuration.Object, seed);

            // Act
            IEnumerable<QuizQuestion> actual1 = underTest.GetQuestions(questionCount, true);
            IEnumerable<QuizQuestion> actual2 = underTest.GetQuestions(questionCount, true);

            // Assert
            Assert.That(actual1.Select(x => x.Id), Is.Not.EquivalentTo(actual2.Select(x => x.Id)));
        }

        private static JsonFileClient CreateUnderTest(BasicMocks mocks)
        {
            return new JsonFileClient(mocks.QuestionConverter.Object, mocks.Configuration.Object);
        }

        private class BasicMocks
        {
            public Mock<IJsonQuestionConverter> QuestionConverter { get; }
            public Mock<IConfiguration> Configuration { get; }

            public BasicMocks()
            {
                Configuration = new Mock<IConfiguration>();
                Configuration.Setup(x => x[JsonFileClient.ConfigFileKey]).Returns(FakeQuestionFile);

                QuestionConverter = new Mock<IJsonQuestionConverter>();
                int nextId = 0;
                QuestionConverter.Setup(x => x.Convert(It.IsAny<JsonQuestion>()))
                    .Returns(() =>
                    {
                        QuizQuestion question = CreateFakeQuestion(nextId);
                        return (question, question.Answers.First().Id);
                    })
                    .Callback(() => nextId++);


                QuizQuestion CreateFakeQuestion(int i)
                {
                    List<AnswerOption> answers = new List<AnswerOption>();
                    for (int x = 0; x < AnswersPerQuestion; x++)
                        answers.Add(CreateAnswer(i, x));

                    return new QuizQuestion
                    {
                        Id = $"QuestionId-{i}",
                        QuestionText = $"Fake question {i}",
                        Answers = answers
                    };
                }

                AnswerOption CreateAnswer(int i, int j)
                {
                    return new AnswerOption
                    {
                        Id = $"AnswerId-{i}{j}",
                        Text = $"Fake answer {i}-{j}"
                    };
                }
            }
        }
    }
}
