namespace KB.ConsoleCompanionAPI.Data;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandMethodAttribute : Attribute
{
    public CommandMethodAttribute(string name, bool hasSubCommands)
    {
        Name = name;
        HasSubCommands = hasSubCommands;
    }

    public string Name { get; }
    public bool HasSubCommands { get; }
}
