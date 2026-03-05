namespace BankTransfer.External;

public class GlobalState
{
    public static int TransferCount = 0;
    public static bool Maintenance = false;
    public static Dictionary<string, int> Cache = new();
}