# TLibrary

![Release (latest by date)](https://img.shields.io/github/v/release/TavstalDev/TLibrary?style=plastic-square)
![Workflow Status](https://img.shields.io/github/actions/workflow/status/TavstalDev/TLibrary/release.yml?branch=stable&label=build&style=plastic-square)
![License](https://img.shields.io/github/license/TavstalDev/TLibrary?style=plastic-square)
![Downloads](https://img.shields.io/github/downloads/TavstalDev/TLibrary/total?style=plastic-square)
![Isues](https://img.shields.io/github/issues/TavstalDev/TLibrary?style=plastic-square)

TLibrary is a versatile Unturned RocketMod 4 library designed to provide a range of helper functions and extensions for various tasks. It is intended to simplify common operations and enhance productivity.

### Features

#### Easier SQL Database Management
Enhance the `DatabaseHelper` to include more intuitive methods for common database operations, such as CRUD operations, connection pooling, and transaction management.

#### Subcommand System
Improve the subcommand system by adding support for command aliases, argument validation, and better error handling. Ensure that commands can be easily extended and customized.

#### Commonly Used Extensions and Helpers
Expand the library of extensions and helpers to cover more use cases. Ensure that they are well-documented and include examples of how to use them effectively.

#### Rich Chat and Console Formatter
Enhance the `FormatHelper` to support more formatting options, such as nested formatting, custom color schemes, and better handling of special characters.

#### Custom Rich Logger
Improve the `TLogger` to include more logging levels (e.g., trace, debug, info, warn, error, fatal), support for structured logging, and integration with external logging services.

#### Discord Webhook
Enhance the `DiscordWebhook` service to support more message types, such as embeds, file attachments, and reactions. Ensure that it can handle rate limiting and retries.

#### Hook Manager for Plugins
Improve the `HookManager` to support more hook types, better error handling, and easier integration with other parts of the library. Ensure that hooks can be dynamically registered and unregistered.

#### Better Localization and Configuration Using JSON
Enhance the localization and configuration system to support more advanced features, such as nested configurations, environment-specific settings, and better validation of configuration files. Ensure that the system is easy to use and well-documented.

## Installation

To use TLibrary in your project, follow these steps:

1. **Download the Library**: Obtain the latest version of TLibrary from the repository.
2. **Add to Project**: Include the TLibrary files in your project directory.
3. **Reference in Code**: Reference the TLibrary namespace in your code files to access its features.

## Usage

### General Helpers

- **MathHelper**: Provides functions for random number generation, clamping values, and more.
- **StringHelper**: Includes functions for generating random strings, converting colors, and formatting dates.

### Extensions

- **ArrayExtensions**: Adds methods to arrays for checking valid indices, shuffling elements, and more.
- **StringExtensions**: Enhances string functionality with methods for case-insensitive comparisons, random string generation, and more.
- **IntegerExtensions**: Adds clamping functionality to integers and floating-point numbers.

### MySQL Extensions

- **MySqlExtensions**: Simplifies reading and writing JSON objects, handling floating-point values, and managing database connections.

### Unturned Extensions

- **PlayerExtensions**: Provides methods for checking player status, managing inventory, and teleporting players in the Unturned game.

### Plugin Extensions

- **RocketPluginExtensions**: Facilitates working with Rocket plugins by providing methods to access configuration fields and properties.

## Contributing

Contributions to TLibrary are welcome. If you have suggestions for improvements or new features, please submit a pull request or open an issue in the repository.

## License

This project is licensed under the GNU General Public License v3.0. See the `LICENSE` file for more details.

## Contact

For issues or feature requests, please use the [GitHub issue tracker](https://github.com/TavstalDev/TLibrary/issues).

# How To Use

Please check the <a href="https://github.com/TavstalDev/TExample/tree/master">template project</a>.
