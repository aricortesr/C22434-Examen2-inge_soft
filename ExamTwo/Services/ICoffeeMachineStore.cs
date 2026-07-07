namespace ExamTwo.Services;

public interface ICoffeeMachineStore
{
    IReadOnlyDictionary<string, int> CoffeeStock { get; }

    IReadOnlyDictionary<string, int> CoffeePrices { get; }

    IReadOnlyDictionary<int, int> ChangeInventory { get; }

    bool TryGetCoffeePrice(string coffeeType, out int price);

    bool TryGetCoffeeStock(string coffeeType, out int amount);

    void DecreaseCoffeeStock(string coffeeType, int amount);

    void DecreaseChangeInventory(Dictionary<int, int> changeUsed);
}