using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Book.ViewModels
{
    public class BooksVM: EditImageVM
    {

        [Required]
        [Display(Name = "Book Name")]
        public string BookName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Edition { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Publishing Date")]
        public DateTime PublishingDate { get; set; }

        [Required]
        public string Venue { get; set; }

    }
}
