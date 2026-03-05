namespace BankTransfer;

public class IllegalStateException(string maintenance) : Exception
{
    public string Maintenance { get; } = maintenance;
}