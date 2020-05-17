using System.Text.Json.Serialization;

namespace SimpleQuiz.Backend.Models
{
    /// <summary>
    /// Represents a single answer option for a multiple-choice quiz question.
    /// </summary>
    /// <remarks>
    /// This may be extended in future to include other media types besides plain text (e.g. image, etc).
    /// </remarks>
    public class AnswerOption
    {
        /// <summary>
        /// A unique identifier for a single answer choice. This identifier should be used to identify
        /// the selected answer when processing the user's response, as it remains unmodified
        /// regardless of any variations in the order or display of the quiz questions.
        /// </summary>
        /// <remarks>
        /// This is guaranteed to be unique among options for a single question, but not necessarily unique
        /// among all possible answers in a quiz.
        /// </remarks>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The text to display for the answer.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
