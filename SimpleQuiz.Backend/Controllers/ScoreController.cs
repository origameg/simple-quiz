using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.Controllers
{
    /// <summary>
    /// Calculates the score for a completed set of quiz questions.
    /// </summary>
    [Route("api/v1/scoring/")]
    public class ScoreController : Controller
    {
        private readonly IScoreCalculator _scoreCalculator;

        public ScoreController(IScoreCalculator scoreCalculator)
        {
            _scoreCalculator = scoreCalculator;
        }

        /// <summary>
        /// Get the number of questions available for generating a quiz.
        /// </summary>
        /// <returns>The number of available questions.</returns>
        /// <response code="200">Success</response>
        /// <response code="500">Internal error</response>
        [ProducesResponseType(200, Type = typeof(Score))]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> CalculateScoreAsync([FromBody] QuizResponse quizResponse)
        {
            return await Task.FromResult(new OkObjectResult(new Score()));
        }
    }
}