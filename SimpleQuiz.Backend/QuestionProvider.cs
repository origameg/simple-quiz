using System;
using System.Collections.Generic;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend
{
    internal class QuestionProvider : IQuestionProvider
    {
        public IList<QuizQuestion> GetFixedQuestionList(int questionCount, bool fixedOrder)
        {
            IList<QuizQuestion> questions = new List<QuizQuestion>();
            for (int i = 0; i < questionCount; i++)
            {
                questions.Add(new QuizQuestion { Id = Guid.NewGuid().ToString() });
            }

            return questions;
        }
    }
}
