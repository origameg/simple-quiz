using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.DataClient.FileClient
{
    /// <summary>
    /// Used to convert between the <see cref="JsonQuestion"/> model used in the raw file format and the more generalized
    /// <see cref="Question"/> application model.
    /// </summary>
    internal interface IJsonQuestionConverter
    {
        /// <summary>
        /// Converts a deserialized <see cref="JsonQuestion"/> to a new <see cref="Question"/> instance, and
        /// returns the question along with the <see cref="AnswerOption.Id"/> of the correct answer.
        /// </summary>
        /// <param name="jsonQuestion">The object to convert.</param>
        /// <returns>A new <see cref="Question"/> instance.</returns>
        (Question question, string correctAnswerId) Convert(JsonQuestion jsonQuestion);
    }
}
