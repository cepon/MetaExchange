using MetaExchange.Domain.Interfaces;
using MetaExchange.Domain.Models;

namespace MetaExchange.Application
{
  public class ExchangeService : IExchangeService
  {
    private readonly IExchangeRepository _exchangesRepository;

    public ExchangeService(IExchangeRepository exchangesRepository)
    {
      _exchangesRepository = exchangesRepository;
    }

    public async Task<List<OrderObject>> BuyAsync(decimal amount)
    {
      var exchanges = await _exchangesRepository.GetAllAsync();
      var executionPlan = new List<OrderObject>();
      var remainingAmount = amount;

      while (remainingAmount > 0)
      {
        var buyOrder = GetBestBuyOrder(remainingAmount, exchanges);
        executionPlan.Add(buyOrder);
        remainingAmount -= buyOrder.Order.Amount;
      }

      return executionPlan;
    }

    public async Task<List<OrderObject>> SellAsync(decimal amount)
    {
      var exchanges = await _exchangesRepository.GetAllAsync();
      List<OrderObject> executionPlan = new List<OrderObject>();
      var remainingAmount = amount;
      while (remainingAmount > 0)
      {
        var buyOrder = GetBestSellOrder(remainingAmount, exchanges);
        executionPlan.Add(buyOrder);
        remainingAmount -= buyOrder.Order.Amount;
      }

      return executionPlan;
    }

    private OrderObject GetBestBuyOrder(decimal amount, List<OrderBook> exchanges)
    {
      var bestExchange = exchanges
        .Where(exchange => exchange.Balance > 0)
        .SelectMany(exchange => exchange.Asks, (exchange, ask) => new { Exchange = exchange, Ask = ask })
        .OrderBy(pair => pair.Ask.Order.Price)
        .FirstOrDefault()?.Exchange;

      if (!bestExchange.Bids.Any())
      {
        throw new Exception("No enough asks");
      }

      var bestOrder = bestExchange.Asks.FirstOrDefault();
      var tradeAmount = new[] { amount, bestOrder.Order.Amount, bestExchange.Balance }.Min();
      var order = new OrderObject
      {
        Order = new OrderData
        {
          Type = bestOrder.Order.Type,
          Amount = tradeAmount,
          Price = bestOrder.Order.Price
        }
      };

      bestExchange.Balance -= tradeAmount;

      if (bestOrder.Order.Amount == tradeAmount)
      {
        bestExchange.Asks.Remove(bestOrder);
      }

      return order;
    }

    private OrderObject GetBestSellOrder(decimal amount, List<OrderBook> exchanges)
    {
      var bestExchange = exchanges
        .Where(exchange => exchange.Balance > 0)
        .SelectMany(exchange => exchange.Bids, (exchange, bid) => new { Exchange = exchange, Bid = bid })
        .OrderByDescending(pair => pair.Bid.Order.Price)
        .FirstOrDefault()?.Exchange;

      if (bestExchange == null || !bestExchange.Bids.Any())
      {
        throw new Exception("No enough bids");
      }

      var bestOrder = bestExchange.Bids.FirstOrDefault();
      var tradeAmount = new[] { amount, bestOrder.Order.Amount, bestExchange.Balance }.Min();
      var order = new OrderObject
      {
        Order = new OrderData
        {
          Type = bestOrder.Order.Type,
          Amount = tradeAmount,
          Price = bestOrder.Order.Price
        }
      };

      bestExchange.Balance -= tradeAmount;

      if (bestOrder.Order.Amount == tradeAmount)
      {
        bestExchange.Bids.Remove(bestOrder);
      }

      return order;
    }
  }
}
