using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Book.Models;
using Book.ViewModels;

namespace Book.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Books, BooksVM>();
            CreateMap<BooksVM, Books>();
        }
    }
}
