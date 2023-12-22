using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using KB.AvaloniaCore.Injection;
using KB.AvaloniaCore.Math;

namespace KB.AvaloniaCore.Controls
{
    public partial class ZoomDecorator : ILogicalScrollable
    {
        #region ILogicalScrollable_Fields

        private bool _canHorizontallyScroll;
        private bool _canVerticallyScroll;

        #endregion // ILogicalScrollable_Fields

        #region IScrollable_Fields

        private Size _extent;
        private Vector _offset;
        private Size _viewport;

        #endregion // IScrollable_Fields

        #region ILogicalScrollable_Properties
        /// <inheritdoc />
        bool ILogicalScrollable.CanHorizontallyScroll
        {
            get => _canHorizontallyScroll;
            set
            {
                _canHorizontallyScroll = value;
                InvalidateMeasure();
            }
        }

        /// <inheritdoc />
        bool ILogicalScrollable.CanVerticallyScroll
        {
            get => _canVerticallyScroll;
            set
            {
                _canVerticallyScroll = value;
                InvalidateMeasure();
            }
        }

        /// <inheritdoc />
        bool ILogicalScrollable.IsLogicalScrollEnabled => true;

        /// <inheritdoc />
        Size ILogicalScrollable.ScrollSize => new Size(1, 1);

        /// <inheritdoc />
        Size ILogicalScrollable.PageScrollSize => new Size(10, 10);

        /// <inheritdoc />
        public event EventHandler? ScrollInvalidated;



        #endregion // ILogicalScrollable_Properties

        #region IScrollable_Properties

        /// <inheritdoc/>
        Size IScrollable.Extent => _extent;

        /// <inheritdoc/>
        Vector IScrollable.Offset
        {
            get => _offset;
            set
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
                    var (x, y) = _offset;
                    var dx = x - value.X;
                    var dy = y - value.Y;

                    _offset = value;

                    _matrix = MatrixMath.CreateScaleAndTranslate(_zoomX, _zoomY, _matrix.GetTranslateX() + dx, _matrix.GetTranslateY() + dy);
                    _Invalidate(!this.IsPointerOver);
                }
            }
        }

        /// <inheritdoc/>
        Size IScrollable.Viewport => _viewport;

        #endregion // IScrollable_Properties

        #region ILogicalScrollable_Methods
        /// <inheritdoc />
        bool ILogicalScrollable.BringIntoView(Control target, Rect targetRect)
        {
            return false;
        }

        /// <inheritdoc />
        Control? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control? from)
        {
            return null;
        }

        /// <inheritdoc />
        void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
        {
            ScrollInvalidated?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// Calculate scrollable properties.
        /// </summary>
        /// <param name="source">The source bounds.</param>
        /// <param name="matrix">The transform matrix.</param>
        /// <param name="extent">The extent of the scrollable content.</param>
        /// <param name="viewport">The size of the viewport.</param>
        /// <param name="offset">The current scroll offset.</param>
        private static void s_CalculateScrollable(Rect source, Matrix matrix, out Size extent, out Size viewport, out Vector offset)
        {
            Rect bounds = new Rect(0, 0, source.Width, source.Height);

            viewport = bounds.Size;

            Rect transformed = bounds.TransformToAABB(matrix);

            double width = transformed.Size.Width;
            double height = transformed.Size.Height;

            if (width < viewport.Width)
            {
                width = viewport.Width;

                if (transformed.Position.X < 0.0)
                {
                    width += System.Math.Abs(transformed.Position.X);
                }
                else
                {
                    double widthTranslated = transformed.Size.Width + transformed.Position.X;
                    if (widthTranslated > width)
                    {
                        width += widthTranslated - width;
                    }
                }
            }
            else if (!(width > viewport.Width))
            {
                width += System.Math.Abs(transformed.Position.X);
            }

            if (height < viewport.Height)
            {
                height = viewport.Height;

                if (transformed.Position.Y < 0.0)
                {
                    height += System.Math.Abs(transformed.Position.Y);
                }
                else
                {
                    double heightTranslated = transformed.Size.Height + transformed.Position.Y;
                    if (heightTranslated > height)
                    {
                        height += heightTranslated - height;
                    }
                }
            }
            else if (!(height > viewport.Height))
            {
                height += System.Math.Abs(transformed.Position.Y);
            }

            extent = new Size(width, height);

            double ox = transformed.Position.X;
            double oy = transformed.Position.Y;

            double offsetX = ox < 0 ? System.Math.Abs(ox) : 0;
            double offsetY = oy < 0 ? System.Math.Abs(oy) : 0;

            offset = new Vector(offsetX, offsetY);
        }
    }
}
