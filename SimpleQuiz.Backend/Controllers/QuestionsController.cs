using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleQuiz.Backend.Controllers.Models;

namespace SimpleQuiz.Backend.Controllers
{
    /// <summary>
    /// Provides access to the questions and answers to construct and display the quiz.
    /// </summary>
    [Route("api/v1/questions/")]
    public class QuestionsController : Controller
    {
        internal const int DefaultCount = 10;

        /// <summary>
        /// Get a set of fixed set questions for a quiz.
        /// </summary>
        /// <param name="count">
        /// The number of questions to include. If this parameter is omitted, a default number of questions (10)
        /// will be provided.
        /// </param>
        /// <returns>A list of questions with their possible answer options.</returns>
        /// <response code="200">Success</response>
        /// <response code="500">Internal error</response>
        /// <response code="404">Source not found</response>
        [ProducesResponseType(200, Type = typeof(IEnumerable<QuizQuestion>))]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        [HttpGet]
        public async Task<IActionResult> GetFixedQuestionSet(int count = DefaultCount)
        {
            IList<QuizQuestion> questions = new List<QuizQuestion>();
            OkObjectResult result = new OkObjectResult(questions);
            for (int i = 0; i < count; i++)
            {
                questions.Add(new QuizQuestion());
            }
            return await Task.FromResult(result);
        }
    }
}