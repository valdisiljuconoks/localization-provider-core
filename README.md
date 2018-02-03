# LocalizationProvider .Net Core

Database driven localization provider for .Net Core applications.

[![Build status](https://ci.appveyor.com/api/projects/status/algu4ji9oebedb2b?svg=true)](https://ci.appveyor.com/project/ValdisIljuconoks/localization-provider-core)
[![Build Status](https://travis-ci.org/valdisiljuconoks/localization-provider-core.svg?branch=master)](https://travis-ci.org/valdisiljuconoks/localization-provider-core)

## What is the LocalizationProvider project?

LocalizationProvider project is Asp.Net Mvc web application localization provider on steriods.

Giving you main following features:
* Database driven localization provider for .Net applications
* Easy resource registrations via code
* Supports hierarchical resource organization (with help of child classes)

## Project Structure

Database localization provider is split into main [abstraction projects](https://github.com/valdisiljuconoks/LocalizationProvider) and .Net Core support project (this).

## Getting Started

* [Getting Started](docs/getting-started-netcore.md)
* [Localizing App Content](docs/localizing-content-netcore.md)

## GitHub Source Code Structure

.Net Core support project has its own repo while main abstraction projects are included as [submodules](https://gist.github.com/gitaarik/8735255) here.

# More Info

* [Part 1: Resources and Models](http://blog.tech-fellow.net/2016/03/16/db-localization-provider-part-1-resources-and-models/)
* [Part 2: Configuration and Extensions](http://blog.tech-fellow.net/2016/04/21/db-localization-provider-part-2-configuration-and-extensions/)
* [Part 3: Import and Export](http://blog.tech-fellow.net/2017/02/22/localization-provider-import-and-export-merge/)
* [Part 4: Resource Refactoring and Migrations](https://blog.tech-fellow.net/2017/10/10/localizationprovider-tree-view-export-and-migrations/)
