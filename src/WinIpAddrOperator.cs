using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Networking.WinSock;
using Windows.Win32.NetworkManagement.IpHelper;
using Windows.Win32.NetworkManagement.Ndis;
using Windows.Win32.System.Diagnostics.Debug;

namespace NebriaChip.Ip.Win;

/// <summary>WinIpAddrOperator のオプション設定</summary>
/// <param name="ThrowOnError">
/// APIエラー時に例外を送出するか否か。
/// 戻り値が <see cref="WinIpError"/> のメソッドでAPI呼び出しエラーが生じた場合、
/// この設定値が false であればエラー種別を返却し、true であれば例外として送出します。
/// </param>
public record WinIpAddrOperatorOptions(bool ThrowOnError = false);

/// <summary>Windows 用のIP操作</summary>
/// <param name="options">オプション設定</param>
public class WinIpAddrOperator(WinIpAddrOperatorOptions? options = null)
{
    // 公開メソッド
    #region 追加
    /// <summary>ユニキャストIPアドレスを追加する</summary>
    /// <remarks>
    /// このメソッドで追加されるIPアドレスは一時的なものです。
    /// コンピュータの再起動やネットワーク インターフェイスのリセットなどで破棄されます。
    /// IPアドレスの追加には管理者権限が必要です。
    /// </remarks>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="addr">追加するIPアドレス</param>
    /// <param name="prefix">ネットワークプレフィックス</param>
    /// <param name="optimistic">楽観的な DAD を利用するか否か。これを true にすると、アドレスのDAD初期状態が Tentative ではなく Preferredになります。</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError CreateUnicastAddress(NetworkInterface nwif, IPAddress addr, int prefix, bool optimistic = false)
        => createUnicastAddress(adapter(nwif), new(addr, checked((byte)prefix)), optimistic);

    /// <summary>ユニキャストIPアドレスを追加する</summary>
    /// <remarks>
    /// このメソッドで追加されるIPアドレスは一時的なものです。
    /// コンピュータの再起動やネットワーク インターフェイスのリセットなどで破棄されます。
    /// IPアドレスの追加には管理者権限が必要です。
    /// </remarks>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="address">追加するIPアドレス/プレフィックス</param>
    /// <param name="optimistic">楽観的な DAD を利用するか否か。これを true にすると、アドレスのDAD初期状態が Tentative ではなく Preferredになります。</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError CreateUnicastAddress(NetworkInterface nwif, IPNetAddress address, bool optimistic = false)
        => createUnicastAddress(adapter(nwif), address, optimistic);

    /// <summary>ユニキャストIPアドレスを追加する</summary>
    /// <remarks><see cref="CreateUnicastAddress(NetworkInterface, IPAddress, int, bool)"/></remarks>
    /// <param name="alias">ネットワークインタフェース名</param>
    /// <param name="addr">追加するIPアドレス</param>
    /// <param name="prefix">ネットワークプレフィックス</param>
    /// <param name="optimistic">楽観的な DAD を利用するか否か。これを true にすると、アドレスのDAD初期状態が Tentative ではなく Preferredになります。</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError CreateUnicastAddress(string alias, IPAddress addr, int prefix, bool optimistic = false)
        => createUnicastAddress(adapter(alias), new(addr, checked((byte)prefix)), optimistic);

    /// <summary>ユニキャストIPアドレスを追加する</summary>
    /// <remarks><see cref="CreateUnicastAddress(NetworkInterface, IPAddress, int, bool)"/></remarks>
    /// <param name="alias">ネットワークインタフェース名</param>
    /// <param name="address">追加するIPアドレス/プレフィックス</param>
    /// <param name="optimistic">楽観的な DAD を利用するか否か。これを true にすると、アドレスのDAD初期状態が Tentative ではなく Preferredになります。</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError CreateUnicastAddress(string alias, IPNetAddress address, bool optimistic = false)
        => createUnicastAddress(adapter(alias), address, optimistic);
    #endregion

    #region 削除
    /// <summary>ユニキャストIPアドレスを削除する</summary>
    /// <remarks>
    /// IPアドレスの削除には管理者権限が必要です。
    /// </remarks>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="addr">削除するIPアドレス</param>
    /// <param name="prefix">ネットワークプレフィックス</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError DeleteUnicastAddress(NetworkInterface nwif, IPAddress addr, int prefix)
        => deleteUnicastAddress(adapter(nwif), new(addr, checked((byte)prefix)));

