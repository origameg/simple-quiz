﻿using System;
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

        public async Task<int> GetQuestionCount()
        {
            return await _dataClient.GetAvailableQuestionCount();
        }

        public Task<IEnumerable<QuizQuestion>> GetFixedQuestionList(int questionCount, Shuffling shuffling)
        {
            return CreateQuestionList(questionCount, false, shuffling);
        }

        public Task<IEnumerable<QuizQuestion>> GetRandomQuestionList(int questionCount, Shuffling shuffling)
        {
            return CreateQuestionList(questionCount, true, shuffling);
        }

        private async Task<IEnumerable<QuizQuestion>> CreateQuestionList(int questionCount, bool randomSelection, Shuffling shuffling)
        {
            QuizQuestion[] questions = (await _dataClient.GetQuestions(questionCount, randomSelection)).ToArray();

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
