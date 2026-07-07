using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamTwo.Exceptions;
using ExamTwo.Models;
using ExamTwo.Services;

namespace ExamTwo.Controllers
{
    public class CoffeeMachineController : Controller
    {

        private readonly ICoffeeMachineService _coffeeMachineService;

        public CoffeeMachineController(ICoffeeMachineService coffeeMachineService)
        {
            _coffeeMachineService = coffeeMachineService;
        }

        [HttpGet("getCoffees")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            return Ok(_coffeeMachineService.GetCoffeeStock());
        }

        [HttpGet("getCoffeePricesInCents")]
        public ActionResult<Dictionary<string, int>> GetCoffeePricesInCents()
        {
            return Ok(_coffeeMachineService.GetCoffeePrices());
        }

        [HttpGet("getQuantity")]
        public ActionResult<Dictionary<string, int>> GetQuantity()
        {
            return Ok(_coffeeMachineService.GetAvailableChange());
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            try
            {
                return Ok(_coffeeMachineService.BuyCoffee(request));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InsufficientCoffeeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotEnoughChangeException ex)
            {
                return Conflict(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
