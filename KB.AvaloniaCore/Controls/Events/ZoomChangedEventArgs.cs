using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls.Events;

public class ZoomChangedEventArgs : EventArgs
{
    public ZoomChangedEventArgs(double zoomX, double zoomY, double offsetX, double offsetY)
    {
        ZoomX = zoomX;
        ZoomY = zoomY;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public double ZoomX { get; }
    public double ZoomY { get; }
    public double OffsetX { get; }
    public double OffsetY { get; }
}
