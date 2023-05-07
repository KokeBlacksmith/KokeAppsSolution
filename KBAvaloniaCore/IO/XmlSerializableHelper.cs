using System.Reflection;
using System.Xml.Serialization;
using JetBrains.Annotations;
using KBAvaloniaCore.Miscellaneous;

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

    public static Result<T> Load<T>(string path)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return new Result<T>((T)xmlSerializer.Deserialize(fileStream), true);
            }
            catch (Exception e)
            {
                return new Result<T>(e.Message);
            }
        }
    }

    public static Result Load<T>(string path, [NotNull] T toFillObject)
        where T : class
    {
        if (toFillObject == null)
            throw new ArgumentNullException(nameof(toFillObject));

        Result<T> deserializedResult = XmlSerializableHelper.Load<T>(path);
        if (deserializedResult.IsSuccess)
        {
            XmlSerializableHelper._Load(toFillObject, deserializedResult.Value);
        }

        return deserializedResult.ToResult();
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