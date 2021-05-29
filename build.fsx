#r "paket:
version 6.0.0-rc001-rc001
framework: netstandard20
source https://api.nuget.org/v3/index.json
nuget Be.Vlaanderen.Basisregisters.Build.Pipeline 5.0.3 //"

#load "packages/Be.Vlaanderen.Basisregisters.Build.Pipeline/Content/build-generic.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.FileSystemOperators
open ``Build-generic``

let assemblyVersionNumber = (sprintf "%s.0")
let nugetVersionNumber = (sprintf "%s")

let buildSource = build assemblyVersionNumber
let buildTest = buildTest assemblyVersionNumber
let publishSource = publish assemblyVersionNumber
let pack = packSolution nugetVersionNumber

supportedRuntimeIdentifiers <- [ "linux-x64" ]

// Library ------------------------------------------------------------------------
Target.create "Lib_Build" (fun _ ->
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Testing"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing.NUnit"
  buildSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing.Xunit"
  buildTest "Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Tests"
  buildTest "Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Tests"
  buildTest "Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Tests"
)

Target.create "Lib_Test" (fun _ ->
  [
    "test" @@ "Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Tests"
    "test" @@ "Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner.Tests"
    "test" @@ "Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Tests" ]
  |> List.iter testWithDotNet
)

Target.create "Lib_Publish" (fun _ ->
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector.Testing"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.LastChangedList"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Runner"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore.Autofac"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Syndication.Testing"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing.NUnit"
  publishSource "Be.Vlaanderen.Basisregisters.ProjectionHandling.Testing.Xunit"
)
Target.create "Lib_Pack" (fun _ -> pack "Be.Vlaanderen.Basisregisters.ProjectionHandling")

// --------------------------------------------------------------------------------
Target.create "PublishAll" ignore
Target.create "PackageAll" ignore

// Publish ends up with artifacts in the build folder
"DotNetCli"
==> "Clean"
==> "Restore"
==> "Lib_Build"
==> "Lib_Test"
==> "Lib_Publish"
==> "PublishAll"

// Package ends up with local NuGet packages
"PublishAll"
==> "Lib_Pack"
==> "PackageAll"

Target.runOrDefault "Lib_Test"
