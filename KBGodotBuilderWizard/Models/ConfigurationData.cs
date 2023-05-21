using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Serialization;
using KBAvaloniaCore.DataAnnotations;
using KBAvaloniaCore.IO;
using KBAvaloniaCore.Miscellaneous;

namespace KBGodotBuilderWizard.Models;

[Serializable]
[XmlRoot(nameof(ConfigurationData))]
public class ConfigurationData
{
    public readonly static Path DefaultConfigurationFile = Path.Combine(System.IO.Path.GetTempPath(), Assembly.GetCallingAssembly().GetName().Name!, "SaveDataConfiguration.xml");
    private Path? _installVersionsPath;
    
    
    [XmlElement(nameof(ConfigurationData.InstallVersionsPath))]
    [RequiredPath(ErrorMessage = "Please enter the path to install Godot executables.", AllowNonExistingPath = false)]
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

    public bool IsValid(out IEnumerable<ValidationResult> validationResults)
    {
        ValidationContext validationContext = new ValidationContext(this, null, null);
        validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(this, validationContext, validationResults as List<ValidationResult>, true);
    }

    public Result Save()
    {
        ConfigurationData.DefaultConfigurationFile.CreateDirectory();
        return XmlSerializableHelper.Save(this, (string)ConfigurationData.DefaultConfigurationFile);
    }

    public Result Load()
    {
        if (!ConfigurationData.DefaultConfigurationFile.Exists())
        {
            return Result.CreateFailure($"Configuration file '{ConfigurationData.DefaultConfigurationFile}' does not exist.");
        }

        return XmlSerializableHelper.Load((string)ConfigurationData.DefaultConfigurationFile, this);
    }
}