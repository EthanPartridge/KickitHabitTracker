using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kickit.Models
{
    public class Habit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Habit Name")]
        public string HabitName { get; set; }
        [DisplayName("Last Day Completed")]
        public string LastDayCompleted { get; set; }
        [Required]
        [MaxLength(320)]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }
        [ForeignKey("UserEmail")]
        public virtual User User { get; set; }
    }
}
