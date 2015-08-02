

# Introduction #

This very simple little pattern can be used in everything from websites implementing complicated features like undo, to implementing very simple features like command line parsing in a console application.

# Motivating Example #
  * A command-line order management system
  * Existing order may be edited, and a log of all changes must be kept
In order to explain the command pattern I'm going to take the simplest possible example that I can so we can focus on the abstraction of the pattern itself. We're gonna take a command-line order management system, where orders can be created and edited and shipped, and a log of everything that happens to an order has to be kept. And I'm gonna focus on one of these commands that needs to be handled the editing command. Just to give you a little bit of context, I have a program here this is my main app, there is static void main here, and it is just gonna go of an run my first program.
```
class Program
{
  static void Main(String[] arga)
  {
    Program1.Run(args);
  }
}
```
I have a second program that looks a little bit different that uses the command pattern strategy that we will look at later. But lets go ahead for now and look at the Run method.
```
public class Program1
{
  public static void Run(string[] args)
  {
    if(args.Length == 0)
    {
      PrintUsage();
      return;
    }
    var processor = new CommandExecutor();
    processor.ExecuteCommand(args);
  }

  private static void PrintUsage()
  {
    Console.WriteLine("Usage: LoggingDemo CommandName Arguments");
    Console.WriteLine("Commands:");
    Console.WriteLine("  UpdateQuantity number");
    Console.WriteLine("  CreateOrder");
    Console.WriteLine("  ShipOrder");
  }
}
```
In the Run method we basically take a look at the arguments that are coming in, this is a command-line application after all. And if we are not passed any arguments on the command-line we go ahead and print out usage information so people can figure out how to use our app. Assuming arguments are passed we create a little helper called CommandExecutor and we call ExecuteCommand, now this is not the command pattern, this is just a little helper class thats gona go of and do this work and get us out of our main program.
```
public class CommandExecutor
{
  internal void ExecuteCommand(string[] args)
  {
    switch(args[0])
    {
      case "UdateQuantity":
        UpdateQuantity(int.Parse(args[1]));
        break;
      case "CreateOrder":
        CreateOrder();
        break;
      case "ShipOrder":
        ShipOrder();
        break;
      default:
        Console.WriteLine("Unrecognized command");
        break;
    }
  }
}
```
Now the first smell that I see here when I walk into this command is a big switch statement, and any time we see a switch statement we should be thinking to ourselves, Hmm maybe there is a better way to do this. But we'll come back to that and deal with that later. Basically what we do is we look at the first argument which tells us which command is to be executed. But the only one that I have implemented is UpdateQuantity.
```
private void UpdateQuantity(int newQuantity)
{
  // simulate updating a database
  const int oldQuantity = 5;
  Console.WriteLine("DATABASE: Updated");

  // simulate logging
  Console.WriteLine("LOG: Updated order quantity from{0} to {1}", oldQuantity, newQuantity);
}

void CreateOrder() {}
void ShipOrder() {}
```
Here we are simulating a update to database and logging. These represent two different dependencies, dependency on repository of orders, dependency on a log system as well. We only have one order in the system, that keeps me from having to pass around identifiers and things like that, I really want to keep this simple to just focus on the abstraction of the command.

One of the requirement of the system is that whenever a change is made to an order, we want to see details of what change was made and if I was building this for real I also want to know who made the change.

Now what I am interested in doing in this example is simplifying this CommandExecutor, it is basically doing all the work inside that one class, and if we implement more methods there might be awful lot of dependencies. And this is going to become a class that is doing awful lot, and have way too many responsibilities. So we are going to figure out a way to use the command pattern to break this thing up into separate commands that do their own thing and get rid of this class.

# Intent and Applicability #
  * Represent an action as an object

  * Decouple clients that execute the command from the details and dependencies of the command logic

  * Enables delayed execution
    * Can queue commands for later execution
    * If command objects are also persistent, can delay across process restarts

The intent of this pattern is really simple, we want to represent a piece of business logic or just a piece of logic in general, some action, as an object that implements an interface. Lets see how that interface could look like.
```
public interface ICommand
{
  void Execute();
}
```
This is the simplest form, we have an interface that has a method on it that doesn't take any arguments and returns nothing. It is basically just a way for our client to be given given something to execute it at a later time when it makes sense.

