using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
/**
 * @author     cfHxqA
 * @copyright  2013 - 2021 (C) by cfHxqA
 *
 * @package    System
 * @category   GitHub.Updater.Utilities
 *
 * @license    Attribution-NonCommercial-NoDerivs 4.0 Unported <http://creativecommons.org/licenses/by-nc-nd/4.0/>
 */
namespace System
{
  public static class StringUtil {
    #region Public
    #region Methods    
    /// <summary>
    /// Shas the checksum.
    /// </summary>
    /// <param name="Input">The input.</param>
    /// <returns><see cref="string"/>.</returns>
    public static string SHAChecksum(this string Input) {
      if (string.IsNullOrWhiteSpace(Input)) return string.Empty;
      using SHA512 SHA = new SHA512Managed();

      return Convert.ToBase64String(SHA.ComputeHash(Encoding.UTF8.GetBytes(Input)));
    }

    /// <summary>
    /// Returns sha1 hash sum of given filename.
    /// </summary>
    /// <param name="Input"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string SHAChecksumFilename(this string Input) {
      if (!File.Exists(Input)) return null;

      using SHA512Managed SHA = new();
      using FileStream Stream = File.OpenRead(Input);

      return string.Join("", SHA.ComputeHash(Stream).Select(Row => Row.ToString("x2")));
    }
    #endregion
    #endregion
  }
}
