namespace BankTransfer.External;

public static class GlobalState
{
    public static int TransferCount = 0;
    public static bool Maintenance = false;
    public static readonly Dictionary<string, int> Cache = new();
}