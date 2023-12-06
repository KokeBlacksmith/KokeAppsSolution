using Avalonia.Input;
using KB.SharpCore.DesignPatterns.Singleton;

namespace KB.AvaloniaCore.Utils;
public sealed class CursorManager : BaseSingleton<CursorManager>, IDisposable
{
    private CursorManager()
    {
    }

    public Cursor CursorDefault { get; } = Cursor.Default;
    public Cursor CursorArror { get; } = new Cursor(StandardCursorType.Arrow);
    public Cursor CursorIbeam { get; } = new Cursor(StandardCursorType.Ibeam);
    public Cursor CursorWait { get; } = new Cursor(StandardCursorType.Wait);
    public Cursor CursorCross { get; } = new Cursor(StandardCursorType.Cross);
    public Cursor CursorUpArrow { get; } = new Cursor(StandardCursorType.UpArrow);
    public Cursor CursorSizeWestEast { get; } = new Cursor(StandardCursorType.SizeWestEast);
    public Cursor CursorSizeNorthSouth { get; } = new Cursor(StandardCursorType.SizeNorthSouth);
    public Cursor CursorSizeAll { get; } = new Cursor(StandardCursorType.SizeAll);
    public Cursor CursorNo { get; } = new Cursor(StandardCursorType.No);
    public Cursor CursorHand { get; } = new Cursor(StandardCursorType.Hand);
    public Cursor CursorAppStarting { get; } = new Cursor(StandardCursorType.AppStarting);
    public Cursor CursorHelp { get; } = new Cursor(StandardCursorType.Help);
    public Cursor CursorTopSide { get; } = new Cursor(StandardCursorType.TopSide);
    public Cursor CursorBottomSide { get; } = new Cursor(StandardCursorType.BottomSide);
    public Cursor CursorLeftSide { get; } = new Cursor(StandardCursorType.LeftSide);
    public Cursor CursorRightSide { get; } = new Cursor(StandardCursorType.RightSide);
    public Cursor CursorTopLeftCorner { get; } = new Cursor(StandardCursorType.TopLeftCorner);
    public Cursor CursorTopRightCorner { get; } = new Cursor(StandardCursorType.TopRightCorner);
    public Cursor CursorBottomLeftCorner { get; } = new Cursor(StandardCursorType.BottomLeftCorner);
    public Cursor CursorBottomRightCorner { get; } = new Cursor(StandardCursorType.BottomRightCorner);
    public Cursor CursorDragMove { get; } = new Cursor(StandardCursorType.DragMove);
    public Cursor CursorDragCopy { get; } = new Cursor(StandardCursorType.DragCopy);
    public Cursor CursorDragLink { get; } = new Cursor(StandardCursorType.DragLink);
    public Cursor CursorNone { get; } = new Cursor(StandardCursorType.None);

    public void Dispose()
    {
        CursorArror.Dispose();
        CursorIbeam.Dispose();
        CursorWait.Dispose();
        CursorCross.Dispose();
        CursorUpArrow.Dispose();
        CursorSizeWestEast.Dispose();
        CursorSizeNorthSouth.Dispose();
        CursorSizeAll.Dispose();
        CursorNo.Dispose();
        CursorHand.Dispose();
        CursorAppStarting.Dispose();
        CursorHelp.Dispose();
        CursorTopSide.Dispose();
        CursorBottomSide.Dispose();
        CursorLeftSide.Dispose();
        CursorRightSide.Dispose();
        CursorTopLeftCorner.Dispose();
        CursorTopRightCorner.Dispose();
        CursorBottomLeftCorner.Dispose();
        CursorBottomRightCorner.Dispose();
        CursorDragMove.Dispose();
        CursorDragCopy.Dispose();
        CursorDragLink.Dispose();
        CursorNone.Dispose();
    }
}
