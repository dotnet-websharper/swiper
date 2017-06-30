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

        }

#if ZAFIR
    let RunTests() =
        Runner.RunTests [
            Tests
        ]
#endif