So this completely decouples the client thats gona call execute from any of the details of how that command will get executed. The client doesn't have to care about any dependencies, all of that stuff will be set up beforehand, and then we can just wire our system together with commands. One of the things this also enables is delayed execution, I can que commands up and execute them later on in a certain order if I like. I can also persist commands if I decide to go further and make my command object support persistence in some way. That I might be able to actually
put that thing on disk and bring it back up and execute it later on, without caring exactly what is is going to do I just know that I need to have those instructions run. And this can be incredibly valuable implementing complicated features like undo in a web farm scenario.

**Also Knows As**
  * Action, Transaction
You may have heard this command pattern referred to as Action pattern or Transactions those are also very common names for these things.

**Applicability**
  * Logging
  * Validation
  * Undo
We talk about this a little bit more as we go thru some code, but a number of things you can do with the command pattern is enforce Logging, if your command encapsulates the details of executing some action and then logging the fact that it was executed, if clients are always using commands to execute actions then you know that you're logging is always going to happen, thats one thing we actually use on our website at pluralsight.com. When critical things are happening that need to be logged.

Validation is another thing that is useful with the command pattern as we'll talk about a little bit later.

And Undo functionality in a website, this is another great example of where in a website or in any application, where command object can be used.

If a command has all of the information that it needs to perform some action, it can record everything that it needs to know while it is performing the action in order to be able to reverse that action.

# Structure and Consequences #

<img src='http://s11.postimg.org/wceavt58j/command_pattern.png' />

So here is a pictorial view of the command pattern. There is a client out there somewhere that has a reference to a command object that implements a method called Execute or Do or RunTransaction it does not matter what it is called, it is a method that does not take any arguments, the client does not need to provide any context, it just executes that command. The command object itself may hold references to several dependencies in order to get its work done, that is fine as long as the client is shielded from those dependencies. We could also take this one step further id for instance you wanted to implement Validation, one very interesting thing you could do is add a Validate method to you command, so now you have Validate and Execute. And what can happen is that the client might accept commands from a command-line console or the GUI or somthing like that, and batch them up for later execution. Perhaps it is a batch job that is gona run at night. But we might wanna do Validation on those commands early to make sure that all of the context for those commands is correct, The input we provided for each of those commands is good. So we can run the Validate method on each of those commands to make sure that their good before we actually execute them. And then we can perhaps Undo a command, if you put Undo on your command interface than that means that all of your commands should be able to be undone or if you call undo they should do nothing, but very likely if you put an undo method on your command interface youre saying to the world all of my commands can be undone. There are other ways of implementing undo with command objects that don't require you to put undo on your command. For example you might have a command execute and return a compensating command that can act as the undo, and if a command is undoable you can return null. There's a lot of different ways of playing with this. The key thing to think about with the command pattern, is that you have a method that somebody can call without having to have any extra data to call it with.

<img src='http://s14.postimg.org/rwubiqpxd/command_pattern2.png' />

**Consequences**
  * commands must be completely self contained
    * The client doesn't pass in any arguments
  * Easy to add new commands
    * Just add a new class (open/closed principal)

Now because the client does not pass any arguments into this command, this makes it a little bit different than something like strategy. With strategy you have a very specific thing that you are doing and you are passing in some arguments that give the strategy some context. And it is just done in a different way depending on the strategy that is chosen. The command pattern is more general than that, it is not doing any one particular thing, a command can represent anything, could represent screwing in a lightbulb or painting a car whatever it could represent, but the client does not care about those details does not have to pass any arguments in, but a consequence of that is that the command has to be constructed with everything it needs in order to get its job done, before the client can execute it, when the client executes it that object needs to have all the context that it needs, its not going to get any context from the client. It needs to be able to have its own context when it is build. This often argus for having factories to build these things and you see an example of that when I show you some code in a minute. It is easy to add new commands to the system, this is an example of the Open Closed principle. If we have a system that is based on commands, then instead of going and modifying lets say the CommandExecutor class in our CommandExecutor for example when we want to add a new command we have to modify that class, so this class is not closed to modification which is what the Open Closed principle tells us is a good idea, it is a better idea to make sure that we don't have to update it every time we have a new command that we want to run, we like to just extend the system by creating a new command object, and when we use the command pattern that is the place that you get to. So with the command pattern it is pretty easy to satisfy the Open Closed principle by simply adding new commands when you need a new functionality.

