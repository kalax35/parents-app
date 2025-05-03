using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ParentsApp.Models;
using ParentsApp.Services;
using System.Text;

namespace ParentsApp.Controllers
{
    public class ParentsController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _filePath;
        private readonly QuestionProvider _questionProvider;

        public ParentsController(IWebHostEnvironment env, IOptions<AppSettings> options,
            QuestionProvider questionProvider)
        {
            _env = env;
            _filePath = options.Value.ParentFilePath;
            _questionProvider = questionProvider;

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        }

        public IActionResult Form()
        {
            string question = _questionProvider.GetRandomQuestion();

            ViewBag.Question = question;
            return View(new Parent { Question = question });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Parent parent)
        {
            if (!ModelState.IsValid)
                return BadRequest("Formularz zawiera błędy.");

            string name = parent.Name.Trim().ToLowerInvariant();
            string surname = parent.Surname.Trim().ToLowerInvariant();

            if (System.IO.File.Exists(_filePath))
            {
                var lines = await System.IO.File.ReadAllLinesAsync(_filePath);

                bool exists = lines.Any(line =>
                {
                    var parts = line.Split(';');
                    return parts.Length >= 3 &&
                           parts[1].Trim().ToLowerInvariant() == name &&
                           parts[2].Trim().ToLowerInvariant() == surname;
                });

                if (exists)
                    return Conflict("Rodzic już istnieje.");
            }

            var lineToSave = string.Join(";", parent.ParentType, parent.Name, parent.Surname,
                                         parent.ChildrenCount, parent.Question, parent.QuestionAnswer);
            await System.IO.File.AppendAllTextAsync(_filePath, lineToSave + Environment.NewLine, Encoding.UTF8);

            return Ok("Rodzic został zapisany pomyślnie.");
        }
    }
}
