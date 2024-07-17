using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.InventoryDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class InventoryService : IInventoryService
{
    private readonly IBatchRepository _batchRepository;
    private readonly IParametersRepository _parametersRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOverrideService _overrideService;
    private readonly IMapper _mapper;


    public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper, IBatchRepository batchRepository, IParametersRepository parametersRepository, IOverrideService overrideService)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _batchRepository = batchRepository;
        _parametersRepository = parametersRepository;
        _overrideService = overrideService;
    }

    public async Task<IActionResult> CreateInventoryAsync(CreateInventoryDto inventoryDto)
    {
        var inventory = _mapper.Map<Inventory>(inventoryDto);
        await _inventoryRepository.CreateInventoryAsync(inventory);
        var createdInventoryDto = _mapper.Map<ListInventoryDto>(inventory);
        return new OkObjectResult(createdInventoryDto);
    }

    public async Task<IActionResult> GetAllInventoriesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords, inventories) = await _inventoryRepository.GetAllInventoriesAsync(skip, take, orderBy, isAscending);
        var inventoriesDto = _mapper.Map<IEnumerable<ListInventoryDto>>(inventories);
        return new OkObjectResult(new PagedResultDto<ListInventoryDto>(totalRecords, inventoriesDto));
    }

    public async Task<IActionResult> GetInventoryByIdAsync(int id)
    {
        var inventory = await _inventoryRepository.GetInventoryByIdAsync(id);

        if (inventory is null)
            return new NotFoundResult();

        var inventoryDto = _mapper.Map<ListInventoryDto>(inventory);
        return new OkObjectResult(inventoryDto);
    }

    public async Task<IActionResult> ValidateInventoryParametersAsync(NoteBase note)
    {
        var parameters = await _parametersRepository.GetParameterByInventoryIdAndProductIdAsync(note.InventoryId, note.Product.Id);
        if (parameters is null)
            return new OkResult();

        var totalAvailableQuantity = await GetTotalAvailableQuantityByInventoryIdAndProductIdAsync(note.InventoryId, note.Product.Id);

        await _overrideService.ValidateOverrideAsync(note, totalAvailableQuantity, parameters);

        return new OkResult();
    }

    public async Task<(IEnumerable<Batch> batches, IActionResult validationResult)> ValidateAvailableBatchesAsync(int inventoryId, int productId, double requestedQuantity)
    {
        var availableBatchesOfThisProduct = await _batchRepository.GetAvaliableBatchesByInventoryIdAndProductIdAsync(inventoryId, productId);
        if (availableBatchesOfThisProduct is null)
            return ([], new BadRequestObjectResult("This inventory does not have batches of this product."));

        var totalAvailableQuantity = availableBatchesOfThisProduct.Sum(b => b.Quantity);
        if (totalAvailableQuantity < requestedQuantity)
            return ([], new BadRequestObjectResult($"You tried to take {requestedQuantity} units from inventory, but the inventory has {totalAvailableQuantity} units of product id {productId}."));

        return (availableBatchesOfThisProduct, new OkResult());
    }

    public async Task<double> GetTotalAvailableQuantityByInventoryIdAndProductIdAsync(int inventoryId, int productId)
    {
        var availableBatches = await _batchRepository.GetAvaliableBatchesByInventoryIdAndProductIdAsync(inventoryId, productId);
        return availableBatches.Sum(b => b.Quantity);
    }
}
