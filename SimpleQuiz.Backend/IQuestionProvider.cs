using System.Collections.Generic;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend
{
    /// <summary>
    /// Handles the business logic for retrieving and structuring a set of questions.
    /// This may include reordering questions and/or answers, reformatting or translating text, etc. 
    /// </summary>
    internal interface IQuestionProvider
    {
        /// <summary>
        /// Gets a list of quiz questions of the given size. This method returns that same set of questions
        /// across multiple calls. Although the questions are the same, they only appear in the same order when
        /// <paramref name="fixedOrder"/> is true.
        /// </summary>
        /// <param name="questionCount">The number of questions to return.</param>
        /// <param name="fixedOrder">Indicates whether the questions should appear in a repeatable order.</param>
        /// <returns></returns>
        IList<QuizQuestion> GetFixedQuestionList(int questionCount, bool fixedOrder);
    }
}
