using System;
using System.Collections.Generic;
using System.Linq;
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
    internal class QuestionsControllerTest
    {
        [Test]
        public async Task GetQuestionCount_Returns_200OK()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetQuestionCount();

            // Assert
            ObjectResult actual = result as ObjectResult;
            Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetQuestionCount_Returns_CountFromProvider()
        {
            // Arrange
            const int expected = 13;
            BasicMocks mocks = new BasicMocks();
            mocks.QuestionProvider.Setup(p => p.GetQuestionCount()).ReturnsAsync(expected);
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetQuestionCount();

            // Assert
            ObjectResult actual = result as ObjectResult;
            Assert.That(actual.Value, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetFixedQuestionSet_DefaultCount_Returns_200OK()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet();

            // Assert
            ObjectResult actual = result as ObjectResult;
            Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetFixedQuestionSet_Returns_200OK([Values(1, 2, 10, 15)] int questionCount)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet(questionCount);

            // Assert
            ObjectResult actual = result as ObjectResult;
            Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetFixedQuestionSet_DefaultCount_Returns_QuestionsFromProvider()
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IEnumerable<QuizQuestion> expected = CreateFakeQuestions(QuestionsController.DefaultCount, prefix:"expected");
            mocks.QuestionProvider.Setup(p => p.GetFixedQuestionList(QuestionsController.DefaultCount, It.IsAny<Shuffling>()))
                .ReturnsAsync(expected);
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet();

            // Assert
            IEnumerable<QuizQuestion> actual = GetResponseObject<IEnumerable<QuizQuestion>>(result);
            Assert.That(actual.Select(x => x.Id), Is.EqualTo(expected.Select(x => x.Id)));
        }

        [Test]
        public async Task GetFixedQuestionSet_Returns_QuestionsFromProvider([Values(1, 2, 10, 15)] int questionCount)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            IEnumerable<QuizQuestion> expected = CreateFakeQuestions(questionCount, prefix: "expected");
            mocks.QuestionProvider.Setup(p => p.GetFixedQuestionList(questionCount, It.IsAny<Shuffling>()))
                .ReturnsAsync(expected);
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet(questionCount);

            // Assert
            IEnumerable<QuizQuestion> actual = GetResponseObject<IEnumerable<QuizQuestion>>(result);
            Assert.That(actual.Select(x => x.Id), Is.EqualTo(expected.Select(x => x.Id)));
        }

        [Test]
        public async Task GetFixedQuestionSet_InvalidCount_Returns_400BadRequest([Values(-1, 0)] int questionCount)
        {
            // Arrange
            BasicMocks mocks = new BasicMocks();
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet(questionCount);

            // Assert
            StatusCodeResult actual = result as StatusCodeResult;
            Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task GetFixedQuestionSet_CountTooHigh_Returns_400BadRequest()
        {
            // Arrange
            const int availableQuestionCount = 10;
            BasicMocks mocks = new BasicMocks();
            mocks.QuestionProvider.Setup(p => p.GetFixedQuestionList(It.IsAny<int>(), It.IsAny<Shuffling>()))
                .ThrowsAsync(new ArgumentOutOfRangeException());
            mocks.QuestionProvider.Setup(p => p.GetQuestionCount())
                .ReturnsAsync(availableQuestionCount);
            QuestionsController underTest = CreateUnderTest(mocks);

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet(11);

            // Assert
            StatusCodeResult actual = result as StatusCodeResult;
            Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }


        private T GetResponseObject<T>(IActionResult result) where T : class
        {
            ObjectResult objectResult = result as ObjectResult;
            return objectResult?.Value as T;
        }

        private static QuestionsController CreateUnderTest(BasicMocks mocks)
        {
            return new QuestionsController(mocks.QuestionProvider.Object);
        }

        private static IEnumerable<QuizQuestion> CreateFakeQuestions(int count, string prefix = "id")
        {
            for (int i = 0; i < count; i++)
                yield return new QuizQuestion { Id = $"{prefix}-{i}" };
        }

        private class BasicMocks
        {
            public Mock<IQuestionProvider> QuestionProvider { get; }

            public BasicMocks()
            {
                QuestionProvider = new Mock<IQuestionProvider>();
                QuestionProvider.Setup(p => p.GetQuestionCount())
                    .ReturnsAsync(15);
                QuestionProvider.Setup(p => p.GetFixedQuestionList(It.IsAny<int>(), It.IsAny<Shuffling>()))
                    .ReturnsAsync((int count, Shuffling shuffling) => CreateFakeQuestions(count));
            }
        }
    }
}
