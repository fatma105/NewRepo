using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Book.Data;
using Book.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Book.Controllers
{
    public class LibrarianRecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;


        public LibrarianRecordController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            this._context = context;
            this._webHost = webHost;
        }


        public IActionResult Index()
        {
            List<Librarian> librarians;
            librarians = _context.Librarians.ToList();
            return View(librarians);
        }


        [HttpGet]
        public IActionResult Create()
        {
            Librarian librarian = new Librarian();
            librarian.Experiences.Add(new Experience() { ExperienceId = 1 });
            //librarian.Experiences.Add(new Experience() { ExperienceId = 2 });
            //librarian.Experiences.Add(new Experience() { ExperienceId = 3 });
            return View(librarian);
        }


        [HttpPost]
        public IActionResult Create(Librarian librarian)
        {

            string uniqueFileName = GetUploadedFileName(librarian);
            librarian.PhotoUrl = uniqueFileName;

            _context.Add(librarian);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        private string GetUploadedFileName(Librarian librarian)
        {
            string uniqueFileName = null;

            if (librarian.ProfilePhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "Uploads/"); // Uploads/
                uniqueFileName = Guid.NewGuid().ToString() + "_" + librarian.ProfilePhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    librarian.ProfilePhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
    
}
