using System;
using System.Threading.Tasks;
using SimpleQuiz.Backend.DataClient;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend
{
    internal class ScoreCalculator : IScoreCalculator
    {
        private readonly IQuizDataClient _dataClient;

        public ScoreCalculator(IQuizDataClient dataClient)
        {
            _dataClient = dataClient;
        }

        public async Task<Score> CalculateAsync(QuizResponse response)
        {
            int totalQuestions = 0;
            int correctQuestions = 0;

            foreach (QuestionResponse questionResponse in response.Responses)
            {
                totalQuestions++;

                AnswerOption correctAnswer = await _dataClient.GetCorrectAnswer(questionResponse.QuestionId);
                if (correctAnswer.Id == questionResponse.AnswerId)
                    correctQuestions++;
            }

            Score score = new Score
            {
                CorrectCount = correctQuestions, 
                TotalCount = totalQuestions,
                PercentageCorrect = (double) correctQuestions / totalQuestions
            };
            return score;
        }
    }
}
