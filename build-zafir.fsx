#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.Swiper")
        .VersionFrom("WebSharper")

let main =
    bt.WebSharper4.Extension("WebSharper.Swiper")
        .SourcesFromProject()
        .Embed([])
        .References(fun r -> [])

let tests =
    bt.WebSharper4.SiteletWebsite("WebSharper.Swiper.Tests")
        .SourcesFromProject()
        .Embed([])
        .References(fun r ->
            [
                r.Project(main)
                r.NuGet("WebSharper.Testing").Latest(true).Reference()
                r.NuGet("WebSharper.UI.Next").Latest(true).Reference()
            ])

bt.Solution [
    main
    tests

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "WebSharper bindings for Swiper"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/https://github.com/intellifactory/websharper.swiper"
                Description = "Swiper API for WebSharper"
                RequiresLicenseAcceptance = true })
        .Add(main)
]
|> bt.Dispatch