    /// <summary>ユニキャストIPアドレスを削除する</summary>
    /// <remarks>
    /// IPアドレスの削除には管理者権限が必要です。
    /// </remarks>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="address">追加するIPアドレス/プレフィックス</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError DeleteUnicastAddress(NetworkInterface nwif, IPNetAddress address)
        => deleteUnicastAddress(adapter(nwif), address);

    /// <summary>ユニキャストIPアドレスを削除する</summary>
    /// <remarks><see cref="DeleteUnicastAddress(NetworkInterface, IPAddress, int)"/></remarks>
    /// <param name="alias">ネットワークインタフェース名</param>
    /// <param name="addr">削除するIPアドレス</param>
    /// <param name="prefix">ネットワークプレフィックス</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError DeleteUnicastAddress(string alias, IPAddress addr, int prefix)
        => deleteUnicastAddress(adapter(alias), new(addr, checked((byte)prefix)));

    /// <summary>ユニキャストIPアドレスを削除する</summary>
    /// <remarks><see cref="DeleteUnicastAddress(NetworkInterface, IPAddress, int)"/></remarks>
    /// <param name="alias">ネットワークインタフェース名</param>
    /// <param name="address">追加するIPアドレス/プレフィックス</param>
    /// <returns>結果ステータス。オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が true の場合はエラー時に例外送出するため、戻り値を確認する必要はありません。</returns>
    public WinIpError DeleteUnicastAddress(string alias, IPNetAddress address)
        => deleteUnicastAddress(adapter(alias), address);
    #endregion

    #region 情報取得
    /// <summary>IPアドレスのDAD(Duplicate Address Detection)状態を取得する</summary>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <returns>
    /// DAD状態。true は有効状態、false は向こう状態、null は検出中または状態が特定できないことを示す。
    /// オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が false かつ、ネットワークインタフェースを特定できない場合は null の返却となる。
    /// </returns>
    public bool? GetUnicastDadState(NetworkInterface nwif, IPAddress addr)
        => getUnicastDadState(adapter(nwif), addr);

    /// <summary>IPアドレスのDAD(Duplicate Address Detection)状態を取得する</summary>
    /// <param name="alias">ネットワークインタフェース名</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <returns>
    /// DAD状態。true は有効状態、false は向こう状態、null は検出中または状態が特定できないことを示す。
    /// オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> が false かつ、ネットワークインタフェースを特定できない場合は null の返却となる。
    /// </returns>
    public bool? GetUnicastDadState(string alias, IPAddress addr)
        => getUnicastDadState(adapter(alias, throwOnError: true), addr);
    #endregion

    #region 状態監視
    /// <summary>DAD状態が確定するのを待機する</summary>
    /// <remarks>このメソッドでは、オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> によらず、APIエラー時に例外を送出します。</remarks>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <param name="cancelToken">キャンセルトークン</param>
    /// <returns>DAD確定値を得るタスク</returns>
    public Task<bool> WaitDadStateAsync(NetworkInterface nwif, IPAddress addr, CancellationToken cancelToken = default)
    => waitDadStableAsync(adapter(nwif, throwOnError: true), addr, cancelToken);

    /// <summary>DAD状態が確定するのを待機する</summary>
    /// <remarks>このメソッドでは、オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> によらず、APIエラー時に例外を送出します。</remarks>
    /// <param name="alias">ネットワークインタフェース名</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <param name="cancelToken">キャンセルトークン</param>
    /// <returns>DAD確定値を得るタスク</returns>
    public Task<bool> WaitDadStateAsync(string alias, IPAddress addr, CancellationToken cancelToken = default)
    => waitDadStableAsync(adapter(alias, throwOnError: true), addr, cancelToken);

    /// <summary>DAD状態が確定するまでポーリングする</summary>
    /// <remarks>このメソッドでは、オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> によらず、APIエラー時に例外を送出します。</remarks>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <param name="cancelToken">キャンセルトークン</param>
    /// <returns>DAD確定値を得るタスク</returns>
    public Task<bool> PollingDadStateAsync(NetworkInterface nwif, IPAddress addr, CancellationToken cancelToken = default)
    => pollingDadStableAsync(adapter(nwif, throwOnError: true), addr, TimeSpan.FromMilliseconds(500), cancelToken);

