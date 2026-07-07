namespace ExamTwo.Exceptions
{
    public sealed class NotEnoughChangeException : Exception
    {
        public string ChangeMessage = "Fallo al realizar la compra.";


        public NotEnoughChangeException(string message) : base(message)
        {
            ChangeMessage = message;
        }
    }
}