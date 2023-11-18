using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KB.AvaloniaCore.Controls.GraphEditor;

public partial class Node : IEditableControl
{
    double IEditableControl.X { get; set; }
    double IEditableControl.Y { get; set; }
    bool IEditableControl.IsSelected { get; set; }
}
