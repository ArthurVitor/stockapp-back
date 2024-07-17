using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Dtos.Parameters;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class ParameterService : IParameterService
{
    private readonly IMapper _mapper;

    private readonly IParametersRepository _parametersRepository;

    private readonly IProductRepository _productRepository;

    private readonly IInventoryRepository _inventoryRepository;

    public ParameterService(IMapper mapper, IParametersRepository parametersRepository, IProductRepository productRepository, IInventoryRepository inventoryRepository)
    {
        _mapper = mapper;
        _parametersRepository = parametersRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IActionResult> CreateParameter(CreateParameterDto parameterDto)
    {
        var parameter = _mapper.Map<Parameters>(parameterDto);

        var validationResult = await ValidateParameterAsync(parameter);

        if (validationResult is not null)
            return validationResult;

        var createdObject = await _parametersRepository.CreateProductParameterAsync(parameter);
        return new OkObjectResult(_mapper.Map<ListParameterDto>(createdObject));
    }

    public async Task<IActionResult> GetAllParameters(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords, products) = await _parametersRepository.GetAllParametersAsync(skip, take, orderBy, isAscending);
        var listParameterDtos = products.Select(p => _mapper.Map<ListParameterDto>(p));
        return new OkObjectResult(new PagedResultDto<ListParameterDto>(totalRecords, listParameterDtos));
    }

    public async Task<IActionResult> GetParameterById(int id)
    {
        var parameter = await _parametersRepository.GetParameterByIdAsync(id);

        if (parameter == null)
            return new NotFoundObjectResult($"Parameter with Id: {id} doesn't exist");

        return new OkObjectResult(_mapper.Map<ListParameterDto>(parameter));
    }

    public async Task<IActionResult> GetParameterByInventoryIdAndProductIdAsync(int inventoryId, int productId)
    {
        var parameter = await _parametersRepository.GetParameterByInventoryIdAndProductIdAsync(inventoryId, productId);

        if (parameter == null)
            return new NotFoundObjectResult($"Parameter with inventory id {inventoryId} and product id {productId} doesn't exist");

        return new OkObjectResult(_mapper.Map<ListParameterDto>(parameter));
    }

    private async Task<IActionResult> ValidateParameterAsync(Parameters parameter)
    {

        if (!_productRepository.Exists(parameter.ProductId))
            return new NotFoundObjectResult($"Product with Id {parameter.ProductId} doesn't exist.");

        if (await _inventoryRepository.GetInventoryByIdAsync(parameter.InventoryId) is null)
            return new NotFoundObjectResult($"Inventory with Id {parameter.InventoryId} doesn't exist.");

        if (await _parametersRepository.GetParameterByInventoryIdAndProductIdAsync(parameter.InventoryId, parameter.ProductId) is not null)
            return new BadRequestObjectResult($"A parameter with ProductId {parameter.ProductId} and InventoryId {parameter.InventoryId} already exists.");

        return null;
    }
}