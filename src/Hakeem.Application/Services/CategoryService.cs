using Hakeem.Application.DTOs.Category;
using Hakeem.Application.Interfaces;
using Hakeem.Domain.Entities;
using Hakeem.Domain.Interfaces;

namespace Hakeem.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        
        return categories.Select(c => new CategoryResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            IconUrl = c.IconUrl
        });
    }

    public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        if (category == null) throw new Exception("Category not found");

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            IconUrl = category.IconUrl
        };
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            IconUrl = dto.IconUrl
        };

        await _unitOfWork.Repository<Category>().AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            IconUrl = category.IconUrl
        };
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        if (category == null) throw new Exception("Category not found");

        category.Name = dto.Name;
        category.IconUrl = dto.IconUrl;

        _unitOfWork.Repository<Category>().Update(category);
        await _unitOfWork.SaveChangesAsync();

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            IconUrl = category.IconUrl
        };
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
        if (category == null) throw new Exception("Category not found");

        _unitOfWork.Repository<Category>().Delete(category);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
}
