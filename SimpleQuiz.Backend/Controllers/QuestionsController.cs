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
        public async Task<IActionResult> GetQuestionCountAsync()
        {
            int questionCount = await _questionProvider.GetQuestionCountAsync();
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
        /// <response code="200">Bad Request</response>
        /// <response code="500">Internal error</response>
        [ProducesResponseType(200, Type = typeof(Quiz))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        [HttpGet("fixed")]
        public async Task<IActionResult> GetFixedQuestionSetAsync(int count = DefaultCount)
        {
            return await GetQuestionSetAsync(count, false);
        }

        /// <summary>
        /// Get a set of random questions for a quiz. This endpoint will return a varying set of questions on
        /// subsequent calls.
        /// </summary>
        /// <param name="count">
        /// The number of questions to include. If this parameter is omitted, a default number of questions (10)
        /// will be provided.
        /// </param>
        /// <returns>A list of questions with their possible answer options.</returns>
        /// <response code="200">Success</response>
        /// <response code="200">Bad Request</response>
        /// <response code="500">Internal error</response>
        [ProducesResponseType(200, Type = typeof(Quiz))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomQuestionSetAsync(int count = DefaultCount)
        {
            return await GetQuestionSetAsync(count, true);
        }

        private async Task<IActionResult> GetQuestionSetAsync(int count, bool randomSelection)
        {
            if (count <= 0)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);

            try
            {
                IEnumerable<Question> questions = randomSelection ?
                        await _questionProvider.GetRandomQuestionListAsync(count, Shuffling.Questions)
                        : await _questionProvider.GetFixedQuestionListAsync(count, Shuffling.Questions);

                Quiz quiz = new Quiz { Questions = questions };
                return new OkObjectResult(quiz);
            }
            catch (ArgumentOutOfRangeException)
            {
                int availableQuestionCount = await _questionProvider.GetQuestionCountAsync();
                if (count > availableQuestionCount)
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                throw;
            }
        }
    }
}