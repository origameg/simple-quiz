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
    internal class ScoreCalculatorTest
    {
        [Test]
        public async Task CalculateAsync_Calls_DataClient_ForEachQuestion()
        {
            // Arrange
            QuizResponse quizResponse = CreateResponse();

            BasicMocks mocks = new BasicMocks();
            IScoreCalculator underTest = CreateUnderTest(mocks);

            // Act
            await underTest.CalculateAsync(quizResponse);

            // Assert
            foreach (QuestionResponse questionResponse in quizResponse.Responses)
                mocks.QuizDataClient.Verify(c => c.GetCorrectAnswerIdAsync(questionResponse.QuestionId));
        }

        [Test]
        public async Task CalculateAsync_AllCorrect_Returns_CorrectScore(
            [Values(1, 2, 15)] int questionCount)
        {
            // Arrange
            QuizResponse quizResponse = CreateResponse(questionCount);
            BasicMocks mocks = new BasicMocks();
            foreach (QuestionResponse questionResponse in quizResponse.Responses)
            {
                mocks.QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(questionResponse.QuestionId))
                    .ReturnsAsync(questionResponse.AnswerId);
            }
            IScoreCalculator underTest = CreateUnderTest(mocks);

            // Act
            Score actual = await underTest.CalculateAsync(quizResponse);

            // Assert
            Assert.That(actual.TotalCount, Is.EqualTo(questionCount));
            Assert.That(actual.CorrectCount, Is.EqualTo(questionCount));
            Assert.That(actual.PercentageCorrect, Is.EqualTo(1.0).Within(1e-5));
        }

        [Test]
        public async Task CalculateAsync_AllIncorrect_Returns_CorrectScore(
            [Values(1, 2, 15)] int questionCount)
        {
            // Arrange
            QuizResponse quizResponse = CreateResponse(questionCount);
            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(It.IsAny<string>()))
                .ReturnsAsync("Different-Answer-Id");
            IScoreCalculator underTest = CreateUnderTest(mocks);

            // Act
            Score actual = await underTest.CalculateAsync(quizResponse);

            // Assert
            Assert.That(actual.TotalCount, Is.EqualTo(questionCount));
            Assert.That(actual.CorrectCount, Is.EqualTo(0));
            Assert.That(actual.PercentageCorrect, Is.EqualTo(0.0).Within(1e-5));
        }

        [TestCase(5, 10)]
        [TestCase(9, 10)]
        [TestCase(5, 15)]
        public async Task CalculateAsync_SomeIncorrect_Returns_CorrectScore(int correct, int total)
        {
            // Arrange
            QuizResponse quizResponse = CreateResponse(total);
            BasicMocks mocks = new BasicMocks();
            mocks.QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(It.IsAny<string>()))
                .ReturnsAsync("Different-Answer-Id");
            foreach (QuestionResponse questionResponse in quizResponse.Responses.Take(correct))
            {
                mocks.QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(questionResponse.QuestionId))
                    .ReturnsAsync(questionResponse.AnswerId);
            }
            IScoreCalculator underTest = CreateUnderTest(mocks);

            // Act
            Score actual = await underTest.CalculateAsync(quizResponse);

            // Assert
            Assert.That(actual.TotalCount, Is.EqualTo(total));
            Assert.That(actual.CorrectCount, Is.EqualTo(correct));
            Assert.That(actual.PercentageCorrect, Is.EqualTo((double) correct / total).Within(1e-5));
        }

        [TestCase(1, 0, 1)]
        [TestCase(2, 0, 1)]
        [TestCase(5, 1, 3)]
        [TestCase(5, 2, 3)]
        public async Task CalculateAsync_CountsUnrecognizedQuestionsAsIncorrect(int total, int incorrect, int unrecognized)
        {
            // Arrange
            QuizResponse quizResponse = CreateResponse(total);
            BasicMocks mocks = new BasicMocks();

            QuestionResponse[] responses = quizResponse.Responses.ToArray();
            for (int i = 0; i < incorrect; i++)
            {
                mocks.QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(responses[i].QuestionId))
                    .ReturnsAsync("different-answer-id");
            }
            for (int i = 0; i < unrecognized; i++)
            {
                // Even if the answer is set to null, it should still be incorrect
                responses[incorrect + i].AnswerId = null;
                mocks.QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(responses[incorrect + i].QuestionId))
                    .ReturnsAsync((string)null);
            }
            for (int i = incorrect + unrecognized; i < total; i++)
            {
                mocks.QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(responses[i].QuestionId))
                    .ReturnsAsync(responses[i].AnswerId);
            }
            quizResponse.Responses = responses;

            IScoreCalculator underTest = CreateUnderTest(mocks);

            // Act
            Score actual = await underTest.CalculateAsync(quizResponse);

            // Assert
            int correct = total - incorrect - unrecognized;
            Assert.That(actual.TotalCount, Is.EqualTo(total));
            Assert.That(actual.CorrectCount, Is.EqualTo(correct));
            Assert.That(actual.PercentageCorrect, Is.EqualTo((double)correct / total).Within(1e-5));
        }

        private static Dictionary<string, string> CreateAnswerKey(int questionCount)
        {
            Dictionary<string, string> answerKey = new Dictionary<string, string>();
            for (int i = 0; i < questionCount; i++)
            {
                string questionId = $"question-{i}";
                string answerId = $"answer-{i}";
                answerKey.Add(questionId, answerId);
            };
            return answerKey;
        }

        private static QuizResponse CreateResponse(int questionCount = 10)
        {
            QuestionResponse[] questionResponses = new QuestionResponse[questionCount];
            for (int i = 0; i < questionCount; i++)
                questionResponses[i] = new QuestionResponse
                {
                    QuestionId = $"question-{i}",
                    AnswerId = $"answer-{i}"
                };
            return new QuizResponse { Responses = questionResponses };
        }

        private static IScoreCalculator CreateUnderTest(BasicMocks mocks)
        {
            return new ScoreCalculator(mocks.QuizDataClient.Object);
        }
        
        private class BasicMocks
        {
            public Mock<IQuizDataClient> QuizDataClient { get; }

            public BasicMocks()
            {
                QuizDataClient = new Mock<IQuizDataClient>();
                QuizDataClient.Setup(c => c.GetCorrectAnswerIdAsync(It.IsAny<string>()))
                    .ReturnsAsync("fake-id");
            }
        }
    }
}