    /// <summary>DAD状態が確定するまでポーリングする</summary>
    /// <remarks>このメソッドでは、オプションの <see cref="WinIpAddrOperatorOptions.ThrowOnError"/> によらず、APIエラー時に例外を送出します。</remarks>
    /// <param name="alias">ネットワークインタフェース名</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <param name="cancelToken">キャンセルトークン</param>
    /// <returns>DAD確定値を得るタスク</returns>
    public Task<bool> PollingDadStateAsync(string alias, IPAddress addr, CancellationToken cancelToken = default)
    => pollingDadStableAsync(adapter(alias, throwOnError: true), addr, TimeSpan.FromMilliseconds(500), cancelToken);
    #endregion

    // 非公開型
    #region 中間表現
    /// <summary>ネットワークインタフェース中間表現型</summary>
    /// <param name="ThrowOnError">APIエラー時に例外を発行するか否か</param>
    /// <param name="Name">ネットワークインタフェース名</param>
    /// <param name="Interface">ネットワークインタフェース</param>
    private record NwAdapter(bool ThrowOnError, string? Name = default, NetworkInterface? Interface = default)
    {
        /// <summary>ネットワークインタフェースのLUIDを取得する</summary>
        /// <param name="luid">ネットワークインタフェースのLUID</param>
        /// <returns>LUID取得結果</returns>
        public WinIpError ToLuid(out NET_LUID_LH luid)
        {
            var apiResult = WIN32_ERROR.ERROR_NOT_SUPPORTED;
            if (this.Interface != null)
            {
                var nwGuid = Guid.Parse(this.Interface.Id);
                apiResult = PInvoke.ConvertInterfaceGuidToLuid(nwGuid, out luid);
            }
            else if (this.Name != null)
            {
                apiResult = PInvoke.ConvertInterfaceAliasToLuid(this.Name, out luid);
            }
            else
            {
                luid = default;
            }
            return convertApiResult(apiResult, this.ThrowOnError);
        }
    }
    #endregion

    #region 状態管理
    /// <summary>DAD完了コンテキスト情報</summary>
    /// <param name="Completion">待機完了タスクソース</param>
    /// <param name="Row">対象IPの情報取得用ROW</param>
    private record DadWaitingContext(TaskCompletionSource<bool> Completion, MIB_UNICASTIPADDRESS_ROW Row);
    #endregion

    // 非公開メソッド
    #region ユーティリティ：データ生成
    /// <summary>ネットワークインタフェース名からネットワークインタフェース中間表現データを生成する</summary>
    /// <param name="nwif">対象ネットワークインタフェース</param>
    /// <param name="throwOnError">APIエラー時に例外を発行するか否か</param>
    /// <returns>ネットワークインタフェース中間表現データ</returns>
    private NwAdapter adapter(NetworkInterface nwif, bool? throwOnError = null) => new NwAdapter(throwOnError ?? options?.ThrowOnError == true, Interface: nwif);

    /// <summary>ネットワークインタフェース名からネットワークインタフェース中間表現データを生成する</summary>
    /// <param name="name">ネットワークインタフェース名</param>
    /// <param name="throwOnError">APIエラー時に例外を発行するか否か</param>
    /// <returns>ネットワークインタフェース中間表現データ</returns>
    private NwAdapter adapter(string name, bool? throwOnError = null) => new NwAdapter(throwOnError ?? options?.ThrowOnError == true, Name: name);
    #endregion

    #region ユーティリティ：データ変換
    /// <summary>Win32 APIエラーを設定に基づいてラッパー型または例外に変換する</summary>
    /// <param name="error">Win32 APIエラー</param>
    /// <returns>変換されたエラー種別</returns>
    private WinIpError convertApiResult(WIN32_ERROR error)
        => convertApiResult(error, options?.ThrowOnError == true);

    /// <summary>Win32 APIエラーを設定に基づいてラッパー型または例外に変換する</summary>
    /// <param name="error">Win32 APIエラー</param>
    /// <param name="throwOnError">エラーを例外に変換するか否か</param>
    /// <returns>変換されたエラー種別</returns>
    private static WinIpError convertApiResult(WIN32_ERROR error, bool throwOnError)
    {
        // Win32 APIエラーをラッパー型に変換
        var result = error switch
        {
            WIN32_ERROR.NO_ERROR => WinIpError.Success,
            WIN32_ERROR.ERROR_ACCESS_DENIED => WinIpError.AccessDenied,
            WIN32_ERROR.ERROR_INVALID_PARAMETER => WinIpError.InvalidParameter,
            WIN32_ERROR.ERROR_FILE_NOT_FOUND => WinIpError.NotFound,
            WIN32_ERROR.ERROR_NOT_FOUND => WinIpError.NotFound,
            WIN32_ERROR.ERROR_NOT_SUPPORTED => WinIpError.NotSupported,
            WIN32_ERROR.ERROR_OBJECT_ALREADY_EXISTS => WinIpError.AlreadyExists,
            _ => WinIpError.Other,
        };

        // エラー且つ例外変換が有効であれば例外変換する
        if (result != WinIpError.Success && throwOnError)
        {
            // 例外送出
            var message = convertApiErrorMessage(error);
            throw new WinIpException(result, message);
        }

        return result;
    }

