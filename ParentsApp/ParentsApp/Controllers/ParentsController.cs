using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using ParentsApp.Models;
using ParentsApp.Services;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
            var model = new Parent
            {
                Question = _questionProvider.GetRandomQuestion(),
                ParentTypes = GetParentTypeSelectList()
            };

            return View(model);
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


        public async Task<IActionResult> List()
        {
            var parents = new List<Parent>();

            if (System.IO.File.Exists(_filePath))
            {
                var lines = await System.IO.File.ReadAllLinesAsync(_filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length >= 6)
                    {
                        parents.Add(new Parent
                        {
                            ParentType = Enum.Parse<eParentType>(parts[0]),
                            Name = parts[1],
                            Surname = parts[2],
                            ChildrenCount = int.Parse(parts[3]),
                            Question = parts[4],
                            QuestionAnswer = parts[5]
                        });
                    }
                }
            }

            return View(parents);
        }

        private List<SelectListItem> GetParentTypeSelectList()
        {
            return Enum.GetValues(typeof(eParentType))
                       .Cast<eParentType>()
                       .Select(e => new SelectListItem
                       {
                           Value = e.ToString(),
                           Text = e.GetType()
                                   .GetMember(e.ToString())
                                   .First()
                                   .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                       })
                       .ToList();
        }
    }
}
