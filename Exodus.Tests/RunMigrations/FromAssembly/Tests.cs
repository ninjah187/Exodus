﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Exodus.Tests.RunMigrations.FromAssembly
{
    public class Tests
    {
        [Fact]
        public async Task RunMigrationsFromAssembly_OnEmptyExistingDatabase_AllMigrationsApplied()
        {
            var database = new DatabaseMock
            {
                DatabaseExists = true
            };
            var migrator = new Migrator(database);

            await migrator
                .FromAssembly("Exodus.Tests.Migrations")
                .MigrateAsync();

            Assert.True(database.DatabaseExists);
            Assert.True(database.MigrationsTableExists);
            Assert.Equal(0, database.AppliedMigrationVersions.Length);
            Assert.Equal(2, database.AppliedMigrations.Count);
            Assert.Equal(0, database.CreateIfNotExistsCounter);
            Assert.Equal(0, database.DropIfExistsCounter);
            Assert.Equal(1, database.CreateMigrationsTableIfNotExistsCounter);
            Assert.Equal(1, database.GetAppliedMigrationVersionsCounter);
            Assert.Equal(2, database.RunMigrationCounter);

            var appliedMigration = database.AppliedMigrations[0];
            Assert.Equal(1, appliedMigration.Version);
            Assert.Equal("TestMigration 01", appliedMigration.Name);
            Assert.Equal("-- Test migration 01", appliedMigration.Script);

            appliedMigration = database.AppliedMigrations[1];
            Assert.Equal(2, appliedMigration.Version);
            Assert.Equal("TestMigration 02", appliedMigration.Name);
            Assert.Equal("-- Test migration 02", appliedMigration.Script);
        }
    }
}
