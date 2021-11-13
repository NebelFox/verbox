### In short, a `Style` defines how the output looks and how the input is interpreted.

You can set the style for a box via `BoxBuilder.Style` method, 
that accepts a `Style` instance.  
As the `Style` is a `record`, defining a custom style 
is recommended via `with` on the default style 
for inheritance purposes, as shown in the example below:
```c#
new BoxBuilder()
    /*...*/
   .Style(Style.Default with {DailogueGreeting = "Hi there!", 
                              DialogueFarewell = "See you soon!"})
   .Build()
   .StartDialogue();
```