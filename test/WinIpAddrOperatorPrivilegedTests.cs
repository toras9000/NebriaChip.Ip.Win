using System.Net;
using System.Net.NetworkInformation;
using AwesomeAssertions;
using Lestaly;

namespace NebriaChip.Ip.Win.Tests;

[TestClass]
public class WinIpAddrOperatorPrivilegedTests
{
    public static bool Privileged { get; set; }

    public TestContext TestContext { get; set; }

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        Privileged = WinUAC.IsElevated();
    }

    [TestInitialize]
    public async Task TestInit()
    {
        if (!Privileged) Assert.Inconclusive("UAC昇格が必要なテスト");
    }

    [TestMethod]
    public async Task CreateAndCheckAndDeleteAddress_WaitDad()
    {
        var nwif = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nw => nw.Supports(NetworkInterfaceComponent.IPv4) || nw.Supports(NetworkInterfaceComponent.IPv6))
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .First();
        var addrRanges = new[] { 0x7F, 0xFE, 0xFE, 0xFE, };
        var address = new IPAddress(addrRanges.Select(m => (byte)Random.Shared.Next(1, m + 1)).ToArray());
        var prefix = 24;

        var addrOperator = new WinIpAddrOperator();
        var createResult = addrOperator.CreateUnicastAddress(nwif, address, prefix);
        createResult.Should().Be(WinIpError.Success);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var dadResult = await addrOperator.WaitDadStateAsync(nwif, address, timeout.Token);

        var dadCheck = addrOperator.GetUnicastDadState(nwif, address);
        dadCheck.Should().NotBeNull();
        dadResult.Should().Be(dadCheck.Value);

        var deleteResult = addrOperator.DeleteUnicastAddress(nwif, address, prefix);
        deleteResult.Should().Be(WinIpError.Success);
    }

    [TestMethod]
    public async Task CreateAndCheckAndDeleteAddress_PollDad()
    {
        var nwif = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nw => nw.Supports(NetworkInterfaceComponent.IPv4) || nw.Supports(NetworkInterfaceComponent.IPv6))
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .First();
        var addrRanges = new[] { 0x7F, 0xFE, 0xFE, 0xFE, };
        var address = new IPAddress(addrRanges.Select(m => (byte)Random.Shared.Next(1, m + 1)).ToArray());
        var prefix = 24;

        var addrOperator = new WinIpAddrOperator();
        var createResult = addrOperator.CreateUnicastAddress(nwif, address, prefix);
        createResult.Should().Be(WinIpError.Success);

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var dadResult = await addrOperator.PollingDadStateAsync(nwif, address, timeout.Token);

        var dadCheck = addrOperator.GetUnicastDadState(nwif, address);
        dadCheck.Should().NotBeNull();
        dadResult.Should().Be(dadCheck.Value);

        var deleteResult = addrOperator.DeleteUnicastAddress(nwif, address, prefix);
        deleteResult.Should().Be(WinIpError.Success);
    }

}
