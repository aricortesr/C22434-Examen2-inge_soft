using ExamTwo.Exceptions;
using ExamTwo.Services;

namespace ExamTwo.Controllers
{
    public class Database : ICoffeeMachineStore
    {
        private readonly Dictionary<string, int> coffeeTypeAmount = new()
        {
            { "Americano", 10 },
            { "Cappuccino", 8 },
            { "Lates", 10 },
            { "Mocaccino", 15}
        };

        private readonly Dictionary<string, int> coffeeTypePrice = new()
        {
            { "Americano", 950 },
            { "Cappuccino", 1200 },
            { "Lates", 1350 },
            { "Mocaccino", 1500}
        };

        private readonly Dictionary<int, int> initialChange = new()
        {
            { 500, 20 },
            { 100, 30 },
            { 50, 50 },
            { 25, 25}
        };

            public IReadOnlyDictionary<string, int> CoffeeStock => coffeeTypeAmount;

    public IReadOnlyDictionary<string, int> CoffeePrices => coffeeTypePrice;

    public IReadOnlyDictionary<int, int> ChangeInventory => initialChange;

    public bool TryGetCoffeePrice(string coffeeType, out int price)
    {
        return coffeeTypePrice.TryGetValue(coffeeType, out price);
    }

    public bool TryGetCoffeeStock(string coffeeType, out int amount)
    {
        return coffeeTypeAmount.TryGetValue(coffeeType, out amount);
    }

        public void DecreaseCoffeeStock(string coffeeType, int amount)
        {
            if (string.IsNullOrWhiteSpace(coffeeType))
            {
                throw new ArgumentException("El tipo de café es obligatorio.");
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "La cantidad debe ser mayor que cero.");
            }

            if (!coffeeTypeAmount.TryGetValue(coffeeType, out var currentAmount))
            {
                throw new ArgumentException($"El café {coffeeType} no existe en la máquina.");
            }

            if (currentAmount < amount)
            {
                throw new InsufficientCoffeeException(coffeeType);
            }

            coffeeTypeAmount[coffeeType] = currentAmount - amount;
        }

        public void DecreaseChangeInventory(Dictionary<int, int> changeUsed)
        {
            if (changeUsed is null)
            {
                throw new ArgumentNullException(nameof(changeUsed));
            }

            foreach (var change in changeUsed)
            {
                if (!initialChange.TryGetValue(change.Key, out var currentAmount))
                {
                    throw new ArgumentException($"La moneda de {change.Key} no existe en la máquina.");
                }

                if (change.Value <= 0)
                {
                    continue;
                }

                if (currentAmount < change.Value)
                {
                    throw new NotEnoughChangeException("Fallo al realizar la compra");
                }
                initialChange[change.Key] = currentAmount - change.Value;
            }
        }
    }
}
