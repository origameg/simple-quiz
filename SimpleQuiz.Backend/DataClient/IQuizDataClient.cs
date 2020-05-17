using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<int> GetAvailableQuestionCountAsync();

        /// <summary>
        /// Gets a set of questions from the given data source. If <paramref name="randomSelection"/> is false,
        /// the same questions should be returned every time, in the same order.
        /// </summary>
        /// <param name="questionCount">The number of questions to return.</param>
        /// <param name="randomSelection">Indicates whether a varying selection of questions should be returned.</param>
        /// <returns>A selection of questions with their corresponding answer options.</returns>
        Task<IEnumerable<Question>> GetQuestionsAsync(int questionCount, bool randomSelection);

        /// <summary>
        /// Gets the correct answer for the given <paramref name="questionId"/>.
        /// </summary>
        /// <param name="questionId">The unique identifier of the question.</param>
        /// <returns>The correct answer.</returns>
        Task<string> GetCorrectAnswerIdAsync(string questionId);
    }
}