# Implementation Example #
  * A command-line order management system
  * Existing orders may be edited, and a log of all changes must be kept

Now lets create the same command-line application but using the command pattern instead of the CommandExecutor class.
```
class Program
{
  static void Main(string[] args)
  {
    Program2.Run(args);
  }
}
```

We are using Program2 instead of Program1 and now we are using the command pattern.

```
public class Program2
{
  public static void Run(string[] args)
  {
    var availableCommands = GetAvailableCommands();

    if(args.Lenght == 0)
    {
      PrintUsage(availableCommands);
      return;
    }

    var parser = new CommandParser(availableCommands);
    var command = parser.ParseCommand(args);

    command.Execute();
  }

  static IEnumerable<ICommandFactory> GetAvailableCommands()
  {
    return new ICommandFactory[]
      {
        new CreateOrderCommand(),
        new UpdateQuantityCommand(),
        new ShipOrderCommand(),
      };
  }

  private static void PrintUsage(IEnumerable<ICommandFactory> availableCommands)
  {
    Console.WriteLine("Usage: LoggingDemo CommandName Arguments");
    Console.WriteLine("Commands:");
    foreach(var command in availableCommands)
      Console.WriteLine(" {0}", command.Description);
  }
}
```
If we look at the Run method you can see that there is a command that comes back from ParseCommand method and the command object implements ICommand.
```
public interface ICommand
{
  void Execute();
}
```
Each command is a separate class that implements the ICommand. Now lets look at that command class.
```
public class UpdateQuantityCommand : ICommand, ICommandFactory
{
  public int NewQuantity { get; set; }

  public void Execute()
  {
    // simulate updating a database
    const int oldQuantity = 5;
    Console.WriteLine("DATABASE: Updated");

    // simulate logging
    Console.WriteLine("LOG: Updated order quantity from {0} to {1}", oldQuantity, NewQuantity);
  }

  public string CommandName { get { return "UpdateQuantity"; } }
  public string Description { get { return "UpdateQuantity number"; } }

  public ICommand MakeCommand(string[] arguments)
  {
    return new UpdateQuantityCommand {NewQuantity = int.Parse(arguments[1]) };
  }
}
```
Now the question is when a user passes a command argument how do we create these commands, that is where factories come in. We have something called ICommandFactory its job is to know something about the commands that it creates, the command name, description and how to create a instance of the command by parsing command-line arguments.
```
public interface ICommandFactory
{
  string CommandName { get; }
  string Description { get; }

  ICommand MakeCommand(string[] arguments);
}
```
The CommandParser takes the available commands and uses them to discover which command we want.
```
public class CommandParser
{
  readonly IEnumerable<ICommandFactory> availableCommands;

  public CommandParser(IEnumerable<ICommandFactory> availableCommands)
  {
    this.availableCommands = availableCommands;
  }

  internal ICommand ParseCommand(string[] args)
  {
    var requestedCommandName = args[0];
  
    var command = FindRequestedCommand(requestedCommandName);
    if(null == command)
      return new NotFoundCommand { Name = requestedCommandName };

    return command.MakeCommand(args);
  }

  ICommandFactory FindRequestedCommand(string commandName)
  {
    return availableCommands.FirstOrDefault(cmd => cmd.CommandName == commandName);
  }
}
```

```
public class NotFoundCommand : ICommand
{
  public string Name { get; set; }
  public void Execute()
  {
    Console.WriteLine("Couldn't find command: " + Name);
  }
}
```

# Related Patterns #
  * Factory Pattern
    * Factories are often useful to construct command objects
  * Null Object
    * Often times returning a "null command" can be useful instead of returning null
  * Composite
    * A composite command can be useful
    * Construct it with several "child" commands
    * Execute() on the composite will call Execute on the child commands

Patterns are often times related, in fact the command pattern lends itself very well to be used with factories, and you saw an example of that earlier were we had the command object implement factory interface. We also used the Null object mini pattern, where instead of returning null we return an object that implements the interface that is required but did it in a safe way.

We can also use what is called a Composite command, if we have a command that has several child commands and when executed the execution applies to all the child command to, and that pattern is often used with the command pattern.