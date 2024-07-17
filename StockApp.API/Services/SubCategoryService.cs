using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Extensions;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.SubCategoryDto;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class SubCategoryService : ISubCategoryService
{
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly IMapper _mapper;

    public SubCategoryService(ISubCategoryRepository subCategoryRepository, IMapper mapper)
    {
        _subCategoryRepository = subCategoryRepository;
        _mapper = mapper;
    }

    public async Task<IActionResult> GetAllSubCategoriesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords,subCategories) = await _subCategoryRepository.GetAllSubCategoriesAsync(skip, take, orderBy, isAscending);
        subCategories = subCategories.AsQueryable().OrderByField(orderBy, isAscending);
        
        var subCategoriesDto = _mapper.Map<IEnumerable<ListSubCategoryDto>>(subCategories);
        return new OkObjectResult(new PagedResultDto<ListSubCategoryDto>(totalRecords, subCategoriesDto));
    }

    public async Task<IActionResult> GetSubCategoryByIdAsync(int id)
    {

        var subCategory = await _subCategoryRepository.GetSubCategoryByIdAsync(id);

        if (subCategory is null)
            return new NotFoundResult();

        var subCategoryDto = _mapper.Map<ListSubCategoryDto>(subCategory);
        return new OkObjectResult(subCategoryDto);
    }

    public async Task<IActionResult> CreateSubCategoryAsync(CreateSubCategoryDto subCategoryDto)
    {
        var subCategory = _mapper.Map<SubCategory>(subCategoryDto);
        await _subCategoryRepository.CreateSubCategoryAsync(subCategory);
        var createdSubCategoryDto = _mapper.Map<ListSubCategoryDto>(subCategory);
        return new OkObjectResult(createdSubCategoryDto);
    }
}
