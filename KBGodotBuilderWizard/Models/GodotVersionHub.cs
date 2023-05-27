using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using KBAvaloniaCore.DataAnnotations;
using KBAvaloniaCore.IO;
using KBAvaloniaCore.MessageBox;

namespace KBGodotBuilderWizard.Models;

[DataContract]
public class GodotVersionHub
{
    public readonly static Path GodotInstallsDataPath = Path.Combine(
                                                                System.IO.Path.GetTempPath(), 
                                                                Assembly.GetCallingAssembly().GetName().Name!, 
                                                                $"{nameof(GodotVersionHub)}Data.xml");

    private Path? _installVersionsPath;
    
    public GodotVersionHub()
    {
    }
    
    [RequiredPath(ErrorMessage = "Please enter the path to install Godot executables.", AllowNonExistingPath = false)]
    [DataMember]
    public string InstallVersionsPath 
    {
        get
        {
            return _installVersionsPath?.FullPath ?? String.Empty;
        }
        set
        {
            _installVersionsPath = new Path(value);
        }
    }
    
    [DataMember]
    public List<GodotVersion> Versions { get; set; } = new List<GodotVersion>();
    
    internal bool IsValid(out IEnumerable<ValidationResult> validationResults)
    {
        ValidationContext validationContext = new ValidationContext(this, null, null);
        validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(this, validationContext, validationResults as List<ValidationResult>, true);
    }
    
    public Task<Result> SerializeAsync()
    {
        return DataContractSerializableHelper.SaveAsync(this, GodotVersionHub.GodotInstallsDataPath, DataContractSerializableHelper.ESerializationType.Xml);
    }
    
    public Task<Result> DeserializeAsync()
    {
        return DataContractSerializableHelper.LoadAsync(this, GodotVersionHub.GodotInstallsDataPath, DataContractSerializableHelper.ESerializationType.Xml);
    }
}