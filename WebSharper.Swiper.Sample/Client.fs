namespace WebSharper.Swiper.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Templating
open WebSharper.Swiper

[<JavaScript>]
module Client =

    // The template is the whole document, loaded from the DOM, so editing
    // index.html and refreshing the browser is enough.
    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    // ------------------------------------------------------------------
    // State bound into the template
    // ------------------------------------------------------------------

    // Theme: light is the default; the choice is persisted in localStorage.
    // The storage key is shared with the bootstrap script in index.html.
    let themeStorageKey = "websharper-swiper-theme"
    let theme = Var.Create "light"
    let themeLabel = Var.Create "🌙 Dark mode"

    let status = Var.Create "Initializing the slider…"
    let activeIndex = Var.Create "0"
    let realIndex = Var.Create "0"
    let progress = Var.Create "0%"
    let translate = Var.Create "0 px"
    let slideCount = Var.Create "0"
    let perView = Var.Create "1"
    let direction = Var.Create "horizontal"
    let beginEnd = Var.Create "true / false"
    let animating = Var.Create "false"
    let autoplayState = Var.Create "stopped"
    let zoomScale = Var.Create "1.00"
    let autoplayLabel = Var.Create "Start autoplay"
    let keyboardLabel = Var.Create "Keyboard: on"
    let effectName = Var.Create "slide"

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <- if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try JS.Window.LocalStorage.SetItem(themeStorageKey, theme.Value) with _ -> ()
        applyTheme ()

    // ------------------------------------------------------------------
    // Instances
    // ------------------------------------------------------------------

    let mutable hero : Swiper option = None
    let mutable thumbs : Swiper option = None
    let mutable effect : Swiper option = None

    let withHero (f: Swiper -> unit) =
        match hero with
        | Some sw -> f sw
        | None -> status.Value <- "The slider is not ready yet."

    // ------------------------------------------------------------------
    // Readouts
    // ------------------------------------------------------------------

    /// Copies the typed instance state into the bound Vars.
    let refresh (sw: Swiper) =
        activeIndex.Value <- string sw.ActiveIndex
        realIndex.Value <- string sw.RealIndex
        progress.Value <- sprintf "%.0f%%" (sw.Progress * 100.)
        translate.Value <- sprintf "%.0f px" sw.Translate
        slideCount.Value <- string sw.Slides.Length
        perView.Value <- string (sw.SlidesPerViewDynamic())
        beginEnd.Value <- sprintf "%b / %b" sw.IsBeginning sw.IsEnd
        animating.Value <- string sw.Animating
        zoomScale.Value <- sprintf "%.2f" sw.Zoom.Scale
        autoplayState.Value <- if sw.Autoplay.Running then "running" else "stopped"
        autoplayLabel.Value <- if sw.Autoplay.Running then "Stop autoplay" else "Start autoplay"
        keyboardLabel.Value <-
            if sw.Keyboard.Enabled then "Keyboard: on" else "Keyboard: off"

    let onHandlers () =
        SwiperEventHandlers(
            Init = (fun sw ->
                status.Value <- "Swiper initialised."
                refresh sw),
            SlideChange = (fun sw ->
                status.Value <- sprintf "Slide %d of %d." (sw.ActiveIndex + 1) sw.Slides.Length
                refresh sw),
            Progress = (fun _ value -> progress.Value <- sprintf "%.0f%%" (value * 100.)),
            SlideChangeTransitionEnd = (fun sw -> refresh sw),
            AutoplayStart = (fun _ ->
                autoplayState.Value <- "running"
                autoplayLabel.Value <- "Stop autoplay"),
            AutoplayStop = (fun _ ->
                autoplayState.Value <- "stopped"
                autoplayLabel.Value <- "Start autoplay"),
            ZoomChange = (fun _ scale _ _ -> zoomScale.Value <- sprintf "%.2f" scale),
            Breakpoint = (fun _ _ -> status.Value <- "Breakpoint crossed."))

    // ------------------------------------------------------------------
    // Hero slider
    // ------------------------------------------------------------------

    let heroOptions (thumbsSwiper: Swiper) =
        SwiperOptions(
            Modules =
                [| SwiperModule.Navigation
                   SwiperModule.Pagination
                   SwiperModule.Scrollbar
                   SwiperModule.Keyboard
                   SwiperModule.Mousewheel
                   SwiperModule.Autoplay
                   SwiperModule.Zoom
                   SwiperModule.Manipulation
                   SwiperModule.Thumbs
                   SwiperModule.EffectFade |],
            Effect = Effect.Fade,
            FadeEffect = FadeEffectOptions(CrossFade = true),
            Speed = 450,
            SpaceBetween = 18.,
            SlidesPerView = 1.,
            Rewind = true,
            GrabCursor = true,
            WatchSlidesProgress = true,
            Keyboard = KeyboardOptions(Enabled = true, OnlyInViewport = true),
            Mousewheel = MousewheelOptions(Enabled = true, ForceToAxis = true),
            Autoplay = AutoplayOptions(Enabled = false, Delay = 2800, PauseOnMouseEnter = true),
            Zoom = ZoomOptions(Enabled = true, MaxRatio = 3., MinRatio = 1.),
            Navigation = NavigationOptions(NextEl = "#hero-next", PrevEl = "#hero-prev"),
            Pagination =
                PaginationOptions(
                    El = "#hero-pagination",
                    Type = PaginationType.Bullets,
                    Clickable = true,
                    DynamicBullets = true),
            Scrollbar = ScrollbarOptions(El = "#hero-scrollbar", Draggable = true),
            Thumbs = ThumbsOptions(Swiper = thumbsSwiper),
            On = onHandlers ())

    let createHero () =
        hero |> Option.iter (fun sw -> sw.Destroy())
        match thumbs with
        | None -> status.Value <- "The thumbs slider is not ready yet."
        | Some thumbsSwiper ->
            let sw = new Swiper("#hero-swiper", heroOptions thumbsSwiper)
            // `On` also works on the instance, for handlers added after creation.
            sw.On("reachEnd", fun (sw: Swiper) -> status.Value <- "Reached the last slide.")
            |> ignore
            hero <- Some sw
            // The thumbs swiper receives its `thumbs` object from the hero's module.
            thumbsSwiper.Thumbs.Update(true)
            refresh sw
            status.Value <- "Hero slider ready — use the buttons or swipe."

    // ------------------------------------------------------------------
    // Thumbs slider
    // ------------------------------------------------------------------

    // Two breakpoints keep the thumbs readable on narrow screens.
    let thumbsBreakpoints () =
        let breakpoints = Breakpoints.Empty
        breakpoints.["640"] <- SwiperOptions(SlidesPerView = 2., SpaceBetween = 8.)
        breakpoints.["960"] <- SwiperOptions(SlidesPerView = 3., SpaceBetween = 10.)
        breakpoints

    // Created before the hero: the hero's Thumbs module drives this slider.
    let createThumbs () =
        thumbs |> Option.iter (fun sw -> sw.Destroy())
        let sw =
            new Swiper(
                "#thumbs-swiper",
                SwiperOptions(
                    Modules = [| SwiperModule.Thumbs; SwiperModule.FreeMode; SwiperModule.Grid |],
                    SlidesPerView = 3.,
                    SpaceBetween = 10.,
                    WatchSlidesProgress = true,
                    FreeMode = FreeModeOptions(Enabled = true, Sticky = true),
                    Grid = GridOptions(Rows = 1, Fill = GridFill.Row),
                    Breakpoints = thumbsBreakpoints (),
                    On =
                        SwiperEventHandlers(
                            SlideChange =
                                fun t -> status.Value <- sprintf "Thumb %d." (t.ActiveIndex + 1))))
        thumbs <- Some sw

    // ------------------------------------------------------------------
    // Effects slider (re-created for each effect)
    // ------------------------------------------------------------------

    let effectModules (name: string) : SwiperModule[] =
        match name with
        | "fade" -> [| SwiperModule.EffectFade |]
        | "cube" -> [| SwiperModule.EffectCube |]
        | "coverflow" -> [| SwiperModule.EffectCoverflow |]
        | "flip" -> [| SwiperModule.EffectFlip |]
        | "creative" -> [| SwiperModule.EffectCreative |]
        | "cards" -> [| SwiperModule.EffectCards |]
        | _ -> [||]

    let effectValue (name: string) =
        match name with
        | "fade" -> Effect.Fade
        | "cube" -> Effect.Cube
        | "coverflow" -> Effect.Coverflow
        | "flip" -> Effect.Flip
        | "creative" -> Effect.Creative
        | "cards" -> Effect.Cards
        | _ -> Effect.Slide

    let setActiveTab (name: string) =
        for candidate in [ "slide"; "fade"; "cube"; "coverflow"; "flip"; "creative"; "cards" ] do
            let el = JS.Document.GetElementById("tab-" + candidate)
            if not (isNull el) then
                if candidate = name then
                    el.ClassList.Add("is-active")
                else
                    el.ClassList.Remove("is-active")

    let createEffect (name: string) =
        effect |> Option.iter (fun sw -> sw.Destroy())
        let modules = Array.append [| SwiperModule.Pagination |] (effectModules name)
        let options =
            SwiperOptions(
                Modules = modules,
                Effect = effectValue name,
                Speed = 600,
                SlidesPerView = 1.,
                SpaceBetween = 14.,
                FadeEffect = FadeEffectOptions(CrossFade = true),
                CubeEffect =
                    CubeEffectOptions(
                        SlideShadows = true,
                        Shadow = true,
                        ShadowOffset = 14.,
                        ShadowScale = 0.94),
                CoverflowEffect =
                    CoverflowEffectOptions(
                        SlideShadows = true,
                        Rotate = 36.,
                        Stretch = 0.,
                        Depth = 120.,
                        Modifier = 1.,
                        Scale = 1.),
                FlipEffect = FlipEffectOptions(SlideShadows = true, LimitRotation = false),
                CardsEffect =
                    CardsEffectOptions(
                        SlideShadows = true,
                        Rotate = true,
                        PerSlideRotate = 3.,
                        PerSlideOffset = 8.),
                CreativeEffect =
                    CreativeEffectOptions(
                        Prev =
                            CreativeEffectTransform(
                                Translate = [| box "-40%"; box 0.; box -120. |],
                                Rotate = [| 0.; 0.; -25. |],
                                Opacity = 0.,
                                Scale = 0.85),
                        Next =
                            CreativeEffectTransform(
                                Translate = [| box "40%"; box 0.; box -120. |],
                                Rotate = [| 0.; 0.; 25. |],
                                Opacity = 0.,
                                Scale = 0.85),
                        LimitProgress = 2.,
                        Perspective = true),
                Pagination = PaginationOptions(El = "#effect-pagination", Clickable = true),
                On =
                    SwiperEventHandlers(
                        SlideChange =
                            fun sw -> status.Value <- sprintf "Effect slider: slide %d." (sw.ActiveIndex + 1)))
        effect <- Some (new Swiper("#effect-swiper", options))
        effectName.Value <- name
        setActiveTab name

    // ------------------------------------------------------------------
    // Toolbar actions
    // ------------------------------------------------------------------

    let next () = withHero (fun sw -> sw.SlideNext() |> ignore)

    let prev () = withHero (fun sw -> sw.SlidePrev() |> ignore)

    let slideToThird () =
        withHero (fun sw ->
            let moved = sw.SlideTo(2)
            status.Value <- sprintf "SlideTo(2) returned %b." moved)

    let toggleAutoplay () =
        withHero (fun sw ->
            if sw.Autoplay.Running then
                sw.Autoplay.Stop() |> ignore
                status.Value <- "Autoplay stopped."
            else
                sw.Autoplay.Start() |> ignore
                status.Value <- "Autoplay started.")

    let zoomIn () =
        withHero (fun sw ->
            sw.Zoom.In(1.6)
            status.Value <- "Zoom in.")

    let zoomOut () =
        withHero (fun sw ->
            sw.Zoom.Out()
            status.Value <- "Zoom out.")

    let toggleKeyboard () =
        withHero (fun sw ->
            if sw.Keyboard.Enabled then sw.Keyboard.Disable() else sw.Keyboard.Enable()
            refresh sw
            status.Value <- sprintf "Keyboard enabled: %b." sw.Keyboard.Enabled)

    let toggleDirection () =
        withHero (fun sw ->
            sw.ChangeDirection() |> ignore
            direction.Value <- if direction.Value = "horizontal" then "vertical" else "horizontal"
            refresh sw
            status.Value <- sprintf "Direction changed to %s." direction.Value)

    let appendSlide () =
        withHero (fun sw ->
            let slide =
                Elt.div [ attr.``class`` "swiper-slide" ] [
                    div [ attr.``class`` "hero-slide slide-violet" ] [
                        span [ attr.``class`` "slide-number" ] [ text "NEW" ]
                        h3 [] [ text "Appended slide" ]
                        p [] [ text "Added with AppendSlide and published with Update()." ]
                    ]
                ]
            sw.AppendSlide(As<HTMLElement> slide.Dom)
            sw.Update()
            refresh sw
            status.Value <- sprintf "Appended a slide — %d slides now." sw.Slides.Length)

    let removeLast () =
        withHero (fun sw ->
            if sw.Slides.Length > 2 then
                sw.RemoveSlide(sw.Slides.Length - 1)
                sw.Update()
                refresh sw
                status.Value <- sprintf "Removed the last slide — %d slides left." sw.Slides.Length
            else
                status.Value <- "Keeping at least two slides.")

    let setProgress () =
        withHero (fun sw ->
            sw.SetProgress(0.5, 400)
            status.Value <- "SetProgress(0.5).")

    let toggleEnabled () =
        withHero (fun sw ->
            if sw.Enabled then sw.Disable() else sw.Enable()
            status.Value <- if sw.Enabled then "Slider enabled." else "Slider disabled.")

    let update () =
        withHero (fun sw ->
            sw.Update()
            refresh sw
            status.Value <-
                sprintf "Update() — %d slides, %d per view." sw.Slides.Length (sw.SlidesPerViewDynamic()))

    // ------------------------------------------------------------------
    // Entry point
    // ------------------------------------------------------------------

    [<SPAEntryPoint>]
    let Main () =
        // Restore a previously chosen theme (light is the default).
        let savedTheme =
            try JS.Window.LocalStorage.GetItem(themeStorageKey) with _ -> null
        if savedTheme = "dark" || savedTheme = "light" then
            theme.Value <- savedTheme
        applyTheme ()

        // `.Bind()` renders the whole document body, so the slider markup is
        // only in the DOM (and can be initialised) after this call.
        IndexTemplate()
            .themeLabel(themeLabel.V)
            .autoplayLabel(autoplayLabel.V)
            .keyboardLabel(keyboardLabel.V)
            .activeIndex(activeIndex.V)
            .realIndex(realIndex.V)
            .progress(progress.V)
            .translate(translate.V)
            .slideCount(slideCount.V)
            .perView(perView.V)
            .direction(direction.V)
            .beginEnd(beginEnd.V)
            .animating(animating.V)
            .autoplayState(autoplayState.V)
            .zoomScale(zoomScale.V)
            .status(status.V)
            .effectName(effectName.V)
            .ToggleTheme(fun _ -> toggleTheme ())
            .Prev(fun _ -> prev ())
            .Next(fun _ -> next ())
            .SlideToThird(fun _ -> slideToThird ())
            .ToggleAutoplay(fun _ -> toggleAutoplay ())
            .ZoomIn(fun _ -> zoomIn ())
            .ZoomOut(fun _ -> zoomOut ())
            .ToggleKeyboard(fun _ -> toggleKeyboard ())
            .ToggleDirection(fun _ -> toggleDirection ())
            .AppendSlide(fun _ -> appendSlide ())
            .RemoveLast(fun _ -> removeLast ())
            .SetProgress(fun _ -> setProgress ())
            .ToggleEnabled(fun _ -> toggleEnabled ())
            .Update(fun _ -> update ())
            .Reset(fun _ -> createHero ())
            .UseSlide(fun _ -> createEffect "slide")
            .UseFade(fun _ -> createEffect "fade")
            .UseCube(fun _ -> createEffect "cube")
            .UseCoverflow(fun _ -> createEffect "coverflow")
            .UseFlip(fun _ -> createEffect "flip")
            .UseCreative(fun _ -> createEffect "creative")
            .UseCards(fun _ -> createEffect "cards")
            .Bind()

        createThumbs ()
        createHero ()
        createEffect "slide"