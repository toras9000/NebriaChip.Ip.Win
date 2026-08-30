namespace NebriaChip.Ip.Win;

/// <summary>操作エラーの種別</summary>
public enum WinIpError
{
    /// <summary>成功</summary>
    Success,
    /// <summary>成功</summary>
    NoError = Success,

    /// <summary>アクセスが拒否された</summary>
    /// <remarks>これは通常、権限が不足している(管理者ではない)場合に発生します</remarks>
    AccessDenied,
    /// <summary>パラメーターが無効</summary>
    /// <remarks>名前からネットワークインタフェースを特定できない場合もこのエラーに該当します</remarks>
    InvalidParameter,
    /// <summary>対象が見つからない</summary>
    /// <remarks>ネットワークインタフェースが見つからない場合や、IPアドレスとの組み合わせが正しくない場合にはこれに該当します</remarks>
    NotFound,
    /// <summary>要求サポートされていない</summary>
    NotSupported,
    /// <summary>オブジェクトが既に存在する</summary>
    AlreadyExists,
    /// <summary>その他のエラー</summary>
    Other,
}

/// <summary>WinIpAddrOperator のエラーを示す例外</summary>
/// <param name="result">エラーの種類</param>
/// <param name="message">エラーメッセージ</param>
/// <param name="inner">内部例外</param>
public class WinIpException(WinIpError result, string? message = null, Exception? inner = null) : Exception(message, inner)
{
    /// <summary>エラーの種類</summary>
    public WinIpError Result { get; } = result;
}