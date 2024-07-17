using Microsoft.AspNetCore.Mvc;
using StockApp.API.Repositories.Interfaces;
using StockApp.API.Services.Interfaces;
using StockApp.Models.Models;

namespace StockApp.API.Services
{
    public class ExitNoteBatchService : IExitNoteBatchService
    {
        private readonly IExitNoteBatchRepository _exitNoteBatchRepository;
        private readonly IBatchRepository _batchRepository;

        public ExitNoteBatchService(IExitNoteBatchRepository exitNoteBatchRepository, IBatchRepository batchRepository)
        {
            _exitNoteBatchRepository = exitNoteBatchRepository;
            _batchRepository = batchRepository;
        }

        public async Task<IActionResult> CreateExitNoteBatchAsync(ExitNote exitNote, List<UsedBatch> usedBatches)
        {
            var exitNoteBatches = usedBatches.Select(usedBatch => new ExitNoteBatch
            {
                Batch = usedBatch.Batch,
                ExitNoteId = exitNote.Id,
                BatchId = usedBatch.Batch.Id,
                Quantity = usedBatch.QuantityUsed
            }).ToList();

            await _exitNoteBatchRepository.CreateExitNoteBatchesAsync(exitNoteBatches);
            return new OkObjectResult(exitNoteBatches);
        }
    }
}
