namespace BankTransfer.External;

public class NativeSql
{
    public int QueryBalance(string acc)
    {
        throw new ExternalSideEffectException("SQL access forbidden in test");
    }

    public void UpdateBalance(string acc, int value)
    {
        throw new ExternalSideEffectException("SQL update forbidden in test");
    }
}