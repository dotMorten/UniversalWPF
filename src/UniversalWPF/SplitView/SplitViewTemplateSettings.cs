using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UniversalWPF
{
	//
	// Summary:
	//     Provides calculated values that can be referenced as TemplatedParent sources
	//     when defining templates for a SplitView. Not intended for general use.
	public sealed class SplitViewTemplateSettings : DependencyObject
	{
		internal SplitViewTemplateSettings() { }
		//
		// Summary:
		//     Gets the CompactPaneLength value as a GridLength.
		//
		// Returns:
		//     The CompactPaneLength value as a GridLength.
		public GridLength CompactPaneGridLength { get; private set; }
		//
		// Summary:
		//     Gets the negative of the OpenPaneLength value.
		//
		// Returns:
		//     The negative of the OpenPaneLength value.
		public System.Double NegativeOpenPaneLength { get; private set; }
		//
		// Summary:
		//     Gets the negative of the value calculated by subtracting the CompactPaneLength
		//     value from the OpenPaneLength value.
		//
		// Returns:
		//     The negative of the OpenPaneLength value minus the CompactPaneLength value.
		public System.Double NegativeOpenPaneLengthMinusCompactLength { get; private set; }
		//
		// Summary:
		//     Gets the OpenPaneLength value as a GridLength.
		//
		// Returns:
		//     The OpenPaneLength value as a GridLength.
		public GridLength OpenPaneGridLength { get; private set; }
		//
		// Summary:
		//     Gets the OpenPaneLength value.
		//
		// Returns:
		//     The OpenPaneLength value.
		public System.Double OpenPaneLength { get; private set; }
		//
		// Summary:
		//     Gets a value calculated by subtracting the CompactPaneLength value from the OpenPaneLength
		//     value.
		//
		// Returns:
		//     The value calculated by subtracting the CompactPaneLength value from the OpenPaneLength
		//     value.
		public System.Double OpenPaneLengthMinusCompactLength { get; private set; }
	}


}
