using Newtonsoft.Json;

using PostSharp.Patterns.Contracts;

using RestSharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
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
  public record RepositoryDependency {
    #region Internal
    #region Methods
    /// <summary>
    /// Validates the repository dependency.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <returns><c>true</c> if <see cref="Dependency"/> successfully validated, <c>false</c> otherwise.</returns>
    /// <exception cref="AggregateException">Update service is currently NOT available.</exception>
    /// <exception cref="AggregateException">The file '{Value}' could not be found!</exception>
    private async Task<bool> ValidateRepositoryDependency([Required] string Value) {
      IRestRequest Request = new Utilities.Http.RestRequestFactory().Create($"{this.RepositoryUsername}/{this.RepositoryName}/{this.RepositoryBranch}/{Value}.json", Method.HEAD);
      IRestResponse Response = await this.Instance.Session.ExecuteAsync(Request, CancellationToken.None);

      if (Response?.StatusCode != HttpStatusCode.OK) {
        if (Response?.StatusCode == 0)
          throw new AggregateException("Update service is currently NOT available.");
        else 
          throw new AggregateException($"The file '{Value}' could not be found!");
      } else if (Response?.StatusCode == HttpStatusCode.OK) {
        try {
          Response = await this.Instance.Session.ExecuteGetAsync(Request, CancellationToken.None);

          this.DependencyFiles = JsonConvert.DeserializeObject<RepositoryDependencyFile[]>(Response.Content);
          return true;
        } catch (JsonSerializationException) {
          throw new AggregateException($"The file '{Value}' content are not valid scheme!");
        } // end try-catch
      } // end statement

      return false;
    }

    /// <summary>
    /// Validates the repository dependency file.
    /// </summary>
    /// <param name="DependencyFile">The dependency file.</param>
    /// <exception cref="AggregateException">The '{nameof(this.DependencyFile)}' is empty or whitespace!</exception>
    /// <exception cref="AggregateException">The file '{Filename}' checksum mismatch.</exception>
    /// <exception cref="AggregateException">The file '{DependencyFile.Url}' could not be found!</exception>
    private async Task ValidateRepositoryDependencyFile([Required] RepositoryDependencyFile DependencyFile) {
      if (string.IsNullOrEmpty(this.DependencyFile) || string.IsNullOrWhiteSpace(this.DependencyFile))
        throw new AggregateException($"The '{nameof(this.DependencyFile)}' is empty or whitespace!");

      if (this.Environment is null)
        this.Environment = new string[] { this.DependencyFile };

      ValidateEnvironment(this.Environment);

      string Filename = this.Environment.Where(Entity => DependencyFile.Url.Contains(Entity, StringComparison.CurrentCulture)).OrderByDescending(Entity => DependencyFile.Url.IndexOf(Entity)).Select(Entity => {
        int Pos = DependencyFile.Url.IndexOf(Entity);
        return Pos == 0 ? Path.GetFileName(DependencyFile.Url) : DependencyFile.Url[Pos..];
      })?.FirstOrDefault();

      if (!string.IsNullOrWhiteSpace(Path.GetDirectoryName(Filename)) && !Directory.Exists(Path.GetDirectoryName(Filename)))
        Directory.CreateDirectory(Path.GetDirectoryName(Filename));

      if (File.Exists($"{Filename}.bak"))
        File.Delete($"{Filename}.bak");

      if (!File.Exists(Filename) || File.Exists(Filename) && Filename.SHAChecksumFilename() != DependencyFile.SHA) {
        if (File.Exists(Filename))
          File.Move(Filename, $"{Filename}.bak");

        IRestRequest Request = new Utilities.Http.RestRequestFactory().Create(DependencyFile.Url, Method.GET);
        IRestResponse Response = await Client.GetInstance().Session.DownloadDataAsync(Request, (Received, Size) => this.OnDependencyFileDownload?.Invoke(Filename, Received, Size), CancellationToken.None);

        if (Response?.StatusCode == HttpStatusCode.OK) {
          this.OnDependencyFileDownloadComplete?.Invoke(Filename);
          await File.WriteAllBytesAsync(Filename, Response.RawBytes);

          if (!File.Exists(Filename) || File.Exists(Filename) && Filename.SHAChecksumFilename() != DependencyFile.SHA)
            throw new AggregateException($"The file '{Filename}' checksum mismatch.");
        } else throw new AggregateException($"The file '{DependencyFile.Url}' could not be found!");
      } // end statement
    }

    /// <summary>
    /// Validates the environment.
    /// </summary>
    /// <param name="Value">The value.</param>
    private static void ValidateEnvironment([Required] IEnumerable<string> Value) {
      foreach (string EnvironmentPath in Value.Where(Entity => !(string.IsNullOrEmpty(Entity) || string.IsNullOrWhiteSpace(Entity)) && !Directory.Exists(Entity)))
        Directory.CreateDirectory(EnvironmentPath);
    }
    #endregion

    #region Properties    
    /// <summary>
    /// Gets or sets the instance.
    /// </summary>
    /// <value>The instance.</value>
    private Client Instance { get; init; }
    #endregion
    #endregion

    #region Public
    #region Delegates        
    /// <summary>
    /// Delegate OnDependencyFileDownloadHandle.
    /// </summary>
    /// <param name="Filename">The filename.</param>
    /// <param name="Received">The received.</param>
    /// <param name="Size">The size.</param>
    public delegate void OnDependencyFileDownloadHandle(string Filename, long Received, long Size);

    /// <summary>
    /// Delegate OnDependencyFileDownloadCompleteHandle.
    /// </summary>
    /// <param name="Filename">The filename.</param>
    public delegate void OnDependencyFileDownloadCompleteHandle(string Filename);
    #endregion

    #region Methods
    /// <summary>
    /// The on dependency file download.
    /// </summary>
    public OnDependencyFileDownloadHandle OnDependencyFileDownload;

    /// <summary>
    /// The on dependency file download complete.
    /// </summary>
    public OnDependencyFileDownloadCompleteHandle OnDependencyFileDownloadComplete;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryDependency"/> class.
    /// </summary>
    public RepositoryDependency()
      => this.Instance = Client.GetInstance();

    /// <summary>
    /// Updates the dependency.
    /// </summary>
    /// <param name="MaxParallelism">The maximum parallelism.</param>
    /// <returns><see cref="Task{TResult}"/>.</returns>
    public async Task<bool> UpdateDependency(int MaxParallelism = 2) {
      if (!await this.ValidateRepositoryDependency(this.DependencyFile))
        return false;

      try {
        IEnumerable<Task> DependencyFileTasks = this.DependencyFiles.Select(Entity => Task.Run(async () => await this.ValidateRepositoryDependencyFile(Entity)));
        List<Task> ActiveDependencyFileTasks  = new(MaxParallelism);

        foreach (Task DependencyFileTask in DependencyFileTasks) {
          ActiveDependencyFileTasks.Add(DependencyFileTask);
          if (ActiveDependencyFileTasks.Count == MaxParallelism) {
            await Task.WhenAny(ActiveDependencyFileTasks.ToArray());
            _ = ActiveDependencyFileTasks.RemoveAll(t => t.IsCompleted);
          } // end statement
        } // end foreach-loop

        return await Task.WhenAll(ActiveDependencyFileTasks.ToArray()).ContinueWith(Result => Result.Exception is null);
      } catch {
        return false;
      } // end statement
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the dependency file.
    /// </summary>
    /// <value>The dependency file.</value>
    public string DependencyFile { get; set; }

    /// <summary>
    /// Gets the dependency files.
    /// </summary>
    /// <value>The dependency files.</value>
    public IEnumerable<RepositoryDependencyFile> DependencyFiles { get; private set; }

    /// <summary>
    /// Gets or sets the environment.
    /// </summary>
    /// <value>The environment.</value>
    public IEnumerable<string> Environment { get; set; }

    /// <summary>
    /// Gets or sets the repository branch.
    /// </summary>
    /// <value>The repository branch.</value>
    public string RepositoryBranch { get; set; } = "master";

    /// <summary>
    /// Gets or sets the name of the repository.
    /// </summary>
    /// <value>The name of the repository.</value>
    public string RepositoryName { get; set; }

    /// <summary>
    /// Gets or sets the repository username.
    /// </summary>
    /// <value>The repository username.</value>
    public string RepositoryUsername { get; set; }
    #endregion
    #endregion
  }
}
