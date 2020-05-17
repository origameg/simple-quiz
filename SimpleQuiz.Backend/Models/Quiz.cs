using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleQuiz.Backend.Models
{
    /// <summary>
    /// A set of questions and possible answers.
    /// </summary>
    public class Quiz
    {
        /// <summary>
        /// A list of questions with answer choices.
        /// </summary>
        [JsonPropertyName("questions")]
        public IEnumerable<Question> Questions { get; set; }
    }
}
