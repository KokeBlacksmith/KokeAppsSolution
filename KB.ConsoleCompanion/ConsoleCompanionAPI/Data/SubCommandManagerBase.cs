using KB.SharpCore.Utils;
using System.Reflection;

namespace KB.ConsoleCompanionAPI.Data;

public abstract class SubCommandManagerBase
{
    public ConsoleCommand ExecuteCommand(ConsoleCommand command)
    {
        Result keywordsResult = command.TryExtractKeywords(out string? subManagerKey, out string? commandMethodKey, out string? parameterKey);
        if(keywordsResult.IsFailure)
        {
            return ConsoleCommand.CreateResponseError(command, $"Failed to retrieve keywords for the command '{command.Command}'.");
        }

        // Get the method that has the attribute with value coomandMethodKey
        MethodInfo? targetMethod = null;
        CommandMethodAttribute? targetCommandMethodAttribute = null;
        foreach (MethodInfo method in this.GetType().GetMethods())
        {
            CommandMethodAttribute? commandMethodAttribute = method.GetCustomAttribute<CommandMethodAttribute>();
            if(commandMethodAttribute == null)
            {
                continue;
            }

            if(String.Equals(commandMethodAttribute.Name, commandMethodKey, StringComparison.InvariantCultureIgnoreCase))
            {
                targetMethod = method;
                targetCommandMethodAttribute = commandMethodAttribute;
                break;
            }
        }

        if(targetMethod == null) 
        {
            return ConsoleCommand.CreateResponseError(command, $"Failed to retrieve a method to execute the command '{commandMethodKey}' in the submanager '{subManagerKey}'.");
        }

        if(targetCommandMethodAttribute!.ParameterType != null)
        {
            if(String.IsNullOrWhiteSpace(parameterKey)) 
            {
                return ConsoleCommand.CreateResponseError(command, $"The command '{commandMethodKey}' is missing a parameter of type '{targetCommandMethodAttribute!.ParameterType.Name}'");
            }

            object? parameter = Convert.ChangeType(parameterKey, targetCommandMethodAttribute!.ParameterType);
            if(parameter == null) 
            {
                return ConsoleCommand.CreateResponseError(command, $"Couldn't convert the paremeter '{parameterKey}' to '{targetCommandMethodAttribute!.ParameterType.Name}'");
            }

            return (ConsoleCommand)targetMethod.Invoke(this, new object[] { command, parameter! })!;
        }
        else if(!String.IsNullOrWhiteSpace(parameterKey))
        {
            return ConsoleCommand.CreateResponseError(command, $"The command '{commandMethodKey}' does not have parameters but a parameter with value '{parameterKey}' was provided.");
        }
        else
        {
            return (ConsoleCommand)targetMethod.Invoke(this, new object[] { command })!;
        }
    }

    public IEnumerable<string> GetAvailableCommandsStringCollection()
    {
        string identifier = GetIndentifier(this.GetType());
        List<string> commands = new List<string>();
        IEnumerable<MethodInfo> methods = this.GetType().GetMethods()
                                .Where(m => m.GetCustomAttributes(typeof(CommandMethodAttribute), true).Any());

        foreach (MethodInfo method in this.GetType().GetMethods()) 
        {
            CommandMethodAttribute? attribute = method.GetCustomAttribute(typeof(CommandMethodAttribute), true) as CommandMethodAttribute;
            if(attribute == null)
            {
                continue;
            }

            commands.Add(identifier + " " + attribute.Name.ToUpper());
        }

        return commands;
    }

    public static string GetIndentifier(Type subCommandManagerType)
    {
        SubCommandManagerAttribute? attribute = subCommandManagerType.GetCustomAttribute(typeof(SubCommandManagerAttribute), true) as SubCommandManagerAttribute;
        if(attribute == null)
        {
            throw new InvalidDataException($"{subCommandManagerType.Name} does not have the attribute {nameof(SubCommandManagerAttribute)}");
        }

        return attribute.Identifier.ToUpper();
    }
}
