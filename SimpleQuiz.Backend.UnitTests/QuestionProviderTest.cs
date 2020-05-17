using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task GetQuestionCountAsync_Returns_ValueFromDataClient()
        {
            // Arrange
            int expected = 13;
            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetAvailableQuestionCountAsync())
                .ReturnsAsync(expected);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            int actual = await underTest.GetQuestionCountAsync();

            // Assert
            mocks.QuizDataClient.Verify(c => c.GetAvailableQuestionCountAsync());
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetFixedQuestionListAsync_Returns_QuestionsFromDataClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [ValueSource(nameof(AllShufflingValues))] Shuffling shuffling)
        {
            // Arrange
            IEnumerable<Question> expected = CreateFakeQuestions(questionCount, prefix:"expected");
            IEnumerable<Question> fromClient = CreateFakeQuestions(questionCount, prefix: "expected");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), false))
                .ReturnsAsync(fromClient);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetFixedQuestionListAsync(questionCount, shuffling);

            // Assert
            Assert.That(actual.Select(x => x.Id), Is.EquivalentTo(expected.Select(x => x.Id)));
        }

        [Test]
        public async Task GetFixedQuestionListAsync_NotShufflingQuestions_Returns_QuestionsInOrderFromClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.None, Shuffling.Answers)] Shuffling shuffling)
        {
            // Arrange
            IEnumerable<Question> expected = CreateFakeQuestions(questionCount, prefix: "expected");
            IEnumerable<Question> fromClient = CreateFakeQuestions(questionCount, prefix: "expected");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), false))
                .ReturnsAsync(fromClient);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetFixedQuestionListAsync(questionCount, shuffling);

            // Assert
            Assert.That(actual.Select(x => x.Id), Is.EqualTo(expected.Select(x => x.Id)));
        }

        [Test]
        public async Task GetFixedQuestionListAsync_ShufflingQuestions_Returns_QuestionsInVaryingOrder(
            [Values(5, 10, 15)] int questionCount,
            [Values(Shuffling.Questions, Shuffling.Questions | Shuffling.Answers)] Shuffling shuffling)
        {
            // Arrange
            const int seed = 11235;
            BasicMocks mocks = new BasicMocks();
            IQuestionProvider underTest = CreateUnderTest(mocks, seed);

            // Act
            IEnumerable<Question> actual1 = await underTest.GetFixedQuestionListAsync(questionCount, shuffling);
            IEnumerable<Question> actual2 = await underTest.GetFixedQuestionListAsync(questionCount, shuffling);

            // Assert
            Assert.That(actual1.Select(x => x.Id), Is.Not.EqualTo(actual2.Select(x => x.Id)));
            Assert.That(actual1.Select(x => x.Id), Is.EquivalentTo(actual2.Select(x => x.Id)));
        }

        [Test]
        public async Task GetFixedQuestionListAsync_NotShufflingAnswers_Returns_AnswersInOrderFromClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.None, Shuffling.Questions)] Shuffling shuffling)
        {
            // Arrange
            IEnumerable<Question> expected = CreateFakeQuestions(questionCount, prefix: "expected");
            IEnumerable<Question> fromClient = CreateFakeQuestions(questionCount, prefix: "expected");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), false))
                .ReturnsAsync(fromClient);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetFixedQuestionListAsync(questionCount, shuffling);

            // Assert
            foreach (Question expectedQuestion in expected)
            {
                Question actualQuestion = actual.First(x => x.Id == expectedQuestion.Id);
                Assert.That(actualQuestion.Answers.Select(x => x.Id), Is.EqualTo(expectedQuestion.Answers.Select(x => x.Id)));
            }
        }

        [Test]
        public async Task GetFixedQuestionListAsync_ShufflingAnswers_Returns_AnswersInVaryingOrder(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.Answers, Shuffling.Questions | Shuffling.Answers)] Shuffling shuffling)
        {
            // Arrange
            const int seed = 11235;

            BasicMocks mocks = new BasicMocks();
            // Increase the number of answers to make it statistically unlikely to return the same order for the same question
            mocks.QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), false))
                .ReturnsAsync(() => CreateFakeQuestions(questionCount, answerCount: 10));
            IQuestionProvider underTest = CreateUnderTest(mocks, seed);

            // Act
            IEnumerable<Question> actual1 = await underTest.GetFixedQuestionListAsync(questionCount, shuffling);
            IEnumerable<Question> actual2 = await underTest.GetFixedQuestionListAsync(questionCount, shuffling);

            // Assert
            foreach (Question expectedQuestion in actual1)
            {
                Question actualQuestion = actual2.First(x => x.Id == expectedQuestion.Id);
                Assert.That(actualQuestion.Answers.Select(x => x.Id), Is.Not.EqualTo(expectedQuestion.Answers.Select(x => x.Id)));
                Assert.That(actualQuestion.Answers.Select(x => x.Id), Is.EquivalentTo(expectedQuestion.Answers.Select(x => x.Id)));
            }
        }

        [Test]
        public async Task GetRandomQuestionListAsync_Returns_QuestionsFromDataClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [ValueSource(nameof(AllShufflingValues))] Shuffling shuffling)
        {
            // Arrange
            IEnumerable<Question> expected = CreateFakeQuestions(questionCount, prefix: "random");
            IEnumerable<Question> fromClient = CreateFakeQuestions(questionCount, prefix: "random");
            
            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), true))
                .ReturnsAsync(fromClient);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetRandomQuestionListAsync(questionCount, shuffling);

            // Assert
            Assert.That(actual.Select(x => x.Id), Is.EquivalentTo(expected.Select(x => x.Id)));
        }

        [Test]
        public async Task GetRandomQuestionListAsync_NotShufflingAnswers_Returns_AnswersInOrderFromClient(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.None, Shuffling.Questions)] Shuffling shuffling)
        {
            // Arrange
            IEnumerable<Question> expected = CreateFakeQuestions(questionCount, prefix: "random");
            IEnumerable<Question> fromClient = CreateFakeQuestions(questionCount, prefix: "random");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), true))
                .ReturnsAsync(fromClient);
            IQuestionProvider underTest = CreateUnderTest(mocks);

            // Act
            IEnumerable<Question> actual = await underTest.GetRandomQuestionListAsync(questionCount, shuffling);

            // Assert
            foreach (Question expectedQuestion in expected)
            {
                Question actualQuestion = actual.First(x => x.Id == expectedQuestion.Id);
                Assert.That(actualQuestion.Answers.Select(x => x.Id), Is.EqualTo(expectedQuestion.Answers.Select(x => x.Id)));
            }
        }

        [Test]
        public async Task GetRandomQuestionListAsync_ShufflingAnswers_Returns_AnswersInVaryingOrder(
            [Values(1, 2, 10, 15)] int questionCount,
            [Values(Shuffling.Answers, Shuffling.Questions | Shuffling.Answers)] Shuffling shuffling)
        {
            // Arrange
            const int seed = 11235;
            // Increase the number of answers to make it statistically unlikely to return the same order for the same question
            IEnumerable<Question> expected = CreateFakeQuestions(questionCount, answerCount: 10, prefix: "random");
            IEnumerable<Question> fromClient = CreateFakeQuestions(questionCount, answerCount: 10, prefix: "random");

            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), true))
                .ReturnsAsync(fromClient);
            IQuestionProvider underTest = CreateUnderTest(mocks, seed);

            // Act
            IEnumerable<Question> actual = await underTest.GetRandomQuestionListAsync(questionCount, shuffling);

            // Assert
            foreach (Question expectedQuestion in expected)
            {
                Question actualQuestion = actual.First(x => x.Id == expectedQuestion.Id);
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

        private static IEnumerable<Question> CreateFakeQuestions(int count, int answerCount=4, string prefix="id")
        {
            Question[] questions = new Question[count];
            for (int i = 0; i < count; i++)
            {
                questions[i] = new Question
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
                QuizDataClient.Setup(c => c.GetQuestionsAsync(It.IsAny<int>(), It.IsAny<bool>()))
                    .ReturnsAsync((int count, bool random) => CreateFakeQuestions(count));
            }
        }
    }
}
