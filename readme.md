# NebriaChip.Ip.Win

[![NugetShield]][NugetPackage]

[NugetPackage]: https://www.nuget.org/packages/NebriaChip.Ip.Win
[NugetShield]: https://img.shields.io/nuget/v/NebriaChip.Ip.Win

This library is a simple wrapper around the Windows API for manipulating temporary IP addresses.  
The IP addresses you add are temporary and will be discarded when the system is rebooted or when the network interface is reset.  
It is intended for scenarios where a temporary IP address is needed, such as testing.

For more details, see [CreateUnicastIpAddressEntry](https://learn.microsoft.com/windows/win32/api/netioapi/nf-netioapi-createunicastipaddressentry).

## Examples

Add an IP address and wait for DAD (Duplicate Address Detection) to complete.

```csharp
using NebriaChip.Ip.Win;

var adapter = "Local Area Connection";
var address = new IPAddress([192, 168, 100, 10]);
var prefix = 24;
var addrOperator = new WinIpAddrOperator(new() { ThrowOnError = true });

addrOperator.CreateUnicastAddress(adapter, address, prefix);

using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
var state = await addrOperator.WaitDadStateAsync(nwif, address, timeout.Token);
Console.WriteLine($"DAD State: {state}");
```

Polling-based DAD completion wait.

```csharp
var state = await addrOperator.PollingDadStateAsync(nwif, address, timeout.Token);
```

Add an IP address with optimistic DAD (initial state set to Preferred).

```csharp
await addrOperator.CreateUnicastAddress(adapter, address, prefix, optimistic: true);
```
