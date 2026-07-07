namespace ExamTwo.Exceptions
{
    public sealed class InsufficientCoffeeException : Exception
    {
        public string CoffeeType { get; }

        public InsufficientCoffeeException(string coffeeType) : base($"No hay suficientes {coffeeType} en la máquina.")
        {
            CoffeeType = coffeeType;
        }
    }
}