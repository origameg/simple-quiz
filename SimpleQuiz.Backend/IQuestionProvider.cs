using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleQuiz.Backend.DataClient;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend
{
    /// <summary>
    /// Handles the business logic for retrieving and structuring a set of questions.
    /// This may include reordering questions and/or answers, reformatting or translating text, etc. 
    /// </summary>
    public interface IQuestionProvider
    {
        /// <summary>
        /// Gets the maximum number of questions available.
        /// </summary>
        /// <returns>The number of unique questions available.</returns>
        Task<int> GetQuestionCount();

        /// <summary>
        /// Gets a list of quiz questions of the given size. This method returns that same set of questions
        /// across multiple calls. The order of questions and answers can be controlled by the <see cref="Shuffling"/>.
        /// </summary>
        /// <param name="questionCount">The number of questions to return.</param>
        /// <param name="shuffling">Indicates whether the questions and/or answers should be shuffled.</param>
        /// <returns></returns>
        Task<IEnumerable<QuizQuestion>> GetFixedQuestionList(int questionCount, Shuffling shuffling);
    }

    /// <summary>
    /// Indicates what, if any, parts of a question list should be shuffled
    /// </summary>
    [Flags]
    public enum Shuffling
    {
        /// <summary>
        /// Questions and answers are returned in the order provided from the <see cref="IQuizDataClient"/>
        /// </summary>
        None = 0,

        /// <summary>
        /// The order of the questions will be shuffled
        /// </summary>
        Questions = 1,

        /// <summary>
        /// The order of the answers for each question will be shuffled
        /// </summary>
        Answers = 2
    }
}
