using MetaExchange.Domain.Interfaces;
using MetaExchange.Domain.Models;
using System.Text.Json;

namespace MetaExchange.Infrastructure.Repositories
{
  public class ExchangeRepository : IExchangeRepository
  {
    public async Task<List<OrderBook>> GetAllAsync()
    {
      var filePath = "shared/order_books_data";
      var data = await File.ReadAllLinesAsync(filePath);
      List<OrderBook> exchanges = new List<OrderBook>();
      foreach (var exchangeData in data)
      {
        var exchangeDataSplit = exchangeData.Split('\t');
        if (exchangeDataSplit.Length != 2)
        {
          throw new Exception("Data not valid");
        }

        OrderBook orderBook = JsonSerializer.Deserialize<OrderBook>(exchangeDataSplit[1]);
        orderBook.Asks = orderBook.Asks.OrderBy(x => x.Order.Price).ToList();
        orderBook.Bids = orderBook.Bids.OrderByDescending(x => x.Order.Price).ToList();
        orderBook.Balance = Convert.ToDecimal(exchangeDataSplit[0]);
        exchanges.Add(orderBook);
      }

      return exchanges;
    }
  }
}
