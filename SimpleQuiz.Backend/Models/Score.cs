using System.Text.Json.Serialization;

namespace SimpleQuiz.Backend.Models
{
    /// <summary>
    /// Response object containing quiz score.
    /// </summary>
    public class Score
    {
        /// <summary>
        /// The number of correct answers
        /// </summary>
        [JsonPropertyName("correct")]
        public int CorrectCount { get; set; }

        /// <summary>
        /// The total number of questions
        /// </summary>
        [JsonPropertyName("total")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Percentage of correct answers
        /// </summary>
        [JsonPropertyName("percent")]
        public double PercentageCorrect { get; set; }
    }
}
