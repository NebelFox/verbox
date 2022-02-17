_**styles.json**_
```json
{
    "styles": {
        "common": {
            "dialogue": {
                "greeting": "Hello world!",
                "farewell": "See you soon"
            }
        },
        "guest": {
            "base": "common",
            "dialogue": {
                "greeting": "Welcome to authorization menu",
                "prompt-indicator": "[guest]~$ "
            }
        }
    }
}
```
---
_**boxes.json**_
```json
{
    "boxes": {
        "common": {
            "style": "common",
            "commands": [
                {
                    "name": "exit",
                    "brief": "terminates the program"
                }
            ]
        },
        "guest": {
            "base": "common",
            "style": "guest",
            "insert": [
                {
                    "at": "begin",
                    "commands": [
                        {
                            "name": "login",
                            "brief": "authorize via specific credentials",
                            "params": [
                                "login",
                                "password"
                            ]
                        },
                        {
                            "name": "register",
                            "brief": "register a new account via specific credentials",
                            "params": [
                                "login",
                                "password"
                            ]
                        }
                    ]
                }
            ]
        },
        "user": {
            "base": "common",
            "style": {
                "base": "common",
                "dialogue": {
                    "greeting": "Welcome, user",
                    "prompt-indicator": "[user]~$ "
                }
            },
            "insert": [
                {
                    "before": "exit",
                    "commands": [
                        "do-nothing",
                        {
                            "name": "print",
                            "commands": ["username"]
                        },
                        {
                            "name": "change-password",
                            "params": [
                                "current-password",
                                "new-password",
                                "repeat-new-password"
                            ]
                        }
                    ]
                }
            ]
        }
    }
}
```
---
_**Program.cs**_
```c#
using System;
using System.Collections.Generic;
using Verbox;
using Verbox.Text.Serialization;

namespace Foo
{
    class Program
    {
        private static void Main()
        {
            var serializer = new Serializer();
            serializer.Deserialize("styles.json");
            serializer.Deserialize("boxes.json");
            BoxBuilder builder = serializer.Boxes["user"];
            builder.SetActionsByPaths(new Dictionary<string, Action<Context>>
            {
                ["change-password"] = context => Console.WriteLine("Failed to change a password"),
                ["print username"] = _ => Console.WriteLine("User"),
                ["exit"] = context => context.Box.Terminate()
            });
            builder.SetMissingActionsToDummy(); // sets action for "do-nothing"
            builder.Build().StartDialogue();
        }
    }
}
```
