using AutoMapper;
using EFCore_DBLibrary;
using InventoryDatabaseLayer;
using InventoryModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBusinessLayer
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepo _dbRepo;
        public CategoriesService(InventoryDbContext context, IMapper mapper)
        {
            _dbRepo = new CategoriesRepo(mapper, context);
        }

        public async Task<List<CategoryDTO>> ListCategoriesAndDetails()
        {
            return await _dbRepo.ListCategoriesAndDetails();
        }
    }
}
