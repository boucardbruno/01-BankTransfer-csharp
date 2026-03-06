using BankTransfer.External;
using HttpClient = BankTransfer.External.HttpClient;

namespace BankTransfer;

public class MegaTransferEngine(IProvideSqlDatabase sqlDatabase, IProvideHttpClient httpClient)
{
    public MegaTransferEngine(): this(new SqlDatabase(new NativeSql()), new HttpClient(new HttpRiskClient()))
    {
    }
    
    private readonly TransactionLedger _transactionLedger = new();

    public bool MakeTransfer(BankTransfer bankTransfer, KindOfChannel channel)
    {
        _transactionLedger.IncrementTransactions();

        var (balance, netAmount) = ComputeTransferFinancials(bankTransfer, channel);

        if (!TransferToClient(bankTransfer, netAmount)) return false;

        UpdateDatabase(bankTransfer, balance, netAmount);

        if (_transactionLedger.ExceededTransferLimit(100)) _transactionLedger.ReleasePosition();
        return true;
    }

    private (int balance, int netAmount) ComputeTransferFinancials(BankTransfer bankTransfer, KindOfChannel channel)
    {
        var fees = ComputeFees(bankTransfer, channel);
        return (balance: ComputeBalance(bankTransfer), netAmount: ComputeNetAmount(bankTransfer, fees));
    }

    private static int ComputeFees(BankTransfer bankTransfer, KindOfChannel channel)
    {
        var fees = 0;
        switch (channel)
        {
            case KindOfChannel.Mobile:
                fees += 2;
                break;
            case KindOfChannel.Web:
                fees += 1;
                break;
        }

        if (bankTransfer.Vip) fees--;
        return fees;
    }

    public void UpdateDatabase(BankTransfer bankTransfer, int balance, int netAmount)
    {
        sqlDatabase.UpdateBalance(bankTransfer.From, balance - bankTransfer.Amount);

        if (netAmount % 2 == 0)
        {
            sqlDatabase.UpdateBalance(bankTransfer.To, netAmount);
        }
        else
        {
            sqlDatabase.UpdateBalance(bankTransfer.To, netAmount - 1);
            sqlDatabase.UpdateBalance(bankTransfer.To, 1);
        }
    }

    private bool TransferToClient(BankTransfer bankTransfer, int netAmount)
    {
        var makeTransfer = true;
        if (httpClient.Risky(bankTransfer.From, netAmount))
            if (!bankTransfer.Vip)
                return false;

        return makeTransfer;
    }

    private int ComputeBalance(BankTransfer bankTransfer)
    {
        int balance;
        if (GlobalState.Cache.ContainsKey(bankTransfer.From))
        {
            balance = GlobalState.Cache[bankTransfer.From];
        }
        else
        {
            balance = sqlDatabase.QueryBalance(bankTransfer.From);
            GlobalState.Cache[bankTransfer.From] = balance;
        }

        return balance;
    }

    private int ComputeNetAmount(BankTransfer bankTransfer, int fees)
    {
        var netAmount = 0;

        if (bankTransfer.Amount > 0 && bankTransfer.Amount - fees >= 0) netAmount = bankTransfer.Amount - fees;

        return netAmount;
    }
}