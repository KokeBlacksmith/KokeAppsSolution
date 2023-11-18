using Avalonia.Controls;
using KB.AvaloniaCore.Controls.GraphEditor;
using KB.SharpCore.Enums;

namespace KB.ConsoleCompanion.Nodes;
public sealed partial class TimerNode : Node
{
    public TimerNode()
    {
        InitializeComponent();
        NodeConnectionPin pin1 = new NodeConnectionPin() { Width = 10, Height = 10};
        NodeConnectionPin pin2 = new NodeConnectionPin() { Width = 10, Height = 10};
        NodeConnectionPin pin3 = new NodeConnectionPin() { Width = 10, Height = 10};
        NodeConnectionPin pin11 = new NodeConnectionPin() { Width = 10, Height = 10};
        NodeConnectionPin pin12 = new NodeConnectionPin() { Width = 10, Height = 10};
        NodeConnectionPin pin13 = new NodeConnectionPin();

        AddConnectionPin(pin1, ESide.Left);
        AddConnectionPin(pin2, ESide.Left);
        AddConnectionPin(pin3, ESide.Left);
        AddConnectionPin(pin11, ESide.Top);
        AddConnectionPin(pin12, ESide.Top);
        AddConnectionPin(pin13, ESide.Top);
    }
}
