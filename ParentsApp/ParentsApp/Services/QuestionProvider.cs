namespace ParentsApp.Services
{
    public class QuestionProvider
    {
        private readonly List<string> _questions;

        public QuestionProvider(string filePath)
        {
            _questions = File.ReadAllLines(filePath).ToList();
        }

        public string GetRandomQuestion()
        {
            var random = new Random();
            return _questions[random.Next(_questions.Count)];
        }
    }
}
