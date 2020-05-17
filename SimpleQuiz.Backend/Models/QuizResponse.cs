using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleQuiz.Backend.Models
{
    /// <summary>
    /// Represents a set of submitted quiz answers.
    /// </summary>
    public class QuizResponse
    {
        /// <summary>
        /// A list of question responses.
        /// </summary>
        [JsonPropertyName("responses")]
        public IEnumerable<QuestionResponse> Responses { get; set; }

    }
}
