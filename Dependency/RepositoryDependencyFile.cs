using Newtonsoft.Json;
/**
 * @author     cfHxqA
 * @copyright  2019-2021 (C) by cfHxqA
 *
 * @package    GitHub.Updater
 * @subpackage GitHub.Updater.Dependency
 *
 * @license   Attribution-NonCommercial-NoDerivs 4.0 Unported <http://creativecommons.org/licenses/by-nc-nd/4.0/>
 */
namespace GitHub.Updater.Dependency
{
  [JsonObject]
  public class RepositoryDependencyFile {
    #region Public
    #region Properties
    /// <summary>
    /// Gets or sets the sha.
    /// </summary>
    /// <value>The md5.</value>
    [JsonProperty("sha", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Populate)]
    public string SHA { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    /// <value>The URL.</value>
    [JsonProperty("url", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Url { get; set; }
    #endregion
    #endregion
  }
}
