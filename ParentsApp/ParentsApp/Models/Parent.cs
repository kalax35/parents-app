using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ParentsApp.Models
{
    public class Parent
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public eParentType ParentType { get; set; }
        [Required]
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
