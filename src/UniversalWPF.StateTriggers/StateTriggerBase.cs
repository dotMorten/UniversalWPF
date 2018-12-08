using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversalWPF
{
    /// <summary>
    /// Represents the base class for state triggers.
    /// </summary>
    public abstract class StateTriggerBase : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the StateTriggerBase class.
        /// </summary>
        protected StateTriggerBase() { }

        /// <summary>
        /// Sets the value that indicates whether the state trigger is active.
        /// </summary>
        /// <param name="IsActive">true if the system should apply the trigger; otherwise, false.</param>
        protected void SetActive(bool IsActive)
        {
            //if (IsTriggerActive != IsActive)
            {
                IsTriggerActive = IsActive;
                Owner?.SetActive(IsTriggerActive);
            }
        }

        internal bool IsTriggerActive { get; private set; }

        internal VisualStateUwp Owner { get; set; }

    }
}
