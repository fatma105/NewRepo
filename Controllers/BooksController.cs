using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Book.Data;
using Book.Models;
using Book.ViewModels;
using Book.Mapping;
using AutoMapper;
using Microsoft.EntityFrameworkCore;





namespace Book.Controllers
{
    public class BooksController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public BooksController(IMapper mapper, ApplicationDbContext context)
        {
            this._context = context;
            this._mapper = mapper;

        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
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


            //In Classical MVC 

            //int pageSize = 3;
            //int pageNumber2 = (pageNumber ?? 1)
            //    return View(teacher.ToPagedList(pageNumber2, pageSize));

        }


        //public  async Task<IActionResult> Index()
        //{
        //    return View(await _context.Books.ToListAsync());
        //}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksVM booksVM)
        {

            var book = _mapper.Map<BooksVM, Books>(booksVM);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var oneBook = await _context.Books.FindAsync(id);
            var book = _mapper.Map<Books, BooksVM>(oneBook);
            return View(book);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(BooksVM booksVM, int id)
        {
            var oneBook = await _context.Books.FindAsync(id);
            
            //Mapper.Map<BooksVM, Books>(booksVM, oneBook);

            _mapper.Map(booksVM, oneBook);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var oneBook = await _context.Books.FindAsync(id);
            var book = _mapper.Map<Books, BooksVM>(oneBook);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Delete(int id)
        {
            var oneBook = await _context.Books.FindAsync(id);
            _context.Remove(oneBook);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            var oneBook = await _context.Books.FindAsync(id);
            var book = _mapper.Map<Books, BooksVM>(oneBook);
            return View(book);
        }

    }
}