    /// <summary>Win32 APIエラーに対応するエラーメッセージを取得する</summary>
    /// <param name="error">Win32 APIエラー</param>
    /// <returns>エラーメッセージ</returns>
    private static string? convertApiErrorMessage(WIN32_ERROR error)
    {
        unsafe
        {
            // エラー種別からエラーメッセージを取得する
            var buffer = new char[16 * 1024];
            var length = PInvoke.FormatMessage(
                FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_OPTIONS.FORMAT_MESSAGE_IGNORE_INSERTS,
                null,
                (uint)error,
                0,
                buffer,
                (uint)buffer.Length,
                null
            );
            var message = 0 < length && length <= int.MaxValue ? buffer.AsSpan(..(int)length).ToString() : null;
            return message;
        }
    }
    #endregion

    #region IP操作
    /// <summary>ユニキャストIPアドレスを追加する</summary>
    /// <param name="adapter">ネットワークインタフェース中間表現</param>
    /// <param name="netAddr">追加するIPアドレス/プレフィックス</param>
    /// <param name="optimistic">楽観的な DAD を利用するか否か</param>
    /// <returns>結果ステータス</returns>
    private WinIpError createUnicastAddress(NwAdapter adapter, IPNetAddress netAddr, bool optimistic)
    {
        // ネットワークインタフェースのLUID取得
        var luidResult = adapter.ToLuid(out var luid);
        if (luidResult != WinIpError.Success) return luidResult;

        // IP追加用のROW構造体設定
        PInvoke.InitializeUnicastIpAddressEntry(out var addrRow);
        addrRow.InterfaceLuid = luid;
        addrRow.SetAddress(netAddr);
        if (optimistic) addrRow.DadState = NL_DAD_STATE.IpDadStatePreferred;

        // IP追加
        var apiResult = PInvoke.CreateUnicastIpAddressEntry(addrRow);
        return convertApiResult(apiResult);
    }

    /// <summary>ユニキャストIPアドレスを削除する</summary>
    /// <param name="adapter">ネットワークインタフェース中間表現</param>
    /// <param name="netAddr">追加するIPアドレス/プレフィックス</param>
    /// <returns>結果ステータス</returns>
    private WinIpError deleteUnicastAddress(NwAdapter adapter, IPNetAddress netAddr)
    {
        // ネットワークインタフェースのLUID取得
        var luidResult = adapter.ToLuid(out var luid);
        if (luidResult != WinIpError.Success) return luidResult;

        // IP削除用のROW構造体設定
        PInvoke.InitializeUnicastIpAddressEntry(out var addrRow);
        addrRow.InterfaceLuid = luid;
        addrRow.SetAddress(netAddr);

        // IP削除
        var apiResult = PInvoke.DeleteUnicastIpAddressEntry(addrRow);
        return convertApiResult(apiResult);
    }
    #endregion

    #region DAD
    /// <summary>IPアドレスのDAD状態を取得する</summary>
    /// <param name="adapter">ネットワークインタフェース中間表現</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <returns>DAD状態</returns>
    private bool? getUnicastDadState(NwAdapter adapter, IPAddress addr)
    {
        // ネットワークインタフェースのLUID取得
        var luidResult = adapter.ToLuid(out var luid);
        if (luidResult != WinIpError.Success) return null;

        // IP情報取得用のROW構造体設定
        PInvoke.InitializeUnicastIpAddressEntry(out var addrRow);
        addrRow.InterfaceLuid = luid;
        addrRow.SetAddress(addr);

        // IP情報取得
        var result = PInvoke.GetUnicastIpAddressEntry(ref addrRow);
        convertApiResult(result, throwOnError: true);

        // DAD情報を介錯
        return addrRow.DadState switch
        {
            NL_DAD_STATE.IpDadStateTentative => null,
            NL_DAD_STATE.IpDadStatePreferred => true,
            _ => false,
        };
    }

