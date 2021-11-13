# How it works

### Type matching abort calls on mismatched input:
```c#
new BoxBuilder()
   .Type<int>("int", int.TryParse)
   .Command(new Command("plus-one", "...")
           .Parameters("<value:int>")
           .Action(context => 
           {
               var value = (int)context["value"];
               Console.WriteLine(value+1);
           }))
   .Build()
   .Execute("plus-one 10") // prints "11"
   .Execute("plus-one text") // aborts execution and prints an error
```

---

### It also stops adding values to collective or optional parameter on mismatch:
```c#
new BoxBuilder()
   .Type<int>("int", int.TryParse)
   .Command(new Command("demo", "...")
           .Parameters("<values:int...>",
                       "<tail>")
           .Action(context => 
           {
               var values = string.Join(',', (object[])context["values"]);
               Console.WriteLine($"Values: {values}; Tail: {context["tail"]}");
           }))
   .Build()
   .Execute("demo 1 tail") // outputs "Values: 1; Tail: tail"
   .Execute("demo 1 2 3 tail") // outputs "Values: 1, 2, 3; Tail: tail"
```
>**Note**. It won't work like that if the collective/optional 
> string parameter is preceding another parameters. As the `string` type matches everything, 
> it would capture everything until the input end or an option.
> For the example above let's swap parameter types like that: `.Parameters("<values...>", "[<tail:int>]")`
> (The second parameter is made optional to avoid error).  
> Then the first execute call would output "Values: 1, tail; Tail: "
> And the second - "Values: 1, 2, 3, tail; Tail: "

---

# Adding types

### Add via `ParseFunction`
```c#
// A ParseFunction accepts a string (called 'token')
// and returns an object on success, otherwise null
new BoxBuilder()
   .Type("ten", token => token == "10" ? 10 : null)
```

---

### Add via generic TryParse function
```c#
// A TryParse function accepts a token,
// also has an out parameter of specified generic type.
// And returns a bool indicating success
new BoxBuilder()
   .Type<bool>("bool-using-built-in", bool.TryParse)
   .Type("ten-with-custom-function",
        (string token, out int value) =>
        {
            if (token == "10")
            {
                value = 10;
                return true;
            }
            value = 0;
            return false;
        })
```
> **Note**. Explicit generic type specification 
> is not mandatory, and may be omitted in most cases, like in the `ten-with-custom-function` example.

---

### Add type from enum
```c#
public enum Age
{
    Young,
    Mature,
    Old
}
/*...*/
new BoxBuilder()
   .Type<Age>() // the name is "age"
   // or
   .Type<Age>("your-custom-name")
```
> **Note**. Generated name is the name of the enum, 
> converted to *dash-case*, e.g. *LogLevel* becomes *log-level*.  
> Generated function expects strings in *dash-case*, 
> as it converts them to *PascalCase*, and only then tries to parse 
> to an enum value.  
> Case-conversion examples for clarity:  
> *Word - word, SeveralWords - several-words*.

---