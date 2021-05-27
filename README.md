I have already tried a few update solutions myself and have never been satisfied with the result. It was either completely "overcrowded" or too time-consuming to work with for quick implementation.

That's why I'm presenting my minimalist solution here, based on a GitHub source. It is easy to use and offers a lot of flexibility.

### Features
- Catch file download currently (see `OnDependencyFileDownloadHandle`)
- Catch file download completed (see `OnDependencyFileDownloadComplete`)
- Declare single- or multiple environment for each dependency
- Support multiple repository- and repository dependencies
- Support multi-threading

### How to use it?
```csharp
// init updater
var instance = GitHub.Updater.Client.GetInstance();
    
// create dependency from given repository
var RepoDependency = new GitHub.Updater.Dependency.RepositoryDependency() { RepositoryUsername = "username", RepositoryName = "repository-name", DependencyFile = "Dependencies" };
          
// apply repository dependencies
instance.AddDependency(RepoDependency);
    
// check for update
await instance.Update();
```

### Sample
```csharp
// init updater
var instance = GitHub.Updater.Client.GetInstance();
    
// create dependency from given repository
var RepoDependency = new GitHub.Updater.Dependency.RepositoryDependency() { RepositoryUsername = "cfHxqA", RepositoryName = "GitHub.Updater", DependencyFile = "Example/Dependencies", Environment = new string[] { "Dependencies" } };
          
// apply repository dependencies
instance.AddDependency(RepoDependency);
    
// check for update
await instance.Update(MaxParallelism: 1);
```