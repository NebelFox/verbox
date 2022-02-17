## Definition Syntax

### Positional

Mandatory part: `positional-name`  
Optional parts:
  - value type: `name:type-name`. Defaults to `string`
  - collective tag: `name...` or typed variant `name:type-name...`
  - optional tag: `[name]`  

  All the optional parts can be combined.  
  Examples: `count:int`, `[optional-names...]`

> **Note**. The *collective* tag results into array of parameter type instances.
> e.g. `name... -> string[]`, `digits:int... -> int[]`.  
> But if the parameter is optional and it was omitted, then it's value is set to null.

### Switch
`--switch-name` - as simple as that.

### Option
Just a **switch**, followed by a **positional**, and optional default value after `=`,  
e.g. `--option param:type...=default`, `--option [param...]`.  
If default value omitted, it's set to `null`.