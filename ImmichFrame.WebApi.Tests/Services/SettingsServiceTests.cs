using ImmichFrame.Core.Interfaces;
using ImmichFrame.WebApi.Database;
using ImmichFrame.WebApi.Helpers.Config;
using ImmichFrame.WebApi.Models;
using ImmichFrame.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace ImmichFrame.WebApi.Tests.Services
{
    [TestFixture]
    public class SettingsServiceTests
    {
        private string _configDir;
        private ILoggerFactory _loggerFactory;

        [SetUp]
        public void Setup()
        {
            _configDir = Path.Combine(Path.GetTempPath(), "immichframe-tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_configDir);
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        }

        [TearDown]
        public void TearDown()
        {
            _loggerFactory.Dispose();
            if (Directory.Exists(_configDir))
            {
                Directory.Delete(_configDir, true);
            }
        }

        private SettingsService CreateService()
        {
            var options = new DbContextOptionsBuilder<SettingsDbContext>()
                .UseSqlite($"Data Source={Path.Combine(_configDir, "immichframe.db")}")
                .Options;
            return new SettingsService(
                new TestDbContextFactory(options),
                new ConfigLoader(_loggerFactory.CreateLogger<ConfigLoader>()),
                _loggerFactory.CreateLogger<SettingsService>(),
                new SettingsServiceOptions(_configDir));
        }

        private void WriteSettingsJson(string json)
        {
            File.WriteAllText(Path.Combine(_configDir, "Settings.json"), json);
        }

        private const string ValidV2Json = """
            {
                "General": { "Interval": 10, "AuthenticationSecret": "secret" },
                "Accounts": [ { "ImmichServerUrl": "http://immich.local", "ApiKey": "key123" } ]
            }
            """;

        [Test]
        public async Task InitializeAsync_ImportsFileConfigOnFirstRun()
        {
            WriteSettingsJson(ValidV2Json);

            var service = CreateService();
            await service.InitializeAsync();

            Assert.Multiple(() =>
            {
                Assert.That(service.Current.GeneralSettings.Interval, Is.EqualTo(10));
                Assert.That(service.Current.Accounts.Single().ApiKey, Is.EqualTo("key123"));
                Assert.That(File.Exists(Path.Combine(_configDir, "immichframe.db")), Is.True);
            });
        }

        [Test]
        public async Task InitializeAsync_DbWinsOverChangedFile()
        {
            WriteSettingsJson(ValidV2Json);
            var first = CreateService();
            await first.InitializeAsync();

            // Change the file afterwards — the DB was already populated and must win
            WriteSettingsJson(ValidV2Json.Replace("\"Interval\": 10", "\"Interval\": 99"));

            var second = CreateService();
            await second.InitializeAsync();

            Assert.That(second.Current.GeneralSettings.Interval, Is.EqualTo(10));
        }

        [Test]
        public async Task InitializeAsync_NoConfig_StartsWithDefaultsAndDoesNotPersist()
        {
            var service = CreateService();
            await service.InitializeAsync();

            Assert.Multiple(() =>
            {
                Assert.That(service.Current.Accounts, Is.Empty);
                Assert.That(service.Current.GeneralSettings.Interval, Is.EqualTo(new GeneralSettings().Interval));
            });

            var options = new DbContextOptionsBuilder<SettingsDbContext>()
                .UseSqlite($"Data Source={Path.Combine(_configDir, "immichframe.db")}")
                .Options;
            await using var db = new SettingsDbContext(options);
            Assert.That(await db.SettingsDocuments.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task InitializeAsync_ApiKeyFileStaysRawButResolvesInCurrent()
        {
            var keyFile = Path.Combine(_configDir, "apikey.txt");
            await File.WriteAllTextAsync(keyFile, "file-key\n");
            WriteSettingsJson($$"""
                {
                    "General": { "Interval": 10 },
                    "Accounts": [ { "ImmichServerUrl": "http://immich.local", "ApiKeyFile": {{System.Text.Json.JsonSerializer.Serialize(keyFile)}} } ]
                }
                """);

            var service = CreateService();
            await service.InitializeAsync();

            Assert.Multiple(() =>
            {
                // Runtime settings have the resolved key
                Assert.That(service.Current.Accounts.Single().ApiKey, Is.EqualTo("file-key"));
                // Raw settings keep ApiKeyFile unresolved and ApiKey empty (round-trip safe)
                var rawAccount = service.GetRawSettings().AccountsImpl.Single();
                Assert.That(rawAccount.ApiKeyFile, Is.EqualTo(keyFile));
                Assert.That(rawAccount.ApiKey, Is.Empty);
            });

            // A second load from the DB must not trip the "both ApiKey and ApiKeyFile" validation
            var second = CreateService();
            await second.InitializeAsync();
            Assert.That(second.Current.Accounts.Single().ApiKey, Is.EqualTo("file-key"));
        }

        [Test]
        public async Task UpdateAsync_PersistsAndRaisesEventWithFlags()
        {
            var service = CreateService();
            await service.InitializeAsync();

            SettingsChangedEventArgs? received = null;
            service.SettingsChanged += (_, args) => received = args;

            var settings = service.GetRawSettings();
            settings.AccountsImpl = new List<ServerAccountSettings>
            {
                new() { ImmichServerUrl = "http://immich.local", ApiKey = "key123" }
            };
            await service.UpdateAsync(settings);

            Assert.Multiple(() =>
            {
                Assert.That(received, Is.Not.Null);
                Assert.That(received!.AccountsChanged, Is.True);
                Assert.That(service.Current.Accounts.Count(), Is.EqualTo(1));
            });

            // Survives a restart
            var second = CreateService();
            await second.InitializeAsync();
            Assert.That(second.Current.Accounts.Single().ApiKey, Is.EqualTo("key123"));
        }

        [Test]
        public async Task UpdateAsync_GeneralOnlyChange_DoesNotFlagAccounts()
        {
            WriteSettingsJson(ValidV2Json);
            var service = CreateService();
            await service.InitializeAsync();

            SettingsChangedEventArgs? received = null;
            service.SettingsChanged += (_, args) => received = args;

            var settings = service.GetRawSettings();
            settings.GeneralSettingsImpl!.Interval = 60;
            await service.UpdateAsync(settings);

            Assert.Multiple(() =>
            {
                Assert.That(received!.AccountsChanged, Is.False);
                Assert.That(received.GeneralChanged, Is.True);
                Assert.That(service.Current.GeneralSettings.Interval, Is.EqualTo(60));
            });
        }

        [Test]
        public async Task UpdateAsync_InvalidSettings_ThrowsAndKeepsCurrent()
        {
            WriteSettingsJson(ValidV2Json);
            var service = CreateService();
            await service.InitializeAsync();

            var settings = service.GetRawSettings();
            settings.AccountsImpl = new List<ServerAccountSettings>
            {
                new() { ImmichServerUrl = "http://other.local" } // no ApiKey/ApiKeyFile
            };

            Assert.ThrowsAsync<ImmichFrame.Core.Exceptions.SettingsNotValidException>(() => service.UpdateAsync(settings));
            Assert.That(service.Current.Accounts.Single().ApiKey, Is.EqualTo("key123"));
        }

        [Test]
        public async Task TryClaimSetupAsync_OnlyFirstConcurrentClaimWins()
        {
            var service = CreateService();
            await service.InitializeAsync();

            // Mirrors AdminAuthService.SetupRequired: no password anywhere yet.
            bool SetupRequired() => string.IsNullOrWhiteSpace(service.Current.GeneralSettings.AdminPassword)
                && service.IsUnconfigured;

            var results = await Task.WhenAll(
                Enumerable.Range(0, 8).Select(i => Task.Run(() => service.TryClaimSetupAsync($"password-{i}", SetupRequired))));

            Assert.Multiple(() =>
            {
                Assert.That(results.Count(r => r == SetupResult.Claimed), Is.EqualTo(1));
                Assert.That(results.Count(r => r == SetupResult.AlreadyClaimed), Is.EqualTo(7));
                Assert.That(service.Current.GeneralSettings.AdminPassword, Does.StartWith("password-"));
                Assert.That(service.IsUnconfigured, Is.False);
            });
        }


        [Test]
        public async Task CalendarMigration_PreservesIdsAcrossRestart()
        {
            WriteSettingsJson("""
                {
                    "General": {
                        "Webcalendars": [
                            "https://example.com/personal.ics",
                            "https://example.com/family.ics"
                        ]
                    },
                    "Accounts": []
                }
                """);

            var first = CreateService();
            await first.InitializeAsync();

            var firstCalendars = first.Current.GeneralSettings.Calendars!;

            Assert.That(firstCalendars, Has.Count.EqualTo(2));

            var firstIds = firstCalendars.Select(x => x.Id).ToArray();

            var second = CreateService();
            await second.InitializeAsync();

            var secondCalendars = second.Current.GeneralSettings.Calendars!;

            Assert.Multiple(() =>
            {
                Assert.That(secondCalendars, Has.Count.EqualTo(2));
                Assert.That(
                    secondCalendars.Select(x => x.Id),
                    Is.EqualTo(firstIds));
                Assert.That(
                    secondCalendars.Select(x => x.Url),
                    Is.EqualTo(new[]
                    {
                        "https://example.com/personal.ics",
                        "https://example.com/family.ics"
                    }));
            });
        }

        [Test]
        public async Task CalendarMigration_EmptyListDoesNotRestoreLegacyUrls()
        {
            WriteSettingsJson("""
                {
                    "General": {
                        "Webcalendars": [
                            "https://example.com/old.ics"
                        ]
                    },
                    "Accounts": []
                }
                """);

            var service = CreateService();
            await service.InitializeAsync();

            var settings = service.GetRawSettings();
            settings.GeneralSettingsImpl!.Calendars =
                new List<ImmichFrame.Core.Models.CalendarSource>();

            await service.UpdateAsync(settings);

            var restarted = CreateService();
            await restarted.InitializeAsync();

            Assert.That(
                restarted.Current.GeneralSettings.Calendars,
                Is.Empty);
        }


        [Test]
        public async Task CalendarMigration_ExistingDatabaseMigratesOnce()
        {
            // Create the database using the normal initialization path.
            var initial = CreateService();
            await initial.InitializeAsync();

            var settings = initial.GetRawSettings();
            settings.GeneralSettingsImpl!.Webcalendars = new List<string>
            {
                "https://example.com/personal.ics",
                "https://example.com/family.ics"
            };

            await initial.UpdateAsync(settings);

            // Simulate an existing v1.0.39.2 settings document.
            var options = new DbContextOptionsBuilder<SettingsDbContext>()
                .UseSqlite($"Data Source={Path.Combine(_configDir, "immichframe.db")}")
                .Options;

            await using (var db = new SettingsDbContext(options))
            {
                var row = await db.SettingsDocuments.FindAsync(1);

                Assert.That(row, Is.Not.Null);

                using var document = System.Text.Json.JsonDocument.Parse(row!.Json);

                var root = System.Text.Json.Nodes.JsonNode.Parse(row.Json)!;
                var general = root["General"]!.AsObject();

                general.Remove("Calendars");

                row.Json = root.ToJsonString();
                row.Version++;

                await db.SaveChangesAsync();
            }

            // First restart should migrate legacy calendar URLs.
            var migrated = CreateService();
            await migrated.InitializeAsync();

            var calendars = migrated.Current.GeneralSettings.Calendars!;

            Assert.That(calendars, Has.Count.EqualTo(2));

            var ids = calendars.Select(x => x.Id).ToArray();

            Assert.That(ids.All(x => x != Guid.Empty), Is.True);

            // Second restart must preserve the same IDs.
            var restarted = CreateService();
            await restarted.InitializeAsync();

            Assert.That(
                restarted.Current.GeneralSettings.Calendars!
                    .Select(x => x.Id),
                Is.EqualTo(ids));
        }


        [Test]
        public async Task StructuredCalendars_SaveReloadEditAndDelete()
        {
            var service = CreateService();
            await service.InitializeAsync();

            var firstId = Guid.NewGuid();
            var secondId = Guid.NewGuid();

            var settings = service.GetRawSettings();
            settings.GeneralSettingsImpl!.Calendars =
                new List<ImmichFrame.Core.Models.CalendarSource>
                {
                    new()
                    {
                        Id = firstId,
                        Name = "Family",
                        Url = "https://example.com/family.ics",
                        Color = "#E69B69",
                        Enabled = true
                    },
                    new()
                    {
                        Id = secondId,
                        Name = "Personal",
                        Url = "https://example.com/personal.ics",
                        Color = "#6299EC",
                        Enabled = false
                    }
                };

            await service.UpdateAsync(settings);

            var restarted = CreateService();
            await restarted.InitializeAsync();

            var calendars = restarted.Current.GeneralSettings.Calendars!;

            Assert.Multiple(() =>
            {
                Assert.That(calendars, Has.Count.EqualTo(2));
                Assert.That(calendars[0].Id, Is.EqualTo(firstId));
                Assert.That(calendars[0].Name, Is.EqualTo("Family"));
                Assert.That(calendars[0].Color, Is.EqualTo("#E69B69"));
                Assert.That(calendars[1].Id, Is.EqualTo(secondId));
                Assert.That(calendars[1].Enabled, Is.False);
            });

            var edited = restarted.GetRawSettings();
            edited.GeneralSettingsImpl!.Calendars![0].Name = "Family Updated";
            edited.GeneralSettingsImpl.Calendars[0].Color = "#A88BDC";
            edited.GeneralSettingsImpl.Calendars.RemoveAt(1);

            await restarted.UpdateAsync(edited);

            var final = CreateService();
            await final.InitializeAsync();

            var remaining = final.Current.GeneralSettings.Calendars!;

            Assert.Multiple(() =>
            {
                Assert.That(remaining, Has.Count.EqualTo(1));
                Assert.That(remaining[0].Id, Is.EqualTo(firstId));
                Assert.That(remaining[0].Name, Is.EqualTo("Family Updated"));
                Assert.That(remaining[0].Color, Is.EqualTo("#A88BDC"));
            });

            var cleared = final.GetRawSettings();
            cleared.GeneralSettingsImpl!.Calendars!.Clear();

            await final.UpdateAsync(cleared);

            var empty = CreateService();
            await empty.InitializeAsync();

            Assert.That(empty.Current.GeneralSettings.Calendars, Is.Empty);
        }

        private class TestDbContextFactory(DbContextOptions<SettingsDbContext> _options) : IDbContextFactory<SettingsDbContext>
        {
            public SettingsDbContext CreateDbContext() => new(_options);
        }
    }
}
