using System.Collections.Generic;

namespace App1.Modulo
{
    public class QuestionData
    {
        public string Title { get; }
        public string Body { get; }
        public List<Answer> Answers { get; }

        public QuestionData(string title, string body)
        {
            Title = title;
            Body = body;
            Answers = new List<Answer>();
        }

        public void AddAnswer(string text, bool isCorrect)
        {
            Answers.Add(new Answer(text, isCorrect));
        }
    }
}
