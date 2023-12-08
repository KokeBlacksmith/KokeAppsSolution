using Avalonia.Controls;
using KB.SharpCore.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls
{
    /// <summary>
    /// Object that can be dragged, resized and moved around within a <see cref="EditorCanvas"/>.
    /// </summary>
    public interface IEditableControl
    {
        /// <summary>
        /// Event raised when the position X of the control changes.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<double>>? PositionXChanged;

        /// <summary>
        /// Event raised when the position Y of the control changes.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<double>>? PositionYChanged;

        /// <summary>
        /// Event raised when the width of the control changes.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<double>>? WidthChanged;

        /// <summary>
        /// Event raised when the height of the control changes.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<double>>? HeightChanged;

        /// <summary>
        /// Event raised when the <see cref="IsSelected"/> property changes.
        /// </summary>
        event EventHandler<ValueChangedEventArgs<bool>>? IsSelectedChanged;

        double PositionX { get; set; }
        double PositionY { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        bool IsSelected { get; set; }

        Control Control { get; }
    }
}
