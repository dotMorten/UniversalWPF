using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalWPF
{
	// Summary:
	//     Provides event data for the SplitView.PaneClosing event.
	public sealed class SplitViewPaneClosingEventArgs : EventArgs
	{
		internal SplitViewPaneClosingEventArgs() { }

		// Summary:
		//     Gets or sets a value that indicates whether the pane closing action should be
		//     canceled.
		//
		// Returns:
		//     true to cancel the pane closing action; otherwise, false.
		public System.Boolean Cancel { get; set; }
	}
}
