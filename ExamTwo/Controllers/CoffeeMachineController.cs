using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamTwo.Models;

namespace ExamTwo.Controllers
{
    public class CoffeeMachineController : Controller
    {

        private readonly Database _db;

        public CoffeeMachineController(Database db)
        {
            _db = db;
        }

        [HttpGet("getCoffees")]
        public ActionResult<Dictionary<string, int>> GetCoffeePrices()
        {
            return Ok(_db.initialCoffeeTypeAmount);
        }

        [HttpGet("getCoffeePricesInCents")]
        public ActionResult<Dictionary<string, int>> GetCoffeePricesInCents()
        {
            return Ok(_db.coffeeTypePrice);
        }

        [HttpGet("getQuantity")]
        public ActionResult<Dictionary<string, int>> GetQuantity()
        {
            return Ok(_db.initialChange);
        }

        [HttpPost("buyCoffee")]
        public ActionResult<string> BuyCoffee([FromBody] OrderRequest request)
        {
            if (request.Order == null || request.Order.Count == 0)
                return BadRequest("Orden vacia.");

            if (request.Payment.TotalAmount <= 0)
                return BadRequest("Dinero insuficiente ");

            try
            {
                var totalCost = request.Order.Sum(o => _db.coffeeTypePrice.First(c => c.Key == o.Key).Value * o.Value);

                if (request.Payment.TotalAmount < totalCost)
                { 
                    return BadRequest("Dinero insuficiente ");
                }


                foreach (var coffee in request.Order)
                {
                    var selected = _db.initialCoffeeTypeAmount.First(c => c.Key == coffee.Key).Key;
                    if (coffee.Value > _db.initialCoffeeTypeAmount[selected])
                    {
                        return $"No hay suficientes {selected} en la máquina.";
                    }
                    _db.initialCoffeeTypeAmount[selected] -= coffee.Value;
                }

                var change = request.Payment.TotalAmount - totalCost;
                String result = $"Su vuelto es de: {change} colones. Desglose:";

                foreach (var coin in _db.initialChange.Keys.OrderByDescending(c => c))
                {
                    var count = Math.Min(change / coin, _db.initialChange[coin]);
                    if (count > 0)
                    {
                        result +=  $" {count} moneda de {coin},  ";              
                        change -= coin * count;
                    }
                }


                if (change > 0)
                {
                    return StatusCode(500, "No hay suficiente cambio en la máquina.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}
