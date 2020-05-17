using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SimpleQuiz.Backend.Models;

namespace SimpleQuiz.Backend.UnitTests
{
    [TestFixture]
    internal class QuestionProviderTest
    {
        [Test]
        public void GetFixedQuestionList_Returns_CorrectNumberOfQuestions(
            [ValueSource(nameof(QuestionCountCases))] int questionCount, 
            [Values(true, false)] bool fixedOrder)
        {
            // Arrange
            IQuestionProvider underTest = new QuestionProvider();

            // Act
            IList<QuizQuestion> actual = underTest.GetFixedQuestionList(questionCount, fixedOrder);

            // Assert
            Assert.That(actual.Count, Is.EqualTo(questionCount));
        }

        [Test]
        public void GetFixedQuestionList_Returns_NoDuplicates(
            [ValueSource(nameof(QuestionCountCases))] int questionCount,
            [Values(true, false)] bool fixedOrder)
        {
            // Arrange
            IQuestionProvider underTest = new QuestionProvider();

            // Act
            IList<QuizQuestion> actual = underTest.GetFixedQuestionList(questionCount, fixedOrder);

            // Assert
            string[] allIds = actual.Select(x => x.Id).ToArray();
            Assert.That(allIds, Has.All.Not.Null.And.Not.Empty);
            Assert.That(allIds, Is.Unique);
        }

        [Test]
        public void GetFixedQuestionList_FixedOrderTrue_Returns_QuestionsInSameOrder()
        {
            // Arrange
            const int questionCount = 5;
            IQuestionProvider underTest = new QuestionProvider();

            // Act
            IList<QuizQuestion> actual1 = underTest.GetFixedQuestionList(questionCount, true);
            IList<QuizQuestion> actual2 = underTest.GetFixedQuestionList(questionCount, true);

            // Assert
            Assert.That(actual1.Select(x => x.Id), Is.EqualTo(actual2.Select(x => x.Id)));
        }


        internal static int[] QuestionCountCases = { 1, 2, 5, 10, 15 };
    }
}
