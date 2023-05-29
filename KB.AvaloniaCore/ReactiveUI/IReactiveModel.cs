namespace KB.AvaloniaCore.ReactiveUI;

/// <summary>
/// For viewmodels that are based on a model
/// </summary>
public interface IReactiveModel<TModel>
{
    TModel Model { get; }

    /// <summary>
    /// Updates the viewmodel from the model
    /// </summary>
    /// <param name="model"></param>
    void FromModel(TModel model);
    
    /// <summary>
    /// Updates the model from the viewmodel
    /// </summary>
    /// <param name="model"></param>
    void UpdateModel();
}