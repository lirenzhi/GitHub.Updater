using Newtonsoft.Json;

using PostSharp.Constraints;
using PostSharp.Patterns.Contracts;

using RestSharp;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
/**
* @author    cfHxqA
* @copyright 2019-2021 (C) by cfHxqA
*
* @package   GitHub.Updater
*
* @license   Attribution-NonCommercial-NoDerivs 4.0 Unported <http://creativecommons.org/licenses/by-nc-nd/4.0/>
*/
namespace GitHub.Updater
{
  [OnSingletonContstraint]
  public sealed class Client {
    #region Internal
    #region Properties
    /// <summary>
    /// Contains object instance of <see cref="ImmutableList{T}"/>.
    /// </summary>
    private ImmutableList<Dependency.RepositoryDependency> Dependencies { get; set; } = ImmutableList.Create<Dependency.RepositoryDependency>();

    /// <summary>
    /// Contains object instance of <see cref="Lazy{T}"/>.
    /// </summary>
    private static readonly Lazy<Client> Instance = new(() => new(), true);
    #endregion
    #endregion

    #region Public
    #region Methods
    /// <summary>
    /// Adds the dependency.
    /// </summary>
    /// <param name="RepositoryDependency">The repository dependency.</param>
    /// <returns><see cref="UpdateSessionViewModel"/>.</returns>
    /// <exception cref="AggregateException">
    /// The dependency '{RepositoryDependency.DependencyFile}' does already exists in list!
    /// </exception>
    public void AddDependency(Dependency.RepositoryDependency RepositoryDependency) {
      if (this.Dependencies.Any(Entity =>
        Entity.RepositoryBranch == RepositoryDependency.RepositoryBranch &&
        Entity.RepositoryName == RepositoryDependency.RepositoryName &&
        Entity.RepositoryUsername == RepositoryDependency.RepositoryUsername &&
        Entity.DependencyFile == RepositoryDependency.DependencyFile
      )) throw new AggregateException($"The dependency '{RepositoryDependency.DependencyFile}' does already exists in list!");
      else {
        this.Dependencies = this.Dependencies.Add(RepositoryDependency);
      } // end statement
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    public Client() {
      // apply variables
      Utilities.Http.RestClientFactory RestFactory = new() { BaseUrl = "https://raw.githubusercontent.com/" };
      this.Session = RestFactory.Create(true);

      // apply settings
      this.Session.ReadWriteTimeout = 30;
      this.Session.UserAgent = $"GitHub.Updater/Build-{typeof(Client).Assembly.GetName().Version}";
    }

    /// <summary>
    /// Gets the dependencies.
    /// </summary>
    /// <returns><see cref="IEnumerable{T}"/>.</returns>
    public IEnumerable<Dependency.RepositoryDependency> GetDependencies() => this.Dependencies;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <returns><see cref="Client"/>.</returns>
    [return: Required]
    public static Client GetInstance()
      => Instance.Value;

    /// <summary>
    /// Updates the specified maximum parallelism.
    /// </summary>
    /// <param name="MaxParallelism">The maximum parallelism.</param>
    /// <returns><see cref="Task{TResult}"/>.</returns>
    public async Task<bool> Update(int MaxParallelism = 2) {
      bool Result = this.Dependencies.Count > 0;

      ImmutableList<Dependency.RepositoryDependency> Entities = this.Dependencies.GetRange(0, this.Dependencies.Count);
      foreach (Dependency.RepositoryDependency Entity in Entities) {
        if (!await Entity.UpdateDependency(MaxParallelism)) {
          Result = false;
          break;
        } // end statement
      } // end foreach-loop

      return Result;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Contains object instance of <see cref="Utilities.Http.RestClientFactory"/>.
    /// </summary>
    public readonly IRestClient Session;
    #endregion
    #endregion
  }
}
