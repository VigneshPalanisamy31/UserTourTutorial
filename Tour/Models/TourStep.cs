using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tour.Models
{
    public class TourStep
    {
        public string Tag { get; set; } //Tag of the control to highlight
        public string ElementName { get; set; }  // Name of the control to highlight
        public string Title { get; set; }//Title of the tourstep
        public string Description { get; set; }  // Tooltip text

    }
}
