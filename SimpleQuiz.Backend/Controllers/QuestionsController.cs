using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.Controllers
{
    /// <summary>
    /// Provides access to the questions and answers to construct and display the quiz.
    /// </summary>
    [Route("api/v1/questions/")]
    public class QuestionsController : Controller
    {
        internal const int DefaultCount = 10;

        private readonly IQuestionProvider _questionProvider;

        public QuestionsController(IQuestionProvider questionProvider)
        {
            _questionProvider = questionProvider;
        }

        /// <summary>
        /// Get the number of questions available for generating a quiz.
        /// </summary>
        /// <returns>The number of available questions.</returns>
        /// <response code="200">Success</response>
        /// <response code="500">Internal error</response>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        [HttpGet("count")]
        public async Task<IActionResult> GetQuestionCount()
        {
            int questionCount = await _questionProvider.GetQuestionCount();
            return new OkObjectResult(questionCount);
        }

        /// <summary>
        /// Get a set of fixed questions for a quiz. This endpoint will always return the same set of questions.
        /// </summary>
        /// <param name="count">
        /// The number of questions to include. If this parameter is omitted, a default number of questions (10)
        /// will be provided.
        /// </param>
        /// <returns>A list of questions with their possible answer options.</returns>
        /// <response code="200">Success</response>
        /// <response code="200">Bad Request (e.g. count is too high)</response>
        /// <response code="500">Internal error</response>
        [ProducesResponseType(200, Type = typeof(IEnumerable<QuizQuestion>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        [HttpGet("fixed")]
        public async Task<IActionResult> GetFixedQuestionSet(int count = DefaultCount)
        {
            if (count <= 0)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            try
            {
                IEnumerable<QuizQuestion> questions = await _questionProvider.GetFixedQuestionList(count, Shuffling.Questions);
                return new OkObjectResult(questions);
            }
            catch (ArgumentOutOfRangeException)
            {
                int availableQuestionCount = await _questionProvider.GetQuestionCount();
                if (count > availableQuestionCount)
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                throw;
            }
        }
    }
}