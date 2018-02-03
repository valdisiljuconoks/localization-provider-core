# LocalizationProvider .Net Core

Database driven localization provider for .Net Core applications.

[<img src="https://tech-fellow-consulting.visualstudio.com/_apis/public/build/definitions/f63fd8ab-e3f1-48c1-bca0-f027727a53c4/9/badge"/>](https://tech-fellow-consulting.visualstudio.com/localization-provider-core/_build/index?definitionId=9)

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
