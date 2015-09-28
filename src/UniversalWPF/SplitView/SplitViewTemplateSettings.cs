using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversalWPF
{
	/// <summary>
	/// Provides calculated values that can be referenced as TemplatedParent sources
	/// when defining templates for a SplitView. Not intended for general use.
	/// </summary>
	public sealed class SplitViewTemplateSettings : DependencyObject
	{
		internal SplitViewTemplateSettings() { }
		/// <summary>
		/// Gets the CompactPaneLength value as a GridLength.
		/// </summary>
		/// <value>
		/// The CompactPaneLength value as a GridLength.
		/// </value>
		public GridLength CompactPaneGridLength { get; private set; }
		/// <summary>
		/// Gets the negative of the OpenPaneLength value.
		/// </summary>
		/// <value>
		/// The negative of the OpenPaneLength value.
		/// </value>
		public double NegativeOpenPaneLength { get; private set; }
		/// <summary>
		/// Gets the negative of the value calculated by subtracting the CompactPaneLength
		/// value from the OpenPaneLength value.
		/// </summary>
		/// <value>
		/// The negative of the OpenPaneLength value minus the CompactPaneLength value.
		/// </value>
		public double NegativeOpenPaneLengthMinusCompactLength { get; private set; }
		/// <summary>
		/// Gets the OpenPaneLength value as a GridLength.
		/// </summary>
		/// <value>
		/// The OpenPaneLength value as a GridLength.
		/// </value>
		public GridLength OpenPaneGridLength { get; private set; }
		/// <summary>
		/// Gets the OpenPaneLength value.
		/// </summary>
		/// <value>
		/// The OpenPaneLength value.
		/// </value>
		public double OpenPaneLength { get; private set; }
		/// <summary>
		/// Gets a value calculated by subtracting the CompactPaneLength value from the OpenPaneLength
		/// value.
		/// </summary>
		/// <value>
		/// The value calculated by subtracting the CompactPaneLength value from the OpenPaneLength
		/// value.
		/// </value>
		public double OpenPaneLengthMinusCompactLength { get; private set; }
	}


}
