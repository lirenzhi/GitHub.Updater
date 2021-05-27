/**
 * @author     cfHxqA
 * @copyright  2019-2021 (C) by cfHxqA
 *
 * @package    GitHub.Updater
 * @category   GitHub.Updater.Utilities.Http
 *
 * @license    Attribution-NonCommercial-NoDerivs 4.0 Unported <http://creativecommons.org/licenses/by-nc-nd/4.0/>
 */
namespace GitHub.Updater.Utilities.Http
{
  public interface IRestClientFactory {
    #region Public
    #region Methods    
    /// <summary>
    /// Creates the specified invalidate cookies.
    /// </summary>
    /// <param name="InvalidateCookies">if set to <c>true</c> [invalidate cookies].</param>
    /// <returns></returns>
    RestSharp.IRestClient Create(bool InvalidateCookies = false);
    #endregion

    #region Properties    
    /// <summary>
    /// Gets or sets the base URL.
    /// </summary>
    /// <value>
    /// The base URL.
    /// </value>
    string BaseUrl { get; set; }
    #endregion
    #endregion
  }
}
