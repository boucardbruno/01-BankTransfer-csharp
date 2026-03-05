namespace BankTransfer;

public interface IProvideHttpClient
{
    bool Risky(string account, int amount);
}