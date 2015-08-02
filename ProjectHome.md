# Command Pattern #

This very simple little pattern can be used in everything from websites implementing complicated features like undo, to implementing very simple features like command line parsing in a console application.

# Intent and Applicability #
  * Represent an action as an object

  * Decouple clients that execute the command from the details and dependencies of the command logic

  * Enables delayed execution
    * Can queue commands for later execution
    * If command objects are also persistent, can delay across process restarts

The intent of this pattern is really simple, we want to represent a piece of business logic or just a piece of logic in general, some action, as an object that implements an interface.

**Also Knows As**
  * Action, Transaction
You may have heard this command pattern referred to as Action pattern or Transactions those are also very common names for these things.

**Consequences**
  * commands must be completely self contained
    * The client doesn't pass in any arguments
  * Easy to add new commands
    * Just add a new class (open/closed principal)

<img src='http://s14.postimg.org/rwubiqpxd/command_pattern2.png' width='500px' />

# Related Patterns #
  * Factory Pattern
    * Factories are often useful to construct command objects
  * Null Object
    * Often times returning a "null command" can be useful instead of returning null
  * Composite
    * A composite command can be useful
    * Construct it with several "child" commands
    * Execute() on the composite will call Execute on the child commands