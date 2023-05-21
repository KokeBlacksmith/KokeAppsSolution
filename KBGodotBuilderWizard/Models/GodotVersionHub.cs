using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using KBAvaloniaCore.IO;

namespace KBGodotBuilderWizard.Models;

[DataContract]
public class GodotVersionHub
{
    public readonly static Path GodotInstallsDataPath = Path.Combine(
                                                                System.IO.Path.GetTempPath(), 
                                                                Assembly.GetCallingAssembly().GetName().Name!, 
                                                                $"{nameof(GodotVersionHub)}Data.xml");
    public GodotVersionHub()
    {
    }
    
    [DataMember]
    public List<GodotVersion> Versions { get; set; } = new List<GodotVersion>();
    
    public Task SerializeAsync()
    {
        return DataContractSerializableHelper.SaveAsync(this, GodotVersionHub.GodotInstallsDataPath, DataContractSerializableHelper.ESerializationType.Xml);
    }
    
    public Task DeserializeAsync()
    {
        return DataContractSerializableHelper.LoadAsync(this, GodotVersionHub.GodotInstallsDataPath, DataContractSerializableHelper.ESerializationType.Xml);
    }
}