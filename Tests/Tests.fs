namespace net.dunkyl.Maud.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

open net.dunkyl.Maud

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.Zero () =
        let x = maud { () }
        Assert.AreEqual("", x.ToString())

    [<TestMethod>]
    member this.Strings () =
        let x = maud {
            "hi<"
            "hello"
        }
        Assert.AreEqual("hi&lt;\nhello", x.ToString())

    [<TestMethod>]
    member this.PreEscaped () =
        let x = maud {
            PreEscaped "<script>alert('hi')</script>"
        }
        Assert.AreEqual("<script>alert('hi')</script>", x.ToString())


    [<TestMethod>]
    member this.Tags () =
        let x = maud {
            h1 { "Poem" }
            let file = System.IO.File.ReadAllText "example.txt"
            for word in file.Split " " ->
                span { word }
            p {
                strong { "Rock," }
                " you are a rock."
            }
        }
        Assert.AreEqual("""<h1>Poem</h1>
            <p><strong>Rock,</strong> you are a rock.</p>
        """, x.ToString())

    [<TestMethod>]
    member this.Anchor () =
        let x = maud {
            link' (rel="stylesheet", href="poetry.css") { () }
        }
        Assert.AreEqual("""<a rel="stylesheet" href="poetry.css"></a>""", x.ToString())

    [<TestMethod>]
    member this.Li () =
        let li = HtmlBuilder (NonVoid "li", [])
        let x = maud {
            li' (class'="x") { () }
            li' () { () }
            {  li with cls = "test-class" } { () }
            // li.test-class { }
            (li <.> "test-class") { () }
        }
        Assert.AreEqual("""<a rel="stylesheet" href="poetry.css"></a>""", x.ToString())

    [<TestMethod>]
    member this.Br () =
        let x = maud {
            p {
                "Rock, you are a rock."
                br
                "Gray, you are gray,"
                br
                "Like a rock, which you are."
                br
                "Rock."
            }
        }
        Assert.AreEqual("""<p>
            Rock, you are a rock.
            <br>
            Gray, you are gray,
            <br>
            Like a rock, which you are.
            <br>
            Rock.
        </p>""", x.ToString())