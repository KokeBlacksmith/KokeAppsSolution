/// Thanks to wieslawsoltes
/// Based on his code found at:
///https://github.com/wieslawsoltes/PanAndZoom/

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using KB.AvaloniaCore.Controls.Events;
using KB.AvaloniaCore.Injection;
using KB.AvaloniaCore.Math;

namespace KB.AvaloniaCore.Controls;

public partial class ZoomDecorator : Decorator
{
    /// <summary>
    /// Calculate pan and zoom matrix based on provided stretch mode.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="mode">The stretch mode.</param>
    public static Matrix CalculateMatrix(double panelWidth, double panelHeight, double elementWidth, double elementHeight, Stretch mode)
    {
        double scaleX = panelWidth / elementWidth;
        double scaleY = panelHeight / elementHeight;
        double centerX = elementWidth * 0.5d;
        double centerY = elementHeight * 0.5d;

        switch (mode)
        {
            default:
            case Stretch.None:
                return Matrix.Identity;
            case Stretch.Fill:
                return MatrixMath.CreateScaleAt(scaleX, scaleY, centerX, centerY);
            case Stretch.Uniform:
                {
                    double zoom = System.Math.Min(scaleX, scaleY);
                    return MatrixMath.CreateScaleAt(zoom, zoom, centerX, centerY);
                }
            case Stretch.UniformToFill:
                {
                    double zoom = System.Math.Max(scaleX, scaleY);
                    return MatrixMath.CreateScaleAt(zoom, zoom, centerX, centerY);
                }
        }
    }
    

    /// <summary>
    /// Initializes a new instance of the <see cref="ZoomDecorator"/> class.
    /// </summary>
    public ZoomDecorator()
    {
        _isPanning = false;
        _matrix = Matrix.Identity;
        _captured = false;

        Focusable = true;

        AttachedToVisualTree += PanAndZoom_AttachedToVisualTree;
        DetachedFromVisualTree += PanAndZoom_DetachedFromVisualTree;
    }

   
    /// <summary>
    /// Arranges the control's child.
    /// </summary>
    /// <param name="finalSize">The size allocated to the control.</param>
    /// <returns>The space taken.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (_child == null || !_child.IsMeasureValid)
        {
            return size;
        }

        AutoFit(size.Width, size.Height, _child.Bounds.Width, _child.Bounds.Height);

