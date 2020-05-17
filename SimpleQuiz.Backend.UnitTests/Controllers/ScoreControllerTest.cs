using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SimpleQuiz.Backend.Controllers;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.UnitTests.Controllers
{
    [TestFixture]
    internal class ScoreControllerTest
    {
        private static ScoreController CreateUnderTest(BasicMocks mocks)
        {
            return new ScoreController(mocks.ScoreCalculator.Object);
        }

        private static IEnumerable<Question> CreateFakeQuestions(int count, string prefix = "id")
        {
            for (int i = 0; i < count; i++)
                yield return new Question { Id = $"{prefix}-{i}" };
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
