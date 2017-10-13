# Exodus

## Simple database migrator for .NET Core 2.0.

- SQL file script based approach.
- Full async/await API support.
- Friendly fluent API.
- Forward-only migrations.
- Database setup functions (useful especially in development environments).

## Supported databases:

- SQL Server.
- PostgreSQL (tested on 9.5+).

## Supported frameworks:

- .NET Core 2.0.

## Usage:

### Installation

All released Exodus versions can be obtained via NuGet Package Manager.

| Package          | Description                       | Version  | Build                                                                                                                             |
| ---------------- | --------------------------------- | -------- | --------------------------------------------------------------------------------------------------------------------------------- |
| Exodus.Core      | Core components and abstractions. | 1.1.2-rc | [![Build Status](https://travis-ci.org/ninjah187/Exodus.svg?branch=release%2Fcore)](https://travis-ci.org/ninjah187/Exodus)       |
| Exodus.SqlServer | Migrator for SQL Server.          | 1.1.2-rc | [![Build Status](https://travis-ci.org/ninjah187/Exodus.svg?branch=release%2Fsql-server)](https://travis-ci.org/ninjah187/Exodus) |
| Exodus.Npgsql    | Migrator for PostgreSQL.          | 1.1.2-rc | [![Build Status](https://travis-ci.org/ninjah187/Exodus.svg?branch=release%2Fpostgresql)](https://travis-ci.org/ninjah187/Exodus) |

### Migration

Migration is a file. It should contain SQL script which executed transforms database state from one version to another.

By convention, the file name has to be in given format:

```
[version number] - [migration name].sql
```

Example correct migration file names:

```
1 - InitializingMigration.sql
1 - Initializing Migration.sql
1-InitializingMigration.sql
0001 - InitializingMigration.sql
```

Migrations can be loaded from directory or assembly. Both topics are covered later in the documentation.

Migrations are run in ascending order by version.

Informations about applied migrations are stored in database in table named "Migrations".

### Creating migrator

In order to perform migrations, we need to create Migrator object.

- SQL Server:

```cs
var connectionString =
	"Server=(localDb)\\MSSQLLocalDB;" +
	"Database=exodus.dev;" +
	"Trusted_Connection=True;";
var migrator = new SqlServerMigrator(connectionString);
```

- PostgreSQL:

```cs
var connectionString = 
	"User ID=postgres;" +
	"Password=postgres;" +
	"Host=localhost;" +
	"Port=5433;" +
	"Database=exodus.dev;" +
	"Pooling=true;";
var migrator = new NpgsqlMigrator(connectionString);
```

### Running migrations

Running migrations in default setup is as easy as:

```cs
await migrator.MigrateAsync();
```

Or in synchronous way:

```cs
migrator.Migrate();
```

### Default setup

Default setup is running migrations from directory where Exodus executable is located ("./" path).

### Running migrations from directory

Specifying directory which will be recursively searched through for migrations looks like:

```cs
await migrator
	.FromDirectory("../src/DevMigrations")
	.MigrateAsync();
```

### Running migrations from assembly

Specifying assembly which will be searched through for migrations looks like:

```cs
await migrator
	.FromAssembly("Exodus.Examples.Migrations")
	.MigrateAsync();
```

**IMPORTANT:**
In order to load migrations from assembly, the assembly must be referenced in a calling project.
Additionally, every migration file must have set properties:
```
Build Action: Embedded resource
Copy to Output Directory: Copy if newer
```
You can configure this in Visual Studio by right mouse button clicking on the file in Solution Explorer and selecting Properties from context menu.

### Logging

Logging to console is just:

```cs
await migrator
	.LogToConsole()
	.MigrateAsync();
```

Custom logging is also simple:

```cs
await migrator
	.Log(message => 
	{
		// custom logging logic here
		// for example:
		var formatted = $"Exodus > {message}";
		Console.WriteLine(formatted);
	})
	.MigrateAsync();
```

### Database setup

Note, that you need sufficient database server permissions to run these commands.

Create database if not exists:

```cs
await migrator
	.CreateDatabaseIfNotExists()
	.MigrateAsync();
```

Always recreate database:

```cs
await migrator
	.DropCreateDatabase()
	.MigrateAsync();
```

**IMPORTANT**:
Database setup features were tested only in dev environments. Probably they are not useful in any other way.
**It is strongly not recommended to use database setup features in production.**

### Common use case

Common configuration, often used in development environments:

```cs
await migrator
	.DropCreateDatabase()
	.FromAssembly("MyProject.Migrations")
	.LogToConsole()
	.MigrateAsync();
```

More examples can be seen in source code, in examples projects.

## Roadmap:

It's not strict roadmap, rather features which are highly possible to show in the future, especially if anybody would request them explicitly.

- Support for more database engines (looking at you, MySQL).
- ASP.NET Core integration.
- CLI.
- Integration tests for database engines.