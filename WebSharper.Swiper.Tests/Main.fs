namespace WebSharper.Swiper.Tests

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next

module Site =
    open WebSharper.UI.Next.Server
    open WebSharper.UI.Next.Html

    type Templ = Templating.Template<"Main.html">

    let Maaaain (body: Doc) (title:string) =
        Templ().body(body).title(title).Doc()

    [<Website>]
    let Main =
        Application.SinglePage (fun ctx ->
            Content.Page(
                Maaaain (client <@ Client.RunTests() @>) "WebSharper.Swiper Tests"
//                Body = [
//#if ZAFIR
                    
//#else
//                    WebSharper.Testing.Runner.Run [
//                        System.Reflection.Assembly.GetExecutingAssembly()
//                    ]
//                    |> Doc.WebControl
//#endif
//                ]
            )
        )
