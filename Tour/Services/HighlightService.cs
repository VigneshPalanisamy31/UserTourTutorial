using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tour.Abstractions;
using Tour.Models;

namespace Tour.Services
{
    public static class HighlightService
    {
        /// <summary>
        /// Highlights the control,positions an overlay glow around it and displays the tooltip.
        /// </summary>
        /// <param name="step">The tour step definition</param>
        public static void PositionHighlight(TourStep step, ITourOverlay _overlay, FrameworkElement _parent)
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
           OverlayMaskService.CreateOverlayMask(pos, width, height,_overlay);

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
        /// Recursively searches the visual tree to find a FrameworkElement matching a name and an optional tag value
        /// </summary>
        /// <param name="parent">Parent element to search from</param>
        /// <param name="name">Name of the target child</param>
        /// <param name="tag">Optional tag to match</param>
        /// <returns>The matching framework element if found, else null</returns>
        public static FrameworkElement FindChildByNameAndTag(DependencyObject parent, string name, object tag = null)
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
