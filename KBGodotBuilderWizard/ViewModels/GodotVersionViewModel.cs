using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using KBAvaloniaCore.ReactiveUI;
using KBGodotBuilderWizard.Models;
using ReactiveUI;

namespace KBGodotBuilderWizard.ViewModels;

public class GodotVersionViewModel : BaseViewModel, IReactiveModel<GodotVersion>
{
    private AvaloniaList<GodotExecutableViewModel> _installs = new AvaloniaList<GodotExecutableViewModel>();
    private string _version;

    public GodotVersionViewModel(GodotVersion model)
    {
        this.Model = model;
        this.FromModel(model);
    }

    public string Version
    {
        get { return _version; }
        set { this.RaiseAndSetIfChanged(ref _version, value); }
    }

    public AvaloniaList<GodotExecutableViewModel> Installs
    {
        get { return _installs; }
        private set { this.RaiseAndSetIfChanged(ref _installs, value); }
    }
    
    public async Task FetchAvailableDownloads()
    {
        await Model.FetchAvailableDownloads();
        this.Installs = new AvaloniaList<GodotExecutableViewModel>(Model.Executables.Select(exe => new GodotExecutableViewModel(exe)));
    }


    #region IReactiveModel

    public GodotVersion Model { get; }
    
    public void FromModel(GodotVersion model)
    {
        this.Version = model.Version;
        this.Installs = new AvaloniaList<GodotExecutableViewModel>(Model.Executables.Select(exe => new GodotExecutableViewModel(exe)));
    }

    public void UpdateModel()
    {
        Model.Version = this.Version;

        List<GodotExecutable> executables = new List<GodotExecutable>(this.Installs.Count);
        foreach (GodotExecutableViewModel vm in Installs)
        {
            vm.UpdateModel();
            executables.Add(vm.Model);
        }

        Model.Executables = executables;
    }

    #endregion
}