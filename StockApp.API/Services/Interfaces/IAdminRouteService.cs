using Microsoft.AspNetCore.Mvc;
using StockApp.Models.Dtos.AdminRoute;
using StockApp.Models.Dtos.Transactions;

namespace StockApp.API.Services.Interfaces;

public interface IAdminRouteService
{
    Task<IActionResult> ReallocateBatch(CreateReallocateProductDto reallocateProductDto);

    Task<IActionResult> ReverseTransactionAsync(ReverseTransactionDto reverseTransactionDto);
  
    Task<IActionResult> GetAllExpiredBatches(int skip, int take, string orderBy, bool isAscending);

    Task<IActionResult> DeleteAllPerishedBatches();
}