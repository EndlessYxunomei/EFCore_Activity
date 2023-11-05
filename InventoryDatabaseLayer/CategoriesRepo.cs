using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCore_DBLibrary;
using InventoryModels.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryDatabaseLayer
{
    public class CategoriesRepo : ICategoriesRepo
    {
        private readonly IMapper _mapper;
        private readonly InventoryDbContext _context;
        public CategoriesRepo(IMapper mapper, InventoryDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<CategoryDTO>> ListCategoriesAndDetails()
        {
            return await _context.Categories.Include(x => x.CategoryDetail)
                .ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
