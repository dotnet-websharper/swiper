# Swiper

[Swiper](http://idangero.us/swiper/#.WVpdM4SGO70) is a Javascript
library to easily make modern, touch-driven slides on your website.
It is designed for iOS but also works on most Android devices and
Desktop browsers.

## Configuring Swiper

Using Swiper with WebSharper is just like using the original
Javascript library. The core HTML code will remain the same,
for example:

```html
<div class="swiper-container">
    <div class="swiper-wrapper">
        <div class="swiper-slide" data-hash="slide1">Slide 1</div>
        <div class="swiper-slide" data-hash="slide2">Slide 2</div>
        <!-- ... -->
        <div class="swiper-slide" data-hash="slide9">Slide 9</div>
        <div class="swiper-slide" data-hash="slide10">Slide 10</div>
    </div>
</div>
```

You have two ways to instantiate the Swiper object: You can pass
the HTML selector for yor swiper contrainer (`.swiper-container` in
this case) or you can pass the selector and a configuration object.
The syntax to create the configuration file is a little different
in F#.

This is how a usual Javascipt snippet would look like:
```javascript
    var swiper = new Swiper ('.swiper-container', {
        direction: 'vertical',
        loop: true,
        onSlideChangeEnd: function(swiper) {
            console.log("Current slide index: " + swiper.activeIndex);
        }
    });
```

And this is how it will go in F#:
```fsharp
    open WebSharper.Swiper

    let swiper =
        new Swiper(
            ".swiper-container",
            SwipeParameters(
                Direction = Direction.Vertical,
                Loop = true,
                OnSlideChangeEnd = (fun swiper ->
                    Console.Log("Current slide index: " + (string swiper.ActiveIndex))
                )
            )
        )
```

## Using Swiper

The API is mostly the same as the original
[(see the documentation here)](http://idangero.us/swiper/api/). There
only minor differences, which are listed below.

### Differences

By F# naming conventions, all class-level names (e.g. methods, properties)
start with a capital letter.

To pass Dom elements (`WebSharper.Javasctript.Dom.Element`)
to functions, you can call the `Dom` property of any `Elt`,
or you can also call the `Html` property of any `Elt` to get
the HTML text representing that `Elt`.

In the config object when a field can have multiple types, it appears as
a `WebSharper.JavaScript.Union<_,_>`. You have a range of functions in the
`WebSharper.JavaScript` namespace for dealing with unions. Let's look at
an example:

Some parameters in the config object can be assigned `string` or a
`Dom.Element` so they'll have the type
`Union<Dom.Element, string>`. To instantiate such a type with a `string`
value, use `Union2Of2 "example"`. To create it with a `Dom.Element`,
use `Union1Of2 elt.Dom`. The pattern is the same for different types
and sizes of unions.

Also to provide some more type safety to the programmer, the F# bind uses
enums where the Javascript expects a small set of (usually) string constants.
The new enums and their fields are the following:

* **Direction** - `Vertical`, `Horizontal`
* **ColumnFill** - `Row`, `Column`
* **TouchEventTargets** - `Container`, `Wrapper`
* **PaginationType** - `Bullets`, `Fraction`, `Progress`, `Custom`
* **Auto** - `Auto`
* **ControlBy** - `Slide`, `Container`
* **Effect** - `Slide`, `Fade`, `Cube`, `Coverflow`, `Flip`

Smaller config objects also have their on representiaion. These classes
and their fields are listed below.

* **Cube** - `SlideShadows`, `Shadow`, `ShadowOffset`, `ShadowScale`
* **Fade** - `CrossFade`
* **Coverflow** - `Rotate`, `Stretch`, `Depth`, `Modifier`, `SlideShadows`
* **Flip** - `SlideShadows`, `LimitRotation`

Below are all the occurences all these new types:

* SwiperParameters
  * Directon
  * Effect
  * Fade
  * Cube
  * Coverflow
  * Flip
  * SlidesPerView
  * TouchEventTarget
  * PaginationType
  * Control
  * ControlBy

* Swipe
  * Params
  * Touches
