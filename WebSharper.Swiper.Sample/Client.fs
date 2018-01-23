namespace WebSharper.Swiper.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Templating

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.

    [<SPAEntryPoint>]
    let Main =
        let currentSlideIndex = Var.Create 0

        let config = WebSharper.Swiper.SwipeParameters()
        config.OnSlideChangeStart <- (fun swiper -> Var.Set currentSlideIndex swiper.ActiveIndex)

        let swiper =
            new WebSharper.Swiper.Swiper(
                ".swiper-container",
                config
            )

        let answerButtons =
            Doc.Concat [
                text "Pick an answer: "
                Doc.Button "A" [] (fun () -> ())
                Doc.Button "B" [] (fun () -> ())
                Doc.Button "C" [] (fun () ->
                    swiper.UnlockSwipeToNext()
                    swiper.SlideNext()
                    let newSlide () =
                        div [attr.``class`` "swiper-slide"] [
                            text "You have finished the tutorial!"
                        ]
                    swiper.AppendSlide( (newSlide()).Dom )
                )
            ]

        currentSlideIndex.View
        |> View.Map (fun index ->
            let questionSlideIndex = 8
            if index = questionSlideIndex then
                swiper.LockSwipeToNext()
                answerButtons
            else
                swiper.UnlockSwipeToNext()
                Doc.Empty
        )
        |> Doc.EmbedView
        |> Doc.RunAppendById "container"
