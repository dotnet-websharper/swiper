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
namespace WebSharper.Swiper.Tests

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.JQuery

[<JavaScript>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.Testing
    open WebSharper.UI
    open WebSharper.UI.Client
    open WebSharper.UI.Html

    let Tests =

        let swiper =
            (*WebSharper.Swiper.Swiper(".swp-cont",
                WebSharper.Swiper.SwipeParameters(WrapperClass = "swp-wrp", SlideClass = "swp-slide")
            )*)
            WebSharper.Swiper.Swiper(".swiper-container")

        TestCategory "General" {
            (*

            Test "Default test" {
                equalMsg (swiper.ActiveIndex) 0 "The active index is 0 at the start."
            }

            *)
            Test "SlideTo test" {
                swiper.SlideTo 0
                equalMsg swiper.ActiveIndex 0 "SlideTo 0"
                swiper.SlideTo 4
                equalMsg swiper.ActiveIndex 4 "SlideTo 4"
                swiper.SlideTo 3
                equalMsg swiper.ActiveIndex 3 "SlideTo 3"
                swiper.SlideTo 0
                equalMsg swiper.ActiveIndex 0 "SlideTo 0"
            }
            
            Test "Sliding around the start of the slides" {
                swiper.SlideTo 0
                equal swiper.ActiveIndex 0
                swiper.SlidePrev()
                equalMsg swiper.ActiveIndex 0 "Can not swipe left from the first slide"
                swiper.SlideTo 0
                swiper.SlideNext()
                equalMsg swiper.ActiveIndex 1 "Slide right from the first slide"
            }
            (*

            Test "Sliding around the end of the slides" {
                let lastSlide = 4
                swiper.SlideTo lastSlide
                equal swiper.ActiveIndex 4
                swiper.SlideNext()
                equalMsg swiper.ActiveIndex lastSlide "Can not swipe right from the last slide"
                swiper.SlideTo lastSlide
                swiper.SlidePrev()
                equalMsg swiper.ActiveIndex (lastSlide-1) "Slide left from the last slide"
            }

            Test "Lock next and prev sliding" {
                let testSlideId = 2
                swiper.SlideTo testSlideId
                swiper.LockSwipeToNext()
                swiper.SlideNext()
                equalMsg swiper.ActiveIndex testSlideId "LockSwipeToNext() works"
                swiper.UnlockSwipeToNext()
                swiper.SlideNext()
                equalMsg swiper.ActiveIndex (testSlideId+1) "UnlockSwipeToNext() works"

                swiper.SlideTo testSlideId
                equal swiper.ActiveIndex testSlideId

                swiper.LockSwipeToPrev()
                swiper.SlidePrev()
                equalMsg swiper.ActiveIndex testSlideId "LockSwipeToNext() works"
                swiper.UnlockSwipeToPrev()
                swiper.SlidePrev()
                equalMsg swiper.ActiveIndex (testSlideId-1) "UnlockSwipeToNext() works"
            }
            *)
        }

    [<SPAEntryPoint>]
    let RunTests() =
        Runner.RunTests [
            Tests
        ]
        |> fun a -> a.ReplaceInDom (JS.Document.QuerySelector("#container"))
