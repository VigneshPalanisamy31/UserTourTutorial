using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tour.Abstractions;

namespace Tour.Views
{
    /// <summary>
    /// Interaction logic for TourOverlay.xaml
    /// </summary>
    public partial class TourOverlay : UserControl, ITourOverlay
    {
        public TourOverlay()
        {
            InitializeComponent();
        }
        public Button PrevButton => PrevBtn;           
        public Button NextButton => NextBtn;
        public Grid HighlightBorder => HighlightElementBorder;
        public FrameworkElement DarkOverlay => DarkOverlayy;
        public Border TooltipPanel => TooltipPanelBorder;
        public TextBlock StepTitle => TourStepTitle;
        public TextBlock StepDescription => TourStepDescription;
    }
}
