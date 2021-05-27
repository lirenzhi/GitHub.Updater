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
  public class RestRequestFactory : IRestRequestFactory {
    #region Public
    #region Methods    
    /// <summary>
    /// Creates the specified URL.
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="method">The method.</param>
    /// <returns></returns>
    public IRestRequest Create(string Url, Method method)
      => new RestRequest(Url, method);
    #endregion
    #endregion
  }
}