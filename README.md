# MonoGame

### This is a fork.
Go to [MonoGame](https://github.com/MonoGame/MonoGame) for the full README.

[![Join the chat at https://discord.gg/tsuucV4](https://img.shields.io/discord/355231098122272778?color=%237289DA&label=MonoGame&logo=discord&logoColor=white)](https://discord.gg/tsuucV4) [![Join the chat at https://gitter.im/MonoGame/MonoGame](https://badges.gitter.im/MonoGame/MonoGame.svg)](https://gitter.im/MonoGame/MonoGame?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

 * [Build Status](#build-status)
 * [Supported Platforms](#supported-platforms)
 * [Support and Contributions](#support-and-contributions)
 * [Source Code](#source-code)
 * [Helpful Links](#helpful-links)
 * [License](#license)



## Supported Platforms

We support a growing list of platforms across the desktop, mobile, and console space.  If there is a platform we don't support, please [make a request](https://github.com/MonoGame/MonoGame/issues) or [come help us](CONTRIBUTING.md) add it.

 * Desktop PCs
   * Windows 10 Store Apps (UWP)
   * Windows Win32 (OpenGL & DirectX)
   * Linux (OpenGL)
   * Mac OS X (OpenGL)
 * Mobile/Tablet Devices
   * Android (OpenGL)
   * iPhone/iPad (OpenGL)
   * Windows Phone 10 (UWP)
 * Consoles (for registered developers)
   * PlayStation 4
   * PlayStation Vita
   * Xbox One (both UWP and XDK)
   * Nintendo Switch
 * Other
   * tvOS (OpenGL)

### Subscription

If you'd like to help the project by supporting it financially, consider support via a subscription for the price of a monthly coffee.

Money goes towards hosting, new hardware and if enough people subscribe a dedicated developer.

There are several options on the [Donation Page](http://www.monogame.net/donate/).


## Source Code

The full source code is available here from GitHub:

 * Clone the source: `git clone https://github.com/MonoGame/MonoGame.git`
 * Set up the submodules: `git submodule update --init`
 * Open the solution for your target platform to build the game framework.
 * Open the Tools solution for your development platform to build the pipeline and content tools.

For the prerequisites for building from source, please look at the [Requirements](REQUIREMENTS.md) file.

A high level breakdown of the components of the framework:

 * The game framework is found in [MonoGame.Framework](MonoGame.Framework).
 * The content pipeline is located in [MonoGame.Framework.Content.Pipeline](MonoGame.Framework.Content.Pipeline).
 * Project templates are in [Templates](Templates).
 * See [Tests](Tests) for the framework unit tests.
 * See [Tools/Tests](Tools/MonoGame.Tools.Tests) for the content pipeline and other tool tests.
 * [mgcb](Tools/MonoGame.Content.Builder) is the command line tool for content processing.
 * [mgfxc](Tools/MonoGame.Effect.Compiler) is the command line effect compiler tool.
 * The [mgcb-editor](Tools/MonoGame.Content.Builder.Editor) tool is a GUI frontend for content processing.


## Helpful Links

 * The official website is [monogame.net](http://www.monogame.net).
 * The [issue tracker](https://github.com/MonoGame/MonoGame/issues) is on GitHub (though don't post issues related to this fork there, open them here instead). 
 * The [community forums](http://community.monogame.net/) for support questions.
 * The [live chat](https://gitter.im/mono/MonoGame?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) with the core developers and other users.
 * The [official documentation](http://www.monogame.net/documentation/) is on our website.
 * Download release and development [installers and packages](http://www.monogame.net/downloads/).
 * Follow [@MonoGameTeam](https://twitter.com/monogameteam) on Twitter.

## License

The MonoGame project is under the [Microsoft Public License](https://opensource.org/licenses/MS-PL) except for a few portions of the code.  See the [LICENSE.txt](LICENSE.txt) file for more details.  Third-party libraries used by MonoGame are under their own licenses.  Please refer to those libraries for details on the license they use.
