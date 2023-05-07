﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Serialization;
using KBAvaloniaCore.DataAnnotations;
using KBAvaloniaCore.IO;

namespace KBGodotBuilderWizard.Models;

[Serializable]
[XmlRoot(nameof(ConfigurationFileData))]
public class ConfigurationFileData
{
    public readonly static Path DefaultConfigurationFile = Path.Combine(System.IO.Path.GetTempPath(), Assembly.GetCallingAssembly().GetName().Name!, "SaveDataConfiguration.xml");

    [XmlElement(nameof(ConfigurationFileData.InstallVersionsPath))]
    [RequiredPath(ErrorMessage = "Please enter the path to install Godot executables.", AllowNonExistingPath = false)]
    public Path InstallVersionsPath { get; set; }

    public bool IsValid(out IEnumerable<ValidationResult> validationResults)
    {
        ValidationContext validationContext = new ValidationContext(this, null, null);
        validationResults = new List<ValidationResult>();

        return Validator.TryValidateObject(this, validationContext, validationResults as List<ValidationResult>, true);
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

        XmlSerializableHelper.Load((string)ConfigurationFileData.DefaultConfigurationFile, this);
    }
}