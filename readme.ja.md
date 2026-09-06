# NebriaChip.Ip.Win

[![NugetShield]][NugetPackage]

[NugetPackage]: https://www.nuget.org/packages/NebriaChip.Ip.Win
[NugetShield]: https://img.shields.io/nuget/v/NebriaChip.Ip.Win

このライブラリは、一時的なIPアドレスの操作を行う Windows API の単純なラッパーです。  
追加するIPアドレスは、再起動やネットワークインタフェースのリセットによって破棄される一時的なものです。  
これは、インタフェースのDHCPアドレスと共存することもできます。  
テストなどで一時的にIPアドレスが必要な場合に向けになります。  

詳しくは [CreateUnicastIpAddressEntry](https://learn.microsoft.com/windows/win32/api/netioapi/nf-netioapi-createunicastipaddressentry) を参照してください。  


## Examples

IPアドレスを追加し、DAD(Duplicate Address Detection)の完了を待機する。  

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

ポーリングでのDAD完了待機。  

```csharp
var state = await addrOperator.PollingDadStateAsync(nwif, address, timeout.Token);
```

楽観的DAD(初期状態をPreferredとする)でのIP追加。  

```csharp
await addrOperator.CreateUnicastAddress(adapter, address, prefix, optimistic: true);
```
