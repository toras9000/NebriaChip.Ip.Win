using System.Net;
using AwesomeAssertions;

namespace NebriaChip.Ip.Win.Tests;

[TestClass]
public class IPNetAddressTests
{
    [TestMethod]
    public void Parse()
    {
        IPNetAddress.Parse("192.168.1.100/16").Should().Be(new IPNetAddress(IPAddress.Parse("192.168.1.100"), 16));
        IPNetAddress.Parse("fe80::1/16").Should().Be(new IPNetAddress(IPAddress.Parse("fe80::1"), 16));
        IPNetAddress.Parse("[fe80::1]/16").Should().Be(new IPNetAddress(IPAddress.Parse("[fe80::1]"), 16));

        IPNetAddress.Parse("  192.168.1.100  /  16  ").Should().Be(new IPNetAddress(IPAddress.Parse("192.168.1.100"), 16));
        IPNetAddress.Parse("  fe80::1  /  16  ").Should().Be(new IPNetAddress(IPAddress.Parse("fe80::1"), 16));
        IPNetAddress.Parse("  [fe80::1]  /  16  ").Should().Be(new IPNetAddress(IPAddress.Parse("[fe80::1]"), 16));

        FluentActions.Invoking(() => IPNetAddress.Parse("abc")).Should().Throw<Exception>();
        FluentActions.Invoking(() => IPNetAddress.Parse("abc/10")).Should().Throw<Exception>();
        FluentActions.Invoking(() => IPNetAddress.Parse("192.168.1.100/-10")).Should().Throw<Exception>();
        FluentActions.Invoking(() => IPNetAddress.Parse("192.168.1.100/abc")).Should().Throw<Exception>();
        FluentActions.Invoking(() => IPNetAddress.Parse("fe80.1/10")).Should().Throw<Exception>();
    }

    [TestMethod]
    public void TryParse()
    {
        static void tryParseSuccess(ReadOnlySpan<char> span, IPNetAddress expect)
        {
            var result = IPNetAddress.TryParse(span, out var addr);
            result.Should().Be(true);
            addr.Should().Be(expect);
        }
        static void tryParseFailed(ReadOnlySpan<char> span)
        {
            var result = IPNetAddress.TryParse(span, out _);
            result.Should().Be(false);
        }

        tryParseSuccess("192.168.1.100/16", new(IPAddress.Parse("192.168.1.100"), 16));
        tryParseSuccess("fe80::1/16", new(IPAddress.Parse("fe80::1"), 16));
        tryParseSuccess("[fe80::1]/16", new(IPAddress.Parse("[fe80::1]"), 16));

        tryParseSuccess("  192.168.1.100  /  16  ", new(IPAddress.Parse("192.168.1.100"), 16));
        tryParseSuccess("  fe80::1  /  16  ", new(IPAddress.Parse("fe80::1"), 16));
        tryParseSuccess("  [fe80::1]  /  16  ", new(IPAddress.Parse("[fe80::1]"), 16));

        tryParseFailed("abc");
        tryParseFailed("abc/10");
        tryParseFailed("192.168.1.100/-10");
        tryParseFailed("192.168.1.100/abc");
        tryParseFailed("fe80.1/10");
    }

    [TestMethod]
    public void FormatString()
    {
        new IPNetAddress(IPAddress.Parse("192.168.1.100"), 16).ToString().Should().Be("192.168.1.100/16");
        new IPNetAddress(IPAddress.Parse("fe80::1"), 16).ToString().Should().Be("fe80::1/16");
    }
}
