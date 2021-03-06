﻿using System;
using System.Collections.Generic;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.DataClient.FileClient
{
    internal class JsonQuestionConverter : IJsonQuestionConverter
    {
        public (Question question, string correctAnswerId) Convert(JsonQuestion jsonQuestion)
        {
            // TODO: Improve error handling (mismatched correct answer, missing properties, etc)

            Question convertedQuestion = new Question();
            string correctAnswerId = null;

            convertedQuestion.QuestionText = jsonQuestion.Question;
            convertedQuestion.Id = Guid.NewGuid().ToString();
            
            List<AnswerOption> convertedAnswers = new List<AnswerOption>();
            foreach (string answerText in jsonQuestion.PossibleAnswers)
            {
                string answerId = Guid.NewGuid().ToString();
                convertedAnswers.Add(new AnswerOption
                {
                    Text = answerText,
                    Id = answerId
                });
                if (answerText == jsonQuestion.CorrectAnswer)
                    correctAnswerId = answerId;
            }
            convertedQuestion.Answers = convertedAnswers;

            return (convertedQuestion, correctAnswerId);
        }
    }
}
