using Avalonia.Controls;
using KB.AvaloniaCore.Controls.GraphEditor;
using KB.SharpCore.Enums;

namespace KB.ConsoleCompanion.Nodes;
public sealed partial class TimerNode : Node
{
    public TimerNode()
    {
        InitializeComponent();
        NodePin pin1 = new NodePin() { Width = 10, Height = 10};
        NodePin pin2 = new NodePin() { Width = 10, Height = 10};
        NodePin pin3 = new NodePin() { Width = 10, Height = 10};
        NodePin pin11 = new NodePin() { Width = 10, Height = 10};
        NodePin pin12 = new NodePin() { Width = 10, Height = 10};
        NodePin pin13 = new NodePin();

        AddConnectionPin(pin1, ESide.Left);
        AddConnectionPin(pin2, ESide.Left);
        AddConnectionPin(pin3, ESide.Left);
        AddConnectionPin(pin11, ESide.Top);
        AddConnectionPin(pin12, ESide.Top);
        AddConnectionPin(pin13, ESide.Top);
    }
}
