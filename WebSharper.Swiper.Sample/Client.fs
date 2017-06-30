namespace WebSharper.Swiper.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Templating

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    
    [<SPAEntryPoint>]
    let Main () =
        let swiper = new WebSharper.Swiper.Swiper(".swiper-container")
        text ("active: " + string swiper.ActiveIndex)
        |>
        Doc.RunAppendById "main"
