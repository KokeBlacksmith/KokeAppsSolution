using Avalonia.Input;

namespace KB.AvaloniaCore.Utils;

/// <summary>
/// Holds a cursor for a view.
/// <para/>
/// View can switch between cursors and return to the previous one.
/// </summary>
public class ViewCursorHolder
{
    public ViewCursorHolder(InputElement view)
    {
        View = new WeakReference<InputElement>(view);
        CurrentCursor = view.Cursor;
        view.DetachedFromVisualTree += (_, _) => CurrentCursor?.Dispose();
    }

    public WeakReference<InputElement> View { get; }
    public Cursor? CurrentCursor { get; set; }
    public Cursor? PreviousCursor { get; private set; }


    public void ReturnToPreviousCursor()
    {
        SetCursor(PreviousCursor);
    }

    public void SetCursor(Cursor? cursor)
    {
        if(cursor == CurrentCursor)
        {
            return;
        }

        if(View.TryGetTarget(out InputElement? view))
        {
            if(cursor != PreviousCursor)
            {
                PreviousCursor?.Dispose();
            }

            PreviousCursor = view.Cursor;
            view.Cursor = cursor;
        }
    }
}
