namespace BankTransfer.External;

public class SqlDatabase(NativeSql nativeSql) : IProvideSqlDatabase
{
    public int QueryBalance(string account)
    {
        return nativeSql.QueryBalance(account);
    }

    public void UpdateBalance(string account, int amount)
    {
        nativeSql.UpdateBalance(account, amount);
    }
}