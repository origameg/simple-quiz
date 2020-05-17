using System.Text.Json.Serialization;

namespace SimpleQuiz.Backend.Models
{
    /// <summary>
    /// Represents the answer submitted for a single question
    /// </summary>
    public class QuestionResponse
    {
        /// <summary>
        /// The id of the question being answered.
        /// </summary>
        [JsonPropertyName("question")]

        public string QuestionId { get; set; }

        /// <summary>
        /// The id of the selected answer.
        /// </summary>
        [JsonPropertyName("answer")]
        public string AnswerId { get; set; }
    }
}
