﻿using EatUp.Meals.DTO;
using EatUp.Meals.Models;

namespace EatUp.Meals.Services
{
    public interface ICategoryService
    {
        Task<Guid> Create(AddCategoryDTO addCategoryDTO);
        Task<PaginationResult<CategoryDTO>> GetPage(int skip, int take);
    }
}