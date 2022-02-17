### Base keyword
In both Box and Style objects the `base` property
could be specified, which takes `a string`
with the name of _**a Box**_ or a _**Style**_ to derive respectively.

The derived object is basically the copy of the base object,
extended with properties, defined in derived object.
> **Note**. It's _**not necessary**_ for the base object
> to be defined in _**the same file**_ or only _**above**_ the derived one.
> _**The**_ _**requirements**_ are that _**both base and derived**_ objects
> must be loaded by _**the same**_ box/style _**deserializer**_ 
> before retrieving _**any**_ of them and there is _**no circular inheritance**_.  

> **Note**. Similar with _**style for box**_ - style may be defined in _**another file**_,
> but the _**box-serializer**_, that deserializes the box,
> must use the _**style-serializer**_, 
> that deserialized the specified style.

### Style inheritance example
```json
{
    "styles": {
        "base-style": {
            "dialogue": {
                "greeting": "Hi there!",
                "farewell": "See ya!"
            }
        },
        "derived-style": {
            "base": "base-style",
            "dialogue.farewell": "Bye..."
        },
        // the derived style is the same as:
        "style": {
            "dialogue": {
                "greeting": "Hi there!",
                "farewell": "Bye..."
            }
        }
    }
}
```

### Insert property of Box
[box-docs]: <https://github.com/NebelFox/verbox/blob/dev/docs/json-deserialization/box.md>
_**Alternatively**_ to the `command` property,
an `insert` or `insert-command-blocks` property
may be specified. It takes _**an array**_ of `commands insert blocks`.
Each `commands insert block` contains a `commands` property,
which is absolutely identical to the same [box][box-docs] property;
and one of three position specifiers: `at`, `before` or `after`:
```json
{
    "insert": [
        {
            // inserts the executables at the beginning.
            // alternatively "end" value may be used 
            // to insert at the end respectively
            "at": "begin",
            "commands": ["foo"]
        },
        {
            // inserts the executables before the specified one,
            // which must be added earlier
            "before": "foo",
            "commands": ["bar"]
        },
        {
            // same as "before", but inserts after
            // the specified executable
            "after": "bar",
            "commands": ["baz"]
        }
    ]
}
// The order would be:
// - bar
// - baz
// - foo
```
> **Note**. Inserts into _**nested**_ namespaces currently are _**not supported**_.

### Box inheritance example
```json
{
    "boxes": {
        "common": {
            // from style inheritance example
            "style": "derived-style",
            "commands": [
                "main",
                "exit"
            ]
        },
        "derived": {
            "style": {
                "base": "derived-style",
                "dialogue.greeting": "Welcome!"
            },
            "insert": [
                {
                    "before": "exit",
                    "commands": ["help"]
                }
            ]
        }
    }
}
```
