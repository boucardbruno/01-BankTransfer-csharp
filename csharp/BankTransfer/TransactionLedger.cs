using BankTransfer.External;

namespace BankTransfer;

public class TransactionLedger
{
    public void ReleasePosition()
    {
        GlobalState.Cache.Clear();
    }

    public void IncrementTransactions()
    {
        if (GlobalState.Maintenance) throw new IllegalStateException("the system is in Maintenance mode");
        GlobalState.TransferCount++;
    }

    public bool ExceededTransferLimit(int limit)
    {
        return GlobalState.TransferCount > limit;
    }

    public static void SetMaintenanceMode(bool mode)
    {
        GlobalState.Maintenance = mode;
    }
}