using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KB.ConsoleCompanionAPI.Data;

public abstract class SubCommandManagerBase
{
    public ConsoleCommand ExecuteCommand(ConsoleCommand command)
    {
        return ConsoleCommand.CreateResponseWarning(command, "Command received but SubCommandManagerBase is still not implemented");
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
