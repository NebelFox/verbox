### Common part:
```json
{
    // Mandatory. 
    "name": "executable-name",
    
    // Optional. Is displayed next to the command name
    // in the help message of the namespace
    // that contains this executable
    "brief": "short one-line description",
    
    // Optional. Is displayed in the executable help message
    "description": "long description"
}
```
Any executable is either a Command or a Namespace.

### Namespace:
`commands` property - the same as in the box. Allows nesting.
### Command:
[parameters-docs]: <https://github.com/NebelFox/verbox/blob/dev/docs//parameters.md>
`parameters` or `params` property - an array of [string parameter definitions][parameters-docs].  

---

> **Note**. If _**neither**_ commands _**nor**_ parameters/params property
> specified, the executable is _**interpreted as a Command**_.
> If both specified - the executable is  _**interpreted as a Namespace**_,
> and parameters property is ignored.
> _**Both**_ properties _**can't**_ be specified in _**the same**_ executable.

[inheritance-docs]: <https://github.com/NebelFox/verbox/blob/dev/docs/json-deserialization/inheritance.md>

> **Note**. A single _**string**_ may be used _**instead**_ of an _**executable object**_.
> It is _**interpreted as a Command**_ with _**just a name**_, set to the value
> of _**this string**_.  
> Examples of usage in [inheritance.md][inheritance-docs]
