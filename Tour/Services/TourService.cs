using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Tour.Abstractions;
using Tour.Models;
using Tour.Views;

namespace Tour.Services
{
    /// <summary>
    /// Service responsible for running guided tours by highlighting UI elements and displaying description tooltips
    /// </summary>
    public class TourService
    {
        private readonly FrameworkElement _parent;
        private readonly List<TourStep> _steps;
        private int _currentIndex = -1;
        private readonly ITourOverlay _overlay;

        public TourService(FrameworkElement parent, List<TourStep> steps, ITourOverlay? overlay = null)
        {
            _parent = parent;
            _steps = steps;
            _overlay = overlay ?? new TourOverlay();
            _overlay.PrevButton.Click += (s, e) => ShowStep(_currentIndex - 1);
            _overlay.NextButton.Click += (s, e) =>
            {
                if (_currentIndex < _steps.Count - 1)
                    ShowStep(_currentIndex + 1);
                else
                    EndTour();
            };

            // Listen to size & layout changes to re-position highlight
            _parent.SizeChanged += (s, e) => RefreshCurrentStep();
            _parent.LayoutUpdated += (s, e) => RefreshCurrentStep();
        }

        /// <summary>
        /// Starts the tour by attaching the overlay to the window and displaying the first step
        /// </summary>
        public void StartTour()
        {
            if (_steps == null || _steps.Count == 0) return;
            var window = Window.GetWindow(_parent);
            if (window == null) return;

            if (window.Content is not Grid grid)
            {
                var newGrid = new Grid();
                if (window.Content is UIElement oldContent)
                {
                    window.Content = null;
                    newGrid.Children.Add(oldContent);
                }
                window.Content = newGrid;
                grid = newGrid;
            }

            if (!grid.Children.Contains(_overlay as UIElement))
            {
                grid.Children.Add(_overlay as UIElement);
                Panel.SetZIndex(_overlay as UIElement, int.MaxValue);
            }

            _currentIndex = -1;
            _parent.Dispatcher.BeginInvoke(new Action(() =>
            {
                ShowStep(0);
            }), DispatcherPriority.Loaded);
        }

        /// <summary>
        /// Displays a particular tour step by index
        /// </summary>
        /// <param name="index">Index of the step to display</param>
         int _retryCount = 0;
         const int _maxRetries = 10;
        private void ShowStep(int index)
        {

            if (index < 0 || index >= _steps.Count)
                return;

            _currentIndex = index;
            var step = _steps[index];

            var window = Window.GetWindow(_parent);
            if (window == null) return;

            var element = FindChildByNameAndTag(window, step.ElementName, step.Tag);

            if (element == null)
            {
                if (_retryCount >= _maxRetries)
                {
                    ShowStep(index + 1);
                    return;
                }
                _retryCount++;
                _parent.Dispatcher.BeginInvoke(new Action(() => ShowStep(index)), DispatcherPriority.Loaded);
                return;
            }
            _retryCount = 0;
            if (!element.IsLoaded)
            {
                element.Loaded += (s, e) => ShowStep(index);
                return;
            }

            PositionHighlight(step);

            _overlay.PrevButton.IsEnabled = _currentIndex > 0;
            _overlay.NextButton.Content = _currentIndex < _steps.Count - 1 ? "Next" : "Finish";
            var fadeIn = ((FrameworkElement)_overlay).Resources["FadeInStoryboard"] as Storyboard;
            fadeIn.Begin(_overlay as FrameworkElement);
        }

        /// <summary>
        /// Refresh a tour step that is currently visible to reposition it in case the layout has changed
        /// </summary>
        private void RefreshCurrentStep()
        {
            if (_currentIndex >= 0 && _currentIndex < _steps.Count)
            {
                PositionHighlight(_steps[_currentIndex]);
            }
        }

