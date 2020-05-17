using System.Threading.Tasks;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend
{
    /// <summary>
    /// Contains the business logic for calculating the quiz score from a set
    /// of responses.
    /// </summary>
    public interface IScoreCalculator
    {
        Task<Score> CalculateAsync(QuizResponse response);
    }
}
