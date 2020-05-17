using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SimpleQuiz.Backend.DataClient;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.UnitTests
{
    [TestFixture]
    internal class QuestionProviderTest
    {
        [Test]
        public void GetFixedQuestionList_Returns_QuestionsFromDataClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [ValueSource(nameof(AllShufflingValues))] Shuffling shuffling)
        {
            // Arrange
            QuizQuestion[] expected = CreateFakeQuestions(questionCount, prefix:"expected");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestions(It.IsAny<int>(), false))
                .Returns(expected);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual = underTest.GetFixedQuestionList(questionCount, shuffling);

            // Assert
            Assert.That(actual.Select(x => x.Id), Is.EquivalentTo(expected.Select(x => x.Id)));
        }

        [Test]
        public void GetFixedQuestionList_NotShufflingQuestions_Returns_QuestionsInOrderFromClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.None, Shuffling.Answers)] Shuffling shuffling)
        {
            // Arrange
            QuizQuestion[] expected = CreateFakeQuestions(questionCount, prefix: "expected");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestions(It.IsAny<int>(), false))
                .Returns(expected);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual = underTest.GetFixedQuestionList(questionCount, shuffling);

            // Assert
            Assert.That(actual.Select(x => x.Id), Is.EqualTo(expected.Select(x => x.Id)));
        }

        [Test]
        public void GetFixedQuestionList_ShufflingQuestions_Returns_QuestionsInVaryingOrder(
            [Values(5, 10, 15)] int questionCount,
            [Values(Shuffling.Questions, Shuffling.Questions | Shuffling.Answers)] Shuffling shuffling)
        {
            // Arrange
            const int seed = 11235;
            BasicMocks mocks = new BasicMocks();
            IQuestionProvider underTest = CreateUnderTest(mocks, seed);

            // Act
            IEnumerable<QuizQuestion> actual1 = underTest.GetFixedQuestionList(questionCount, shuffling);
            IEnumerable<QuizQuestion> actual2 = underTest.GetFixedQuestionList(questionCount, shuffling);

            // Assert
            Assert.That(actual1.Select(x => x.Id), Is.Not.EqualTo(actual2.Select(x => x.Id)));
            Assert.That(actual1.Select(x => x.Id), Is.EquivalentTo(actual2.Select(x => x.Id)));
        }

        [Test]
        public void GetFixedQuestionList_NotShufflingAnswers_Returns_AnswersInOrderFromClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.None, Shuffling.Questions)] Shuffling shuffling)
        {
            // Arrange
            QuizQuestion[] expected = CreateFakeQuestions(questionCount, prefix: "expected");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestions(It.IsAny<int>(), false))
                .Returns(expected);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<QuizQuestion> actual = underTest.GetFixedQuestionList(questionCount, shuffling);

            // Assert
            foreach (QuizQuestion expectedQuestion in expected)
            {
                QuizQuestion actualQuestion = actual.First(x => x.Id == expectedQuestion.Id);
                Assert.That(actualQuestion.Answers.Select(x => x.Id), Is.EqualTo(expectedQuestion.Answers.Select(x => x.Id)));
            }
        }

        [Test]
        public void GetFixedQuestionList_ShufflingAnswers_Returns_AnswersInVaryingOrder(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.Answers, Shuffling.Questions | Shuffling.Answers)] Shuffling shuffling)
        {
            // Arrange
            const int seed = 11235;

            BasicMocks mocks = new BasicMocks();
            // Increase the number of answers to make it statistically unlikely to return the same order for the same question
            mocks.QuizDataClient.Setup(c => c.GetQuestions(It.IsAny<int>(), false))
                .Returns(() => CreateFakeQuestions(questionCount, answerCount: 10));
            IQuestionProvider underTest = CreateUnderTest(mocks, seed);

            // Act
            IEnumerable<QuizQuestion> actual1 = underTest.GetFixedQuestionList(questionCount, shuffling);
            IEnumerable<QuizQuestion> actual2 = underTest.GetFixedQuestionList(questionCount, shuffling);

            // Assert
            foreach (QuizQuestion expectedQuestion in actual1)
            {
                QuizQuestion actualQuestion = actual2.First(x => x.Id == expectedQuestion.Id);
                Assert.That(actualQuestion.Answers.Select(x => x.Id), Is.Not.EqualTo(expectedQuestion.Answers.Select(x => x.Id)));
                Assert.That(actualQuestion.Answers.Select(x => x.Id), Is.EquivalentTo(expectedQuestion.Answers.Select(x => x.Id)));
            }
        }

        private static IEnumerable<Shuffling> AllShufflingValues()
        {
            yield return Shuffling.None;
            yield return Shuffling.Questions;
            yield return Shuffling.Answers;
            yield return Shuffling.Questions | Shuffling.Answers;
        }

        private static QuestionProvider CreateUnderTest(BasicMocks mocks)
        {
            return new QuestionProvider(mocks.QuizDataClient.Object);
        }

        private static QuestionProvider CreateUnderTest(BasicMocks mocks, int seed)
        {
            return new QuestionProvider(mocks.QuizDataClient.Object, seed);
        }

        private static QuizQuestion[] CreateFakeQuestions(int count, int answerCount=4, string prefix="id")
        {
            QuizQuestion[] questions = new QuizQuestion[count];
            for (int i = 0; i < count; i++)
            {
                questions[i] = new QuizQuestion
                {
                    Id = $"{prefix}-{i}",
                    QuestionText = $"Question {i}?",
                    Answers = CreateFakeAnswers(i)
                };
            }
            return questions;

            AnswerOption[] CreateFakeAnswers(int questionIndex)
            {
                AnswerOption[] answers = new AnswerOption[answerCount];
                for (int i = 0; i < answerCount; i++)
                    answers[i] = new AnswerOption
                    {
                        Id = $"{prefix}-{questionIndex}.{i}",
                        Text = $"Answer {questionIndex}.{i}"
                    };
                return answers;
            }
        }

        private class BasicMocks
        {
            public Mock<IQuizDataClient> QuizDataClient { get; }

            public BasicMocks()
            {
                QuizDataClient = new Mock<IQuizDataClient>();
                QuizDataClient.Setup(c => c.GetQuestions(It.IsAny<int>(), It.IsAny<bool>()))
                    .Returns((int count, bool random) => CreateFakeQuestions(count));
            }
        }
    }
}
