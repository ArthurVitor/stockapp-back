using StockApp.API.ServiceFactory.Interfaces;
using StockApp.API.Services.Interfaces;

namespace StockApp.API.ServiceFactory;

public class TransactionsServiceFactory : ITransactionsServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TransactionsServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITransactionsService Create()
    {
        return _serviceProvider.GetRequiredService<ITransactionsService>();
    }
}