using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace KB.AvaloniaCore.TemplateSelectors
{
    public abstract class BaseEnumTemplateSelector<T> : IDataTemplate
    {
        [Content]
        public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();

        public Control? Build(object? param)
        {
            T enumValue = (T)typeof(T).GetProperty(EnumPropertyName)!.GetValue(param)!;
            return Templates[Enum.GetName(typeof(T), enumValue)!].Build(param);
        }

        public bool Match(object? data)
        {
            return data is T;
        }

        protected abstract string EnumPropertyName { get; }
    }
}
