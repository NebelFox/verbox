Building a box is easy via the `BoxBuilder`,
that allows defining a box almost in declarative style.
It also allows chain method calls for even more convenience.
Here is an example of building a box, 
that shows most of current capabilities of `BoxBuilder` .

```c#
public enum Age
{
    Young,
    Mature,
    Old
}

private static void Main(string[] args)
{
    new BoxBuilder()
       .Type<bool>("bool", bool.TryParse) // adding a new type using TryParse delegate
       .Type<Age>() // creating a new type from enum (name is "age")
       .Type<Age>("also-age") // creating a new type from enum with custom type name
       .Command(new Command("print", "Print all the values")
               .Parameters("values...") // single collective string argument
               .Action(context =>
                {
                    // accessing the argument value
                    // arguments are stored boxes in objects,
                    // so they need be unboxed to the parameter type for type-specific use
                    Console.WriteLine(string.Join(' ', (string[])context["values"]));
                })
               .Examples("print 12345",
                         "print \"escaped string\" and single words!"))
       .Command(new Command("exit", "Terminate the program") // parameterless command
                   .Action(context => context.Box.Terminate())) // actually terminates the dialogue
       .Command(new Namespace("namespace", "namespace brief")
               .WithDescription("description")
               .Member(new Command("variants", "some parameters variants")
                      .Parameters("positional-string",
                                  "positional-collective...",
                                  "positional-typed:bool",
                                  "positional-typed-collective:bool...",
                                  "[optional-positional]",
                                  "[optional-positional-typed-collective:bool...]",
                                  "--switch",
                                  "--option parameter-collective...",
                                  "--option parameter=default",
                                  "--option flag:bool=true")
                      .Action(_ => { })))
       .Style(Style.Default with // so this custom style inherits from the default
        {
            DialogueGreeting = "Oh, here we go",
            HelpLobbyTitle = "Super-duper-exiting program",
            HelpLobbyHeader = "A lot of shiny stuff here.\nTry not to break anything",
            DialoguePromptIndicator = "Enter a command: ",
            HelpLobbyFooter = "There could be your ads",
            DialogueFarewell = "Bye for now"
        })
       .Build() // Getting the box
       .StartDialogue(); // calling a box method
}
```

---

By the way, it's absolutely OK to separate the building 
and using a box like that:
```c#
Box box = new BoxBuilder()
              /*...*/
             .Build();
/*Some extra things happening here*/
box.StartDialogue();
box.Execute("print individual command execution")
```

---

It's also possible to run a box "inside" another box:
```c#
Box inner = new BoxBuilder()
               .Command(new Command("demo", "")
                       .Action(_ => "Hi from the inner box!"))
               .Command(new Command("exit", "")
                       .Action(context => context.Box.Terminate()))
               .Style(Style.Default with
                {
                    DialogueGreeting = "You've entered the inner box",
                    DialogueFarewell = "Exiting the inner box..."
                })
                .Build();
                
Box outer = new BoxBuilder()
                .Command(new Command("demo", "")
                       .Action(_ => "Hi from the outer box!"))
                .Command(new Command("inner", "")
                        .Action(_ => inner.StartDialogue()))
                .Style(Style.Default with
                {
                    DialogueGreeting = "You've entered the outer box",
                })
                .Build();
                
outer.StartDialogue();
```
> You've entered the outer box  
> $ demo  
> Hi from the outer box!  
> 
> $ inner  
> You've entered the inner box  
> 
> $ demo  
> Hi from the inner box!  
> 
> $ exit  
> Exiting the inner box...
> 
> $ demo  
> Hi from the outer box!