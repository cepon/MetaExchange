using MetaExchange.Domain.Interfaces;
using MetaExchange.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MetaExchange.Host.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ExchangeController : ControllerBase
  {
    private readonly IExchangeService _exchangeService;

    public ExchangeController(IExchangeService exchangeService)
    {
      _exchangeService = exchangeService;
    }

    [HttpGet("Buy/{amount}")]
    [ProducesResponseType(typeof(List<OrderObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BuyAsync(decimal amount)
    {
      try
      {
        var response = await _exchangeService.BuyAsync(amount);
        return Ok(response);
      }
      catch (Exception ex)
      {
        //log
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("Sell/{amount}")]
    [ProducesResponseType(typeof(List<OrderObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SellAsync(decimal amount)
    {
      try
      {
        var response = await _exchangeService.SellAsync(amount);
        return Ok(response);
      }
      catch (Exception ex)
      {
        //log
        return BadRequest(ex.Message);
      }
    }
  }
}
