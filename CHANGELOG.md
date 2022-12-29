## [11.0.4](https://github.com/informatievlaanderen/projection-handling/compare/v11.0.3...v11.0.4) (2022-12-29)


### Bug Fixes

* move owned instances to DependencyInjection library ([951cdbc](https://github.com/informatievlaanderen/projection-handling/commit/951cdbc44c3ec722def2cd871016d46e81319051))

## [11.0.3](https://github.com/informatievlaanderen/projection-handling/compare/v11.0.2...v11.0.3) (2022-12-28)


### Bug Fixes

* add Microsoft.Extensions.DependencyInjection ([1e92b23](https://github.com/informatievlaanderen/projection-handling/commit/1e92b23637f6f93a232a138f38f5760941a3e043))
* add nuget to dependabot ([68065bf](https://github.com/informatievlaanderen/projection-handling/commit/68065bf32e6aab883e8bb7ab30ec637b1e24733d))
* break out of endless recursion ([027ec9b](https://github.com/informatievlaanderen/projection-handling/commit/027ec9bf87fb049986f5c45fe24c151ccdbfbcc2))
* comply with Serializable pattern ([7ce101d](https://github.com/informatievlaanderen/projection-handling/commit/7ce101db08a2ddb06ccd511663d8b97f9a893a00))
* ctx to context ([9e26140](https://github.com/informatievlaanderen/projection-handling/commit/9e2614086068d3e1fe50b2eca2314e96859f1650))
* fields should not have public accessibility ([700b49c](https://github.com/informatievlaanderen/projection-handling/commit/700b49c555ded38ac26fb811b2bec096f995a14c))
* implement IDisposable correctly ([4167170](https://github.com/informatievlaanderen/projection-handling/commit/4167170cf1ae4a4ed38d7df62cdc1fd48c92d0da))
* make ProjectionStates not nullable ([5dc4197](https://github.com/informatievlaanderen/projection-handling/commit/5dc4197f016f473001a4ed744c2d39aea8fcda5e))
* rename context to contextFactory ([baedadc](https://github.com/informatievlaanderen/projection-handling/commit/baedadc410162ba01ab5ed56d571ddac6ef63936))
* rename parameter ([db87af6](https://github.com/informatievlaanderen/projection-handling/commit/db87af61be2d2d5381b935e96d0f1d316dbb9f7f))
* use VBR_SONAR_TOKEN ([e44114c](https://github.com/informatievlaanderen/projection-handling/commit/e44114cabb69983f955d54736e25dd85b0131e47))

## [11.0.2](https://github.com/informatievlaanderen/projection-handling/compare/v11.0.1...v11.0.2) (2022-04-29)


### Bug Fixes

* run sonar end when release version != none ([e046bdc](https://github.com/informatievlaanderen/projection-handling/commit/e046bdcde99473d3c325189557a41b7ee1a1dd24))

## [11.0.1](https://github.com/informatievlaanderen/projection-handling/compare/v11.0.0...v11.0.1) (2022-04-29)


### Bug Fixes

* redirect sonar to /dev/null ([5d35c53](https://github.com/informatievlaanderen/projection-handling/commit/5d35c53510c3468606b0cb7732dd633c6be5670d))
* relax requirement on `TProjection` for `ConnectedProjectionTest` ([12cf4da](https://github.com/informatievlaanderen/projection-handling/commit/12cf4da466a93bf09d49ecb17a221209bd221d4d))

# [11.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v10.0.1...v11.0.0) (2022-04-13)


### Features

* add IMessage constraint on Envelope ([2a34560](https://github.com/informatievlaanderen/projection-handling/commit/2a34560539120659b01d1810f74fa65646ccafce))


### BREAKING CHANGES

* Introduce IMessage constraint on `Envelope`

## [10.0.1](https://github.com/informatievlaanderen/projection-handling/compare/v10.0.0...v10.0.1) (2022-03-25)


### Bug Fixes

* test in FeedProjectorTests ([43f278a](https://github.com/informatievlaanderen/projection-handling/commit/43f278aa1c915eefa23bdfdad5ab751e1560c85f))

# [10.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v9.1.4...v10.0.0) (2022-03-25)


### Features

* move to dotnet 6.0.3 ([36de26b](https://github.com/informatievlaanderen/projection-handling/commit/36de26bc2a04abbcacfe58777f181a13be35e191))


### BREAKING CHANGES

* move to dotnet 6.0.3

## [9.1.4](https://github.com/informatievlaanderen/projection-handling/compare/v9.1.3...v9.1.4) (2022-03-03)


### Bug Fixes

* IsGenericType instead of IsGenericTypeDefinition ([408aa96](https://github.com/informatievlaanderen/projection-handling/commit/408aa967f45302c79c29e12f5ab6d9d3cc47af37))

## [9.1.3](https://github.com/informatievlaanderen/projection-handling/compare/v9.1.2...v9.1.3) (2022-03-03)


### Bug Fixes

* generic envelope check for projection test ([92a48b9](https://github.com/informatievlaanderen/projection-handling/commit/92a48b9a9dfee1fa649147a5be9daefe69e63415))

## [9.1.2](https://github.com/informatievlaanderen/projection-handling/compare/v9.1.1...v9.1.2) (2022-03-03)


### Bug Fixes

* do not envelope already enveloped events ([eb3e4a1](https://github.com/informatievlaanderen/projection-handling/commit/eb3e4a1ca6de677245a937ba17273faa2d35d25e))

## [9.1.1](https://github.com/informatievlaanderen/projection-handling/compare/v9.1.0...v9.1.1) (2022-03-03)


### Bug Fixes

* revert "feat: add e-tag to LastChangedRecord" ([978f9f3](https://github.com/informatievlaanderen/projection-handling/commit/978f9f30a02758731d70c1b4c90318479a9df5b7))

# [9.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v9.0.0...v9.1.0) (2022-03-03)


### Features

* add e-tag to LastChangedRecord ([adb2d93](https://github.com/informatievlaanderen/projection-handling/commit/adb2d93743addd3143a4be94b73e2dda53173c10))

# [9.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v8.1.0...v9.0.0) (2021-11-25)


### Features

* refactor BuildUri + BuildCacheKey GAWR-666 ([e09e067](https://github.com/informatievlaanderen/projection-handling/commit/e09e0672b9e24b8579f69cd21ba65ef84fc9ab08))
* the RunnerDbContext methods are now overridable ([045cf9b](https://github.com/informatievlaanderen/projection-handling/commit/045cf9b9bf4dbac600524c0e104108acfa53da5b))


### BREAKING CHANGES

* - Replaced UriFormat property with BuildUri method to allow flexibility for uri per accept type
- Replaced CacheKeyFormat property with BuildCacheKey method to allow the same flexibility

# [8.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v8.0.3...v8.1.0) (2021-11-17)


### Features

* add ConnectedProjectionTest ([665f1fa](https://github.com/informatievlaanderen/projection-handling/commit/665f1fa2d05a0e43cbd5b48be4fae5152492a1f5))

## [8.0.3](https://github.com/informatievlaanderen/projection-handling/compare/v8.0.2...v8.0.3) (2021-10-20)


### Bug Fixes

* removed static fields, with no static use ([2b16303](https://github.com/informatievlaanderen/projection-handling/commit/2b163037c1d8373425ae2ee239eaabee47dd5a11))

## [8.0.2](https://github.com/informatievlaanderen/projection-handling/compare/v8.0.1...v8.0.2) (2021-05-28)


### Bug Fixes

* move to 5.0.6 ([b7ec96c](https://github.com/informatievlaanderen/projection-handling/commit/b7ec96c4e774c222b2cc7f57f2e1cd20a730b42c))

## [8.0.1](https://github.com/informatievlaanderen/projection-handling/compare/v8.0.0...v8.0.1) (2021-03-31)


### Bug Fixes

* sync feed only swallows AtomResolveHandlerException ([56c4893](https://github.com/informatievlaanderen/projection-handling/commit/56c48931debe5190268587653f789bf2f3cd52af))

# [8.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v7.1.0...v8.0.0) (2021-03-30)


### Bug Fixes

* use a specific exception if atom handler can't be resolved ([9e92dd1](https://github.com/informatievlaanderen/projection-handling/commit/9e92dd14aeb9f441075b3b95ec2980b27b9fcdf6))


### BREAKING CHANGES

* When resolving a handler for atomprojections a AtomResolveHandlerException is now
thrown

# [7.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v7.0.6...v7.1.0) (2021-03-09)


### Features

* introduce connected projection name, description attribute GRAR-1876 ([96749eb](https://github.com/informatievlaanderen/projection-handling/commit/96749ebfb840c3feea7441a409378d10b0f0dd10))

## [7.0.6](https://github.com/informatievlaanderen/projection-handling/compare/v7.0.5...v7.0.6) (2021-02-02)


### Bug Fixes

* move to 5.0.2 ([8d71498](https://github.com/informatievlaanderen/projection-handling/commit/8d7149883b7871ea5f4f50de45396f675367964e))

## [7.0.5](https://github.com/informatievlaanderen/projection-handling/compare/v7.0.4...v7.0.5) (2021-01-07)


### Bug Fixes

* improve performance for determining to be indexed values GRAR-1673 ([0f1dd14](https://github.com/informatievlaanderen/projection-handling/commit/0f1dd1449d55cfa0af910d522d93caa5a087c4e2))

## [7.0.4](https://github.com/informatievlaanderen/projection-handling/compare/v7.0.3...v7.0.4) (2020-12-18)


### Bug Fixes

* move to 5.0.1 ([f556cab](https://github.com/informatievlaanderen/projection-handling/commit/f556cabe1065140df810ad1171c5b068587a99f4))

## [7.0.3](https://github.com/informatievlaanderen/projection-handling/compare/v7.0.2...v7.0.3) (2020-11-19)


### Bug Fixes

* update aggregatesource reference ([af31a24](https://github.com/informatievlaanderen/projection-handling/commit/af31a245b51cfa1fe6c625de82af3439f5e4d51e))

## [7.0.2](https://github.com/informatievlaanderen/projection-handling/compare/v7.0.1...v7.0.2) (2020-11-19)


### Bug Fixes

* update eventhandling reference ([71a1bfb](https://github.com/informatievlaanderen/projection-handling/commit/71a1bfb7eb0c1ad2b5191fc0209f029164261123))

## [7.0.1](https://github.com/informatievlaanderen/projection-handling/compare/v7.0.0...v7.0.1) (2020-11-18)


### Bug Fixes

* remove set-env usage in gh-actions ([3fbff73](https://github.com/informatievlaanderen/projection-handling/commit/3fbff739fb078cf71bead268c408ce7ada6062a5))

# [7.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v6.2.0...v7.0.0) (2020-10-20)


### Features

* add ErrorMessage to projection state GRAR-1302 ([06e07fd](https://github.com/informatievlaanderen/projection-handling/commit/06e07fd22f652aff2e33b6590439fe0fefb4483d))
* add errormessage to projection state lastchangedlist ([fe22c2c](https://github.com/informatievlaanderen/projection-handling/commit/fe22c2c855a4dea9464ed93b5e75a0218b8481c2))


### BREAKING CHANGES

* the added columns means package users will have to run migrations.

# [6.2.0](https://github.com/informatievlaanderen/projection-handling/compare/v6.1.0...v6.2.0) (2020-10-03)


### Features

* add feedprojector GRAR-1562 ([d182f34](https://github.com/informatievlaanderen/projection-handling/commit/d182f345a117dddb4cd3db212db899b4a4898037))

# [6.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.8...v6.1.0) (2020-09-22)


### Features

* add timeout to lastchangedlist commands ([b59471b](https://github.com/informatievlaanderen/projection-handling/commit/b59471b25c938880449cf196fdbdef6c5359a544))

## [6.0.8](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.7...v6.0.8) (2020-09-21)


### Bug Fixes

* move to 3.1.8 ([f41e55a](https://github.com/informatievlaanderen/projection-handling/commit/f41e55a97d6d33d7d050fbd6eb619c3c7d9a78b8))

## [6.0.7](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.6...v6.0.7) (2020-07-18)


### Bug Fixes

* move to 3.1.6 ([4fba30f](https://github.com/informatievlaanderen/projection-handling/commit/4fba30f02006b646cf8cb5c7f5ba66dab3694f7d))

## [6.0.6](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.5...v6.0.6) (2020-07-02)


### Bug Fixes

* update streamstore ([fdb39a5](https://github.com/informatievlaanderen/projection-handling/commit/fdb39a5b04ced5a0f82a3dff85436066ba2c2def))

## [6.0.5](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.4...v6.0.5) (2020-06-19)


### Bug Fixes

* move to 3.1.5 ([7b3e79e](https://github.com/informatievlaanderen/projection-handling/commit/7b3e79eb84158310319a91111abf76c7f4e9506f))

## [6.0.4](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.3...v6.0.4) (2020-05-18)


### Bug Fixes

* move to 3.1.4 ([d6715bb](https://github.com/informatievlaanderen/projection-handling/commit/d6715bb5a4de1f5ac8d82856bf2ca73d3ff6e851))

## [6.0.3](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.2...v6.0.3) (2020-05-14)


### Bug Fixes

* downgrade dotnet 3.1.4 references ([2c3a4f2](https://github.com/informatievlaanderen/projection-handling/commit/2c3a4f28a8d6bba96d53c12b8422d03a06b117b9))

## [6.0.2](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.1...v6.0.2) (2020-05-13)


### Bug Fixes

* move to GH-actions ([d7a30b1](https://github.com/informatievlaanderen/projection-handling/commit/d7a30b154cdde61b862f13e4665d844e5e29a21f))

## [6.0.1](https://github.com/informatievlaanderen/projection-handling/compare/v6.0.0...v6.0.1) (2020-05-06)


### Bug Fixes

* add missing index on LastChangedList GRAR-1271 ([9c758e3](https://github.com/informatievlaanderen/projection-handling/commit/9c758e37ca92440731fd2ac28a2a2f64e00f2681))

# [6.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v5.4.1...v6.0.0) (2020-04-08)


### Bug Fixes

* update paket.lock ([1d1c620](https://github.com/informatievlaanderen/projection-handling/commit/1d1c620f4f7a0e31558ecebd53725fb5557bc881))
* upgrade build pipeline and sql client usage ([da2878c](https://github.com/informatievlaanderen/projection-handling/commit/da2878c4e824356abe42ca476a09de6cb500e58a))


### chore

* upgrade sql stream store ([#26](https://github.com/informatievlaanderen/projection-handling/issues/26)) ([9f8a139](https://github.com/informatievlaanderen/projection-handling/commit/9f8a1393ef09193da00dce3bb813997472a9b3e1))


### BREAKING CHANGES

* Upgrade SqlStreamStore

## [5.4.1](https://github.com/informatievlaanderen/projection-handling/compare/v5.4.0...v5.4.1) (2020-03-31)


### Bug Fixes

* use correct build user ([f063714](https://github.com/informatievlaanderen/projection-handling/commit/f063714ddb012a6ae4108c6939ef1ef33434c779))

# [5.4.0](https://github.com/informatievlaanderen/projection-handling/compare/v5.3.1...v5.4.0) (2020-03-31)


### Features

* add last error message to last changed list ([c7e8cb7](https://github.com/informatievlaanderen/projection-handling/commit/c7e8cb7462c373a583cbc458c90b985b5a27ff58))

## [5.3.1](https://github.com/informatievlaanderen/projection-handling/compare/v5.3.0...v5.3.1) (2020-03-03)


### Bug Fixes

* bump netcore 3.1.2 ([8f9a71f](https://github.com/informatievlaanderen/projection-handling/commit/8f9a71fb6268857b257733409274d5398e5c961f))

# [5.3.0](https://github.com/informatievlaanderen/projection-handling/compare/v5.2.0...v5.3.0) (2020-02-24)


### Bug Fixes

* make it compile and refactor into one extension method ([b329bd8](https://github.com/informatievlaanderen/projection-handling/commit/b329bd8158fb75e987c9475449f6ed3f0cf219f8))


### Features

* add extension for default command timeout for migrators ([0833d39](https://github.com/informatievlaanderen/projection-handling/commit/0833d3920a676db2073447c131f98a11f87a9127))

# [5.2.0](https://github.com/informatievlaanderen/projection-handling/compare/v5.1.2...v5.2.0) (2020-01-31)


### Features

* upgrade netcoreapp31 and dependencies ([8066b56](https://github.com/informatievlaanderen/projection-handling/commit/8066b567dfd3ae970640e4072fd43417637c84d3))

## [5.1.2](https://github.com/informatievlaanderen/projection-handling/compare/v5.1.1...v5.1.2) (2020-01-23)


### Bug Fixes

* correct nullability for projectionstates ([8fe5b2e](https://github.com/informatievlaanderen/projection-handling/commit/8fe5b2e4285a4d4e4a22a9b4af89c7156388971d))

## [5.1.1](https://github.com/informatievlaanderen/projection-handling/compare/v5.1.0...v5.1.1) (2020-01-15)


### Bug Fixes

* correct nullability for EF classes ([9ca2b17](https://github.com/informatievlaanderen/projection-handling/commit/9ca2b173269677dff5f908c9d084938af3213e5c))

# [5.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v5.0.0...v5.1.0) (2019-12-15)


### Features

* upgrade to netcoreapp31 ([e31f6c7](https://github.com/informatievlaanderen/projection-handling/commit/e31f6c772df8c58a15c7cf839604064e757806b5))

# [5.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v4.6.0...v5.0.0) (2019-11-22)


### Code Refactoring

* upgrade to netcoreapp30 ([2b89673](https://github.com/informatievlaanderen/projection-handling/commit/2b89673))


### BREAKING CHANGES

* Upgrade to .NET Core 3

# [4.6.0](https://github.com/informatievlaanderen/projection-handling/compare/v4.5.2...v4.6.0) (2019-09-26)


### Features

* **runner:** runner only projects events that are mapped ([ceb74c4](https://github.com/informatievlaanderen/projection-handling/commit/ceb74c4))

## [4.5.2](https://github.com/informatievlaanderen/projection-handling/compare/v4.5.1...v4.5.2) (2019-09-16)


### Bug Fixes

* use generic dbtraceconnection ([ead4a90](https://github.com/informatievlaanderen/projection-handling/commit/ead4a90))

## [4.5.1](https://github.com/informatievlaanderen/projection-handling/compare/v4.5.0...v4.5.1) (2019-09-12)


### Bug Fixes

* forgot to add lasterror field ([69b7f10](https://github.com/informatievlaanderen/projection-handling/commit/69b7f10))

# [4.5.0](https://github.com/informatievlaanderen/projection-handling/compare/v4.4.1...v4.5.0) (2019-09-12)


### Features

* keep track of how many times lastchanged has errored ([3790176](https://github.com/informatievlaanderen/projection-handling/commit/3790176))

## [4.4.1](https://github.com/informatievlaanderen/projection-handling/compare/v4.4.0...v4.4.1) (2019-08-28)


### Bug Fixes

* use longer timeout for migrations ([ac8cd4f](https://github.com/informatievlaanderen/projection-handling/commit/ac8cd4f))

# [4.4.0](https://github.com/informatievlaanderen/projection-handling/compare/v4.3.2...v4.4.0) (2019-08-28)


### Features

* add UseExtendedSqlServerMigrations() ([e6e7cff](https://github.com/informatievlaanderen/projection-handling/commit/e6e7cff))

## [4.3.2](https://github.com/informatievlaanderen/projection-handling/compare/v4.3.1...v4.3.2) (2019-08-27)


### Bug Fixes

* make datadog tracing check more for nulls ([716c182](https://github.com/informatievlaanderen/projection-handling/commit/716c182))

## [4.3.1](https://github.com/informatievlaanderen/projection-handling/compare/v4.3.0...v4.3.1) (2019-08-26)


### Bug Fixes

* use fixed datadog tracing ([3e5ff0f](https://github.com/informatievlaanderen/projection-handling/commit/3e5ff0f))

# [4.3.0](https://github.com/informatievlaanderen/projection-handling/compare/v4.2.0...v4.3.0) (2019-08-22)


### Features

* bump to .net 2.2.6 ([d8cc684](https://github.com/informatievlaanderen/projection-handling/commit/d8cc684))

# [4.2.0](https://github.com/informatievlaanderen/projection-handling/compare/v4.1.0...v4.2.0) (2019-08-22)


### Features

* add migration to save states of projectionstates ([edfce41](https://github.com/informatievlaanderen/projection-handling/commit/edfce41))

# [4.1.0](https://github.com/informatievlaanderen/projection-handling/compare/v4.0.0...v4.1.0) (2019-08-22)


### Features

* add EF extension to have an ColumnStoreIndex ([fbd79fe](https://github.com/informatievlaanderen/projection-handling/commit/fbd79fe))

# [4.0.0](https://github.com/informatievlaanderen/projection-handling/compare/v3.6.0...v4.0.0) (2019-08-13)


### Features

* add columns to store user requested state ([b740cf2](https://github.com/informatievlaanderen/projection-handling/commit/b740cf2))
* add extension method to update user requested state ([237e943](https://github.com/informatievlaanderen/projection-handling/commit/237e943))
* make sure a projection state exists when updating desired state ([1fc1b38](https://github.com/informatievlaanderen/projection-handling/commit/1fc1b38))


### BREAKING CHANGES

* the added columns means package users will have to run
migrations.

# [3.6.0](https://github.com/informatievlaanderen/projection-handling/compare/v3.5.2...v3.6.0) (2019-06-24)


### Features

* add Log JsonSerializer settings ([95e7fbf](https://github.com/informatievlaanderen/projection-handling/commit/95e7fbf))

## [3.5.2](https://github.com/informatievlaanderen/projection-handling/compare/v3.5.1...v3.5.2) (2019-05-20)


### Bug Fixes

* sync filter header is now escaped to string ([da4965a](https://github.com/informatievlaanderen/projection-handling/commit/da4965a))

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
