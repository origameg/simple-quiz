using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleQuiz.Backend.DataClient;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend
{
    internal class QuestionProvider : IQuestionProvider
    {
        private readonly IQuizDataClient _dataClient;
        private readonly Random _random;

        public QuestionProvider(IQuizDataClient dataClient, int randomSeed) : this(dataClient)
        {
            _random = new Random(randomSeed);
        }

        public QuestionProvider(IQuizDataClient dataClient)
        {
            _dataClient = dataClient;
            _random = new Random();
        }

        public async Task<int> GetQuestionCountAsync()
        {
            return await _dataClient.GetAvailableQuestionCountAsync();
        }

        public Task<IEnumerable<Question>> GetFixedQuestionListAsync(int questionCount, Shuffling shuffling)
        {
            return CreateQuestionListAsync(questionCount, false, shuffling);
        }

        public Task<IEnumerable<Question>> GetRandomQuestionListAsync(int questionCount, Shuffling shuffling)
        {
            return CreateQuestionListAsync(questionCount, true, shuffling);
        }

        private async Task<IEnumerable<Question>> CreateQuestionListAsync(int questionCount, bool randomSelection, Shuffling shuffling)
        {
            Question[] questions = (await _dataClient.GetQuestionsAsync(questionCount, randomSelection)).ToArray();

            if ((shuffling & Shuffling.Questions) != 0)
                questions = Shuffle(questions);

            if ((shuffling & Shuffling.Answers) != 0)
                foreach (Question question in questions)
                    question.Answers = Shuffle(question.Answers.ToArray());

            return questions;
        }

        private T[] Shuffle<T>(T[] items) where T : class
        {
            // Fisher-Yates shuffle
            for (int n = items.Length - 1; n > 0; --n)
            {
                int k = _random.Next(n + 1);
                T temp = items[n];
                items[n] = items[k];
                items[k] = temp;
            }
            return items;
        }
    }
}
