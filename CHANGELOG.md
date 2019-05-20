## [3.5.1](https://github.com/informatievlaanderen/projection-handling/compare/v3.5.0...v3.5.1) (2019-05-20)

# [3.5.0](https://github.com/informatievlaanderen/projection-handling/compare/v3.4.1...v3.5.0) (2019-05-20)


### Features

* add embed to filter header ([8138ad3](https://github.com/informatievlaanderen/projection-handling/commit/8138ad3))

## [3.4.1](https://github.com/informatievlaanderen/projection-handling/compare/v3.4.0...v3.4.1) (2019-05-02)

# [3.4.0](https://github.com/informatievlaanderen/projection-handling/compare/v3.3.1...v3.4.0) (2019-05-02)


### Features

* make ef options configurable ([8e53aec](https://github.com/informatievlaanderen/projection-handling/commit/8e53aec))

## [3.3.1](https://github.com/informatievlaanderen/projection-handling/compare/v3.3.0...v3.3.1) (2019-04-26)

# [3.3.0](https://github.com/informatievlaanderen/projection-handling/compare/v3.2.0...v3.3.0) (2019-04-23)


### Features

* add overload LastChangedList to have multiple projections ([d25a619](https://github.com/informatievlaanderen/projection-handling/commit/d25a619))

# [3.2.0](https://github.com/informatievlaanderen/projection-handling/compare/v3.1.1...v3.2.0) (2019-04-18)


### Features

* update datadog dependency ([6ae536e](https://github.com/informatievlaanderen/projection-handling/commit/6ae536e))

## [3.1.1](https://github.com/informatievlaanderen/projection-handling/compare/v3.1.0...v3.1.1) (2019-03-08)


### Bug Fixes

* styling fix, trigger rebuild ([9211e5d](https://github.com/informatievlaanderen/projection-handling/commit/9211e5d))

# [3.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v3.0.0...v3.1.0) (2019-03-07)


### Bug Fixes

* change IConfigurationRoot to IConfiguration to be less restrictive ([76e4a27](https://github.com/informatievlaanderen/projection-handling/commit/76e4a27))


### Features

* introduce IRunnerDbContextMigratorFactory interface ([9efc1d9](https://github.com/informatievlaanderen/projection-handling/commit/9efc1d9))

# [3.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v2.0.1...v3.0.0) (2019-03-07)


### Features

* create RunnerDbContext migration infrastucture ([2df9fdd](https://github.com/informatievlaanderen/projection-handling/commit/2df9fdd))


### BREAKING CHANGES

* RunnerDbContextMigratioHelper is replaced by RunnedDbContextMigrationFactory
Factory creates:
- RunnerDbContext for generating migrations
- IRunnerDbContextMigrator for applying the migrations.

## [2.0.1](https://github.com/informatievlaanderen/projection-handling/compare/v2.0.0...v2.0.1) (2019-02-26)

# [2.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v1.3.0...v2.0.0) (2019-02-25)


### Features

* use RunnerDBMigration helper instead of static helper ([0072d0d](https://github.com/informatievlaanderen/projection-handling/commit/0072d0d))


### BREAKING CHANGES

* removed the static MigrationHelper
Use instance method RunMigrationAsync instead of the static StartAsync

# [1.3.0](https://github.com/informatievlaanderen/projection-handling/compare/v1.2.1...v1.3.0) (2019-02-01)


### Features

* add migration helper to Runner ([02c131a](https://github.com/informatievlaanderen/projection-handling/commit/02c131a))

## [1.2.1](https://github.com/informatievlaanderen/projection-handling/compare/v1.2.0...v1.2.1) (2019-01-17)


### Bug Fixes

* add position to envelope for testing projections ([0d74e27](https://github.com/informatievlaanderen/projection-handling/commit/0d74e27))

# [1.2.0](https://github.com/informatievlaanderen/projection-handling/compare/v1.1.0...v1.2.0) (2019-01-15)


### Features

* add GetLastChangedRecords to retrieve all records for a given id ([cabec63](https://github.com/informatievlaanderen/projection-handling/commit/cabec63))

# [1.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v1.0.0...v1.1.0) (2019-01-03)


### Features

* update projection handling with features from private branches ([3b2153d](https://github.com/informatievlaanderen/projection-handling/commit/3b2153d))

# 1.0.0 (2018-12-20)


### Features

* open source with MIT license as 'agentschap Informatie Vlaanderen' ([263cdf9](https://github.com/informatievlaanderen/projection-handling/commit/263cdf9))
