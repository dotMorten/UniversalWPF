using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalWPF
{
	/// <summary>
	/// Defines constants that specify how the pane is shown in a SplitView.
	/// </summary>
	public enum SplitViewDisplayMode
	{
		/// <summary>
		/// The pane covers the content when it's open and does not take up space in the
		/// control layout. The pane closes when the user taps outside of it.
		/// </summary>
		Overlay = 0,
		/// <summary>
		/// The pane is shown side-by-side with the content and takes up space in the control
		/// layout. The pane does not close when the user taps outside of it.
		/// </summary>
		Inline = 1,
		/// <summary>
		/// The amount of the pane defined by the CompactPaneLength property is shown side-by-side
		/// with the content and takes up space in the control layout. The remaining part
		/// of the pane covers the content when it's open and does not take up space in the
		/// control layout. The pane closes when the user taps outside of it.
		/// </summary>
		CompactOverlay = 2,
		/// <summary>
		/// The amount of the pane defined by the CompactPaneLength property is shown side-by-side
		/// with the content and takes up space in the control layout. The remaining part
		/// of the pane pushes the content to the side when it's open and takes up space
		/// in the control layout. The pane does not close when the user taps outside of
		/// it.
		/// </summary>
		CompactInline = 3
	}
}
