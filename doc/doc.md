# Swiper

[Swiper](http://idangero.us/swiper/#.WVpdM4SGO70) is a JavaScript
library to easily make modern, touch-driven slides on your website.
It is designed for iOS but also works on most Android devices and
Desktop browsers.

## Configuring Swiper

Using Swiper with WebSharper is just like using the original
JavaScript library. The core HTML code will remain the same,
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
the HTML selector for your swiper contrainer (`.swiper-container` in
this case) or you can pass the selector and a configuration object.
The syntax to create the configuration file is a little different
in F#.

This is how a usual JavaScript snippet would look like:
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
are some differences in the syntax, which are listed below.

### Method and property names

By F# naming conventions, all class-level names (e.g. methods, properties)
start with a capital letter.

### DOM elements
To pass DOM elements (`WebSharper.Javasctript.Dom.Element`) to functions, you
can call the `Dom` property of an `Elt`, or you can also call the `Html`
property of any `Elt` to get the HTML `string` representing that `Elt`.

### Union types

When a field of the config object can have multiple types, it appears as a
`WebSharper.JavaScript.Union<_,_>`. You have a range of functions in the
`WebSharper.JavaScript` namespace for dealing with unions. Let's look at
an example:

Some parameters in the config object can be assigned either a `string`
or a `Dom.Element`, which means they'll have the type
`Union<Dom.Element, string>`. To instantiate such a type with a `string`
value, use `Union2Of2 "example"`. To create it with a `Dom.Element`,
use `Union1Of2 elt.Dom`. The pattern is the same for different types and
sizes of unions.

When using the `Union<>` type in C# you can simply assign a value of any
of the appropriate generic type parameters since there’s an implicit
conversion for every case.

```fsharp
[SPAEntryPoint]
public static void Main()
{
    SwipeParameters swipeParams = new SwipeParameters() {
        Pagination = ".swiper-pagination", //Union<Element,string>
        PaginationClickable = true,
        NextButton = ".swiper-button-next", //Union<Element,string>
        PrevButton = ".swiper-button-prev", //Union<Element,string>
        Parallax = true,
        Speed = 600
    };

    Swiper swiper = new Swiper(
        ".swiper-container",
        swipeParams
    );

    Doc.RunAppendById("main", p());
}
```

### Enums

Also to provide some more type safety to the programmer, the F# bind uses
_enums_ where the JavaScript API expects a small set of (usually) `string`
constants. The new enums and their fields are the following:

* **Direction** - `Vertical`, `Horizontal`
* **ColumnFill** - `Row`, `Column`
* **TouchEventTargets** - `Container`, `Wrapper`
* **PaginationType** - `Bullets`, `Fraction`, `Progress`, `Custom`
* **Auto** - `Auto`
* **ControlBy** - `Slide`, `Container`
* **Effect** - `Slide`, `Fade`, `Cube`, `Coverflow`, `Flip`

Smaller config objects also have their on representation. These classes
and their fields are listed below.

* **Cube** - `SlideShadows`, `Shadow`, `ShadowOffset`, `ShadowScale`
* **Fade** - `CrossFade`
* **Coverflow** - `Rotate`, `Stretch`, `Depth`, `Modifier`, `SlideShadows`
* **Flip** - `SlideShadows`, `LimitRotation`

Below are all the occurrences of all these new types:

* `SwiperParameters`
  * `Direction`
  * `Effect`
  * `Fade`
  * `Cube`
  * `Coverflow`
  * `Flip`
  * `SlidesPerView`
  * `TouchEventTarget`
  * `PaginationType`
  * `Control`
  * `ControlBy`

* `Swipe`
  * `Params`
  * `Touches`