        return size;
    }

    private void PanAndZoom_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        ChildChanged(Child);
    }

    private void PanAndZoom_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        DetachElement();
    }

    private void Border_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!IsZoomEnabled)
        {
            return;
        }

        Wheel(e);
    }

    private void Border_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Pressed(e);
    }

    private void Border_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Released(e);
    }

    private void Border_PointerMoved(object? sender, PointerEventArgs e)
    {
        _Moved(e);
    }

    private void BoundsChanged(Rect bounds)
    {
        // Log($"[BoundsChanged] {bounds}");
        InvalidateScrollable();
    }
    private void ChildChanged(Control? element)
    {
        if (element != null && element != _child && _child != null)
        {
            DetachElement();
        }

        if (element != null && element != _child)
        {
            AttachElement(element);
        }
    }

    private void AttachElement(Control? element)
    {
        if (element == null)
        {
            return;
        }

        _child = element;
        PointerWheelChanged += Border_PointerWheelChanged;
        PointerPressed += Border_PointerPressed;
        PointerReleased += Border_PointerReleased;
        PointerMoved += Border_PointerMoved;
    }

    private void DetachElement()
    {
        if (_child == null)
        {
            return;
        }

        PointerWheelChanged -= Border_PointerWheelChanged;
        PointerPressed -= Border_PointerPressed;
        PointerReleased -= Border_PointerReleased;
        PointerMoved -= Border_PointerMoved;
        _child.RenderTransform = null;
        _child = null;
    }

    private void Wheel(PointerWheelEventArgs e)
    {
        if (_child == null || _captured)
        {
            return;
        }
        var point = e.GetPosition(_child);
        ZoomDeltaTo(e.Delta.Y, point.X, point.Y);
    }

    private void Pressed(PointerPressedEventArgs e)
    {
        if (!IsPanEnabled)
        {
            return;
        }

        PointerPointProperties properties = e.GetCurrentPoint(this).Properties;
        if (!properties.IsMiddleButtonPressed)
        {
            return;
        }

        if (_child != null && _captured == false && _isPanning == false)
        {
            var point = e.GetPosition(_child);
            BeginPanTo(point.X, point.Y);
            _captured = true;
            _isPanning = true;
        }
    }

    private void Released(PointerReleasedEventArgs e)
    {
        if (!IsPanEnabled)
        {
            return;
        }

        if (_child == null || _captured != true || _isPanning != true)
        {
            return;
        }

        _captured = false;
        _isPanning = false;
    }

    private void _Moved(PointerEventArgs e)
    {
        if (!IsPanEnabled)
        {
            return;
        }

        if (_child == null || _captured != true || _isPanning != true)
        {
            return;
        }

        Point point = e.GetPosition(_child);
        ContinuePanTo(point.X, point.Y, true);
    }

    /// <summary>
    /// Raises <see cref="ZoomChanged"/> event.
    /// </summary>
    /// <param name="e">Zoom changed event arguments.</param>
    protected virtual void m_OnZoomChanged(ZoomChangedEventArgs e)
    {
        ZoomChanged?.Invoke(this, e);
    }

    private void _RaiseZoomChanged()
    {
        ZoomChangedEventArgs args = new ZoomChangedEventArgs(_zoomX, _zoomY, _offsetX, _offsetY);
        m_OnZoomChanged(args);
    }

    private void _ConstrainMatrix(ref Matrix matrix)
    {
        double zoomX = System.Math.Clamp(_matrix.GetScaleX(), MinZoomX, MaxZoomX);
        double zoomY = System.Math.Clamp(_matrix.GetScaleY(), MinZoomY, MaxZoomY);
        double offsetX = System.Math.Clamp(_matrix.GetTranslateX(), MinOffsetX, MaxOffsetX);
        double offsetY = System.Math.Clamp(_matrix.GetTranslateY(), MinOffsetY, MaxOffsetY);

        bool areParentBoundValid = (Bounds.Width > 0) && (Bounds.Height > 0);

        if(areParentBoundValid && ConstraintOffsetByParentBounds)
        {
            // Do not let child element go outside of parent bounds
            double childWidth = _child!.Bounds.Width * zoomX;
            double childHeight = _child!.Bounds.Height * zoomY;

            if (childWidth < Bounds.Width)
            {
                offsetX = (Bounds.Width - childWidth) * 0.5d;
            }
            else
            {
                offsetX = System.Math.Clamp(offsetX, Bounds.Width - childWidth, 0);
            }

            if (childHeight < Bounds.Height)
            {
                offsetY = (Bounds.Height - childHeight) * 0.5d;
            }
            else
            {
                offsetY = System.Math.Clamp(offsetY, Bounds.Height - childHeight, 0);
            }
        }

        bool isZoomConstrained = (zoomX == MinZoomX) || (zoomY == MinZoomY);
        if (areParentBoundValid && isZoomConstrained)
        {
            // Fit child to the view
            matrix = CalculateMatrix(this.Bounds.Width, this.Bounds.Height, _child!.Bounds.Width, _child!.Bounds.Height, this.StretchMode);
        }
        else
        {
            matrix = new Matrix(zoomX, 0.0, 0.0, zoomY, offsetX, offsetY);
        }
    }

    /// <summary>
    /// Invalidate pan and zoom control.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    private void _Invalidate(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        if (EnableConstrains)
        {
            _ConstrainMatrix(ref _matrix);
        }

        InvalidateProperties();
        InvalidateScrollable();
        InvalidateElement(skipTransitions);
        _RaiseZoomChanged();
    }

    /// <summary>
    /// Invalidate properties.
    /// </summary>
    private void InvalidateProperties()
    {
        SetAndRaise(ZoomXProperty, ref _zoomX, _matrix.GetScaleX());
        SetAndRaise(ZoomYProperty, ref _zoomY, _matrix.GetScaleY());
        SetAndRaise(OffsetXProperty, ref _offsetX, _matrix.GetTranslateX());
        SetAndRaise(OffsetYProperty, ref _offsetY, _matrix.GetTranslateY());
    }

    /// <summary>
    /// Invalidate child element.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    private void InvalidateElement(bool skipTransitions)
    {
        if (_child == null)
        {
            return;
        }

        Avalonia.Animation.Transitions? backupTransitions = null;

        if (skipTransitions)
        {
            Avalonia.Animation.Animatable? anim = _child;

            if (anim != null)
            {
                backupTransitions = anim.Transitions;
                anim.Transitions = null;
            }
        }

        _child.RenderTransformOrigin = new RelativePoint(new Point(0, 0), RelativeUnit.Relative);
        _transformBuilder = new TransformOperations.Builder(1);
        _transformBuilder.AppendMatrix(_matrix);
        _child.RenderTransform = _transformBuilder.Build();

        if (skipTransitions && backupTransitions != null)
        {
            Avalonia.Animation.Animatable? anim = _child as Avalonia.Animation.Animatable;

            if (anim != null)
            {
                anim.Transitions = backupTransitions;
            }
        }

        _child.InvalidateVisual();
    }

    /// <summary>
    /// Set pan and zoom matrix.
    /// </summary>
    /// <param name="matrix">The matrix to set as current.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void SetMatrix(Matrix matrix, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = matrix;
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Reset pan and zoom matrix.
    /// </summary>
    public void ResetMatrix()
    {
        ResetMatrix(false);
    }

    /// <summary>
    /// Reset pan and zoom matrix.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ResetMatrix(bool skipTransitions)
    {
        SetMatrix(Matrix.Identity, skipTransitions);
    }

    /// <summary>
    /// Zoom to provided zoom value and provided center point.
    /// </summary>
    /// <param name="zoom">The zoom value.</param>
    /// <param name="x">The center point x axis coordinate.</param>
    /// <param name="y">The center point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Zoom(double zoom, double x, double y, bool skipTransitions = false)
    {
        if(!_updating.CanExecute())
        {
            return;
        }

        if (!EnableZoom)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = MatrixMath.CreateScaleAt(zoom, zoom, x, y);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Zoom to provided zoom ratio and provided center point.
    /// </summary>
    /// <param name="ratio">The zoom ratio.</param>
    /// <param name="x">The center point x axis coordinate.</param>
    /// <param name="y">The center point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomTo(double ratio, double x, double y, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        if (!EnableZoom)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = MatrixMath.CreateScaleAtPrepend(_matrix, ratio, ratio, x, y);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Zoom in one step positive delta ratio and panel center point.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomIn(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        double x = _child.Bounds.Width * 0.5d;
        double y = _child.Bounds.Height * 0.5d;
        ZoomTo(ZoomSpeed, x, y, skipTransitions);
    }

    /// <summary>
    /// Zoom out one step positive delta ratio and panel center point.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomOut(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        double x = _child.GetHalfBoundsWidth();
        double y = _child.GetHalfBoundsHeight();
        ZoomTo(1 / ZoomSpeed, x, y, skipTransitions);
    }

    /// <summary>
    /// Zoom to provided zoom delta ratio and provided center point.
    /// </summary>
    /// <param name="delta">The zoom delta ratio.</param>
    /// <param name="x">The center point x axis coordinate.</param>
    /// <param name="y">The center point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ZoomDeltaTo(double delta, double x, double y, bool skipTransitions = false)
    {
        double realDelta = System.Math.Sign(delta) * System.Math.Pow(System.Math.Abs(delta), PowerFactor);
        ZoomTo(System.Math.Pow(ZoomSpeed, realDelta), x, y, skipTransitions || System.Math.Abs(realDelta) <= TransitionThreshold);
    }

    /// <summary>
    /// Pan control to provided delta.
    /// </summary>
    /// <param name="dx">The target x axis delta.</param>
    /// <param name="dy">The target y axis delta.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void PanDelta(double dx, double dy, bool skipTransitions = false)
    {
        if(!_updating.CanExecute())
        {
            return;
        }

        if (!EnablePan)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = MatrixMath.CreateScaleAndTranslate(_zoomX, _zoomY, _matrix.GetTranslateX() + dx, _matrix.GetTranslateY() + dy);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Pan control to provided target point.
    /// </summary>
    /// <param name="x">The target point x axis coordinate.</param>
    /// <param name="y">The target point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Pan(double x, double y, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        if (!EnablePan)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = MatrixMath.CreateScaleAndTranslate(_zoomX, _zoomY, x, y);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Set pan origin.
    /// </summary>
    /// <param name="x">The origin point x axis coordinate.</param>
    /// <param name="y">The origin point y axis coordinate.</param>
    public void BeginPanTo(double x, double y)
    {
        _pan = new Point();
        _previous = new Point(x, y);
    }

    /// <summary>
    /// Continue pan to provided target point.
    /// </summary>
    /// <param name="x">The target point x axis coordinate.</param>
    /// <param name="y">The target point y axis coordinate.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void ContinuePanTo(double x, double y, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        if (!EnablePan)
        {
            return;
        }

        using (_updating.Execute())
        {
            var dx = x - _previous.X;
            var dy = y - _previous.Y;
            var delta = new Point(dx, dy);
            _previous = new Point(x, y);
            _pan = new Point(_pan.X + delta.X, _pan.Y + delta.Y);
            _matrix = MatrixMath.CreateTranslatePrepend(_matrix, _pan.X, _pan.Y);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Zoom and pan.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void None(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        if (_child == null)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, Stretch.None);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Zoom and pan to fill panel.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        if (_child == null)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, Stretch.Fill);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        if (_child == null)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, Stretch.Uniform);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio. If aspect of panel is different panel is filled.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (!_updating.CanExecute())
        {
            return;
        }

        if (_child == null)
        {
            return;
        }

        using (_updating.Execute())
        {
            _matrix = CalculateMatrix(panelWidth, panelHeight, elementWidth, elementHeight, Stretch.UniformToFill);
            _Invalidate(skipTransitions);
        }
    }

    /// <summary>
    /// Zoom and pan child element inside panel using stretch mode.
    /// </summary>
    /// <param name="panelWidth">The panel width.</param>
    /// <param name="panelHeight">The panel height.</param>
    /// <param name="elementWidth">The element width.</param>
    /// <param name="elementHeight">The element height.</param>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void AutoFit(double panelWidth, double panelHeight, double elementWidth, double elementHeight, bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        switch (StretchMode)
        {
            case Stretch.Fill:
                Fill(panelWidth, panelHeight, elementWidth, elementHeight, skipTransitions);
                break;
            case Stretch.Uniform:
                Uniform(panelWidth, panelHeight, elementWidth, elementHeight, skipTransitions);
                break;
            case Stretch.UniformToFill:
                UniformToFill(panelWidth, panelHeight, elementWidth, elementHeight, skipTransitions);
                break;
            case Stretch.None:
                break;
        }
    }

    /// <summary>
    /// Set next stretch mode.
    /// </summary>
    public void ToggleStretchMode()
    {
        switch (StretchMode)
        {
            case Stretch.None:
                StretchMode = Stretch.Fill;
                break;
            case Stretch.Fill:
                StretchMode = Stretch.Uniform;
                break;
            case Stretch.Uniform:
                StretchMode = Stretch.UniformToFill;
                break;
            case Stretch.UniformToFill:
                StretchMode = Stretch.None;
                break;
        }
    }

    /// <summary>
    /// Zoom and pan.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void None(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        None(Bounds.Width, Bounds.Height, _child.Bounds.Width, _child.Bounds.Height, skipTransitions);
    }

    /// <summary>
    /// Zoom and pan to fill panel.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Fill(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        Fill(Bounds.Width, Bounds.Height, _child.Bounds.Width, _child.Bounds.Height, skipTransitions);
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void Uniform(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        Uniform(Bounds.Width, Bounds.Height, _child.Bounds.Width, _child.Bounds.Height, skipTransitions);
    }

    /// <summary>
    /// Zoom and pan to panel extents while maintaining aspect ratio. If aspect of panel is different panel is filled.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void UniformToFill(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        UniformToFill(Bounds.Width, Bounds.Height, _child.Bounds.Width, _child.Bounds.Height, skipTransitions);
    }

    /// <summary>
    /// Zoom and pan child element inside panel using stretch mode.
    /// </summary>
    /// <param name="skipTransitions">The flag indicating whether transitions on the child element should be temporarily disabled.</param>
    public void AutoFit(bool skipTransitions = false)
    {
        if (_child == null)
        {
            return;
        }

        AutoFit(Bounds.Width, Bounds.Height, _child.Bounds.Width, _child.Bounds.Height, skipTransitions);
    }

    private void InvalidateScrollable()
    {
        if (_child == null)
        {
            return;
        }

        s_CalculateScrollable(_child.Bounds, _matrix, out var extent, out var viewport, out var offset);

        _extent = extent;
        _offset = offset;
        _viewport = viewport;

        (this as ILogicalScrollable).RaiseScrollInvalidated(EventArgs.Empty);
    }
}
