using System.Windows;
using System.Windows.Controls;

namespace Tour.Abstractions
{
        /// <summary>
        /// Interface the overlay must implement so the TourService can control it.
        /// </summary>
        public interface ITourOverlay
        {
            Button PrevButton { get; }
            Button NextButton { get; }
            Grid HighlightBorder { get; }
            FrameworkElement DarkOverlay { get; }
            Border TooltipPanel { get; }
            TextBlock StepTitle { get; }
            TextBlock StepDescription { get; }
        }
}
