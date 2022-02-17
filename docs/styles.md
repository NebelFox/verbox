### In short, a `Style` defines how the output looks and how the input is interpreted.

You can set the style for a box via `BoxBuilder.Style` method, 
that accepts a `Style` instance.  
Alternatively, a IReadOnlyDictionary could be used as the argument.
It's just for a convenience, 
as it creates a style instance from the given dictionary.
```c#
new BoxBuilder()
    /*...*/
   .Style(new Dictionary<string, string>
    {
        ["dialogue.greeting"] = "Hi there!",
        ["dialogue.farewell"] = "See you soon!"
    })
   .Build()
   .StartDialogue();
```