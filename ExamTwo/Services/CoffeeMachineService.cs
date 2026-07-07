using ExamTwo.Exceptions;
using ExamTwo.Models;

namespace ExamTwo.Services;

public sealed class CoffeeMachineService : ICoffeeMachineService
{
    private readonly ICoffeeMachineStore _coffeeMachineStore;

    public CoffeeMachineService(ICoffeeMachineStore coffeeMachineStore)
    {
        _coffeeMachineStore = coffeeMachineStore;
    }

    public IReadOnlyDictionary<string, int> GetCoffeeStock()
    {
        return _coffeeMachineStore.CoffeeStock;
    }

    public IReadOnlyDictionary<string, int> GetCoffeePrices()
    {
        return _coffeeMachineStore.CoffeePrices;
    }

    public IReadOnlyDictionary<int, int> GetAvailableChange()
    {
        return _coffeeMachineStore.ChangeInventory;
    }

    public string BuyCoffee(OrderRequest request)
    {
        ValidateRequest(request);

        var totalCost = CalculateTotalCost(request.Order);

        if (request.Payment.TotalAmount < totalCost)
        {
            throw new ArgumentException("Dinero insuficiente.");
        }

        var changeBreakdown = CalculateChangeBreakdown(request.Payment.TotalAmount - totalCost);

        foreach (var coffee in request.Order)
        {
            _coffeeMachineStore.DecreaseCoffeeStock(coffee.Key, coffee.Value);
        }

        return FormatChangeResult(request.Payment.TotalAmount - totalCost, changeBreakdown);
    }

    private static void ValidateRequest(OrderRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.Order is null || request.Order.Count == 0)
        {
            throw new ArgumentException("Orden vacia.");
        }

        if (request.Payment is null)
        {
            throw new ArgumentException("El pago es obligatorio.");
        }

        if (request.Payment.TotalAmount <= 0)
        {
            throw new ArgumentException("Dinero insuficiente.");
        }
    }

    private int CalculateTotalCost(Dictionary<string, int> order)
    {
        var totalCost = 0;

        foreach (var coffee in order)
        {
            if (coffee.Value <= 0)
            {
                throw new ArgumentException($"La cantidad solicitada para {coffee.Key} debe ser mayor que cero.");
            }

            if (!_coffeeMachineStore.TryGetCoffeePrice(coffee.Key, out var price))
            {
                throw new ArgumentException($"El café {coffee.Key} no existe en la máquina.");
            }

            totalCost += price * coffee.Value;
        }

        return totalCost;
    }

    private Dictionary<int, int> CalculateChangeBreakdown(int changeAmount)
    {
        var remainingChange = changeAmount;
        var changeBreakdown = new Dictionary<int, int>();

        foreach (var coin in _coffeeMachineStore.ChangeInventory.Keys.OrderByDescending(coin => coin))
        {
            var availableCoins = _coffeeMachineStore.ChangeInventory[coin];
            var coinsToUse = Math.Min(remainingChange / coin, availableCoins);

            if (coinsToUse <= 0)
            {
                continue;
            }

            changeBreakdown[coin] = coinsToUse;
            remainingChange -= coin * coinsToUse;
        }

        if (remainingChange > 0)
        {
            throw new NotEnoughChangeException("Fallo al realizar la compra");
        }

        return changeBreakdown;
    }

    private static string FormatChangeResult(int changeAmount, Dictionary<int, int> changeBreakdown)
    {
        var result = $"Su vuelto es de: {changeAmount} colones. Desglose:";

        foreach (var coin in changeBreakdown.OrderByDescending(coin => coin.Key))
        {
            result += $" {coin.Value} moneda de {coin.Key},";
        }

        return result;
    }
}