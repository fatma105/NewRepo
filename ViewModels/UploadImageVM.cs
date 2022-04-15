using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Book.ViewModels
{
    public class UploadImageVM
    {
        [Required]
        [Display(Name = "Picture")]
        public IFormFile BookPicture { get; set; }

    }
}
