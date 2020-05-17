using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleQuiz.Backend.Controllers.Models
{
    /// <summary>
    /// Represents a single question to be presented in the quiz.
    /// </summary>
    public class QuizQuestion
    {
        /// <summary>
        /// A unique identifier for the question. This should be used when processing the user's response, as it
        /// remains unmodified regardless of any variations in the order or display of the quiz questions.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The text to display for the question.
        /// </summary>
        [JsonPropertyName("question")]
        public string Question { get; set; }

        /// <summary>
        /// The possible answers choices to present to the user.
        /// </summary>
        [JsonPropertyName("answers")]
        public IEnumerable<AnswerOption> Answers { get; set; }
    }
}
