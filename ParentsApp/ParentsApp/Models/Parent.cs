using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ParentsApp.Models
{
    public class Parent
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Imię jest wymagane")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Wybierz typ rodzica")]
        public eParentType ParentType { get; set; }
        [Required(ErrorMessage = "Podaj liczbę dzieci")]
        [Range(1, 100, ErrorMessage = "Liczba dzieci musi być większa niż 1")]
        public int ChildrenCount { get; set; }
        public string Question { get; set; }
        public string QuestionAnswer { get; set; } = String.Empty;
        public List<SelectListItem> ParentTypes { get; set; } = new();
    }


    public enum eParentType
    {
        [Display(Name = "Mama")]
        Mother = 0,

        [Display(Name = "Tata")]
        Father = 1
    }
}
