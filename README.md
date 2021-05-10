[![Build status](https://ci.appveyor.com/api/projects/status/github/bytedev/ByteDev.Azure.Cosmos.Table?branch=master&svg=true)](https://ci.appveyor.com/project/bytedev/ByteDev-Azure-Cosmos-Table/branch/master)
[![NuGet Package](https://img.shields.io/nuget/v/ByteDev.Azure.Cosmos.Table.svg)](https://www.nuget.org/packages/ByteDev.Azure.Cosmos.Table)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/ByteDev/ByteDev.Azure.Cosmos.Table/blob/master/LICENSE)

# ByteDev.Azure.Cosmos.Table

Library of Azure Cosmos Table Storage (Table API) related functionality based on the `Microsoft.Azure.Cosmos.Table` package.

## Installation

ByteDev.Azure.Cosmos.Table has been written as a .NET Standard 2.0 library, so you can consume it from a .NET Core or .NET Framework 4.6.1 (or greater) application.

ByteDev.Azure.Cosmos.Table is hosted as a package on nuget.org.  To install from the Package Manager Console in Visual Studio run:

`Install-Package ByteDev.Azure.Cosmos.Table`

Further details can be found on the [nuget page](https://www.nuget.org/packages/ByteDev.Azure.Cosmos.Table/).

## Release Notes

Releases follow semantic versioning.

Full details of the release notes can be viewed on [GitHub](https://github.com/ByteDev/ByteDev.Azure.Cosmos.Table/blob/master/docs/RELEASE-NOTES.md).

## Usage

```csharp
var createTableIfNotExists = true;

ITableRepository<MyEntity> client = new TableRepository<MyEntity>("ConnectionString", 
    "MyEntityTableName", 
    createTableIfNotExists);

var result = await client.GetAllAsync();
```

`TableRepository` (`ITableRepository`) methods:

Retrieve
- ExistsAsync
- GetAllAsync
- GetCountAsync
- GetByKeysAsync
- FindByAsync
- FindInAsync
- QueryAsync

Insert
- InsertAsync
- InsertOrReplaceAsync
- InsertOrMergeAsync

Replace
- ReplaceAsync
- ReplaceIfExistsAsync

Merge
- MergeAsync
- MergeIfExistsAsync

Delete
- DeleteAsync
- DeleteAllAsync
- DeleteIfExistsAsync
- DeleteIfOlderThanAsync
