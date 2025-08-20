using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tour.Models;
using Tour.Services;

namespace Tour.ViewModels
{
    public class TourViewModel
    {
        public ObservableCollection<TourStep> Steps { get; set; }
        public int CurrentStepIndex { get; set; } = 0;

        public TourStep CurrentStep => Steps?[CurrentStepIndex];

        /// <summary>
        /// Moves to next step if available
        /// </summary>
        public void NextStep()
        {
            if (Steps != null && CurrentStepIndex < Steps.Count - 1) CurrentStepIndex++;
        }

        /// <summary>
        /// Moves to previous step if available
        /// </summary>
        public void PreviousStep()
        {
            if (CurrentStepIndex > 0) CurrentStepIndex--;
        }
    }
}
