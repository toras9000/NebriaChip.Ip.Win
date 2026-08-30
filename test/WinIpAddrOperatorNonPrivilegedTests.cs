using System.Net;
using System.Net.NetworkInformation;
using AwesomeAssertions;
using Lestaly;
using Lestaly.Cx;

namespace NebriaChip.Ip.Win.Tests;

[TestClass]
public sealed class WinIpAddrOperatorNonPrivilegedTests
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
        if (Privileged) Assert.Inconclusive("UAC昇格していない状態が必要なテスト");
    }

    [TestMethod]
    public async Task CreateUnicastAddress()
    {
        var nwif = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nw => nw.Supports(NetworkInterfaceComponent.IPv4) || nw.Supports(NetworkInterfaceComponent.IPv6))
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .First();
        var addrBytes = Enumerable.Range(0, 4).Select(_ => (byte)Random.Shared.Next(1, 255)).ToArray();
        var address = new IPAddress(addrBytes);
        var prefix = 24;
        var addrOperator = new WinIpAddrOperator();
        var result = addrOperator.CreateUnicastAddress(nwif, address, prefix);
        result.Should().Be(WinIpError.AccessDenied);
    }

    [TestMethod]
    public async Task DeleteUnicastAddress()
    {
        var nwif = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nw => nw.Supports(NetworkInterfaceComponent.IPv4) || nw.Supports(NetworkInterfaceComponent.IPv6))
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .First();
        var addrBytes = Enumerable.Range(0, 4).Select(_ => (byte)Random.Shared.Next(1, 255)).ToArray();
        var address = new IPAddress([101, 102, 103, 104]);
        var prefix = 24;
        var addrOperator = new WinIpAddrOperator();
        var result = addrOperator.DeleteUnicastAddress(nwif, address, prefix);
        result.Should().Be(WinIpError.AccessDenied);
    }

    [TestMethod]
    public async Task GetUnicastDadState()
    {
        var nwif = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nw => nw.Supports(NetworkInterfaceComponent.IPv4) || nw.Supports(NetworkInterfaceComponent.IPv6))
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .First();
        var address = nwif.GetIPProperties().UnicastAddresses.First().Address;
        var addrOperator = new WinIpAddrOperator();
        var result = addrOperator.GetUnicastDadState(nwif, address);
        result.Should().Be(true);
    }

    [TestMethod]
    public async Task WaitDadStateAsync()
    {
        var nwif = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nw => nw.Supports(NetworkInterfaceComponent.IPv4) || nw.Supports(NetworkInterfaceComponent.IPv6))
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .First();
        var address = nwif.GetIPProperties().UnicastAddresses.First().Address;
        var addrOperator = new WinIpAddrOperator();
        var result = await addrOperator.WaitDadStateAsync(nwif, address);
        result.Should().Be(true);
    }

    [TestMethod]
    public async Task PollingDadStateAsync()
    {
        var nwif = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nw => nw.Supports(NetworkInterfaceComponent.IPv4) || nw.Supports(NetworkInterfaceComponent.IPv6))
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Where(nw => nw.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
            .First();
        var address = nwif.GetIPProperties().UnicastAddresses.First().Address;
        var addrOperator = new WinIpAddrOperator();
        var result = await addrOperator.PollingDadStateAsync(nwif, address);
        result.Should().Be(true);
    }

}