    /// <summary>DAD状態が確定するのを待機する</summary>
    /// <param name="adapter">ネットワークインタフェース中間表現</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <param name="breaker">キャンセルトークン</param>
    /// <returns>DAD確定値を得るタスク</returns>
    private async Task<bool> waitDadStableAsync(NwAdapter adapter, IPAddress addr, CancellationToken breaker)
    {
        // ネットワークインタフェースのLUID取得
        var luidResult = adapter.ToLuid(out var luid);
        if (luidResult != WinIpError.Success) throw new WinIpException(luidResult);

        // IP情報取得用のROW構造体設定
        PInvoke.InitializeUnicastIpAddressEntry(out var addrRow);
        addrRow.InterfaceLuid = luid;
        addrRow.SetAddress(addr);

        // 完了フラグ
        var completion = new TaskCompletionSource<bool>();

        // キャンセルトークンによる中止をバインド
        using var cancelHandle = breaker.Register(() => completion.SetCanceled(breaker));

        // DAD完了の待機情報
        var waiting = new DadWaitingContext(completion, addrRow);

        // 待機情報をオブジェクトにハンドルを割当
        var waitingHandle = GCHandle.Alloc(waiting);

        try
        {
            // IPアドレス変更通知の登録ハンドル (解除に必要)
            var watchHandle = HANDLE.Null;

            unsafe
            {
                // IPアドレス変更監視ハンドラ
                [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
                static void changeHandler(void* context, MIB_UNICASTIPADDRESS_ROW* addrRow, MIB_NOTIFICATION_TYPE type)
                {
                    // コンテキストデータから待機情報を取り出し
                    var waitingHandle = GCHandle.FromIntPtr((nint)context);
                    var waiting = (DadWaitingContext?)waitingHandle.Target;
                    if (waiting == null) return;

                    // ROWポインタが指定されている場合、目的のIPアドレスであるかをチェック。(なお、登録時初回呼び出しではこのポインタはNULLとなる。)
                    if (addrRow != null)
                    {
                        // 与えられたIPアドレスが監視対象でない場合はなにもしない
                        if (!addrRow->Address.Match(waiting.Row.Address)) return;
                    }

                    // IP情報を取得する。
                    // なお、渡される addrRow はAPIで情報取得を行うための最低限の情報しか含まないため、必ず GetUnicastIpAddressEntry する必要がある。
                    // であれば NULL かどうかを気にしなくて良い自前情報で取得する
                    var checkRow = waiting.Row;
                    var apiResult = PInvoke.GetUnicastIpAddressEntry(ref checkRow);
                    if (apiResult != WIN32_ERROR.NO_ERROR)
                    {
                        // エラーが生じた場合、監視をエラー完了とする
                        var result = convertApiResult(apiResult, throwOnError: false);
                        var message = convertApiErrorMessage(apiResult);
                        waiting.Completion.SetException(new WinIpException(result, message));
                        return;
                    }

                    // DAD状態が未確定である場合はなにもしない
                    if (checkRow.DadState == NL_DAD_STATE.IpDadStateTentative) return;

                    // DAD確定状態をもって待機タスクを完了する
                    var dadResult = checkRow.DadState == NL_DAD_STATE.IpDadStatePreferred;
                    waiting.Completion.SetResult(dadResult);
                }

                // IPアドレスの変更監視ハンドラを登録する
                var watchResult = PInvoke.NotifyUnicastIpAddressChange(addrRow.Address.si_family, &changeHandler, (void*)GCHandle.ToIntPtr(waitingHandle), InitialNotification: true, NotificationHandle: ref watchHandle);

                // 結果値を検証。エラーは例外に変換
                convertApiResult(watchResult, throwOnError: true);
            }

            try
            {
                // DAD完了、または中止を待機する
                var dadResult = await completion.Task;
                return dadResult;
            }
            finally
            {
                // 変更監視ハンドラを解除
                await Task.Run(() => PInvoke.CancelMibChangeNotify2(watchHandle));
            }
        }
        finally
        {
            // オブジェクトハンドルを解放
            waitingHandle.Free();
        }

    }

    /// <summary>IPアドレスのDAD状態が確定するまでポーリングで監視する</summary>
    /// <param name="adapter">ネットワークインタフェース中間表現</param>
    /// <param name="addr">対象IPアドレス</param>
    /// <param name="interval">監視インターバル</param>
    /// <param name="breaker">キャンセルトークン</param>
    /// <returns>DAD確定値を得るタスク</returns>
    private async Task<bool> pollingDadStableAsync(NwAdapter adapter, IPAddress addr, TimeSpan interval, CancellationToken breaker)
    {
        // ネットワークインタフェースのLUID取得
        var luidResult = adapter.ToLuid(out var luid);
        if (luidResult != WinIpError.Success) throw new WinIpException(luidResult);

        // IP情報取得用のROW構造体設定
        PInvoke.InitializeUnicastIpAddressEntry(out var addrRow);
        addrRow.InterfaceLuid = luid;
        addrRow.SetAddress(addr);

        // DAD状態の監視
        while (true)
        {
            // IP情報取得
            var result = PInvoke.GetUnicastIpAddressEntry(ref addrRow);
            convertApiResult(result, throwOnError: true);

            // 未確定状態であれば一定時間待機してリトライ
            if (addrRow.DadState == NL_DAD_STATE.IpDadStateTentative)
            {
                await Task.Delay(interval, breaker);
                continue;
            }

            // 確定済みであればDAD結果を返却
            return addrRow.DadState == NL_DAD_STATE.IpDadStatePreferred;
        }
    }
    #endregion
}

/// <summary>IPアドレスAPI関連拡張メソッド</summary>
file static class IPAddressExtensions
{
    /// <summary>MIB_UNICASTIPADDRESS_ROW にIPアドレスを設定する</summary>
    /// <param name="self">設定対象の MIB_UNICASTIPADDRESS_ROW</param>
    /// <param name="address">追加するIPアドレス/プレフィックス</param>
    public static void SetAddress(this ref MIB_UNICASTIPADDRESS_ROW self, IPNetAddress address)
    {
        // アドレス設定
        self.SetAddress(address.IP);

        // プレフィクス設定
        self.OnLinkPrefixLength = address.Prefix;
    }

    /// <summary>MIB_UNICASTIPADDRESS_ROW にIPアドレスを設定する</summary>
    /// <param name="self">設定対象の MIB_UNICASTIPADDRESS_ROW</param>
    /// <param name="addr">設定するIPアドレス</param>
    public static void SetAddress(this ref MIB_UNICASTIPADDRESS_ROW self, IPAddress addr)
    {
        // アドレス設定
        self.Address.SetFrom(addr);

        // アドレスファミリの判別
        if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            // スコープの設定
            if (addr.ScopeId < 0 || uint.MaxValue < addr.ScopeId) throw new WinIpException(WinIpError.InvalidParameter, "Unexpected address family");
            self.ScopeId.Value = (uint)addr.ScopeId;
        }
    }

