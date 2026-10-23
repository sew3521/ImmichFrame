# Changelog

Significant changes to ImmichFrame - Artificially Enhanced
are documented here.

This is an unofficial fork of ImmichFrame.

## [Unreleased] - v2.1.0.0

### Changed

- Rebranded the fork as ImmichFrame - Artificially Enhanced.
- Updated documentation to clearly disclose AI-assisted development.

### Security

- Updated SQLitePCLRaw dependencies from 2.1.6 to 2.1.14.
- Addressed a high-severity advisory affecting the bundled SQLite
  native library.
- Retained compatibility with Entity Framework Core 8.
- Verified that the updated Web API dependency graph reports no
  known NuGet vulnerabilities.
- Validated SQLite database initialization, integrity, and restart
  behavior.

### Dependencies

- Updated ASP.NET Core, Entity Framework Core, and related
  Microsoft 8.x packages from 8.0.20 to 8.0.32.
- Updated Microsoft.Extensions.Caching.Memory from 9.0.5 to 9.0.21.
- Updated NSwag.ApiDescription.Client from 14.6.3 to 14.7.1.
- Updated AwesomeAssertions from 9.2.0 to 9.6.0.
- Updated Moq from 4.20.72 to 4.21.0.
- Updated NUnit.Analyzers from 4.10.0 to 4.15.0.
- Retained NUnit 4.4.0 after testing identified compilation
  incompatibilities with NUnit 4.6.1.
- Verified all 188 backend tests pass after these updates.
- Additional compatible dependency updates are being evaluated.

### Planned

- Optional Google Fonts support.
- Optional EXIF and camera information.
- Optional age display for tagged people.
- Improved split-view portrait pairing.
- Configurable photo timezone fallback.
- Older Chromium browser compatibility improvements.

Upstream contributors will be credited as their work is incorporated.

## [2.0.0.0]

- Multiple calendars with individual colors and names.
- Calendar lookahead configuration.
- Calendar event deduplication.
- Calendar health reporting and failure recovery.
- Adjustable Frosted Glass opacity.
- Improved administration interface and version reporting.
- Expanded automated test coverage.
