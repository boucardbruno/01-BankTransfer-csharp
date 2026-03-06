using BankTransfer;
using BankTransfer.External;
using NFluent;
using NSubstitute;
using NUnit.Framework;

namespace TransferEngineTest;

public class MegaTransferEngineTest
{
    private IProvideHttpClient _httpRiskClient = BuildFakeHttpClient();
    private IProvideSqlDatabase _sqlDatabase = BuildFakeSqlDatabase();

    [SetUp]
    public void Setup()
    {
        GlobalState.Maintenance = false;
        _httpRiskClient = BuildFakeHttpClient();
        _sqlDatabase = BuildFakeSqlDatabase();
    }

    [Test]
    public void Should_not_transfer_with_channel_is_Mobile_when_bank_transfer_is_not_VIP()
    {
        Check.That(new MegaTransferEngine(_sqlDatabase, _httpRiskClient)
            .MakeTransfer(new BankTransfer
                    .BankTransfer("Bruno", "Sébastien", 500, false),
                KindOfChannel.Mobile)).IsFalse();
    }

    [Test]
    public void Should_not_transfer_with_channel_is_Web_when_bank_transfer_is_not_VIP()
    {
        Check.That(new MegaTransferEngine(_sqlDatabase, _httpRiskClient)
            .MakeTransfer(new BankTransfer
                    .BankTransfer("Bruno", "Sébastien", 500, false),
                KindOfChannel.Web)).IsFalse();
    }

    [Test]
    public void Should_not_transfer_with_channel_is_Mobile_when_bank_transfer_is_VIP()
    {
        Check.That(new MegaTransferEngine(_sqlDatabase, _httpRiskClient)
            .MakeTransfer(new BankTransfer
                    .BankTransfer("Bruno", "Sébastien", 500, true), 
                KindOfChannel.Mobile)).IsTrue();
    }

    [Test]
    public void Should_not_transfer_with_Web_channel_when_bank_transfer_is_not_VIP()
    {
        Check.That(new MegaTransferEngine(_sqlDatabase, _httpRiskClient)
            .MakeTransfer(new BankTransfer
                    .BankTransfer("Bruno", "Sébastien", 500, false),
                KindOfChannel.Web)).IsFalse();
    }

    [Test]
    public void Should_raise_exception_when_global_state_is_in_maintenance()
    {
        GlobalState.Maintenance = true;

        Check.ThatCode(() => new MegaTransferEngine(_sqlDatabase, _httpRiskClient)
            .MakeTransfer(new BankTransfer.BankTransfer("Bruno", "Sébastien", 500, true),
                KindOfChannel.Web)).Throws<IllegalStateException>();
    }

    [Test]
    public void Should_return_cache_empty_when_transfer_limit_is_exceeded()
    {
        for (var i = 1; GlobalState.TransferCount <= 100; i++)
            new MegaTransferEngine(_sqlDatabase, _httpRiskClient)
                .MakeTransfer(new BankTransfer.BankTransfer("Bruno", "Sébastien", 500, true),
                    i % 2 == 0 ? KindOfChannel.Web : KindOfChannel.Mobile);
        
        Check.That(GlobalState.Cache).HasSize(0);
    }

    [Test]
    public void Should_verify_how_many_received_when_update_balance()
    {
        var httpRiskClient = BuildFakeHttpClient();
        var sqlDatabase = BuildFakeSqlDatabase();

        Check.That(new MegaTransferEngine(sqlDatabase, httpRiskClient)
            .MakeTransfer(new BankTransfer.BankTransfer("Bruno", "Sébastien", 500, true),
                KindOfChannel.Web)).IsTrue();
        sqlDatabase.Received().UpdateBalance("Sébastien", 500);
    }

    private static IProvideSqlDatabase BuildFakeSqlDatabase()
    {
        var sqlDatabase = Substitute.For<IProvideSqlDatabase>();
        sqlDatabase.QueryBalance(Arg.Any<string>()).Returns(1000);
        return sqlDatabase;
    }

    private static IProvideHttpClient BuildFakeHttpClient()
    {
        var httpRiskClient = Substitute.For<IProvideHttpClient>();
        httpRiskClient.Risky(Arg.Any<string>(), Arg.Any<int>()).Returns(true);
        return httpRiskClient;
    }
}