using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Dtos.BatchDto;
using StockApp.Models.Dtos.Pagination;
using StockApp.Models.Models;

namespace StockApp.API.Services;

public class BatchService : IBatchService
{
    private readonly IMapper _mapper;
    private readonly IBatchRepository _batchRepository;

    public BatchService(IMapper mapper, IBatchRepository batchRepository)
    {
        _mapper = mapper;
        _batchRepository = batchRepository;
    }

    public async Task<Batch> CreateBatchAsync(EntryNote note)
    {
        var createBatchDto = _mapper.Map<CreateBatchDto>(note);
        var batch = _mapper.Map<Batch>(createBatchDto);

        await _batchRepository.CreateBatchAsync(batch);

        return batch;
    }

    public async Task<IActionResult> GetAllAvaliableBatchesAsync(int skip, int take, string orderBy, bool isAscending)
    {
        var (totalRecords, batches) = await _batchRepository.GetAllAvaliableBatchesAsync(skip, take, orderBy, isAscending);
        var batchesDto = _mapper.Map<IEnumerable<ListBatchDto>>(batches);
        return new OkObjectResult(new PagedResultDto<ListBatchDto>(totalRecords, batchesDto));
    }

    public async Task<IActionResult> GetAvaliableBatchByIdAsync(int id)
    {
        var batch = await _batchRepository.GetAvaliableBatchByIdAsync(id);
        if (batch == null)
            return new NotFoundResult();

        var batchDto = _mapper.Map<ListBatchDto>(batch);
        return new OkObjectResult(batchDto);
    }

    public async Task<IActionResult> GetExpiredBatchByIdAsync(int id)
    {
        var batch = await _batchRepository.GetExpiredBatchByIdAsync(id);
        if (batch == null)
            return new NotFoundResult();

        var batchDto = _mapper.Map<ListBatchDto>(batch);
        return new OkObjectResult(batchDto);
    }

    public async Task<List<UsedBatch>> DeductQuantityFromBatchesAsync(IEnumerable<Batch> batches, double quantityToDeduct)
    {
        var usedBatches = new List<UsedBatch>();

        foreach (var batch in batches.OrderBy(b => b.ExpiryDate ?? b.EntryNote.NoteGenerationTime))
        {
            if (quantityToDeduct <= 0) break;

            double quantityFromBatch = Math.Min(batch.Quantity, quantityToDeduct);
            double totalValueToDeduct = quantityFromBatch * batch.Price;
            batch.Quantity -= quantityFromBatch;
            batch.TotalValue -= totalValueToDeduct;
            quantityToDeduct -= quantityFromBatch;

            usedBatches.Add(new UsedBatch
            {
                Batch = batch,
                QuantityUsed = quantityFromBatch
            });

            if (batch.Quantity == 0)
                await _batchRepository.DeleteBatchAsync(batch);
            else
                await _batchRepository.UpdateBatchAsync(batch);
        }
        return usedBatches;
    }

    public async Task<IActionResult> DeleteBatchByIdAsync(int id)
    {
        var batch = await _batchRepository.GetBatchByIdAsync(id);
        if (batch == null)
            return new NotFoundResult();

        await _batchRepository.DeleteBatchAsync(batch);
        return new NoContentResult();
    }
}
