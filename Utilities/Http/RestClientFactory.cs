using System.Net;

using RestSharp;
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
  public class RestClientFactory : IRestClientFactory {
    #region Internal
    #region Properties    
    /// <summary>
    /// Contains object instance of <see cref="CookieContainer"/>.
    /// </summary>
    private CookieContainer Cookies { get; set; }
    #endregion
    #endregion

    #region Public
    #region Methods        
    /// <summary>
    /// Creates the specified invalidate cookies.
    /// </summary>
    /// <param name="InvalidateCookies">if set to <c>true</c> [invalidate cookies].</param>
    /// <returns><see cref="RestClient"/></returns>
    public IRestClient Create(bool InvalidateCookies = false) {
      if (InvalidateCookies)
        this.Cookies = new();

      return new RestClient(this.BaseUrl) { CookieContainer = this.Cookies };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RestClientFactory"/> class.
    /// </summary>
    public RestClientFactory()
      => this.Cookies = new();
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the base URL.
    /// </summary>
    /// <value>
    /// The base URL.
    /// </value>
    public string BaseUrl { get; set; } = string.Empty;
    #endregion
    #endregion
  }
}
