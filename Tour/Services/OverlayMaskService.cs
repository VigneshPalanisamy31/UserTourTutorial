using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tour.Abstractions;

namespace Tour.Services
{
    public class OverlayMaskService
    {

        /// <summary>
        /// Updates the overlay's opacity mask to darken the full window while leaving a transparent hole over the highlighted area
        /// </summary>
        /// <param name="highlightPosition">The position of the highlighted control</param>
        /// <param name="width">Width of the highlight rectangle</param>
        /// <param name="height">Height of the highlight rectangle</param>
        public static void CreateOverlayMask(Point highlightPosition, double width, double height,ITourOverlay _overlay)
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
    }
}
