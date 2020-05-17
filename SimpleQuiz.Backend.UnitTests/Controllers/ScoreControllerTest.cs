using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SimpleQuiz.Backend.Controllers;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.UnitTests.Controllers
{
    [TestFixture]
    internal class ScoreControllerTest
    {
        [Test]
        public async Task CalculateScoreAsync_Returns_200OK()
        {
            // Arrange
            QuizResponse response = CreateResponse();
            BasicMocks mocks = new BasicMocks();
            ScoreController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult actual = await underTest.CalculateScoreAsync(response);

            // Assert
            ObjectResult result = actual as ObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task CalculateScoreAsync_Returns_ScoreFromCalculator()
        {
            // Arrange
            QuizResponse response = CreateResponse();
            Score expected = new Score();

            BasicMocks mocks = new BasicMocks();
            mocks.ScoreCalculator.Setup(c => c.CalculateAsync(response))
                .ReturnsAsync(expected);

            ScoreController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult actual = await underTest.CalculateScoreAsync(response);

            // Assert
            ObjectResult result = actual as ObjectResult;
            Assert.That(result.Value, Is.EqualTo(expected));
        }

        [Test]
        public async Task CalculateScoreAsync_Null_Returns_400BadRequest()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            ScoreController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult actual = await underTest.CalculateScoreAsync(null);

            // Assert
            StatusCodeResult result = actual as StatusCodeResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task CalculateScoreAsync_NullResponse_Returns_400BadRequest()
        {
            // Arrange
            QuizResponse response = new QuizResponse();
            BasicMocks mocks = new BasicMocks();
            ScoreController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult actual = await underTest.CalculateScoreAsync(response);

            // Assert
            StatusCodeResult result = actual as StatusCodeResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task CalculateScoreAsync_EmptyResponse_Returns_400BadRequest()
        {
            // Arrange
            QuizResponse response = new QuizResponse { Responses = new QuestionResponse[0] };
            BasicMocks mocks = new BasicMocks();
            ScoreController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult actual = await underTest.CalculateScoreAsync(response);

            // Assert
            StatusCodeResult result = actual as StatusCodeResult;
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
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
            return new QuizResponse {Responses = questionResponses};
        }

        private static ScoreController CreateUnderTest(BasicMocks mocks)
        {
            return new ScoreController(mocks.ScoreCalculator.Object);
        }

        private class BasicMocks
        {
            public Mock<IScoreCalculator> ScoreCalculator { get; }

            public BasicMocks()
            {
                ScoreCalculator = new Mock<IScoreCalculator>();
                ScoreCalculator.Setup(p => p.CalculateAsync(It.IsAny<QuizResponse>()))
                    .ReturnsAsync(new Score());
            }
        }
    }
}
