namespace WebSharper.Swiper.Tests

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next

[<JavaScript>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.Testing
    open WebSharper.UI.Next
    open WebSharper.UI.Next.Client
    open WebSharper.UI.Next.Html

    let Tests =

        let swiper =
            (*WebSharper.Swiper.Swiper(".swp-cont",
                WebSharper.Swiper.SwipeParameters(WrapperClass = "swp-wrp", SlideClass = "swp-slide")
            )*)
            WebSharper.Swiper.Swiper(".swiper-container")

        TestCategory "General" {

            Test "Default test" {
                equalMsg (swiper.ActiveIndex) 0 "The active index is 0 at the start."
            }

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

            Test "Swiping around the start of the slides" {
                swiper.SlidePrev()
                equalMsg swiper.ActiveIndex 0 "Can not swipe left from the first slide"
                swiper.SlideTo(0)
                swiper.SlideNext()
                equalMsg swiper.ActiveIndex 1 "Slide right from the first slide"
                swiper.SlideTo 0
                equalMsg swiper.ActiveIndex 0 "Slide to id 0"
            }



        }

#if ZAFIR
    let RunTests() =
        Runner.RunTests [
            Tests
        ]
#endif