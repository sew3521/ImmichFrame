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

## A Note From Me About AI

Howdy!

I know some of you aren't going to like the fact that this project was "vibe coded." I get it. I really do.

I'm extremely comfortable writing Batch and PowerShell scripts, and I can stumble my way through Python when necessary. I've been writing scripts since long before AI was even part of the conversation. But when it comes to most traditional programming tasks? I'm pretty terrible at them.

I'm not a software developer, and I'm not going to pretend to be one.

So how did we get here?

Originally, I was just trying to get ImmichFrame working properly on my digital photo frame. The APK was triggering a security popup on my device, so I enlisted ChatGPT to help me modify the APK and resolve the issue.

Once that was fixed, I noticed the calendar feature was behaving strangely with certain events. Naturally, I asked ChatGPT to help me fix that too.

Then I wanted a way to distinguish between events on my calendar and events on my wife's calendar.

And that's where the feature creep started.

One change led to another, which led to another, and before I knew it, I was maintaining my own version of an application that I originally just wanted to display some family photos.

Once I had everything working the way I wanted, I started looking through the original project's proposed features and pull requests. There were several improvements I hadn't even considered, so I figured, what the hell, let's see if ChatGPT can help me incorporate some of those too.

Then I started looking at the dependencies and noticed some potential security vulnerabilities.

So naturally, we had to deal with those as well.

At this point, what started as fixing an annoying popup has turned into a much bigger project than I ever intended.

**Do I feel a little dirty about putting this together with so much help from AI?**

Actually, yeah. A little.

But here's something I want to make clear: I haven't been using an autonomous AI coding agent to make these changes for me.

I'm sitting here copying and pasting code and commands from a web browser into VS Code or a terminal window. I'm reviewing the output, running tests, troubleshooting errors, and trying to understand what we're changing and why.

Why do it this way? Because I actually want to have some idea of what's going on.

Does that make me a developer? Absolutely not. Does it mean I understand every line of code in this project? Hell no. But I'm learning things along the way, and I'm having a lot of fun doing it.

Do you have to like that this project was developed with AI? No.

Do you have to agree with my decision to make it? Also no.

Am I having a ton of fun improving an application I genuinely enjoy using?

**You bet your ass I am.**

So, if you'd like to run this version, please do! And if you encounter problems, feel free to let me know. I'll do my best to help.

I'm making this project primarily for myself, but I'm happy to share it with anyone else who finds it useful.

Maybe one of these days I'll sit down and seriously try to get better at programming. Truth be told, though, I've never particularly enjoyed it.

And these days, between having two kids and a wife, a senior IT job, involvement in local government, and approximately zero free time, this is the approach that works for me.

I'm not claiming this is the right way to develop software. I'm not claiming the code is perfect. And I'm certainly not claiming to be some brilliant developer.

I'm just a guy who likes an open-source project, wanted a few things to work differently, and found a way to make that happen.

**Sorry, not sorry, if that offends you. :)**

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

## Dependencies and security updates

One of my goals with Artificially Enhanced is to keep its
dependencies reasonably current.

I try to incorporate newer package versions, security fixes,
and compatibility improvements when doing so makes sense.

That doesn't mean every dependency will always be the newest
version available. Major upgrades can introduce breaking changes,
and I'd rather have stable software than brag about version numbers
while everything catches fire.

Updates are evaluated for compatibility and tested before release.
Known security vulnerabilities receive particular attention.

The goal is reasonably current, secure, and stable software.
Preferably all three at the same time.

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
