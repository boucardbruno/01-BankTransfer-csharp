namespace BankTransfer.External;

public class IllegalStateException(string maintenance) : Exception
{
    public string Maintenance { get; } = maintenance;
}