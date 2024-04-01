using KB.SharpCore.Utils;

namespace KB.ConsoleCompanionAPI.Data;
public abstract class MainCommandManagerBase
{
    protected Dictionary<string, SubCommandManagerBase>? m_subManagersDictionary;

    public MainCommandManagerBase() 
    { 
        m_InitializeSubManagers();
    }

    protected abstract void m_InitializeSubManagers();

    private Result _TryGetSubCommandManagerFromCommand(ConsoleCommand command, out SubCommandManagerBase? subManager)
    {
        subManager = null;
        Result extractKeywordsResult = command.TryExtractKeywords(out string? subManagerKey, out _, out _);
        if(extractKeywordsResult.IsFailure)
        {
            return extractKeywordsResult;
        }

        if (m_subManagersDictionary!.TryGetValue(subManagerKey!.ToUpper(), out subManager))
        {
            return Result.CreateSuccess();
        }

        return Result.CreateFailure($"Couldn't retrieve the submanager '{subManagerKey}'.");
    }

    public ConsoleCommand ExecuteCommand(ConsoleCommand command)
    {
        Result getSubmanagerResult = _TryGetSubCommandManagerFromCommand(command, out SubCommandManagerBase? subManager);
        if(getSubmanagerResult.IsFailure) 
        { 
            return ConsoleCommand.CreateResponseError(command, getSubmanagerResult.MessagesAsString!);
        }

        return subManager!.ExecuteCommand(command);
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
        Result getSubmanagerResult = _TryGetSubCommandManagerFromCommand(parentCommand, out SubCommandManagerBase? subManager);
        if (getSubmanagerResult.IsFailure)
        {
            return ConsoleCommand.CreateResponseError(parentCommand, getSubmanagerResult.MessagesAsString!);
        }

        return null;
    }
}
