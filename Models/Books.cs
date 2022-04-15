using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using System.ComponentModel.DataAnnotations;

namespace Book.Models
{
    public class Books
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string BookName { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        [StringLength(100)]
        public int Edition { get; set; }

        //[Required]
        [DataType(DataType.Date)]
        [Display(Name = "Publishing Date")]
        public DateTime PublishingDate { get; set; }

        [Required]
        [StringLength(255)]
        public string Venue { get; set; }

        [Required]
        [Display(Name = "Image")]
        public string BookPicture { get; set; }
    }
}
