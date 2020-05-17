using System;
using System.Collections.Generic;
using SimpleQuiz.Backend.Controllers.Models;

namespace SimpleQuiz.Backend
{
    internal class QuestionProvider : IQuestionProvider
    {
        public IList<QuizQuestion> GetFixedQuestionList(int count, bool fixedOrder)
        {
            throw new NotImplementedException();
        }
    }
}
