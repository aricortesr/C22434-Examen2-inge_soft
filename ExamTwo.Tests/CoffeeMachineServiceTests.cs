using ExamTwo.Controllers;
using ExamTwo.Exceptions;
using ExamTwo.Models;
using ExamTwo.Services;
using NUnit.Framework;

namespace ExamTwo.Tests;

public class CoffeeMachineServiceTests
{
    public ICoffeeMachineService _coffeeMachineService;

    public class CoffeeMachineServiceTests
    {
        private static CoffeeMachineService CreateService()
        {
            return new CoffeeMachineService(new Database());
        }

        [Tests]
        public void GetCoffeeStock_ReturnsInitialInventory()
        {
            // Arrange
            var service = CreateService();

            // Act
            var stock = service.GetCoffeeStock();

            // Asert
            Assert.That(stock["Americano"], Is.EqualTo(10));
            Assert.That(stock["Cappuccino"], Is.EqualTo(8));
            Assert.That(stock["Lates"], Is.EqualTo(10));
            Assert.That(stock["Mocaccino"], Is.EqualTo(15));
        }

        [Tests]
        public void BuyCoffee_ValidOrder_UpdatesStockAndReturnsChange()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = service.BuyCoffee(new OrderRequest
            {
                Order = new Dictionary<string, int>
                {
                    ["Americano"] = 1
                },
                Payment = new Payment
                {
                    TotalAmount = 1000
                }
            });

            // Assert
            Assert.That(result, Is.EqualTo("Su vuelto es de: 50 colones. Desglose: 1 moneda de 50,"));
            Assert.That(service.GetCoffeeStock()["Americano"], Is.EqualTo(9));
        }

        [Tests]
        public void BuyCoffee_WhenRequestExceedsStock_ThrowsInsufficientCoffeeException()
        {
            // Arrange
            var service = CreateService();

            var ex = Assert.Throws<InsufficientCoffeeException>(() => service.BuyCoffee(new OrderRequest
            {
                Order = new Dictionary<string, int>
                {
                    ["Cappuccino"] = 9
                },
                Payment = new Payment
                {
                    TotalAmount = 20000
                }
            }));

            Assert.That(ex!.Message, Is.EqualTo("No hay suficientes Cappuccino en la máquina."));
        }

    }
}
