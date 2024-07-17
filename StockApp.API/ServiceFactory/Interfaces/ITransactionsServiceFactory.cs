using StockApp.API.Services.Interfaces;

namespace StockApp.API.ServiceFactory.Interfaces;

public interface ITransactionsServiceFactory
{
    ITransactionsService Create();
}