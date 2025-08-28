using System.Collections.ObjectModel;
using Tour.Models;

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
