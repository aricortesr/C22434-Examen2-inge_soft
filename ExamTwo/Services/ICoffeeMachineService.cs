using ExamTwo.Models;

namespace ExamTwo.Services;

public interface ICoffeeMachineService
{
    IReadOnlyDictionary<string, int> GetCoffeeStock();

    IReadOnlyDictionary<string, int> GetCoffeePrices();

    IReadOnlyDictionary<int, int> GetAvailableChange();

    string BuyCoffee(OrderRequest request);
}