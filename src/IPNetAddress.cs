using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace NebriaChip.Ip.Win;

/// <summary>IPアドレスとネットワークプレフィクスをセットで扱うデータ型</summary>
/// <param name="IP">IPアドレス</param>
/// <param name="Prefix">ネットワークプレフィクス長</param>
public record IPNetAddress(IPAddress IP, byte Prefix)
{
    /// <summary>文字列から IPNetAddress インスタンスに変換する</summary>
    /// <param name="span">文字列</param>
    /// <returns>パース結果</returns>
    public static IPNetAddress Parse(ReadOnlySpan<char> span)
        => IPNetAddress.TryParse(span, out var addr) ? addr : throw new FormatException();

    /// <summary>文字列から IPNetAddress インスタンスに変換を試みる</summary>
    /// <param name="span">文字列</param>
    /// <param name="address">変換結果</param>
    /// <returns>変換成否</returns>
    public static bool TryParse(ReadOnlySpan<char> span, [NotNullWhen(true)] out IPNetAddress? address)
    {
        address = null;
        var sepaPos = span.IndexOf('/');
        if (sepaPos < 0) return false;
        if (!IPAddress.TryParse(span[..sepaPos].Trim(), out var ip)) return false;
        if (!byte.TryParse(span[(sepaPos + 1)..], out var prefix)) return false;
        address = new(ip, prefix);
        return true;
    }

    /// <summary>インスタンスを文字列化する</summary>
    /// <returns>文字列表現</returns>
    public override string ToString()
    {
        return $"{this.IP}/{this.Prefix}";
    }
}
