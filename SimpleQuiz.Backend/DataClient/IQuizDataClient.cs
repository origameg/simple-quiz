using System.Collections.Generic;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.DataClient
{
    /// <summary>
    /// Represents a source for question and answer data. This may be a database, an external service,
    /// or (as in the current demo implementation) a simple file reader. 
    /// </summary>
    internal interface IQuizDataClient
    {
        /// <summary>
        /// Gets the maximum number of questions available from the given data source.
        /// </summary>
        /// <returns>The number of unique questions available.</returns>
        int GetAvailableQuestionCount();

        /// <summary>
        /// Gets a set of questions from the given data source.
        /// </summary>
        /// <param name="questionCount">The number of questions to return.</param>
        /// <param name="randomSelection">Indicates whether a varying selection of questions should be returned.</param>
        /// <returns>A selection of questions with their corresponding answer options.</returns>
        IEnumerable<QuizQuestion> GetQuestions(int questionCount, bool randomSelection);
    }
}
