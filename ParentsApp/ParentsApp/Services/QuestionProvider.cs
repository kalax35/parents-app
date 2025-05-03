namespace ParentsApp.Services
{
    public class QuestionProvider
    {
        private static readonly List<string> Questions = new()
    {
        "Ulubione hobby?",
        "Jak spędzasz weekendy?",
        "Twoje ulubione jedzenie?",
        "Co lubisz robić z dziećmi?",
        "Jakie miejsce chciałbyś odwiedzić?"
    };

        public static string GetRandomQuestion()
        {
            var rand = new Random();
            return Questions[rand.Next(Questions.Count)];
        }
    }
}
