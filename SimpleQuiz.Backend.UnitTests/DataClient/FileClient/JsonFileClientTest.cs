using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;
using System.Threading.Tasks;
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
            // These questions are only used to establish the number of questions.
            // The actual values are overridden by the QuestionConverter mock
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
        public async Task GetAvailableQuestionCountAsync_Returns_CorrectNumber()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            int actual = await underTest.GetAvailableQuestionCountAsync();

            // Assert
            Assert.That(actual, Is.EqualTo(AvailableQuestionCount));
        }

        [Theory]
        public void GetQuestionsAsync_NotEnoughQuestions_Throws_Exception(bool randomSelection)
        {
            // Arrange
            int tooHighCount = AvailableQuestionCount + 1;

            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                underTest.GetQuestionsAsync(tooHighCount, randomSelection));
        }

        [Test]
        public async Task GetQuestionsAsync_Returns_CorrectNumberOfQuestions(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetQuestionsAsync(questionCount, randomSelection);

            // Assert
            Assert.That(actual.Count, Is.EqualTo(questionCount));
        }

        [Test]
        public async Task GetQuestionsAsync_Returns_UniqueQuestions(
            [Values(2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetQuestionsAsync(questionCount, randomSelection);

            // Assert
            Assert.That(actual.Select(x => x.QuestionText), Is.Unique);
        }

        [Test]
        public async Task GetQuestionsAsync_Returns_UniqueQuestionIds(
            [Values(2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetQuestionsAsync(questionCount, randomSelection);

            // Assert
            Assert.That(actual.Select(x => x.Id), Is.Unique);
        }

        [Test]
        public async Task GetQuestionsAsync_Returns_UniqueAnswerIds_PerQuestion(
            [Values(2, 10, 15)] int questionCount,
            [Values(true, false)] bool randomSelection)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetQuestionsAsync(questionCount, randomSelection);

            // Assert
            foreach (Question question in actual)
                Assert.That(question.Answers.Select(x => x.Id), Is.Unique);
        }

        [Test]
        public async Task GetQuestionsAsync_RandomSelection_False_Returns_SameQuestions(
            [Values(1, 2, 10)] int questionCount)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual1 = await underTest.GetQuestionsAsync(questionCount, false);
            IEnumerable<Question> actual2 = await underTest.GetQuestionsAsync(questionCount, false);

            // Assert
            Assert.That(actual1.Select(x => x.Id), Is.EquivalentTo(actual2.Select(x => x.Id)));
        }

        [Test]
        public async Task GetQuestionsAsync_RandomSelection_True_Returns_DifferentQuestions(
            [Values(1, 2, 10)] int questionCount)
        {
            // Arrange
            const int seed = 11235;
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = new JsonFileClient(mocks.QuestionConverter.Object, mocks.Configuration.Object, seed);

            // Act
            IEnumerable<Question> actual1 = await underTest.GetQuestionsAsync(questionCount, true);
            IEnumerable<Question> actual2 = await underTest.GetQuestionsAsync(questionCount, true);

            // Assert
            Assert.That(actual1.Select(x => x.Id), Is.Not.EquivalentTo(actual2.Select(x => x.Id)));
        }

        [Test]
        public async Task GetCorrectAnswerAsync_Returns_ExpectedResult()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            List<(string questionId, string answerId)> expected = new List<(string, string)>();
            int nextId = 0;
            mocks.QuestionConverter.Setup(x => x.Convert(It.IsAny<JsonQuestion>()))
                .Returns(() =>
                {
                    string questionId = $"QuestionId-{nextId}";
                    string answerId = $"AnswerId-{nextId}";
                    expected.Add((questionId, answerId));

                    Question question = new Question {Id = questionId};
                    return (question, answerId);
                })
                .Callback(() => nextId++);
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act & Assert
            foreach ((string questionId, string answerId) in expected)
            {
                string actual = await underTest.GetCorrectAnswerIdAsync(questionId);
                Assert.That(actual, Is.EqualTo(answerId));
            }
        }

        [Test]
        public async Task GetCorrectAnswerAsync_UnknownQuestion_Returns_Null()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IQuizDataClient underTest = CreateUnderTest(mocks);

            // Act
            string actual = await underTest.GetCorrectAnswerIdAsync("unknown-question-id");

            // Assert
            Assert.That(actual, Is.Null);
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
                        Question question = CreateFakeQuestion(nextId);
                        return (question, question.Answers.First().Id);
                    })
                    .Callback(() => nextId++);


                Question CreateFakeQuestion(int i)
                {
                    List<AnswerOption> answers = new List<AnswerOption>();
                    for (int x = 0; x < AnswersPerQuestion; x++)
                        answers.Add(CreateAnswer(i, x));

                    return new Question
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
