﻿using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using joseki.db;
using joseki.db.entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using webapp.Configuration;
using webapp.Database;
using webapp.Database.Models;

using CheckSeverity = webapp.Database.Models.CheckSeverity;

namespace tests.database
{
    [TestClass]
    public class ChecksCacheTests
    {
        [TestMethod]
        public async Task GetNotExistingItemAddOneRecordToDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JosekiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new JosekiDbContext(options);
            var parser = new ConfigurationParser("config.sample.yaml");
            var checksCache = new ChecksCache(parser, context);

            var id = $"azsk.{Guid.NewGuid().ToString()}";
            var check = new Check { Id = id, Category = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString(), Severity = CheckSeverity.High };

            // Act & Assert
            context.Check.Count().Should().Be(0, "context should be empty before GetOrAddItem");
            await checksCache.GetOrAddItem(id, () => check);
            context.Check.Count().Should().Be(1, "context should have a single value after GetOrAddItem");
        }

        [TestMethod]
        public async Task GetExistingItemDoesNotAddNewRecords()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JosekiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new JosekiDbContext(options);
            var parser = new ConfigurationParser("config.sample.yaml");
            var checksCache = new ChecksCache(parser, context);

            var id = $"azsk.{Guid.NewGuid().ToString()}";
            var check = new Check { Id = id, Category = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString(), Severity = CheckSeverity.High };
            context.Check.Add(check.ToEntity());
            await context.SaveChangesAsync();

            // Act & Assert
            context.Check.Count().Should().Be(1, "context should have the only one record before GetOrAddItem");
            await checksCache.GetOrAddItem(id, () => check);
            context.Check.Count().Should().Be(1, "context should still have the only one record after GetOrAddItem");
        }

        [TestMethod]
        public async Task SeveralGetRequestsAddOnlySingleRecordToDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JosekiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new JosekiDbContext(options);
            var parser = new ConfigurationParser("config.sample.yaml");
            var checksCache = new ChecksCache(parser, context);

            var id = $"azsk.{Guid.NewGuid().ToString()}";
            var check = new Check { Id = id, Category = Guid.NewGuid().ToString(), Description = Guid.NewGuid().ToString(), Severity = CheckSeverity.High };

            // Act & Assert
            context.Check.Count().Should().Be(0, "context should be empty before the first GetOrAddItem");
            await checksCache.GetOrAddItem(id, () => check);
            await checksCache.GetOrAddItem(id, () => check);
            await checksCache.GetOrAddItem(id, () => check);
            context.Check.Count().Should().Be(1, "context should have a single value after three GetOrAddItem");
        }

        [TestMethod]
        public async Task ExpiredThresholdCausesAzskRecordUpdate()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JosekiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new JosekiDbContext(options);
            var parser = new ConfigurationParser("config.sample.yaml");
            var checksCache = new ChecksCache(parser, context);

            var id = $"azsk.{Guid.NewGuid().ToString()}";
            var now = DateTime.UtcNow;
            var expirationDate = now.AddDays(-(parser.Get().Cache.AzureCheckTtl + 1));
            var oldCheck = new CheckEntity
            {
                CheckId = id,
                Category = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Severity = joseki.db.entities.CheckSeverity.Medium,
                DateUpdated = expirationDate,
                DateCreated = expirationDate,
            };

            // this is the hack -_-
            // Use sync version, because it does not update DateUpdated & DateCreated
            context.Check.Add(oldCheck);
            context.SaveChanges();

            var newCheck = new Check
            {
                Id = id,
                Category = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Remediation = Guid.NewGuid().ToString(),
                Severity = CheckSeverity.High,
            };

            // Act & Assert
            context.Check.Count().Should().Be(1, "context should have the only one record before GetOrAddItem");
            await checksCache.GetOrAddItem(id, () => newCheck);
            var actualEntity = await context.Check.FirstAsync(i => i.CheckId == id);
            actualEntity.Category.Should().Be(newCheck.Category);
            actualEntity.Description.Should().Be(newCheck.Description);
            actualEntity.Remediation.Should().Be(newCheck.Remediation);
            actualEntity.Severity.Should().Be(joseki.db.entities.CheckSeverity.High);
            actualEntity.DateUpdated.Should().BeOnOrAfter(now);
        }

        [TestMethod]
        public async Task ExpiredThresholdCausesPolarisRecordUpdate()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JosekiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new JosekiDbContext(options);
            var parser = new ConfigurationParser("config.sample.yaml");
            var checksCache = new ChecksCache(parser, context);

            var id = $"polaris.{Guid.NewGuid().ToString()}";
            var now = DateTime.UtcNow;
            var expirationDate = now.AddDays(-(parser.Get().Cache.PolarisCheckTtl + 1));
            var oldCheck = new CheckEntity
            {
                CheckId = id,
                Category = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Severity = joseki.db.entities.CheckSeverity.Medium,
                DateUpdated = expirationDate,
                DateCreated = expirationDate,
            };

            // this is the hack -_-
            // Use sync version, because it does not update DateUpdated & DateCreated
            context.Check.Add(oldCheck);
            context.SaveChanges();

            var newCheck = new Check
            {
                Id = id,
                Category = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Remediation = Guid.NewGuid().ToString(),
                Severity = CheckSeverity.High,
            };

            // Act & Assert
            context.Check.Count().Should().Be(1, "context should have the only one record before GetOrAddItem");
            await checksCache.GetOrAddItem(id, () => newCheck);
            var actualEntity = await context.Check.FirstAsync(i => i.CheckId == id);
            actualEntity.Category.Should().Be(newCheck.Category);
            actualEntity.Description.Should().Be(newCheck.Description);
            actualEntity.Remediation.Should().Be(newCheck.Remediation);
            actualEntity.Severity.Should().Be(joseki.db.entities.CheckSeverity.High);
            actualEntity.DateUpdated.Should().BeOnOrAfter(now);
        }

        [TestMethod]
        public async Task GetImageScanCheckInsertsPredefinedImageScanEntity()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<JosekiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new JosekiDbContext(options);
            var parser = new ConfigurationParser("config.sample.yaml");
            var checksCache = new ChecksCache(parser, context);

            var checkEntity = ChecksCache.ImageScanCheck.ToEntity();

            // Act & Assert
            context.Check.Count().Should().Be(0, "context should be empty before the first GetOrAddItem");
            await checksCache.GetImageScanCheck();
            context.Check.Count().Should().Be(1, "context should have a single value after three GetOrAddItem");

            var actualEntity = await context.Check.FirstAsync(i => i.CheckId == ChecksCache.ImageScanCheck.Id);
            actualEntity.Category.Should().Be(checkEntity.Category);
            actualEntity.Description.Should().Be(checkEntity.Description);
            actualEntity.Remediation.Should().Be(checkEntity.Remediation);
            actualEntity.Severity.Should().Be(checkEntity.Severity);
        }
    }
}