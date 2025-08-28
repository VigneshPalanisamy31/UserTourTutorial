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
        int _retryCount = 0;
        const int _maxRetries = 10;

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
        private void ShowStep(int index)
        {

            if (index < 0 || index >= _steps.Count)
                return;

            _currentIndex = index;
            var step = _steps[index];

            var window = Window.GetWindow(_parent);
            if (window == null) return;

            var element =HighlightService.FindChildByNameAndTag(window, step.ElementName, step.Tag);

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

            HighlightService.PositionHighlight(step,_overlay,_parent);

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
               HighlightService.PositionHighlight(_steps[_currentIndex],_overlay, _parent);
            }
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

    }
}
