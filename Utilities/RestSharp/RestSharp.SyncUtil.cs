using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
/**
 * @author    cfHxqA
 * @copyright 2013 - 2021 (C) by cfHxqA
 *
 * @package   RestSharp
 * @category  GitHub.Updater.Utilities
 *
 * @license   Attribution-NonCommercial-NoDerivs 4.0 Unported <http://creativecommons.org/licenses/by-nc-nd/4.0/>
 */
namespace RestSharp
{
  /// <summary>
  /// Contains RestClient.Sync-related functions.
  /// </summary>
  public static class RestClientSyncUtil {
    #region Public
    #region Methods
    /// <summary>
    /// download data as an asynchronous operation.
    /// </summary>
    /// <param name="Client">The client.</param>
    /// <param name="Request">The request.</param>
    /// <param name="Callback">The callback.</param>
    /// <param name="Token">The token.</param>
    /// <returns><see cref="System.Threading.Tasks.Task{TResult}"/>.</returns>
    public static async Task<IRestResponse> DownloadDataAsync(this IRestClient Client, IRestRequest Request, Action<long, long> Callback, CancellationToken Token, int BufferSize = 1024) {
      using MemoryStream ResultStream = new();

      Request.AdvancedResponseWriter = (ResponseStream, Response) => {
        WebRequest Request = WebRequest.Create(Response.ResponseUri);
        Request.Method = "HEAD";

        using WebResponse resp = Request.GetResponse();
        if (long.TryParse(resp.Headers.Get("Content-Length"), out long ContentLength)) {
          Response.ContentLength = ContentLength;
        } // end statement

        using (ResponseStream) {
          long BytesReadTotal = 0;
          byte[] Buffer = new byte[BufferSize];

          void ReadBytes() {
            int BytesRead = ResponseStream.Read(Buffer);
            ResultStream.Write(Buffer, 0, BytesRead);

            BytesReadTotal += BytesRead;

            if (BytesRead > 0) {
              Callback?.Invoke(BytesReadTotal, Response.ContentLength);
              ReadBytes();
            } // end statement
          } // end method

          ReadBytes();
        } // end using
      };

      IRestResponse Response = await Client.ExecuteGetAsync(Request, Token);
      Response.RawBytes = ResultStream.ToArray();

      return Response;
    }
    #endregion
    #endregion
  }
}