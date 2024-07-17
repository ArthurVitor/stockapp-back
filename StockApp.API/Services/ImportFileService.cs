using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StockApp.API.Exceptions;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.ProductDto;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class ImportFileService : IImportFileService
{
    private readonly ISubCategoryRepository _subCategoryRepository;

    private readonly IMapper _mapper;
    
    private readonly IProductRepository _productRepository;
    
    private readonly IProductSubCategoryService _productSubCategoryService;
    
    public ImportFileService(ISubCategoryRepository subCategoryService, IProductRepository productRepository, IProductSubCategoryService productSubCategoryService, IMapper mapper)
    {
        _subCategoryRepository = subCategoryService;
        _productRepository = productRepository;
        _productSubCategoryService = productSubCategoryService;
        _mapper = mapper;
    }
    
    public IEnumerable<string> GetLines(IFormFile file, string separator)
    {
        if (file == null || file.Length == 0)
        {
            throw new Exception("File can't be null");
        }
        
        using (var stream = new StreamReader(file.OpenReadStream()))
        {
            var fileContent = stream.ReadToEnd();
            var lines = fileContent.Split(separator);

            return lines;
        }
    }

    public async Task<IActionResult> ImportFileSubCategory(IFormFile file)
    {
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var jsonContent = await reader.ReadToEndAsync();

            var subCategories = JsonConvert.DeserializeObject<List<SubCategory>>(jsonContent);
            _subCategoryRepository.BulkCreateSubCategories(subCategories!);

            return new OkObjectResult("File Imported Sucessfully");
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
        
    
    }
    
    public async Task<IActionResult> ImportFileProduct(IFormFile file)
    {
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var jsonContent = await reader.ReadToEndAsync();

            var productDtos = JsonConvert.DeserializeObject<List<CreateImportingProductDto>>(jsonContent);
            foreach (var productDto in productDtos!)
            {
                try
                {
                    var subCategoriesId = _subCategoryRepository.BulkGetSubCategoryIdByName(productDto.SubCategories);
                    var product = await _productRepository.CreateProduct(_mapper.Map<CreateImportingProductDto, Product>(productDto));
                    _productSubCategoryService.CreateProductSubCategory(product, subCategoriesId);
                }
                catch (SubCategoryDoesntExist e)
                {
                    return new NotFoundObjectResult(e.Message);
                }
            }
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
        return new OkObjectResult("File Imported Sucessfully");
    }
}