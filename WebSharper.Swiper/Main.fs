// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2026 IntelliFactory
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
namespace WebSharper.Swiper

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

/// <summary>Typed bindings for <a href="https://swiperjs.com/">Swiper</a> 14.4.0,
/// the modern touch slider.</summary>
/// <remarks>
/// The binding is npm based: the <c>Swiper</c> class is the default import from the
/// <c>swiper</c> package and every feature module is a named import from
/// <c>swiper/modules</c> (<c>SwiperModule.Navigation</c>, <c>SwiperModule.Pagination</c>, …).
/// Modules have to be listed in <see cref="T:WebSharper.Swiper.SwiperOptions"/>.<c>Modules</c>
/// (or installed once with <c>Swiper.Use</c>) before their options and instance APIs work.
/// The sample bundles the npm package with esbuild; there is no global <c>Swiper</c>
/// object and no CDN <c>&lt;script&gt;</c>.
/// </remarks>
module Definition =

    // ------------------------------------------------------------------
    // Forward declarations
    //
    // Several option objects and the Swiper class refer to each other. The
    // classes are created up front and their members are added below, once
    // every type they need exists.
    // ------------------------------------------------------------------

    let SwiperClass = Class "Swiper"

    /// A Swiper feature module, as imported from <c>swiper/modules</c>.
    let SwiperModule = Class "SwiperModule"

    /// An element of the slides wrapper. Swiper annotates slides with the
    /// computed offsets/progress, which are exposed as extra properties.
    let SwiperSlideElement =
        Class "SwiperSlideElement"
        |=> Inherits T<HTMLElement>

    let SwiperEventHandlers = Class "SwiperEventHandlers"

    let Breakpoints = Class "Breakpoints"

    // Module instance APIs: every module decorates the Swiper instance with
    // its own typed sub-object (swiper.navigation, swiper.pagination, …).
    let SwiperNavigation = Class "SwiperNavigation"
    let SwiperPagination = Class "SwiperPagination"
    let SwiperScrollbar = Class "SwiperScrollbar"
    let SwiperAutoplay = Class "SwiperAutoplay"
    let SwiperController = Class "SwiperController"
    let SwiperKeyboard = Class "SwiperKeyboard"
    let SwiperMousewheel = Class "SwiperMousewheel"
    let SwiperZoom = Class "SwiperZoom"
    let SwiperFreeMode = Class "SwiperFreeMode"
    let SwiperGrid = Class "SwiperGrid"
    let SwiperThumbs = Class "SwiperThumbs"
    let SwiperVirtual = Class "SwiperVirtual"
    let SwiperParallax = Class "SwiperParallax"
    let SwiperA11y = Class "SwiperA11y"
    let SwiperHashNavigation = Class "SwiperHashNavigation"
    let SwiperHistory = Class "SwiperHistory"

    // ------------------------------------------------------------------
    // String-union enumerations
    // ------------------------------------------------------------------

    let Direction =
        Pattern.EnumStrings "Direction" [
            "horizontal"
            "vertical"
        ]

    let Effect =
        Pattern.EnumStrings "Effect" [
            "slide"
            "fade"
            "cube"
            "coverflow"
            "flip"
            "creative"
            "cards"
        ]

    let TouchEventsTarget =
        Pattern.EnumStrings "TouchEventsTarget" [
            "container"
            "wrapper"
        ]

    let SlidesPerViewAuto =
        Pattern.EnumStrings "SlidesPerViewAuto" [ "auto" ]

    let GridFill =
        Pattern.EnumStrings "GridFill" [
            "row"
            "column"
        ]

    let PaginationType =
        Pattern.EnumStrings "PaginationType" [
            "bullets"
            "fraction"
            "progressbar"
            "custom"
        ]

    let ControllerBy =
        Pattern.EnumStrings "ControllerBy" [
            "slide"
            "container"
        ]

    let FadeMode =
        Pattern.EnumInlines "FadeMode" [
            "Default", "'default'"
            "CrossFade", "'cross-fade'"
            "OutIn", "'out-in'"
        ]

    let SwipeDirection =
        Pattern.EnumStrings "SwipeDirection" [
            "prev"
            "next"
        ]

    let TranslateAxis =
        Pattern.EnumStrings "TranslateAxis" [
            "x"
            "y"
        ]

    let LanguageDirection =
        Pattern.EnumStrings "LanguageDirection" [
            "rtl"
            "ltr"
        ]

    let SlidesPerViewDynamicView =
        Pattern.EnumStrings "SlidesPerViewDynamicView" [
            "current"
            "previous"
        ]

    let MousewheelEventsTarget =
        Pattern.EnumStrings "MousewheelEventsTarget" [
            "container"
            "wrapper"
        ]

    // ------------------------------------------------------------------
    // Plain object shapes
    // ------------------------------------------------------------------

    /// The browser-support flags Swiper exposes as <c>swiper.support</c>.
    let SwiperSupport =
        Pattern.Config "SwiperSupport" {
            Required = [ "touch", T<bool> ]
            Optional = []
        }

    /// The device flags Swiper exposes as <c>swiper.device</c>.
    let SwiperDevice =
        Pattern.Config "SwiperDevice" {
            Required =
                [
                    "ios", T<bool>
                    "android", T<bool>
                    "os", T<string>
                ]
            Optional = []
        }

    /// The browser flags Swiper exposes as <c>swiper.browser</c>.
    let SwiperBrowser =
        Pattern.Config "SwiperBrowser" {
            Required =
                [
                    "isSafari", T<bool>
                    "isWebView", T<bool>
                    "need3dFix", T<bool>
                ]
            Optional = []
        }

    /// The current touch positions, as exposed by <c>swiper.touches</c>.
    let SwiperTouches =
        Pattern.Config "SwiperTouches" {
            Required =
                [
                    "startX", T<float>
                    "startY", T<float>
                    "currentX", T<float>
                    "currentY", T<float>
                    "diff", T<float>
                ]
            Optional =
                [
                    "previousX", T<float>
                    "previousY", T<float>
                ]
        }

    /// The slide payload of the virtual module: an arbitrary item, rendered by
    /// <c>VirtualOptions.RenderSlide</c>.
    let SwiperVirtualSlide = T<obj>

    /// A slide passed to <c>AppendSlide</c>/<c>PrependSlide</c>/<c>AddSlide</c>/
    /// <c>RemoveSlide</c>: an element, a selector, or an array of either.
    let SlideInput =
        T<HTMLElement>
        + T<string>
        + (!| (T<HTMLElement> + T<string>))

    /// A CSS selector or an element, as accepted by <c>el</c>/<c>nextEl</c>/…
    let SelectorOrElement = T<string> + T<HTMLElement>

    // ------------------------------------------------------------------
    // Effect options
    // ------------------------------------------------------------------

    /// Options for the fade effect (<c>EffectFade</c> module).
    let FadeEffectOptions =
        Pattern.Config "FadeEffectOptions" {
            Required = []
            Optional =
                [
                    "mode", FadeMode.Type
                    "crossFade", T<bool>
                ]
        }

    /// Options for the cube effect (<c>EffectCube</c> module).
    let CubeEffectOptions =
        Pattern.Config "CubeEffectOptions" {
            Required = []
            Optional =
                [
                    "slideShadows", T<bool>
                    "shadow", T<bool>
                    "shadowOffset", T<float>
                    "shadowScale", T<float>
                ]
        }

    /// Options for the coverflow effect (<c>EffectCoverflow</c> module).
    let CoverflowEffectOptions =
        Pattern.Config "CoverflowEffectOptions" {
            Required = []
            Optional =
                [
                    "slideShadows", T<bool>
                    "rotate", T<float>
                    "stretch", T<float> + T<string>
                    "depth", T<float>
                    "scale", T<float>
                    "modifier", T<float> + (T<float> ^-> T<float>)
                ]
        }

    /// Options for the flip effect (<c>EffectFlip</c> module).
    let FlipEffectOptions =
        Pattern.Config "FlipEffectOptions" {
            Required = []
            Optional =
                [
                    "slideShadows", T<bool>
                    "limitRotation", T<bool>
                ]
        }

    /// Options for the cards effect (<c>EffectCards</c> module).
    let CardsEffectOptions =
        Pattern.Config "CardsEffectOptions" {
            Required = []
            Optional =
                [
                    "slideShadows", T<bool>
                    "rotate", T<bool>
                    "perSlideRotate", T<float>
                    "perSlideOffset", T<float>
                ]
        }

    /// One of the two transforms of the creative effect.
    let CreativeEffectTransform =
        Pattern.Config "CreativeEffectTransform" {
            Required = []
            Optional =
                [
                    "translate", !| (T<string> + T<float>)
                    "rotate", !| T<float>
                    "opacity", T<float>
                    "scale", T<float>
                    "shadow", T<bool>
                    "origin", T<string>
                ]
        }

    /// Options for the creative effect (<c>EffectCreative</c> module).
    let CreativeEffectOptions =
        Pattern.Config "CreativeEffectOptions" {
            Required = []
            Optional =
                [
                    "prev", CreativeEffectTransform.Type
                    "next", CreativeEffectTransform.Type
                    "limitProgress", T<float>
                    "shadowPerProgress", T<bool>
                    "progressMultiplier", T<float>
                    "perspective", T<bool>
                ]
        }

    // ------------------------------------------------------------------
    // Module options
    // ------------------------------------------------------------------

    /// Options for the navigation module (<c>SwiperModule.Navigation</c>).
    let NavigationOptions =
        Pattern.Config "NavigationOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "nextEl", SelectorOrElement
                    "prevEl", SelectorOrElement
                    "addIcons", T<bool>
                    "hideOnClick", T<bool>
                    "disabledClass", T<string>
                    "hiddenClass", T<string>
                    "lockClass", T<string>
                    "navigationDisabledClass", T<string>
                ]
        }

    /// Options for the pagination module (<c>SwiperModule.Pagination</c>).
    let PaginationOptions =
        Pattern.Config "PaginationOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "el", SelectorOrElement
                    "type", PaginationType.Type
                    "bulletElement", T<string>
                    "dynamicBullets", T<bool>
                    "dynamicMainBullets", T<int>
                    "hideOnClick", T<bool>
                    "clickable", T<bool>
                    "progressbarOpposite", T<bool>
                    "formatFractionCurrent", T<float> ^-> (T<float> + T<string>)
                    "formatFractionTotal", T<float> ^-> (T<float> + T<string>)
                    "renderBullet", (T<int> * T<string>) ^-> T<string>
                    "renderFraction", (T<string> * T<string>) ^-> T<string>
                    "renderProgressbar", T<string> ^-> T<string>
                    "renderCustom", (SwiperClass * T<int> * T<int>) ^-> T<string>
                    "bulletClass", T<string>
                    "bulletActiveClass", T<string>
                    "modifierClass", T<string>
                    "currentClass", T<string>
                    "totalClass", T<string>
                    "hiddenClass", T<string>
                    "progressbarFillClass", T<string>
                    "progressbarOppositeClass", T<string>
                    "clickableClass", T<string>
                    "lockClass", T<string>
                    "horizontalClass", T<string>
                    "verticalClass", T<string>
                    "paginationDisabledClass", T<string>
                ]
        }

    /// Options for the scrollbar module (<c>SwiperModule.Scrollbar</c>).
    let ScrollbarOptions =
        Pattern.Config "ScrollbarOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "el", SelectorOrElement
                    "hide", T<bool>
                    "draggable", T<bool>
                    "snapOnRelease", T<bool>
                    "dragSize", T<float> + T<string>
                    "lockClass", T<string>
                    "dragClass", T<string>
                    "scrollbarDisabledClass", T<string>
                    "horizontalClass", T<string>
                    "verticalClass", T<string>
                ]
        }

    /// Options for the autoplay module (<c>SwiperModule.Autoplay</c>).
    let AutoplayOptions =
        Pattern.Config "AutoplayOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "delay", T<int>
                    "stopOnLastSlide", T<bool>
                    "disableOnInteraction", T<bool>
                    "reverseDirection", T<bool>
                    "waitForTransition", T<bool>
                    "pauseOnMouseEnter", T<bool>
                ]
        }

    /// Options for the controller module (<c>SwiperModule.Controller</c>).
    let ControllerOptions =
        Pattern.Config "ControllerOptions" {
            Required = []
            Optional =
                [
                    "control",
                        SwiperClass
                        + (!| SwiperClass)
                        + T<string>
                        + T<HTMLElement>
                    "inverse", T<bool>
                    "by", ControllerBy.Type
                ]
        }

    /// Options for the keyboard module (<c>SwiperModule.Keyboard</c>).
    let KeyboardOptions =
        Pattern.Config "KeyboardOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "onlyInViewport", T<bool>
                    "pageUpDown", T<bool>
                    "speed", T<int>
                ]
        }

    /// Options for the mousewheel module (<c>SwiperModule.Mousewheel</c>).
    let MousewheelOptions =
        Pattern.Config "MousewheelOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "forceToAxis", T<bool>
                    "releaseOnEdges", T<bool>
                    "invert", T<bool>
                    "sensitivity", T<float>
                    "eventsTarget",
                        MousewheelEventsTarget
                        + T<string>
                        + T<HTMLElement>
                    "thresholdDelta", T<float>
                    "thresholdTime", T<float>
                    "noMousewheelClass", T<string>
                ]
        }

    /// Options for the zoom module (<c>SwiperModule.Zoom</c>).
    let ZoomOptions =
        Pattern.Config "ZoomOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "limitToOriginalSize", T<bool>
                    "maxRatio", T<float>
                    "minRatio", T<float>
                    "panOnMouseMove", T<bool>
                    "toggle", T<bool>
                    "containerClass", T<string>
                    "zoomedSlideClass", T<string>
                ]
        }

    /// Options for the free mode module (<c>SwiperModule.FreeMode</c>).
    let FreeModeOptions =
        Pattern.Config "FreeModeOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "momentum", T<bool>
                    "momentumRatio", T<float>
                    "momentumVelocityRatio", T<float>
                    "momentumBounce", T<bool>
                    "momentumBounceRatio", T<float>
                    "minimumVelocity", T<float>
                    "sticky", T<bool>
                ]
        }

    /// Options for the grid module (<c>SwiperModule.Grid</c>).
    let GridOptions =
        Pattern.Config "GridOptions" {
            Required = []
            Optional =
                [
                    "rows", T<int>
                    "fill", GridFill.Type
                ]
        }

    /// Options for the thumbs module (<c>SwiperModule.Thumbs</c>).
    let ThumbsOptions =
        Pattern.Config "ThumbsOptions" {
            Required = []
            Optional =
                [
                    "swiper",
                        SwiperClass
                        + T<string>
                        + T<HTMLElement>
                    "slideThumbActiveClass", T<string>
                    "thumbsContainerClass", T<string>
                    "multipleActiveThumbs", T<bool>
                    "autoScrollOffset", T<int>
                ]
        }

    /// Options of a <c>Thumbs.Update</c> call.
    let ThumbsUpdateOptions =
        Pattern.Config "ThumbsUpdateOptions" {
            Required = []
            Optional = [ "autoScroll", T<bool> ]
        }

    /// The data the virtual module hands to <c>RenderExternal</c>.
    let VirtualData =
        Pattern.Config "VirtualData" {
            Required =
                [
                    "offset", T<float>
                    "from", T<int>
                    "to", T<int>
                    "slides", !| SwiperVirtualSlide
                ]
            Optional = []
        }

    /// Options for the virtual module (<c>SwiperModule.Virtual</c>).
    let VirtualOptions =
        Pattern.Config "VirtualOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "slides", !| SwiperVirtualSlide
                    "slidesPerViewAutoSlideSize", T<float>
                    "cache", T<bool>
                    "addSlidesBefore", T<int>
                    "addSlidesAfter", T<int>
                    "renderSlide",
                        (SwiperVirtualSlide * T<int>) ^-> (T<HTMLElement> + T<string>)
                    "renderExternal", VirtualData ^-> T<unit>
                    "renderExternalUpdate", T<bool>
                ]
        }

    /// Options for the parallax module (<c>SwiperModule.Parallax</c>).
    let ParallaxOptions =
        Pattern.Config "ParallaxOptions" {
            Required = []
            Optional = [ "enabled", T<bool> ]
        }

    /// Options for the a11y module (<c>SwiperModule.A11y</c>).
    let A11yOptions =
        Pattern.Config "A11yOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "prevSlideMessage", T<string>
                    "nextSlideMessage", T<string>
                    "firstSlideMessage", T<string>
                    "lastSlideMessage", T<string>
                    "paginationBulletMessage", T<string>
                    "notificationClass", T<string>
                    "containerMessage", T<string>
                    "containerRoleDescriptionMessage", T<string>
                    "containerRole", T<string>
                    "itemRoleDescriptionMessage", T<string>
                    "slideLabelMessage", T<string>
                    "slideRole", T<string>
                    "id", T<string> + T<int>
                    "scrollOnFocus", T<bool>
                    "wrapperLiveRegion", T<bool>
                ]
        }

    /// Options for the hash navigation module
    /// (<c>SwiperModule.HashNavigation</c>).
    let HashNavigationOptions =
        Pattern.Config "HashNavigationOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "watchState", T<bool>
                    "replaceState", T<bool>
                    "getSlideIndex", (SwiperClass * T<string>) ^-> T<int>
                ]
        }

    /// Options for the history module (<c>SwiperModule.History</c>).
    let HistoryOptions =
        Pattern.Config "HistoryOptions" {
            Required = []
            Optional =
                [
                    "enabled", T<bool>
                    "root", T<string>
                    "replaceState", T<bool>
                    "key", T<string>
                    "keepQuery", T<bool>
                ]
        }

    // ------------------------------------------------------------------
    // Swiper options
    // ------------------------------------------------------------------

    /// <summary>The configuration object of <c>new Swiper(el, options)</c>.</summary>
    /// <remarks>
    /// Every module's options live in the same flat object, exactly as in the
    /// JavaScript API: <c>Navigation</c>, <c>Pagination</c>, <c>Scrollbar</c>,
    /// <c>Autoplay</c>, <c>Keyboard</c>, <c>Mousewheel</c>, <c>Zoom</c>,
    /// <c>FreeMode</c>, <c>Grid</c>, <c>Thumbs</c>, <c>Virtual</c>,
    /// <c>Parallax</c>, <c>A11y</c>, <c>Controller</c>, <c>HashNavigation</c>,
    /// <c>History</c>, <c>FadeEffect</c>, <c>CubeEffect</c>, <c>CoverflowEffect</c>,
    /// <c>FlipEffect</c>, <c>CardsEffect</c> and <c>CreativeEffect</c>.
    /// The JavaScript boolean shorthand (<c>navigation: true</c>) is expressed by
    /// that module's <c>Enabled</c> option instead.
    /// </remarks>
    let SwiperOptions =
        Pattern.Config "SwiperOptions" {
            Required = []
            Optional =
                [
                    // Modules
                    "modules", !| SwiperModule
                    "injectStyles", !| T<string>
                    "injectStylesUrls", !| T<string>
                    // Core
                    "el", SelectorOrElement
                    "init", T<bool>
                    "enabled", T<bool>
                    "updateOnWindowResize", T<bool>
                    "resizeObserver", T<bool>
                    "initialSlide", T<int>
                    "direction", Direction.Type
                    "oneWayMovement", T<bool>
                    "swiperElementNodeName", T<string>
                    "speed", T<int>
                    "setWrapperSize", T<bool>
                    "virtualTranslate", T<bool>
                    "width", T<float>
                    "height", T<float>
                    "autoHeight", T<bool>
                    "roundLengths", T<bool>
                    "nested", T<bool>
                    "createElements", T<bool>
                    "eventsPrefix", T<string>
                    "focusableElements", T<string>
                    "uniqueNavElements", T<bool>
                    "effect", Effect.Type
                    "runCallbacksOnInit", T<bool>
                    "watchOverflow", T<bool>
                    "userAgent", T<string>
                    "url", T<string>
                    "cssMode", T<bool>
                    // Slides grid
                    "spaceBetween", T<float> + T<string>
                    "slidesPerView", T<float> + SlidesPerViewAuto.Type
                    "maxBackfaceHiddenSlides", T<int>
                    "slidesPerGroup", T<int>
                    "slidesPerGroupSkip", T<int>
                    "slidesPerGroupAuto", T<bool>
                    "centeredSlides", T<bool>
                    "centeredSlidesBounds", T<bool>
                    "slidesOffsetBefore", T<int>
                    "slidesOffsetAfter", T<int>
                    "normalizeSlideIndex", T<bool>
                    "centerInsufficientSlides", T<bool>
                    "snapToSlideEdge", T<bool>
                    // Interaction
                    "grabCursor", T<bool>
                    "touchEventsTarget", TouchEventsTarget.Type
                    "touchRatio", T<float>
                    "touchAngle", T<float>
                    "simulateTouch", T<bool>
                    "shortSwipes", T<bool>
                    "longSwipes", T<bool>
                    "longSwipesRatio", T<float>
                    "longSwipesMs", T<int>
                    "followFinger", T<bool>
                    "allowTouchMove", T<bool>
                    "threshold", T<float>
                    "touchStartPreventDefault", T<bool>
                    "touchStartForcePreventDefault", T<bool>
                    "touchMoveStopPropagation", T<bool>
                    "edgeSwipeDetection", T<bool> + T<string>
                    "edgeSwipeThreshold", T<int>
                    "touchReleaseOnEdges", T<bool>
                    "passiveListeners", T<bool>
                    "resistance", T<bool>
                    "resistanceRatio", T<float>
                    "preventInteractionOnTransition", T<bool>
                    "allowSlidePrev", T<bool>
                    "allowSlideNext", T<bool>
                    "noSwiping", T<bool>
                    "noSwipingClass", T<string>
                    "noSwipingSelector", T<string>
                    "swipeHandler", SelectorOrElement
                    "preventClicks", T<bool>
                    "preventClicksPropagation", T<bool>
                    "slideToClickedSlide", T<bool>
                    "watchSlidesProgress", T<bool>
                    // Loop
                    "loop", T<bool>
                    "loopAddBlankSlides", T<bool>
                    "loopAdditionalSlides", T<int>
                    "loopPreventsSliding", T<bool>
                    "rewind", T<bool>
                    // Breakpoints and observer
                    "breakpoints", Breakpoints.Type
                    "breakpointsBase", T<string>
                    "observer", T<bool>
                    "observeSlideChildren", T<bool>
                    "observeParents", T<bool>
                    // Class names
                    "containerModifierClass", T<string>
                    "slideClass", T<string>
                    "slideActiveClass", T<string>
                    "slideVisibleClass", T<string>
                    "slideFullyVisibleClass", T<string>
                    "slideBlankClass", T<string>
                    "slideNextClass", T<string>
                    "slidePrevClass", T<string>
                    "wrapperClass", T<string>
                    "lazyPreload", T<bool>
                    "lazyPreloaderClass", T<string>
                    "lazyPreloadPrevNext", T<int>
                    // Events
                    "on", SwiperEventHandlers.Type
                    "onAny", (T<string> * T<obj>) ^-> T<unit>
                    // Modules
                    "navigation", NavigationOptions.Type
                    "pagination", PaginationOptions.Type
                    "scrollbar", ScrollbarOptions.Type
                    "autoplay", AutoplayOptions.Type
                    "controller", ControllerOptions.Type
                    "keyboard", KeyboardOptions.Type
                    "mousewheel", MousewheelOptions.Type
                    "zoom", ZoomOptions.Type
                    "freeMode", FreeModeOptions.Type
                    "grid", GridOptions.Type
                    "thumbs", ThumbsOptions.Type
                    "virtual", VirtualOptions.Type
                    "parallax", ParallaxOptions.Type
                    "a11y", A11yOptions.Type
                    "hashNavigation", HashNavigationOptions.Type
                    "history", HistoryOptions.Type
                    // Effects
                    "fadeEffect", FadeEffectOptions.Type
                    "cubeEffect", CubeEffectOptions.Type
                    "coverflowEffect", CoverflowEffectOptions.Type
                    "flipEffect", FlipEffectOptions.Type
                    "cardsEffect", CardsEffectOptions.Type
                    "creativeEffect", CreativeEffectOptions.Type
                ]
        }

    // ------------------------------------------------------------------
    // Event handlers (the `on` option)
    // ------------------------------------------------------------------

    // `SwiperEventHandlers` is declared above because `SwiperOptions.On` needs
    // it, while the handlers themselves take `Swiper`/`SwiperOptions`/DOM types
    // that are declared in between. It is filled here like a `Pattern.Config`.
    let fillConfig (cls: CodeModel.Class) (required: list<string * Type.Type>) (optional: list<string * Type.Type>) =
        let cls =
            cls
            |+> Instance [
                for (n, t) in required -> Getter n t :> _
                for (n, t) in optional -> Property n t :> _
            ]
        let ctor : Type.Parameters =
            {
                This = None
                Variable = None
                Arguments = [ for (n, t) in required -> (t :> Type.IParameter).Parameter |=> n ]
            }
        cls
        |+> Static [ ObjectConstructor ctor ]

    let EventHandler = SwiperClass ^-> T<unit>
    let EventHandlerWithEvent = (SwiperClass * T<Dom.Event>) ^-> T<unit>

    fillConfig SwiperEventHandlers [] [
        // Core lifecycle
        "init", EventHandler
        "beforeDestroy", EventHandler
        "slidesUpdated", EventHandler
        "slideChange", EventHandler
        "slideChangeTransitionStart", EventHandler
        "slideChangeTransitionEnd", EventHandler
        "slideNextTransitionStart", EventHandler
        "slideNextTransitionEnd", EventHandler
        "slidePrevTransitionStart", EventHandler
        "slidePrevTransitionEnd", EventHandler
        "transitionStart", EventHandler
        "transitionEnd", EventHandler
        "progress", (SwiperClass * T<float>) ^-> T<unit>
        "reachBeginning", EventHandler
        "reachEnd", EventHandler
        "toEdge", EventHandler
        "fromEdge", EventHandler
        "setTranslate", (SwiperClass * T<float>) ^-> T<unit>
        "setTransition", (SwiperClass * T<float>) ^-> T<unit>
        "resize", EventHandler
        "observerUpdate", EventHandler
        "beforeLoopFix", EventHandler
        "loopFix", EventHandler
        "breakpoint", (SwiperClass * SwiperOptions.Type) ^-> T<unit>
        "activeIndexChange", EventHandler
        "snapIndexChange", EventHandler
        "realIndexChange", EventHandler
        "afterInit", EventHandler
        "beforeInit", EventHandler
        "beforeResize", EventHandler
        "beforeSlideChangeStart", EventHandler
        "beforeTransitionStart", (SwiperClass * T<float>) ^-> T<unit>
        "changeDirection", EventHandler
        "destroy", EventHandler
        "momentumBounce", EventHandler
        "orientationchange", EventHandler
        "slideResetTransitionStart", EventHandler
        "slideResetTransitionEnd", EventHandler
        "slidesLengthChange", EventHandler
        "slidesGridLengthChange", EventHandler
        "snapGridLengthChange", EventHandler
        "update", EventHandler
        "lock", EventHandler
        "unlock", EventHandler
        // Touch/click events carry the DOM event
        "touchStart", EventHandlerWithEvent
        "touchMove", EventHandlerWithEvent
        "touchMoveOpposite", EventHandlerWithEvent
        "sliderMove", EventHandlerWithEvent
        "touchEnd", EventHandlerWithEvent
        "sliderFirstMove", EventHandlerWithEvent
        "click", EventHandlerWithEvent
        "tap", EventHandlerWithEvent
        "doubleTap", EventHandlerWithEvent
        "doubleClick", EventHandlerWithEvent
        // Modules
        "autoplay", EventHandler
        "autoplayStart", EventHandler
        "autoplayStop", EventHandler
        "autoplayPause", EventHandler
        "autoplayResume", EventHandler
        "autoplayTimeLeft", (SwiperClass * T<float> * T<float>) ^-> T<unit>
        "navigationHide", EventHandler
        "navigationShow", EventHandler
        "navigationPrev", EventHandler
        "navigationNext", EventHandler
        "paginationRender", (SwiperClass * T<HTMLElement>) ^-> T<unit>
        "paginationUpdate", (SwiperClass * T<HTMLElement>) ^-> T<unit>
        "paginationHide", EventHandler
        "paginationShow", EventHandler
        "scrollbarDragStart", EventHandlerWithEvent
        "scrollbarDragMove", EventHandlerWithEvent
        "scrollbarDragEnd", EventHandlerWithEvent
        "keyPress", (SwiperClass * T<string>) ^-> T<unit>
        "scroll", (SwiperClass * T<Dom.WheelEvent>) ^-> T<unit>
        "hashChange", EventHandler
        "hashSet", EventHandler
        "virtualUpdate", EventHandler
        "zoomChange", (SwiperClass * T<float> * T<HTMLElement> * T<HTMLElement>) ^-> T<unit>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // Breakpoints: a plain object keyed by breakpoint width
    // ------------------------------------------------------------------

    Breakpoints
    |+> Instance [
        Property "" SwiperOptions.Type |> Indexed T<string>
    ]
    |+> Static [
        Getter "Empty" TSelf |> WithGetterInline "({})"
    ]
    |> ignore

    // ------------------------------------------------------------------
    // Module instance APIs
    // ------------------------------------------------------------------

    SwiperNavigation
    |+> Instance [
        "nextEl" =? T<HTMLElement> |> WithComment "The next arrow element."
        "prevEl" =? T<HTMLElement> |> WithComment "The previous arrow element."
        "update" => T<unit> ^-> T<unit> |> WithComment "Re-renders the navigation arrows after `swiper.update()`."
        "init" => T<unit> ^-> T<unit>
        "destroy" => T<unit> ^-> T<unit>
        "enable" => T<unit> ^-> T<unit>
        "disable" => T<unit> ^-> T<unit>
        "arrowSvg" =? T<string>
    ]
    |> ignore

    SwiperPagination
    |+> Instance [
        "el" =? T<HTMLElement> |> WithComment "The pagination container element."
        "bullets" =? !| T<HTMLElement> |> WithComment "The rendered bullets (bullets pagination only)."
        "render" => T<unit> ^-> T<unit>
        "update" => T<unit> ^-> T<unit>
        "init" => T<unit> ^-> T<unit>
        "destroy" => T<unit> ^-> T<unit>
        "enable" => T<unit> ^-> T<unit>
        "disable" => T<unit> ^-> T<unit>
    ]
    |> ignore

    SwiperScrollbar
    |+> Instance [
        "el" =? T<HTMLElement>
        "dragEl" =? T<HTMLElement>
        "updateSize" => T<unit> ^-> T<unit>
        "setTranslate" => T<unit> ^-> T<unit>
        "init" => T<unit> ^-> T<unit>
        "destroy" => T<unit> ^-> T<unit>
        "enable" => T<unit> ^-> T<unit>
        "disable" => T<unit> ^-> T<unit>
    ]
    |> ignore

    SwiperAutoplay
    |+> Instance [
        "running" =? T<bool> |> WithComment "`true` while the autoplay is running."
        "paused" =? T<bool>
        "timeLeft" =? T<float> |> WithComment "Milliseconds left until the next slide."
        "pause" => T<unit> ^-> T<unit>
        "resume" => T<unit> ^-> T<unit>
        "start" => T<unit> ^-> T<bool>
        "stop" => T<unit> ^-> T<bool>
    ]
    |> ignore

    SwiperController
    |+> Instance [
        "control" =? (SwiperClass + (!| SwiperClass))
        "setTranslate" => (T<float> * !? T<bool>?byController) ^-> T<unit>
        "setTransition" => (T<float> * !? T<bool>?byController) ^-> T<unit>
    ]
    |> ignore

    SwiperKeyboard
    |+> Instance [
        "enabled" =? T<bool>
        "enable" => T<unit> ^-> T<unit>
        "disable" => T<unit> ^-> T<unit>
    ]
    |> ignore

    SwiperMousewheel
    |+> Instance [
        "enabled" =? T<bool>
        "enable" => T<unit> ^-> T<unit>
        "disable" => T<unit> ^-> T<unit>
    ]
    |> ignore

    SwiperZoom
    |+> Instance [
        "enabled" =? T<bool>
        "scale" =? T<float> |> WithComment "The current zoom scale (`1` when not zoomed)."
        "enable" => T<unit> ^-> T<unit>
        "disable" => T<unit> ^-> T<unit>
        "in" => !? T<float> ^-> T<unit>
        "out" => T<unit> ^-> T<unit>
        "toggle" => !? T<Dom.Event> ^-> T<unit>
    ]
    |> ignore

    SwiperFreeMode
    |+> Instance [
        "onTouchStart" => T<unit> ^-> T<unit>
        "onTouchMove" => T<unit> ^-> T<unit>
        "onTouchEnd" => T<unit> ^-> T<unit>
    ]
    |> ignore

    SwiperGrid
    |+> Instance [
        "initSlides" => !| T<HTMLElement> ^-> T<unit>
        "unsetSlides" => T<unit> ^-> T<unit>
        "updateSlide" => (T<int> * T<HTMLElement> * !| T<HTMLElement>) ^-> T<unit>
        "updateWrapperSize" => (T<float> * !| T<float>) ^-> T<unit>
    ]
    |> ignore

    SwiperThumbs
    |+> Instance [
        "swiper" =? SwiperClass
        "update" => (!? T<bool> * !? ThumbsUpdateOptions.Type) ^-> T<unit>
        "init" => T<unit> ^-> T<bool>
    ]
    |> ignore

    SwiperVirtual
    |+> Instance [
        "from" =? T<int>
        "to" =? T<int>
        "slides" =? !| SwiperVirtualSlide
        "offset" =? T<float>
        "slidesGrid" =? !| T<float>
        "appendSlide" => (SwiperVirtualSlide + (!| SwiperVirtualSlide)) ^-> T<unit>
        "prependSlide" => (SwiperVirtualSlide + (!| SwiperVirtualSlide)) ^-> T<unit>
        "removeSlide" => (T<int> + (!| T<int>)) ^-> T<unit>
        "removeAllSlides" => T<unit> ^-> T<unit>
        "update" => (!? T<bool> * !? T<bool> * !? T<int>) ^-> T<unit>
    ]
    |> ignore

    SwiperParallax
    |+> Instance [
        "setTranslate" => T<unit> ^-> T<unit>
    ]
    |> ignore

    SwiperA11y
    |+> Instance [
        "clicked" =? T<bool>
    ]
    |> ignore

    SwiperHashNavigation
    |+> Instance []
    |> ignore

    SwiperHistory
    |+> Instance []
    |> ignore

    // ------------------------------------------------------------------
    // The Swiper class
    // ------------------------------------------------------------------

    SwiperClass
    |+> Static [
        Constructor ((T<string> + T<HTMLElement>) * !? SwiperOptions.Type)
        |> WithComment "Creates a Swiper on the element (or matching the CSS selector) with the given options."

        Constructor (!? SwiperOptions.Type)
        |> WithComment "Creates a Swiper from an options object that carries `El` (selector or element)."

        "extendDefaults" => SwiperOptions.Type ^-> T<unit>
        "installModule" => SwiperModule ^-> T<unit>
        "use" => SwiperModule ^-> TSelf
        |> WithComment "Installs one module globally, so it can be omitted from `Modules`."
        "use" => (!| SwiperModule) ^-> TSelf
        |> WithComment "Installs several modules globally."

        "defaults" =? SwiperOptions.Type
        "extendedDefaults" =? SwiperOptions.Type
    ]
    |> ignore

    SwiperClass
    |+> Instance [
        // Configuration
        "params" =? SwiperOptions.Type |> WithComment "The options object merged with the defaults."
        "originalParams" =? SwiperOptions.Type
        "passedParams" =? SwiperOptions.Type
        "support" =? SwiperSupport.Type
        "device" =? SwiperDevice.Type
        "browser" =? SwiperBrowser.Type
        "enabled" =? T<bool> |> WithComment "`true` while the Swiper is enabled."

        // Elements
        "el" =? T<HTMLElement> |> WithComment "The Swiper container element."
        "wrapperEl" =? T<HTMLElement> |> WithComment "The slides wrapper element."
        "slidesEl" =? T<HTMLElement>
        "hostEl" =? T<HTMLElement>
        "classNames" =? !| T<string>

        // Geometry
        "slides" =? !| SwiperSlideElement |> WithComment "The slide elements."
        "slidesGrid" =? !| T<float>
        "snapGrid" =? !| T<float>
        "slidesSizesGrid" =? !| T<float>
        "visibleSlides" =? !| SwiperSlideElement
        "visibleSlidesIndexes" =? !| T<int>
        "loopedSlides" =? T<int>
        "size" =? T<float>
        "width" =? T<float>
        "height" =? T<float>
        "virtualSize" =? T<float>

        // Position
        "activeIndex" =? T<int> |> WithComment "Index of the active slide (shifted in loop mode)."
        "realIndex" =? T<int> |> WithComment "Index of the active slide, corrected for loop mode."
        "previousIndex" =? T<int>
        "snapIndex" =? T<int>
        "previousSnapIndex" =? T<int>
        "clickedIndex" =? T<int>
        "clickedSlide" =? SwiperSlideElement
        "rtl" =? T<bool>
        "rtlTranslate" =? T<bool>
        "wrongRTL" =? T<bool>
        "swipeDirection" =? SwipeDirection.Type
        "translate" =? T<float> |> WithComment "The current wrapper translate (px)."
        "previousTranslate" =? T<float>
        "progress" =? T<float> |> WithComment "The current progress, from `0` (start) to `1` (end)."
        "velocity" =? T<float>

        // State
        "animating" =? T<bool>
        "isBeginning" =? T<bool>
        "isEnd" =? T<bool>
        "isLocked" =? T<bool>
        "allowClick" =? T<bool>
        "allowTouchMove" =? T<bool>
        "allowSlideNext" =? T<bool>
        "allowSlidePrev" =? T<bool>
        "touches" =? SwiperTouches.Type
        "imagesToLoad" =? !| T<HTMLImageElement>
        "imagesLoaded" =? T<int>
        "modules" =? !| SwiperModule

        // Module APIs
        "navigation" =? SwiperNavigation |> WithComment "Navigation module API (requires `SwiperModule.Navigation`)."
        "pagination" =? SwiperPagination |> WithComment "Pagination module API (requires `SwiperModule.Pagination`)."
        "scrollbar" =? SwiperScrollbar |> WithComment "Scrollbar module API (requires `SwiperModule.Scrollbar`)."
        "autoplay" =? SwiperAutoplay |> WithComment "Autoplay module API (requires `SwiperModule.Autoplay`)."
        "controller" =? SwiperController |> WithComment "Controller module API (requires `SwiperModule.Controller`)."
        "keyboard" =? SwiperKeyboard |> WithComment "Keyboard module API (requires `SwiperModule.Keyboard`)."
        "mousewheel" =? SwiperMousewheel |> WithComment "Mousewheel module API (requires `SwiperModule.Mousewheel`)."
        "zoom" =? SwiperZoom |> WithComment "Zoom module API (requires `SwiperModule.Zoom`)."
        "freeMode" =? SwiperFreeMode |> WithComment "Free mode module API (requires `SwiperModule.FreeMode`)."
        "grid" =? SwiperGrid |> WithComment "Grid module API (requires `SwiperModule.Grid`)."
        "thumbs" =? SwiperThumbs |> WithComment "Thumbs module API (requires `SwiperModule.Thumbs`)."
        "virtual" =? SwiperVirtual |> WithComment "Virtual module API (requires `SwiperModule.Virtual`)."
        "parallax" =? SwiperParallax |> WithComment "Parallax module API (requires `SwiperModule.Parallax`)."
        "a11y" =? SwiperA11y |> WithComment "Accessibility module API (requires `SwiperModule.A11y`)."
        "hashNavigation" =? SwiperHashNavigation
        "history" =? SwiperHistory

        // Events
        "on" => (T<string> * (SwiperClass ^-> T<unit>)) ^-> TSelf
        |> WithComment "Adds an event handler. Use the fully typed `On` option for handlers that need extra arguments."
        "on" => (T<string> * (SwiperClass * T<Dom.Event> ^-> T<unit>)) ^-> TSelf
        "on" => (T<string> * (SwiperClass * T<float> ^-> T<unit>)) ^-> TSelf
        "once" => (T<string> * (SwiperClass ^-> T<unit>)) ^-> TSelf
        "once" => (T<string> * (SwiperClass * T<Dom.Event> ^-> T<unit>)) ^-> TSelf
        "off" => (T<string> * (SwiperClass ^-> T<unit>)) ^-> TSelf
        "off" => (T<string> * (SwiperClass * T<Dom.Event> ^-> T<unit>)) ^-> TSelf
        "onAny" => ((T<string> * T<obj>) ^-> T<unit>) ^-> TSelf
        "offAny" => ((T<string> * T<obj>) ^-> T<unit>) ^-> TSelf
        "emit" => (T<string> *+ T<obj>) ^-> TSelf
        |> WithComment "Emits an event on the instance with the given data."

        // Translate and transitions
        "getTranslate" => !? TranslateAxis.Type ^-> T<float>
        "setTranslate" => (T<float> * !? T<bool>) ^-> T<unit>
        "minTranslate" => T<unit> ^-> T<float>
        "maxTranslate" => T<unit> ^-> T<float>
        "translateTo" => (!? T<float> * !? T<int> * !? T<bool> * !? T<bool>) ^-> T<bool>
        "setTransition" => (T<int> * !? T<bool>) ^-> T<unit>
        "transitionStart" => (!? T<bool> * !? SwipeDirection.Type) ^-> T<unit>
        "transitionEnd" => (!? T<bool> * !? SwipeDirection.Type) ^-> T<unit>

        // Sliding
        "slideTo" => (!? T<int> * !? T<int> * !? T<bool>) ^-> T<bool>
        |> WithComment "Slides to the slide with the given index."
        "slideToLoop" => (!? T<int> * !? T<int> * !? T<bool>) ^-> T<bool>
        "slideNext" => (!? T<int> * !? T<bool>) ^-> T<bool>
        "slidePrev" => (!? T<int> * !? T<bool>) ^-> T<bool>
        "slideReset" => (!? T<int> * !? T<bool>) ^-> T<bool>
        "slideToClosest" => (!? T<int> * !? T<bool> * !? T<int>) ^-> T<bool>
        "slideToClickedSlide" => T<unit> ^-> T<unit>
        "setProgress" => (T<float> * !? T<int>) ^-> T<unit>

        // Lifecycle
        "init" => T<unit> ^-> TSelf
        "init" => T<HTMLElement> ^-> TSelf
        "init" => T<string> ^-> TSelf
        "mount" => T<unit> ^-> T<bool>
        "mount" => T<HTMLElement> ^-> T<bool>
        "mount" => T<string> ^-> T<bool>
        "destroy" => (!? T<bool> * !? T<bool>) ^-> T<unit>
        |> WithComment "Destroys the instance and detaches the listeners."
        "update" => T<unit> ^-> T<unit>
        |> WithComment "Recalculates the slider after slides are added/removed or the container is resized."
        "enable" => T<unit> ^-> T<unit>
        "disable" => T<unit> ^-> T<unit>
        "changeDirection" => (!? Direction.Type * !? T<bool>) ^-> TSelf
        "changeLanguageDirection" => LanguageDirection.Type ^-> T<unit>

        // Slide helpers
        "getSlideIndex" => T<Dom.Element>?slideEl ^-> T<int>
        "getSlideIndexByData" => T<int>?index ^-> T<int>
        "getSlideIndexWhenGrid" => T<int>?index ^-> T<int>
        "getSlideClasses" => T<Dom.Element>?slideEl ^-> T<string>
        "slidesPerViewDynamic" => (!? SlidesPerViewDynamicView.Type * !? T<bool>) ^-> T<int>

        // Layout
        "updateSize" => T<unit> ^-> T<unit>
        "updateSlides" => T<unit> ^-> T<unit>
        "updateAutoHeight" => !? T<int> ^-> T<unit>
        "updateSlidesOffset" => T<unit> ^-> T<unit>
        "updateSlidesProgress" => !? T<float> ^-> T<unit>
        "updateProgress" => !? T<float> ^-> T<unit>
        "updateSlidesClasses" => T<unit> ^-> T<unit>
        "updateActiveIndex" => !? T<int> ^-> T<unit>
        "updateClickedSlide" => T<HTMLElement> ^-> T<unit>
        "setBreakpoint" => T<unit> ^-> T<unit>
        "addClasses" => T<unit> ^-> T<unit>
        "removeClasses" => T<unit> ^-> T<unit>
        "checkOverflow" => T<unit> ^-> T<unit>
        "setGrabCursor" => !? T<bool> ^-> T<unit>
        "unsetGrabCursor" => T<unit> ^-> T<unit>
        "attachEvents" => T<unit> ^-> T<unit>
        "detachEvents" => T<unit> ^-> T<unit>

        // Manipulation module (SwiperModule.Manipulation)
        "appendSlide" => SlideInput ^-> T<unit>
        "prependSlide" => SlideInput ^-> T<unit>
        "addSlide" => (T<int> * SlideInput) ^-> T<unit>
        "removeSlide" => (T<int> + (!| T<int>)) ^-> T<unit>
        "removeAllSlides" => T<unit> ^-> T<unit>
    ]
    |> ignore

    SwiperSlideElement
    |+> Instance [
        "swiperSlideOffset" =? T<float>
        "swiperSlideSize" =? T<float>
        "progress" =? T<float>
        "originalProgress" =? T<float>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // Feature modules, imported from `swiper/modules`
    // ------------------------------------------------------------------

    SwiperModule
    |+> Static [
        "Virtual" =? TSelf |> Import "Virtual" "swiper/modules"
        "Keyboard" =? TSelf |> Import "Keyboard" "swiper/modules"
        "Mousewheel" =? TSelf |> Import "Mousewheel" "swiper/modules"
        "Navigation" =? TSelf |> Import "Navigation" "swiper/modules"
        "Pagination" =? TSelf |> Import "Pagination" "swiper/modules"
        "Scrollbar" =? TSelf |> Import "Scrollbar" "swiper/modules"
        "Parallax" =? TSelf |> Import "Parallax" "swiper/modules"
        "Zoom" =? TSelf |> Import "Zoom" "swiper/modules"
        "Controller" =? TSelf |> Import "Controller" "swiper/modules"
        "A11y" =? TSelf |> Import "A11y" "swiper/modules"
        "History" =? TSelf |> Import "History" "swiper/modules"
        "HashNavigation" =? TSelf |> Import "HashNavigation" "swiper/modules"
        "Autoplay" =? TSelf |> Import "Autoplay" "swiper/modules"
        "Thumbs" =? TSelf |> Import "Thumbs" "swiper/modules"
        "FreeMode" =? TSelf |> Import "FreeMode" "swiper/modules"
        "Grid" =? TSelf |> Import "Grid" "swiper/modules"
        "Manipulation" =? TSelf |> Import "Manipulation" "swiper/modules"
        "EffectFade" =? TSelf |> Import "EffectFade" "swiper/modules"
        "EffectCube" =? TSelf |> Import "EffectCube" "swiper/modules"
        "EffectFlip" =? TSelf |> Import "EffectFlip" "swiper/modules"
        "EffectCoverflow" =? TSelf |> Import "EffectCoverflow" "swiper/modules"
        "EffectCreative" =? TSelf |> Import "EffectCreative" "swiper/modules"
        "EffectCards" =? TSelf |> Import "EffectCards" "swiper/modules"
    ]
    |> ignore

    // The default export of the npm `swiper` package is the class itself.
    SwiperClass |> ImportDefault "swiper" |> ignore

    let Assembly =
        Assembly [
            Namespace "WebSharper.Swiper" [
                SwiperModule
                SwiperOptions
                SwiperEventHandlers
                Breakpoints
                SwiperSlideElement
                SwiperSupport
                SwiperDevice
                SwiperBrowser
                SwiperTouches
                // Enumerations
                Direction
                Effect
                TouchEventsTarget
                SlidesPerViewAuto
                GridFill
                PaginationType
                ControllerBy
                FadeMode
                SwipeDirection
                TranslateAxis
                LanguageDirection
                SlidesPerViewDynamicView
                MousewheelEventsTarget
                // Effects
                FadeEffectOptions
                CubeEffectOptions
                CoverflowEffectOptions
                FlipEffectOptions
                CardsEffectOptions
                CreativeEffectOptions
                CreativeEffectTransform
                // Module options
                NavigationOptions
                PaginationOptions
                ScrollbarOptions
                AutoplayOptions
                ControllerOptions
                KeyboardOptions
                MousewheelOptions
                ZoomOptions
                FreeModeOptions
                GridOptions
                ThumbsOptions
                ThumbsUpdateOptions
                VirtualOptions
                VirtualData
                ParallaxOptions
                A11yOptions
                HashNavigationOptions
                HistoryOptions
                // Module APIs
                SwiperNavigation
                SwiperPagination
                SwiperScrollbar
                SwiperAutoplay
                SwiperController
                SwiperKeyboard
                SwiperMousewheel
                SwiperZoom
                SwiperFreeMode
                SwiperGrid
                SwiperThumbs
                SwiperVirtual
                SwiperParallax
                SwiperA11y
                SwiperHashNavigation
                SwiperHistory
                SwiperClass
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member x.Assembly = Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()