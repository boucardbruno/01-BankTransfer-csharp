namespace BankTransfer.External;

public interface IProvideSqlDatabase
{
    int QueryBalance(string account);
    void UpdateBalance(string account, int amount);
}