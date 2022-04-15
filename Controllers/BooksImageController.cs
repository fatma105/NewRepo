using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;
using Book.Data;
using Book.Models;
using Book.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;


namespace Book.Controllers
{
    public class BooksImageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public BooksImageController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;

        }
        public async  Task <IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;


            var book = from b in _context.Books
                       select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                book = book.Where(b => b.BookName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    book = book.OrderByDescending(b => b.BookName);
                    break;
                default:
                    book = book.OrderBy(b => b.BookName);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<Books>.CreateAsync(book.AsNoTracking(), pageNumber ?? 1, pageSize));



            //return View(await _context.Books.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksVM booksVM)
        {

            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(booksVM);
                Books book = new Books
                {
                    BookName = booksVM.BookName,
                    Description = booksVM.Description,
                    Edition = booksVM.Edition,
                    PublishingDate = booksVM.PublishingDate,
                    //PublishingTime = booksVM.PublishingTime,
                    Venue = booksVM.Venue,
                    BookPicture = uniqueFileName
                };

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booksVM);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);

            var bookViewModel = new BooksVM()
            {
                Id = book.Id,
                BookName = book.BookName,
                Description = book.Description,
                Edition = book.Edition,
                PublishingDate = (DateTime)book.PublishingDate,
                //PublishingTime = (DateTime)book.PublishingTime,
                Venue = book.Venue,
                ExistingImage = book.BookPicture
            };

            if (book == null)
            {
                return NotFound();
            }
            return View(bookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BooksVM booksVM)
        {

            if (ModelState.IsValid)
            {

                var book = await _context.Books.FindAsync(booksVM.Id);

                book.BookName = booksVM.BookName;
                book.Description = booksVM.Description;
                book.Edition = booksVM.Edition;
                book.PublishingDate = booksVM.PublishingDate;
                //book.PublishingTime = booksVM.PublishingTime;
                book.Venue = booksVM.Venue;

                if (booksVM.BookPicture != null)
                {
                    if (booksVM.ExistingImage != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder, booksVM.ExistingImage);
                        System.IO.File.Delete(filePath);
                    }

                    book.BookPicture = ProcessUploadedFile(booksVM);
                }


                _context.Update(book);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);


            var bookViewModel = new BooksVM()
            {
                Id = book.Id,
                BookName = book.BookName,
                Description = book.Description,
                Edition = book.Edition,
                PublishingDate = (DateTime)book.PublishingDate,
                //PublishingTime = (DateTime)book.PublishingTime,
                Venue = book.Venue,
                ExistingImage = book.BookPicture
            };

            if (book == null)
            {
                return NotFound();
            }

            return View(bookViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            var CurrentImage = Path.Combine(Directory.GetCurrentDirectory(), FileLocation.DeleteFileFromFolder, book.BookPicture);
            _context.Books.Remove(book);
            if (System.IO.File.Exists(CurrentImage))
            {
                System.IO.File.Delete(CurrentImage);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);

            var bookViewModel = new BooksVM()
            {
                Id = book.Id,
                BookName = book.BookName,
                Description = book.Description,
                Edition = book.Edition,
                PublishingDate = (DateTime)book.PublishingDate,
            
                Venue = book.Venue,
                ExistingImage = book.BookPicture
            };


            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private string ProcessUploadedFile(BooksVM booksVM)
        {
            string uniqueFileName = null;

            if (booksVM.BookPicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, FileLocation.FileUploadFolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + booksVM.BookPicture.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    booksVM.BookPicture.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }


    }
}
