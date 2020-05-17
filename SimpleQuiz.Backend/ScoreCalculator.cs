using System;
using System.Threading.Tasks;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend
{
    internal class ScoreCalculator : IScoreCalculator
    {
        public Task<Score> CalculateAsync(QuizResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