    /// <summary>SOCKADDR_INET にIPアドレスを設定する</summary>
    /// <param name="self">設定対象の SOCKADDR_INET</param>
    /// <param name="addr">設定するIPアドレス</param>
    public static void SetFrom(this ref SOCKADDR_INET self, IPAddress addr)
    {
        // アドレスファミリの判別
        if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            // IPv6アドレスの設定
            self.si_family = ADDRESS_FAMILY.AF_INET6;
            // IPバイト列の格納
            var buffer = self.Ipv6.sin6_addr.u.Byte.AsSpan();
            addr.TryWriteBytes(buffer, out _);
        }
        else if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            // IPv4アドレスの設定
            self.si_family = ADDRESS_FAMILY.AF_INET;
            // IPバイト列の格納
            var span = MemoryMarshal.CreateSpan(ref self.Ipv4.sin_addr.S_un.S_addr, 1);
            var buffer = MemoryMarshal.AsBytes(span);
            addr.TryWriteBytes(buffer, out _);
        }
        else
        {
            throw new WinIpException(WinIpError.InvalidParameter, "Unexpected address family");
        }
    }

    /// <summary>IPアドレスが一致するかを判定する</summary>
    /// <param name="self">比較する SOCKADDR_INET</param>
    /// <param name="addr">比較する SOCKADDR_INET</param>
    /// <returns>一致するか否か</returns>
    public static bool Match(this in SOCKADDR_INET self, SOCKADDR_INET addr)
    {
        // アドレスファミリの比較
        if (self.si_family != addr.si_family) return false;

        // アドレスの比較
        if (self.si_family == ADDRESS_FAMILY.AF_INET6)
        {
            return self.Ipv6.sin6_addr.u.Byte.AsReadOnlySpan().SequenceEqual(addr.Ipv6.sin6_addr.u.Byte.AsReadOnlySpan());
        }
        else if (self.si_family == ADDRESS_FAMILY.AF_INET)
        {
            return self.Ipv4.sin_addr.S_un.S_addr == addr.Ipv4.sin_addr.S_un.S_addr;
        }
        else
        {
            return self.Equals(addr);
        }
    }
}