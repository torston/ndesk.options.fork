# NDesk.Options.Fork

## Build Status
#### Master Branch
[![Build status](https://ci.appveyor.com/api/projects/status/9rkhbjxrka6ra66a/branch/master?svg=true)](https://ci.appveyor.com/project/torston/ndesk-options-fork/branch/master) [![NuGet Badge](https://buildstats.info/nuget/Ndesk.Options.Fork)](https://www.nuget.org/packages/Ndesk.Options.Fork/)

## Example
```c#
using System;
using System.Collections.Generic;
using NDesk.Options.Fork;

class Test {
    static int verbosity;

    public static void Main (string[] args)
    {
        bool show_help = false;
        List<string> names = new List<string> ();
        int repeat = 1;

        var p = new OptionSet () {
            { "n|name=", "the {NAME} of someone to greet.", v => names.Add (v) },
            { "r|repeat=", "the number of {TIMES} to repeat the greeting.", (int v) => repeat = v },
            { "v", "increase debug message verbosity", v => { if (v != null) ++verbosity; } },
            { "h|help",  "show this message and exit", v => show_help = v != null },
        };

        List<string> extra;
        try {
            extra = p.Parse (args);
        }
        catch (OptionException e) {
            Console.Write ("greet: ");
            Console.WriteLine (e.Message);
            Console.WriteLine ("Try `greet --help' for more information.");
            return;
        }

        if (show_help) {
            ShowHelp (p);
            return;
        }

        string message;
        if (extra.Count > 0) {
            message = string.Join (" ", extra.ToArray ());
            Debug ("Using new message: {0}", message);
        }
        else {
            message = "Hello {0}!";
            Debug ("Using default message: {0}", message);
        }

        foreach (string name in names) {
            for (int i = 0; i < repeat; ++i)
                Console.WriteLine (message, name);
        }
    }

    static void ShowHelp (OptionSet p)
    {
        Console.WriteLine ("Usage: greet [OPTIONS]+ message");
        Console.WriteLine ("Greet a list of individuals with an optional message.");
        Console.WriteLine ("If no message is specified, a generic greeting is used.");
        Console.WriteLine ();
        Console.WriteLine ("Options:");
        p.WriteOptionDescriptions (Console.Out);
    }

    static void Debug (string format, params object[] args)
    {
        if (verbosity > 0) {
            Console.Write ("# ");
            Console.WriteLine (format, args);
        }
    }
}
```
## Usage:
```
$ mono greet.exe --help
Usage: greet [OPTIONS]+ message
Greet a list of individuals with an optional message.
If no message is specified, a generic greeting is used.

Options:
  -n, --name=NAME            the NAME of someone to greet.
  -r, --repeat=TIMES         the number of TIMES to repeat the greeting.
  -v                         increase debug message verbosity
  -h, --help                 show this message and exit

$ mono greet.exe -v- -n A -name=B --name=C /name D -nE
Hello A!
Hello B!
Hello C!
Hello D!
Hello E!

$ mono greet.exe -v -n E custom greeting for: {0}
# Using new message: custom greeting for: {0}
custom greeting for: E

$ mono greet.exe -r 3 -n A
Hello A!
Hello A!
Hello A!

$ mono greet.exe -r not-an-int
greet: Could not convert string `not-an-int' to type Int32 for option `-r'.
Try `greet --help' for more information.
```
