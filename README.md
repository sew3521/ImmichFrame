# ImmichFrame - Artificially Enhanced

### Human ideas. Artificial assistance. Questionable time management.

First things first: **ImmichFrame is an awesome project, and I want to give full credit to the original developers.** Without their work, none of this would exist.

Original project: [ImmichFrame](https://github.com/immichFrame/ImmichFrame)

Official documentation: [immichframe.dev](https://immichframe.dev)

This is my personal fork of ImmichFrame. I started working on it because I use ImmichFrame at home and wanted to see a handful of additional features added.

What started as a few relatively simple changes turned into a much larger project than I originally anticipated. Funny how that works.

Rather than keep everything to myself, I figured I would make the modified version available in case anyone else finds these features useful.

**I am not trying to replace the original ImmichFrame project, compete with its developers, or create some entirely separate ecosystem.**

Quite the opposite. My preference would be for these improvements to eventually make their way into the original project.

If and when the upstream developers incorporate the functionality I need, I will likely sunset Artificially Enhanced and return to running the official version. Maintaining a separate fork indefinitely isn't exactly on my bucket list.

In the meantime, welcome to Artificially Enhanced.

**Why the name?** This fork was developed with substantial
assistance from ChatGPT. AI helped write code, troubleshoot
problems, develop tests, and produce documentation.

I provided the requirements, made the design decisions,
and tested the results. The application itself does not
require an AI service to operate.

I believe people should know when software was developed
with AI assistance. If that makes you uncomfortable,
I completely understand. I'd rather be upfront about it.

## What's different?

The current v2.0.0.0 release includes several enhancements to the original ImmichFrame experience.

### Calendar improvements

- Support for multiple calendars, each with its own name, color, and enabled/disabled setting.
- Calendar management through the administration interface.
- Configurable calendar lookahead.
- Automatic removal of duplicate events when the same appointment appears on multiple calendars.
- Improved handling of recurring and multi-day appointments.
- Calendar caching and temporary failure recovery.
- Individual calendar health reporting. If one calendar becomes unavailable, the others continue working normally. When no usable cached data exists, the display shows a warning instead of silently pretending everything is fine.

### Display improvements

- Frosted Glass styling to make information easier to read over photographs.
- Adjustable Frosted Glass opacity, with a default of 45%.
- Improvements to calendar event and photo information presentation.

### Administration and other improvements

- Web-based administration and configuration.
- Version information displayed in the administration interface.
- Updated startup branding and version reporting.
- Additional automated tests covering calendar behavior, configuration, and reliability.

Some features build on functionality already present upstream. This list describes the capabilities of Artificially Enhanced, not necessarily features exclusive to this fork.

## Installation

The Docker image is published through GitHub Container Registry.

Image:

`ghcr.io/sew3521/immichframe:wesche-v2.0.0.0`

Artificially Enhanced is intended for Docker-based deployments, including TrueNAS SCALE.

The application uses the existing ImmichFrame configuration structure with additional settings for the features introduced in this fork.

**Back up your configuration and database before migrating from an existing installation.**

For general installation and configuration information, refer to the [official ImmichFrame documentation](https://immichframe.dev).

## A note about AI assistance (and my apparent laziness)

I want to be upfront about something: **a substantial amount of the development work on Artificially Enhanced was done with assistance from ChatGPT.**

I've been writing BATCH files and PowerShell scripts since long before AI became everyone's favorite excuse to call themselves a software developer. I have some programming experience, understand the fundamentals, and have spent more years than I'd care to admit automating things because doing them manually seemed like a stupid waste of time.

That being said, I'm not going to pretend I'm an expert C# developer. I'm much more comfortable writing scripts and managing infrastructure than developing a full application from scratch.

That's where ChatGPT came in.

I worked with an AI assistant that goes by Ryn. Yes, the AI picked its own name. No, I don't know why I let it do that.

Ryn helped me design features, write and modify code, troubleshoot problems, create automated tests, and generally figure out why something that sounded relatively simple was suddenly consuming hours of my life.

I provided the requirements, made the design decisions, tested everything in my environment, and worked through the inevitable bugs. This wasn't a case of typing "make me an app" into ChatGPT and blindly uploading whatever came out.

**And before anyone asks, yes, I was apparently too damn lazy to even write my own README.**

You're reading something Ryn wrote for me because, after spending all this time working on the actual application, the last thing I wanted to do was write documentation explaining what the hell I had just done.

So there you go. Full disclosure. Even the explanation about how much AI I used was written by AI. At least I'm consistent.

I'm not embarrassed about using AI to help develop this. It's another tool, and a pretty useful one when you know what you're trying to accomplish.

That doesn't mean everything is perfect. AI can produce some spectacularly stupid code, and I've certainly caught my share of it during this project. We've put considerable effort into testing, but bugs are still possible.

If you find one, feel free to report it. Just remember that this is a personal fork maintained by a guy who originally wanted a few extra calendar features, not a commercial software company with an army of developers.

## Future plans

For v2.2, I'm considering configuration import/export functionality to make backups and migrations easier.

Beyond that, I don't have some grand roadmap to turn this into an entirely different application.

My goal is to maintain the features I personally use, fix problems as they come up, and hopefully see some of these improvements incorporated upstream.

## Credits and thanks

A huge thank-you to the original [ImmichFrame developers](https://github.com/immichFrame/ImmichFrame) for building and maintaining the project that made all of this possible.

Thanks also to the [Immich team](https://github.com/immich-app/immich) for creating the photo management platform behind it.

And, apparently, thanks to Ryn for helping me turn a few feature requests into an entire software release.

**Artificially Enhanced is an unofficial fork. It is not affiliated with or endorsed by the original ImmichFrame developers or the Immich project.**

The original project's license and applicable attribution remain in place.

---

# Original ImmichFrame README

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![GPL v3 License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/3rob3/ImmichFrame">
    <img src="design/AppIcon.png" alt="Logo" width="200" height="200">
  </a>

  <h3 align="center">ImmichFrame</h3>

  <p align="center">
    An awesome way to display your photos as a digital photo frame
    <br />
    <a href="https://immich.app/"><strong>Explore immich »</strong></a>
    <br />
    <br />
    <a href="https://immichframe.dev">Documentation</a>
    ·
    <a href="https://demo.immichframe.dev">Demo</a>
    ·
    <a href="https://github.com/3rob3/ImmichFrame/issues">Report Bug</a>
    ·
    <a href="https://github.com/3rob3/ImmichFrame/issues">Request Feature</a>
  </p>
</div>

## 📄 Documentation
You can find the documentation [here](https://immichframe.dev).

## 🖼️ Demo

You can find a working demo [here](https://demo.immichframe.dev).

<img src="/design/demo/web_demo.png" alt="Web Demo">

## ⚠️ Disclaimer

**This project is not affiliated with [immich][immich-github-url]!**

## 📜 License

[GNU General Public License v3.0](LICENSE.txt)

## 🆘 Help

[Discord Channel][support-url]

## 🙏 Acknowledgments

- BIG thanks to the [immich team][immich-github-url] for creating an awesome tool

## 🌟 Star History

[![Star History Chart](https://star-history.dera.page/svg?repos=immichframe/immichframe&type=Date)](https://star-history.dera.page/#immichframe/immichframe&Date)

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[contributors-shield]: https://img.shields.io/github/contributors/immichFrame/ImmichFrame.svg?style=for-the-badge
[contributors-url]: https://github.com/immichFrame/ImmichFrame/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/immichFrame/ImmichFrame.svg?style=for-the-badge
[forks-url]: https://github.com/immichFrame/ImmichFrame/network/members
[stars-shield]: https://img.shields.io/github/stars/immichFrame/ImmichFrame.svg?style=for-the-badge
[stars-url]: https://github.com/immichFrame/ImmichFrame/stargazers
[issues-shield]: https://img.shields.io/github/issues/immichFrame/ImmichFrame.svg?style=for-the-badge
[issues-url]: https://github.com/immichFrame/ImmichFrame/issues
[license-shield]: https://img.shields.io/github/license/immichFrame/ImmichFrame.svg?style=for-the-badge
[license-url]: https://github.com/immichFrame/ImmichFrame/blob/master/LICENSE.txt
[releases-url]: https://github.com/immichFrame/ImmichFrame/releases/latest
[support-url]: https://discord.com/channels/979116623879368755/1217843270244372480
[openweathermap-url]: https://openweathermap.org/
[immich-github-url]: https://github.com/immich-app/immich
[immich-api-url]: https://immich.app/docs/features/command-line-interface#obtain-the-api-key
