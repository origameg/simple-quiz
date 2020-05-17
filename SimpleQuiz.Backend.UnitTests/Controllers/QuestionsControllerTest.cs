using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SimpleQuiz.Backend.Controllers;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.UnitTests.Controllers
{
    [TestFixture]
    internal class QuestionsControllerTest
    {
        [Test]
        public async Task GetFixedQuestionSet_DefaultCount_Returns_200OK()
        {
            // Arrange
            QuestionsController underTest = new QuestionsController();

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet();

            // Assert
            ObjectResult actual = result as ObjectResult;
            Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [TestCaseSource(nameof(QuestionCountCases))]
        public async Task GetFixedQuestionSet_Returns_200OK(int questionCount)
        {
            // Arrange
            QuestionsController underTest = new QuestionsController();

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet(questionCount);

            // Assert
            ObjectResult actual = result as ObjectResult;
            Assert.That(actual.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        }

        [Test]
        public async Task GetFixedQuestionSet_DefaultCount_Returns_CorrectNumberOfQuizQuestions()
        {
            // Arrange
            QuestionsController underTest = new QuestionsController();

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet();

            // Assert
            IEnumerable<QuizQuestion> actual = GetResponseObject<IEnumerable<QuizQuestion>>(result);
            Assert.That(actual, Has.Exactly(QuestionsController.DefaultCount).Items);
        }

        [TestCaseSource(nameof(QuestionCountCases))]
        public async Task GetFixedQuestionSet_Returns_CorrectNumberOfQuizQuestions(int questionCount)
        {
            // Arrange
            QuestionsController underTest = new QuestionsController();

            // Act
            IActionResult result = await underTest.GetFixedQuestionSet(questionCount);

            // Assert
            IEnumerable<QuizQuestion> actual = GetResponseObject<IEnumerable<QuizQuestion>>(result);
            Assert.That(actual, Has.Exactly(questionCount).Items);
        }


        internal static int[] QuestionCountCases = { 1, 2, 5, 15 };

        private T GetResponseObject<T>(IActionResult result) where T : class
        {
            ObjectResult objectResult = result as ObjectResult;
            return objectResult?.Value as T;
        }
    }
}
