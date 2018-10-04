// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
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
                        Elt.div [attr.``class`` "swiper-slide"] [
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
