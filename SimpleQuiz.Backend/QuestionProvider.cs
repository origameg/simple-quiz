using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<QuizQuestion> GetFixedQuestionList(int questionCount, Shuffling shuffling)
        {
            QuizQuestion[] questions = _dataClient.GetQuestions(questionCount, false).ToArray();

            if ((shuffling & Shuffling.Questions) != 0)
                questions = Shuffle(questions);

            if ((shuffling & Shuffling.Answers) != 0)
                foreach (QuizQuestion question in questions)
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
