namespace KB.ConsoleCompanionAPI.Data;

[AttributeUsage(AttributeTargets.Class)]
public class SubCommandManagerAttribute : Attribute
{
    public SubCommandManagerAttribute(string identifier) 
    { 
        Identifier = identifier?.ToUpper() ?? throw new ArgumentNullException($"{nameof(identifier)} can't be null.");
    }

    public string Identifier { get; }
}
