using System.Linq;
using NUnit.Framework;
using SimpleQuiz.Backend.DataClient.FileClient;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.UnitTests.DataClient.FileClient
{
    [TestFixture]
    internal class JsonQuestionConverterTest
    {
        [Test]
        public void Convert_Sets_QuestionText()
        {
            // Arrange
            const string expected = "Fake expected question";
            JsonQuestion jsonQuestion = CreateFakeQuestion();
            jsonQuestion.Question = expected;

            IJsonQuestionConverter underTest = new JsonQuestionConverter();

            // Act
            (QuizQuestion question, string correctAnswerId) actual = underTest.Convert(jsonQuestion);

            // Assert
            Assert.That(actual.question.QuestionText, Is.EqualTo(expected));
        }

        [Test]
        public void Convert_Sets_QuestionId()
        {
            // Arrange
            const string expected = "Fake expected question";
            JsonQuestion jsonQuestion = CreateFakeQuestion();
            jsonQuestion.Question = expected;

            IJsonQuestionConverter underTest = new JsonQuestionConverter();

            // Act
            (QuizQuestion question, string correctAnswerId) actual = underTest.Convert(jsonQuestion);

            // Assert
            Assert.That(actual.question.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Convert_Sets_AnswerTexts()
        {
            // Arrange
            string[] expected = { "Fake expected answer A", "Fake expected answer B" };
            JsonQuestion jsonQuestion = CreateFakeQuestion();
            jsonQuestion.PossibleAnswers = expected;

            IJsonQuestionConverter underTest = new JsonQuestionConverter();

            // Act
            (QuizQuestion question, string correctAnswerId) actual = underTest.Convert(jsonQuestion);

            // Assert
            Assert.That(actual.question.Answers.Select(x => x.Text), Is.EquivalentTo(expected));
        }

        [Test]
        public void Convert_Sets_AnswerIds()
        {
            // Arrange
            JsonQuestion jsonQuestion = CreateFakeQuestion();

            IJsonQuestionConverter underTest = new JsonQuestionConverter();

            // Act
            (QuizQuestion question, string correctAnswerId) actual = underTest.Convert(jsonQuestion);

            // Assert
            Assert.That(actual.question.Answers.Select(x => x.Id), Is.All.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Convert_Sets_UniqueAnswerIds()
        {
            // Arrange
            JsonQuestion jsonQuestion = CreateFakeQuestion();

            IJsonQuestionConverter underTest = new JsonQuestionConverter();

            // Act
            (QuizQuestion question, string correctAnswerId) actual = underTest.Convert(jsonQuestion);

            // Assert
            Assert.That(actual.question.Answers.Select(x => x.Id), Is.Unique);
        }

        [Test]
        public void Convert_Returns_CorrectAnswerId([Range(0,3)] int correctAnswerPosition)
        {
            // Arrange
            const string correctAnswer = "Expected correct answer";
            string[] possibleAnswers = new string[4];
            for (int i = 0; i < possibleAnswers.Length; i++)
                possibleAnswers[i] = i == correctAnswerPosition ? correctAnswer : $"Fake incorrect answer {i}";

            JsonQuestion jsonQuestion = CreateFakeQuestion();
            jsonQuestion.PossibleAnswers = possibleAnswers;
            jsonQuestion.CorrectAnswer = correctAnswer;

            IJsonQuestionConverter underTest = new JsonQuestionConverter();

            // Act
            (QuizQuestion question, string correctAnswerId) actual = underTest.Convert(jsonQuestion);

            // Assert
            string expected = actual.question.Answers.First(x => x.Text == correctAnswer).Id;
            Assert.That(actual.correctAnswerId, Is.EqualTo(expected));
        }


        private static JsonQuestion CreateFakeQuestion()
        {
            return new JsonQuestion
            {
                Question = "Default fake question",
                PossibleAnswers = new[]
                {
                    "Default fake answer 1",
                    "Default fake answer 2",
                    "Default fake answer 3",
                    "Default fake answer 4"
                },
                CorrectAnswer = "Default fake answer 1"
            };
        }
    }
}
