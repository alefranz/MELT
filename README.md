_Power Pack for Microsoft Extension Logging._

[![Build Status](https://alefranz.visualstudio.com/MELPowerPack/_apis/build/status/alefranz.MELPowerPack?branchName=master)](https://alefranz.visualstudio.com/MELPowerPack/_build/latest?definitionId=1?branchName=master) [![](https://img.shields.io/nuget/v/MELPowerPack.svg)](https://www.nuget.org/packages/MELPowerPack/)

## About MELPowerPack

MELPowerPack is a free, open source, Power Pack for the .NET Standard _Microsoft Extension Logging_ library.
It is a solution to easily log to files.

Key benefits:

- Performant file logger
- Plays well with the _Microsoft Extension Logging_ `LoggerFactory`:
  - Doesn't break the integration of other providers (e.g. _Application Insights_, built-in console provider).
  - Configured alongside the built in providers.
- Customize logs format

It is licensed under [Apache License 2.0](https://github.com/alefranz/HttpClientLab/blob/master/LICENSE).

If you like this project please don't forget to *star* it on [GitHub](https//github.com/alefranz/MELPowerPack) or let me know with a [tweet](https://twitter.com/AleFranz).

## Motivation

Unfortunately the _Microsoft Extension Logging_ library doesn't provide the ability to log to file.
On the other hand, most of the logging libraries in the .NET ecosystem, although the can be used with _Microsoft Extension Logging_, they replace the built-in logger factory,
breaking built-in providers like the _Application Insights_ one, as well as replacing the console provider.
Furthermore, the configuration step are different and do not follow a pure DI approach.
