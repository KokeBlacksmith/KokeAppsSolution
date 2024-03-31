using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.ConsoleCompanionAPI.Data;
public abstract class MainCommandManagerBase
{
    protected Dictionary<string, SubCommandManagerBase>? m_subManagersDictionary;

    public MainCommandManagerBase() 
    { 
        m_InitializeSubManagers();
    }

    protected abstract void m_InitializeSubManagers();

    private bool _TryGetSubCommandManagerFromCommand(ConsoleCommand command, out SubCommandManagerBase? subManager)
    {
        subManager = null;
        int endIndentifierIndex = command.Command.IndexOf(" ");
        if (endIndentifierIndex >= 1)
        {
            string managerIdentifier = command.Command.Substring(0, endIndentifierIndex).Trim().ToUpper();
            if (m_subManagersDictionary!.TryGetValue(managerIdentifier, out subManager))
            {
                return true;
            }
        }

        return false;
    }

    public ConsoleCommand ExecuteCommand(ConsoleCommand command)
    {
        if (_TryGetSubCommandManagerFromCommand(command, out SubCommandManagerBase? subManager))
        {
            return subManager!.ExecuteCommand(command);
        }

        return ConsoleCommand.CreateResponseError(command, "Couldn't retrieve a sub manager handling this command.");
    }

    public ConsoleCommand GetAvailableCommands()
    {
        List<string> availableCommands = new List<string>();

        foreach (SubCommandManagerBase subManager in m_subManagersDictionary!.Values)
        {
            availableCommands.AddRange(subManager.GetAvailableCommandsStringCollection());
        }

        return ConsoleCommand.CreateRequestAvailableCommandsResponse(availableCommands);
    }

    public ConsoleCommand GetAvailableSubCommands(ConsoleCommand parentCommand)
    {
        if (_TryGetSubCommandManagerFromCommand(parentCommand, out SubCommandManagerBase? subManager))
        {
            return null;
        }

        return null;
    }
}
