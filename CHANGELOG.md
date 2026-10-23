# Changelog

Significant changes to ImmichFrame - Artificially Enhanced
are documented here.

This is an unofficial fork of ImmichFrame.

## [Unreleased] - v2.1.0.0

### Added

- Configurable split-view portrait pairing lookahead.
  - Adjustable from 2 to 8 images through an administration slider.
  - Defaults to 4 images.
  - Only displayed when Split View is selected.
  - Preserves existing slideshow navigation history.
- Improved age display for recognized people.
  - Shows days for newborns and months for children under two.
  - Shows completed years from age two onward.
  - Handles leap years and shorter calendar months.
  - Preserves names when dates are missing or invalid.
  - Age is calculated when the photograph was taken.

- Optional Google Fonts support for slideshow text.
  - Custom font selection through the administration interface.
  - Inter is the default alternate font.
  - Original fonts remain available as fallbacks.
  - Font stylesheets load only when enabled.
- Configurable calendar description truncation.
  - Enabled by default.
  - Maximum two visible lines.
  - Default character limit of 160, configurable from 40 to 500.
  - Original calendar event data remains unchanged.
- Separate Weather and Calendar administration settings sections.
- Configurable calendar timezone.
  - Defaults to the container's configured timezone.
  - Optional IANA timezone override in Calendar administration.
  - Displays the detected container timezone in the admin UI.
  - Timezone-aware calendar date boundaries and event conversion.
  - Preserves all-day dates and floating event times.
  - Timezone-aware primary and fallback calendar caches.
  - Regression tests for UTC, Chicago, and daylight saving changes.
- Improved image orientation detection.
  - Uses EXIF dimensions and orientation when available.
  - Falls back to asset dimensions when EXIF data is incomplete.
  - Handles rotated images without double-applying orientation.

### Compatibility

- Improved compatibility with older Chromium-based browsers by
  removing unsupported CSS cascade layers during frontend builds.
- Preserved Tailwind utility styles and custom slideshow styling.
- Verified the production CSS contains no remaining `@layer`
  directives.
- Based on upstream ImmichFrame PR #720 by JW-CH:
  https://github.com/immichFrame/ImmichFrame/pull/720

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
- Verified all 247 backend tests pass under both UTC and America/Chicago.
- Updated compatible frontend dependencies within their existing
  package version constraints, including Svelte, Vite, PostCSS,
  date-fns, and development tooling.
- Verified the updated npm lockfile with a clean `npm ci` installation.
- Confirmed Svelte validation reports zero errors and warnings.
- Confirmed the frontend production build completes successfully.
- Verified zero known vulnerabilities in production npm dependencies.
- Seven low-severity findings remain in the full npm dependency audit.
  These are being evaluated separately to avoid forcing breaking
  framework upgrades.
- Retained Node.js 22 and npm 10 for build compatibility.
- Additional compatible dependency updates are being evaluated.

### Planned

- Optional EXIF and camera information.
- Configurable photo timezone fallback.
- Additional older Chromium browser compatibility improvements
  as testing identifies them.

Upstream contributors will be credited as their work is incorporated.

## [2.0.0.0]

- Multiple calendars with individual colors and names.
- Calendar lookahead configuration.
- Calendar event deduplication.
- Calendar health reporting and failure recovery.
- Adjustable Frosted Glass opacity.
- Improved administration interface and version reporting.
- Expanded automated test coverage.
