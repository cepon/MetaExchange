using MetaExchange.Domain.Models;

namespace MetaExchange.Domain.Interfaces
{
  public interface IExchangeRepository
  {
    Task<List<OrderBook>> GetAllAsync();
  }
}
