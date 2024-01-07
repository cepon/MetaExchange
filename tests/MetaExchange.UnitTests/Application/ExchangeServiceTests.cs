using MetaExchange.Application;
using MetaExchange.Domain.Interfaces;
using MetaExchange.Domain.Models;
using NSubstitute;

namespace MetaExchange.UnitTests.Application
{
  public class ExchangeServiceTests
  {
    private readonly IExchangeRepository _exchangeRepository;
    private readonly ExchangeService _exchangeService;

    public ExchangeServiceTests()
    {
      _exchangeRepository = Substitute.For<IExchangeRepository>();
      _exchangeService = new ExchangeService(_exchangeRepository);
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnCorrectExecutionPlan()
    {
      // Arrange
      decimal amount = 3;
      var exchanges = GetTestExchanges();

      _exchangeRepository.GetAllAsync().Returns(exchanges);

      // Act
      var result = await _exchangeService.BuyAsync(amount);

      // Assert
      Assert.NotNull(result);
      Assert.Single(result);
      Assert.Equal(3, result.First().Order.Amount);
      Assert.Equal(3000, result.First().Order.Price);
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnCorrectExecutionPlan_WhenAmountBiggerThanOrderAmount()
    {
      // Arrange
      decimal amount = 5;
      var exchanges = GetTestExchanges();

      _exchangeRepository.GetAllAsync().Returns(exchanges);

      // Act
      var result = await _exchangeService.BuyAsync(amount);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(2, result.Count());

      Assert.Equal(3, result.First().Order.Amount);
      Assert.Equal(3000, result.First().Order.Price);

      Assert.Equal(2, result.ElementAt(1).Order.Amount);
      Assert.Equal(3100, result.ElementAt(1).Order.Price);
    }

    [Fact]
    public async Task BuyAsync_ShouldReturnCorrectExecutionPlan_WhenAmountBiggerThanExchangeBalance()
    {
      // Arrange
      decimal amount = 7;
      var exchanges = GetTestExchanges();

      _exchangeRepository.GetAllAsync().Returns(exchanges);

      // Act
      var result = await _exchangeService.BuyAsync(amount);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(4, result.Count());
    }

    [Fact]
    public async Task SellAsync_ShouldReturnCorrectExecutionPlan()
    {
      // Arrange
      decimal amount = 1;
      var exchanges = GetTestExchanges();

      _exchangeRepository.GetAllAsync().Returns(exchanges);

      // Act
      var result = await _exchangeService.SellAsync(amount);

      // Assert
      Assert.NotNull(result);
      Assert.Single(result);
      Assert.Equal(1, result.First().Order.Amount);
      Assert.Equal(2980, result.First().Order.Price);
    }

    [Fact]
    public async Task SellAsync_ShouldReturnCorrectExecutionPlan_WhenAmountBiggerThanOrderAmount()
    {
      // Arrange
      decimal amount = 12;
      var exchanges = GetTestExchanges();

      _exchangeRepository.GetAllAsync().Returns(exchanges);

      // Act
      var result = await _exchangeService.SellAsync(amount);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(2, result.Count());

      Assert.Equal(10, result.First().Order.Amount);
      Assert.Equal(2980, result.First().Order.Price);

      Assert.Equal(2, result.ElementAt(1).Order.Amount);
      Assert.Equal(2950, result.ElementAt(1).Order.Price);
    }

    [Fact]
    public async Task SellAsync_ShouldReturnCorrectExecutionPlan_WhenAmountBiggerThanExchangeBalance()
    {
      // Arrange
      decimal amount = 101;
      var exchanges = GetTestExchanges();

      _exchangeRepository.GetAllAsync().Returns(exchanges);

      // Act
      var result = await _exchangeService.SellAsync(amount);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task SellAsync_ShouldReturnCorrectExecutionPlan_WhenAmountBiggerThanSumOfAllBalances()
    {
      // Arrange
      decimal amount = 110;
      var exchanges = GetTestExchanges();

      _exchangeRepository.GetAllAsync().Returns(exchanges);

      // Act and Assert
      var exception = await Assert.ThrowsAsync<Exception>(() => _exchangeService.SellAsync(amount));
      Assert.Equal("No enough bids", exception.Message);
    }

    private List<OrderBook> GetTestExchanges()
    {
      return new List<OrderBook>
      {
        // First Order Book
        new OrderBook
        {
          Balance = 5,
          Asks = new List<OrderObject>()
          {
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 3,
                Type = "Sell",
                Price = 3000
              }
            },
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 100,
                Type = "Sell",
                Price = 3100
              }
            }
          },
          Bids = new List<OrderObject>()
          {
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 1,
                Type = "Buy",
                Price = 2900
              }
            },
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 30,
                Type = "Buy",
                Price = 2800
              }
            }
          }
        },
        // Second Order Book
        new OrderBook
        {
          Balance = 2.5m,
          Asks = new List<OrderObject>()
          {
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 1.5m,
                Type = "Sell",
                Price = 3200
              }
            },
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 30,
                Type = "Sell",
                Price = 4200
              }
            }
          },
          Bids = new List<OrderObject>()
          {
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 10,
                Type = "Buy",
                Price = 2500
              }
            },
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 10,
                Type = "Buy",
                Price = 2000
              }
            }
          }
        },
        // Third Order Book
        new OrderBook
        {
          Balance = 100,
          Asks = new List<OrderObject>()
          {
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 0.5m,
                Type = "Sell",
                Price = 3500
              }
            },
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 200m,
                Type = "Sell",
                Price = 4500
              }
            }
          },
          Bids = new List<OrderObject>()
          {
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 10,
                Type = "Buy",
                Price = 2980
              }
            },
            new OrderObject()
            {
              Order = new OrderData()
              {
                Amount = 100,
                Type = "Buy",
                Price = 2950
              }
            }
          }
        }
      };
    }
  }
}
