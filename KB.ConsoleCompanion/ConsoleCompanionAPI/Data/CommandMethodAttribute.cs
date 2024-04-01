namespace KB.ConsoleCompanionAPI.Data;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class CommandMethodAttribute : Attribute
{
    public CommandMethodAttribute(string name) : this(name, null)
    {

    }

    public CommandMethodAttribute(string name, Type? parameterType)
    {
        Name = name;
        ParameterType = parameterType;
    }

    public string Name { get; }
    public Type? ParameterType { get; }
}
