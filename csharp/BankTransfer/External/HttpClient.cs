namespace BankTransfer.External;

public class HttpClient(HttpRiskClient httpRiskClient) : IProvideHttpClient
{
    public bool Risky(string account, int amount)
    {
        return httpRiskClient.Risky(account, amount);
    }
}