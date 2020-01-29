using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    public class LeaveTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,25,ErrorMessage ="Please enter a number between 1 and 25")]
        [Display(Name ="Default number of days.")]
        public int DefaultDays { get; set; }
        [Display(Name="Date Created")]
        public DateTime? DateCreated { get; set; }
    }

}
