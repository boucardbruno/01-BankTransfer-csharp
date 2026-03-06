namespace BankTransfer.External;

public class NativeSql
{
    public int QueryBalance(string account)
    {
        throw new ExternalSideEffectException("SQL access forbidden in test");
    }

    public void UpdateBalance(string account, int amount)
    {
        throw new ExternalSideEffectException("SQL update forbidden in test");
    }
}