## Style object
Consists of key-value pairs, each setting a value to a specific style aspect
```json
{
    "option.prefix": "/",
    "dialogue.greeting": "Hi there!",
    "dialogue.farewell": "See ya!"
}
```
### Aspect subgroups
Dots in aspect names mean informal division
into subgroups. They could be gathered in a nested
style object, so we can reformat the example above as follows:
```json
{
    "option.prefix": "/",
    "dialogue": {
        "greeting": "Hi there!",
        "farewell": "See ya!"
    }
}
```
> **Note**. In this example the "option.prefix" aspect
> could also be written as `"option": {"prefix": "/"}`,
> but there is no much sense of doing it for a single aspect
> from a group

### All the aspects and their default values
```json
{
    "dialogue": {
        "greeting": null,
        "prompt-indicator": "$ ",
        "semantic-separator": "\n",
        "farewell": null
    },
    "input": {
        "separator": " ",
        "quotes": "'\"`",
        "new-line-escape": "\\"
    },
    "help": {
        "lobby": {
            "title": null,
            "header": null,
            "footer": null
        },
        "namespace-member-format": "> {0} - {1}"
    },
    "option.prefix": "--"
}
```
[inheritance-docs]: <https://github.com/NebelFox/verbox/blob/dev/docs/json-deserialization/inheritance.md>
Styles support [inheritance][inheritance-docs]!