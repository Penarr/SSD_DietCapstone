using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDietCapstone.Models
{
    public class Diet
    {
        [Required]
        public Guid DietId { get; set;}

        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public string JsonDiet { get; set; }
    }
}
