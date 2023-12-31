using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Xml;
using KB.SharpCore.Utils;
using Path = KB.SharpCore.IO.Path;

namespace KB.SharpCore.Serialization;

public static class DataContractSerializableHelper
{
    public enum ESerializationType
    {
        Xml,
        Json,
        Binary,
    }

    public static Task<Result> SaveAsync<T>(T serializable, Path path, ESerializationType serializationType)
    {
        return Task.Run(() => DataContractSerializableHelper.Save(serializable, path, serializationType));
    }

    public static Result Save<T>(T serializable, Path path, ESerializationType serializationType)
    {
        if (serializable == null)
        {
            throw new ArgumentNullException(nameof(serializable));
        }

        try
        {
            using (FileStream fileStream = new FileStream(path.FullPath, FileMode.Create))
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                switch (serializationType)
                {
                    case ESerializationType.Xml:
                        dataContractSerializer.WriteObject(fileStream, serializable);
                        break;
                    case ESerializationType.Json:
                        using (XmlDictionaryWriter jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(fileStream, Encoding.UTF8, true, true))
                        {
                            dataContractSerializer.WriteObject(jsonWriter, serializable);
                        }

                        break;
                    case ESerializationType.Binary:
                        dataContractSerializer.WriteObject(fileStream, serializable);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(serializationType), serializationType, null);
                }

                return Result.CreateSuccess();
            }
        }
        catch (Exception e)
        {
            return Result.CreateFailure(e);
        }
    }

    public static Task<Result<T>> LoadAsync<T>(Path path, ESerializationType serializationType)
    {
        return Task.Run(() => DataContractSerializableHelper.Load<T>(path, serializationType));
    }

    public static Result<T> Load<T>(Path path, ESerializationType serializationType)
    {

        if (!path.Exists())
        {
            return Result<T>.CreateFailure($"File '{path.FullPath}' does not exist");
        }
        
        try
        {
            using (FileStream fileStream = new FileStream(path.FullPath, FileMode.Create))
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                switch (serializationType)
                {
                    case ESerializationType.Xml:
                    {
                        using(XmlReader xmlReader = XmlReader.Create(fileStream))
                        {
                            return Result<T>.CreateSuccess((T)dataContractSerializer.ReadObject(xmlReader));
                        }
                    }
                    case ESerializationType.Json:
                    {
                        using (XmlDictionaryReader jsonReader = JsonReaderWriterFactory.CreateJsonReader(fileStream, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null))
                        {
                            return Result<T>.CreateSuccess((T)dataContractSerializer.ReadObject(jsonReader));
                        }
                    }
                    case ESerializationType.Binary:
                    {
                        return Result<T>.CreateSuccess((T)dataContractSerializer.ReadObject(fileStream));                        
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(serializationType), serializationType, null);
                }
            }
        }
        catch (Exception e)
        {
            return Result<T>.CreateFailure(e);
        }
    }

    public static Task<Result> LoadAsync<T>(T fillObject, Path path, ESerializationType serializationType)
    {
        return Task.Run(() => DataContractSerializableHelper.Load(fillObject, path, serializationType));
    }
    
    public static Result Load<T>(T fillObject, Path path, ESerializationType serializationType)
    {
        Result<T> loadResult = DataContractSerializableHelper.Load<T>(path, serializationType);
        if (loadResult.IsFailure)
        {
            return loadResult.ToResult();
        }
        
        DataContractSerializableHelper._FillObjectData(loadResult.Value, fillObject);
        return Result.CreateSuccess();
    }
    
    private static void _FillObjectData<T>(T sourceObject, T destinationObject)
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
                prop.SetValue(destinationObject, item, null);
            }
            else
            {
                // Get the value of the property from the file
                object? value = prop.GetValue(sourceObject, null);
                // Set the value of the property to this instance
                prop.SetValue(destinationObject, value, null);
            }
        }
    }
}