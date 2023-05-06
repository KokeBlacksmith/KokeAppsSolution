using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using KBAvaloniaCore.DataAnnotations;
using KBAvaloniaCore.IO;

namespace KBGodotBuilderWizard.Models;

[Serializable]
[XmlRoot(nameof(ConfigurationFileData))]
public class ConfigurationFileData
{
    public static readonly KBAvaloniaCore.IO.Path DefaultConfigurationFile = KBAvaloniaCore.IO.Path.Combine(
                                                                                        System.IO.Path.GetTempPath(), 
                                                                                        System.Reflection.Assembly.GetCallingAssembly().GetName().Name!, 
                                                                                        "SaveDataConfiguration.xml");

    [XmlElement(nameof(ConfigurationFileData.InstallVersionsPath))]
    [RequiredPath(ErrorMessage = "Please enter the path to install Godot executables.", AllowNonExistingPath = false)]
    public KBAvaloniaCore.IO.Path InstallVersionsPath { get; set; } = null;

    public bool IsValid(out IEnumerable<ValidationResult> validationResults)
    {
        ValidationContext validationContext = new ValidationContext(this, serviceProvider: null, items: null);
        validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(this, validationContext, validationResults as List<ValidationResult>, validateAllProperties: true);
    }
    
    public void Save()
    {
        ConfigurationFileData.DefaultConfigurationFile.CreateDirectory();
        XmlSerializableHelper.Save(this, (string)ConfigurationFileData.DefaultConfigurationFile);
    }
    
    public void Load()
    {
        if (!ConfigurationFileData.DefaultConfigurationFile.Exists())
        {
            return;
        }

        XmlSerializableHelper.Load<ConfigurationFileData>((string)ConfigurationFileData.DefaultConfigurationFile, this);
    }
}