        /// <summary>
        /// Highlights the control,positions an overlay glow around it and displays the tooltip.
        /// </summary>
        /// <param name="step">The tour step definition</param>
        private void PositionHighlight(TourStep step)
        {
            var window = Window.GetWindow(_parent);
            if (window == null) return;
            var element = FindChildByNameAndTag(window, step.ElementName, step.Tag);
            if (element == null) return;
            var pos = element.TransformToAncestor(window).Transform(new Point(0, 0));
            double width = element.ActualWidth;
            double height = element.ActualHeight;

            // Position highlight glow
            _overlay.HighlightBorder.Width = width + 7;
            _overlay.HighlightBorder.Height = height + 7;
            Canvas.SetLeft(_overlay.HighlightBorder, pos.X - 3);
            Canvas.SetTop(_overlay.HighlightBorder, pos.Y - 3);

            // Create mask
            CreateOverlayMask(pos, width, height);

            _overlay.StepTitle.Text = step.Title;
            _overlay.StepDescription.Text = step.Description;
            double tooltipX = pos.X + width + 10;
            double tooltipY = pos.Y;
            double tooltipWidth = _overlay.TooltipPanel.ActualWidth > 0
                ? _overlay.TooltipPanel.ActualWidth
                : _overlay.TooltipPanel.Width;
            double tooltipHeight = _overlay.TooltipPanel.ActualHeight > 0
                ? _overlay.TooltipPanel.ActualHeight
                : _overlay.TooltipPanel.Height;
            double windowWidth = window.ActualWidth;
            double windowHeight = window.ActualHeight;

            if (tooltipX + tooltipWidth > windowWidth)
                tooltipX = pos.X - tooltipWidth - 10;

            if (tooltipY + tooltipHeight > windowHeight)
                tooltipY = windowHeight - tooltipHeight - 10;

            if (tooltipY < 0)
                tooltipY = 10;
            Canvas.SetLeft(_overlay.TooltipPanel, tooltipX);
            Canvas.SetTop(_overlay.TooltipPanel, tooltipY);
        }

        /// <summary>
        /// Updates the overlay's opacity mask to darken the full window while leaving a transparent hole over the highlighted area
        /// </summary>
        /// <param name="highlightPosition">The position of the highlighted control</param>
        /// <param name="width">Width of the highlight rectangle</param>
        /// <param name="height">Height of the highlight rectangle</param>
        private void CreateOverlayMask(Point highlightPosition, double width, double height)
        {
            var overlay = _overlay.DarkOverlay;
            if (overlay.ActualWidth == 0 || overlay.ActualHeight == 0)
                return;

            RectangleGeometry fullRect = new RectangleGeometry(new Rect(0, 0, overlay.ActualWidth, overlay.ActualHeight));
            RectangleGeometry holeRect = new RectangleGeometry(new Rect(highlightPosition.X, highlightPosition.Y, width, height));
            CombinedGeometry combined = new CombinedGeometry(GeometryCombineMode.Exclude, fullRect, holeRect);
            GeometryDrawing drawing = new GeometryDrawing
            {
                Geometry = combined,
                Brush = Brushes.White
            };
            var brush = new DrawingBrush
            {
                Drawing = drawing,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(0, 0, overlay.ActualWidth, overlay.ActualHeight)
            };
            (_overlay.DarkOverlay.OpacityMask as DrawingBrush).Drawing = drawing;
        }

        /// <summary>
        /// Completes the tour and removes the overlay from the visual tree
        /// </summary>
        private void EndTour()
        {
            var fadeOut = ((FrameworkElement)_overlay).Resources["FadeOutStoryboard"] as Storyboard; ;
            fadeOut.Completed += (s, e) =>
            {
                var window = Window.GetWindow(_parent);
                if (window != null && window.Content is Grid grid)
                {
                    grid.Children.Remove(_overlay as UIElement);
                }
            };
            fadeOut.Begin(_overlay as FrameworkElement);
        }

        /// <summary>
        /// Recursively searches the visual tree to find a FrameworkElement matching a name and an optional tag value
        /// </summary>
        /// <param name="parent">Parent element to search from</param>
        /// <param name="name">Name of the target child</param>
        /// <param name="tag">Optional tag to match</param>
        /// <returns>The matching framework element if found, else null</returns>
        private FrameworkElement FindChildByNameAndTag(DependencyObject parent, string name, object tag = null)
        {
            if (parent == null) return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement fe)
                {
                    bool nameMatches = fe.Name == name;
                    bool tagMatches = (tag == null || fe.Tag?.ToString() == tag.ToString());

                    if (nameMatches && tagMatches)
                        return fe;
                }
                var result = FindChildByNameAndTag(child, name, tag);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
