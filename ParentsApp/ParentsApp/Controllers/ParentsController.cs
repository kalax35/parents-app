using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using ParentsApp.Helpers;
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
        private readonly string _errorfilePath;
        private readonly QuestionProvider _questionProvider;

        public ParentsController(IWebHostEnvironment env, IOptions<AppSettings> options,
            QuestionProvider questionProvider)
        {
            _env = env;
            _filePath = options.Value.ParentFilePath;
            _errorfilePath = options.Value.ErrorLogFilePath;
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

            string name = parent.Name?.Trim().ToLowerInvariant();
            string surname = parent.Surname?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname))
                return BadRequest("Imię i nazwisko są wymagane");

            if (await IsDuplicateAsync(name, surname))
                return Conflict("Rodzic już istnieje");

            try
            {
                var line = FormatParentAsLine(parent);
                await AppendParentToFileAsync(line);

                return Ok("Rodzic został zapisany pomyślnie");
            }
            catch (Exception ex)
            {
                var errorMessage = $"[{DateTime.Now}] Błąd zapisu rodzica: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}---{Environment.NewLine}";

                await System.IO.File.AppendAllTextAsync(_errorfilePath, errorMessage);

                return StatusCode(500, "Wystąpił błąd podczas zapisu");
            }
        }



        public async Task<IActionResult> List()
        {
            var parents = new List<Parent>();
            int skippedLines = 0;
            var invalidLines = new List<string>();

            if (System.IO.File.Exists(_filePath))
            {
                var lines = await System.IO.File.ReadAllLinesAsync(_filePath);
                foreach (var line in lines)
                {
                    try
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
                        else
                        {
                            skippedLines++;
                            invalidLines.Add($"Brakujące dane: {line}");
                        }
                    }
                    catch (Exception ex)
                    {
                        skippedLines++;
                        invalidLines.Add($"Błąd parsowania: {line} | {ex.Message}");
                        continue;
                    }
                }

                if (invalidLines.Any())
                {
                    var logContent = string.Join(Environment.NewLine, invalidLines);
                    await System.IO.File.AppendAllTextAsync(_errorfilePath, $"{DateTime.Now}: Błędy odczytu{Environment.NewLine}{logContent}{Environment.NewLine}{new string('-', 40)}{Environment.NewLine}");
                }
            }

            ViewBag.SkippedLines = skippedLines;
            return View(parents);
        }


        private List<SelectListItem> GetParentTypeSelectList()
        {
            return Enum.GetValues(typeof(eParentType))
                       .Cast<eParentType>()
                       .Select(e => new SelectListItem
                       {
                           Value = ((int)e).ToString(),
                           Text = e.GetDisplayName()
                       })
                       .ToList();
        }

        private async Task<bool> IsDuplicateAsync(string name, string surname)
        {
            if (!System.IO.File.Exists(_filePath))
                return false;

            var lines = await System.IO.File.ReadAllLinesAsync(_filePath);
            return lines.Any(line =>
            {
                var parts = line.Split(';');
                return parts.Length >= 3 &&
                       parts[1].Trim().ToLowerInvariant() == name &&
                       parts[2].Trim().ToLowerInvariant() == surname;
            });
        }

        private string FormatParentAsLine(Parent parent)
        {
            return string.Join(";",
                parent.ParentType,
                parent.Name.Trim(),
                parent.Surname.Trim(),
                parent.ChildrenCount,
                parent.Question,
                parent.QuestionAnswer?.Trim() ?? string.Empty
            );
        }

        private async Task AppendParentToFileAsync(string line)
        {
            await System.IO.File.AppendAllTextAsync(_filePath, line + Environment.NewLine, Encoding.UTF8);
        }

    }
}
