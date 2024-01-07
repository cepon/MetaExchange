using MetaExchange.Domain.Models;

namespace MetaExchange.Domain.Interfaces
{
  public interface IExchangeService
  {
    Task<List<OrderObject>> BuyAsync(decimal amount);
    Task<List<OrderObject>> SellAsync(decimal amount);
  }
}
