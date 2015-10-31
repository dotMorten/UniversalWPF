using System.Windows;

namespace UniversalWPF
{
	public abstract class StateTriggerBase : DependencyObject
	{
		/// <summary>
		/// Sets the value that indicates whether the state trigger is active.
		/// </summary>
		/// <param name="isActive">true if the system should apply the trigger; otherwise, false.</param>
		protected void SetActive(bool isActive)
		{
			this.IsTriggerActive = isActive;
		    this.Owner?.SetActive(this.IsTriggerActive);
		}

		internal bool IsTriggerActive { get; private set; }

	    internal VisualStateUwp Owner { get; set; }
	}
}
