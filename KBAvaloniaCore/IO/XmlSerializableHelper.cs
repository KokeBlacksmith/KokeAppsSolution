using System.Reflection;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace KBAvaloniaCore.IO;

public static class XmlSerializableHelper
{
    public static void Save<T>(T obj, string path)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Create))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(fileStream, obj);
        }
    }

    public static T Load<T>(string path)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(fileStream);
        }
    }
    
    public static void Load<T>(string path, [NotNull] T toFillObject)
        where T : class
    {
        if (toFillObject == null)
            throw new ArgumentNullException(nameof(toFillObject));
        
        T deserialized = XmlSerializableHelper.Load<T>(path);
        XmlSerializableHelper._Load(toFillObject, deserialized);
    }
    
    private static void _Load<T>(T toFillObject, T dataObject)
    {
        IList<PropertyInfo> props = new List<PropertyInfo>(typeof(T).GetProperties());
        foreach (PropertyInfo prop in props)
        {
            // Check if it is a collection
            if (prop.PropertyType.IsGenericType)
            {
                // Get the type of the list
                Type itemType = prop.PropertyType.GetGenericArguments()[0];
                // Create an instance of that type
                object? item = Activator.CreateInstance(itemType);
                // Add it to the list
                prop.SetValue(toFillObject, item, null);
            }
            else
            {
                // Get the value of the property from the file
                object? value = prop.GetValue(dataObject, null);
                // Set the value of the property to this instance
                prop.SetValue(toFillObject, value, null);
            }
        }
    }
}