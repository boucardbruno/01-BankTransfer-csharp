namespace BankTransfer;

public class HttpRiskClient
{
    public bool Risky(string acc, int amount)
    {
        throw new ExternalSideEffectException("HTTP risk service forbidden in test");
    }
}