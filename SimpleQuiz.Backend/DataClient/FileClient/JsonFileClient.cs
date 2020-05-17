using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.DataClient.FileClient
{
    /// <summary>
    /// A very simple file-based data source that parses a JSON file.
    /// </summary>
    internal class JsonFileClient : IQuizDataClient
    {
        internal const string ConfigFileKey = "SampleQuestionFile";

        private Random _random;
        private List<Question> _questions;
        private Dictionary<string, string> _answers;

        internal JsonFileClient(IJsonQuestionConverter questionConverter, IConfiguration configuration, int randomSeed)
            : this(questionConverter, configuration)
        {
            _random = new Random(randomSeed);
        }

        public JsonFileClient(IJsonQuestionConverter questionConverter, IConfiguration configuration)
        {
            string filePath = configuration[ConfigFileKey];
            IEnumerable<JsonQuestion> jsonQuestions = ReadFileContents(filePath);

            _random = new Random();

            _questions = new List<Question>();
            _answers = new Dictionary<string, string>();
            foreach (JsonQuestion jsonQuestion in jsonQuestions)
            {
                (Question question, string correctAnswerId) convertedQuestion = questionConverter.Convert(jsonQuestion);
                _questions.Add(convertedQuestion.question);
                _answers.Add(convertedQuestion.question.Id, convertedQuestion.correctAnswerId);
            }
        }

        public Task<int> GetAvailableQuestionCountAsync()
        {
            return Task.FromResult(_questions.Count);
        }

        public Task<IEnumerable<Question>> GetQuestionsAsync(int questionCount, bool randomSelection)
        {
            if (questionCount > _questions.Count)
                throw new ArgumentOutOfRangeException(nameof(questionCount), "Not enough questions available");

            if (!randomSelection)
                return Task.FromResult(_questions.Take(questionCount));

            List<Question> randomQuestions = new List<Question>(questionCount);
            List<int> remainingQuestions = Enumerable.Range(0, _questions.Count).ToList();
            for (int i = 0; i < questionCount; i++)
            {
                int index = _random.Next(remainingQuestions.Count);
                randomQuestions.Add(_questions[remainingQuestions[index]]);
                remainingQuestions.RemoveAt(index);
            }
            return Task.FromResult((IEnumerable<Question>)randomQuestions);
        }

        private static IEnumerable<JsonQuestion> ReadFileContents(string filePath)
        {
            string content = System.IO.File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<JsonQuestion[]>(content);
        }
    }
}
