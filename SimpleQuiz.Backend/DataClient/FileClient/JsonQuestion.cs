using System.Text.Json.Serialization;

namespace SimpleQuiz.Backend.DataClient.FileClient
{
    /// <summary>
    /// Simple JSON representation of a question
    /// </summary>
    internal class JsonQuestion
    {
        [JsonPropertyName("question")]
        public string Question { get; set; }

        [JsonPropertyName("possibleAnswers")]
        public string[] PossibleAnswers { get; set; }

        [JsonPropertyName("correctAnswer")]
        public string CorrectAnswer { get; set; }
    }
}
