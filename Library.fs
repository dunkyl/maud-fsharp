module net.dunkyl.Maud

open System.Text.Encodings.Web
open System.Text.Unicode
open type System.Web.HttpUtility

let serializeAttributes attrs =
    attrs
    |> Seq.map
        (function (k, Some "") -> Some k 
                | (k, None) -> None
                | (k, Some v) ->
                    Some $"{k}=\"{HtmlAttributeEncode v}\"")
    |> Seq.choose id
    |> String.concat " "

type Tag =
| NonVoid of string
// | Void of string
| Omit

type Markup = Markup of string with
    override this.ToString () =
        let (Markup s) = this
        s

type PreEscaped = PreEscaped of string

// type DOCTYPE = DOCTYPE PreEscaped "<!DOCTYPE html>"

type HtmlBuilder  =

    {
        tag: Tag
        attributes: (string * string option) list
        cls: string
    } with

    member this.Zero (()) = ""

    member this.Yield (()) = ""
    member this.Yield (s: string) = HtmlEncode s
    member this.Yield (PreEscaped s) = s
    member this.Yield (Markup s) = s

    // member this.Using (x, f) = fun a -> x + "\n" + f a

    [<CustomOperation("DOCTYPE")>]
    member this.AddDocType (a) =
        a

    [<CustomOperation("br")>]
    member this.AddBreak a =
        Markup "<br>"

    member this.Combine (x: string, f: unit -> string) = x + "\n" + f ()
    member this.Combine (x: Markup, f: unit -> string) = x.ToString() + "\n" + f ()

    member this.For (x: string, f) = this.Combine (x, f)
    member this.For (x: Markup, f) = this.Combine (x, f)

    member this.For (x: string array, f) =
        String.concat "\n" (Seq.map f x)

    member this.Delay (x: string) = fun () -> x
    member this.Delay (f: unit -> string) = f
    
    member this.Run (f: unit -> string) =
        match this.tag with
        | NonVoid tag when this.attributes.IsEmpty ->
            $"<{tag}>{f()}</{tag}>"
        | NonVoid tag ->
            $"<{tag} {serializeAttributes this.attributes}>{f()}</{tag}>"
        // | Void tag ->
        //     $"<{tag}>"
        | Omit -> f()
        |> Markup

let (<.>) html cls = html


let maud = {tag = Omit; attributes = []; cls = ""}

let HtmlBuilder (tag, attributes) = { tag = tag; attributes = attributes; cls = "" }

[<AutoOpen>]
module Elements =
    let html = HtmlBuilder (NonVoid "html", [])
    let head = HtmlBuilder (NonVoid "head", [])
    let body = HtmlBuilder (NonVoid "body", [])

    let h1 = HtmlBuilder (NonVoid "h1", [])
    let p = HtmlBuilder (NonVoid "p", [])

    let strong = HtmlBuilder (NonVoid "strong", [])
    let span = HtmlBuilder (NonVoid "span", [])

    [<AutoOpen>]
    type TagsWithProperties =

        static member li' (?class': string) =
            HtmlBuilder (NonVoid "li", [
                "class", class'
            ])

        static member link' (?rel: string, ?href: string, ?target: string) =
            HtmlBuilder (NonVoid "a", [
                "rel", rel; "href", href
            ])

        static member input' (?type': string, ?name: string) =
            HtmlBuilder (NonVoid "input", [
                             "type", type';  "name", name
            ])

        static member label' (?for': string) =
            HtmlBuilder (NonVoid "label", [
                "for", for'
            ])