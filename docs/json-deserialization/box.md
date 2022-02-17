### Style property
Sets the style for the box. Could be a string - name of a defined style
in `styles`.
```json
{
    "style": "derived-style"
}
```
Or it could be an "anonymous" style object, defined just in place
```json
{
    "style": {
        // As it's a style object, you can use
        // all its features, described in style.md
        "base": "base-style",
        "dialogue": {
            "farewell": "Bye..."
        }
    }
}
```
### Commands property
[executable-docs]: <https://github.com/NebelFox/verbox/blob/dev/docs/json-deserialization/executable.md>
Takes an array of `Executable`([executable.md][executable-docs]).
The order remains the same.

### Example
```json
{
    "style": {
        // from inheritance.md
        "base": "base-style",
        "dialogue.greeting": "Greetings!"
    },
    "commands": [
        "command-with-only-a-name",
        {
            "name": "parameterless-command"
        },
        {
            "name": "max",
            "brief": "prints the max integer",
            "params": ["values:int..."]
        },
        {
            "name": "namespace",
            "commands": [
                "nested-command",
                {
                    "name": "nested-namespace",
                    "commands": ["foo", "bar"]
                }
            ]
        }
    ]
}
```

[inheritance-docs]: <https://github.com/NebelFox/verbox/blob/dev/docs/json-deserialization/inheritance.md>
Boxes support [inheritance][inheritance-docs]!