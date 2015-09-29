using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace UniversalWPF
{
	/// <summary>
	/// Represents a container with two views; one view for the main content and another
	/// view that is typically used for navigation commands.
	/// </summary>
	[ContentProperty("Content")]
	public partial class SplitView : Control
	{
		/// <summary>
		/// Initializes a new instance of the SplitView class.
		/// </summary>
		public SplitView()
		{
			DefaultStyleKey = typeof(SplitView);
			TemplateSettings = new SplitViewTemplateSettings() { OpenPaneLength = OpenPaneLength };
			UpdateTemplateSettings();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var cl1 = GetTemplateChild("ColumnDefinition1");
			var dismissLayer = GetTemplateChild("LightDismissLayer") as UIElement;
			if(dismissLayer != null)
				dismissLayer.MouseDown += DismissLayer_MouseDown;
			ChangeVisualState(false);
		}

		private void DismissLayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			IsPaneOpen = false;
		}

		private void UpdateTemplateSettings()
		{
			if(TemplateSettings == null) return;
            TemplateSettings.CompactPaneGridLength = new GridLength(CompactPaneLength, GridUnitType.Pixel);
			TemplateSettings.OpenPaneGridLength = new GridLength(OpenPaneLength, GridUnitType.Pixel);
			TemplateSettings.NegativeOpenPaneLength = -OpenPaneLength;
			TemplateSettings.OpenPaneLengthMinusCompactLength = OpenPaneLength - CompactPaneLength;
			TemplateSettings.NegativeOpenPaneLengthMinusCompactLength = -TemplateSettings.OpenPaneLengthMinusCompactLength;
			var settings = TemplateSettings;
			TemplateSettings = null;
			TemplateSettings = settings; //Trigger rebind
        }


		private void UpdateDisplayMode(SplitViewDisplayMode oldValue, SplitViewDisplayMode newValue)
		{
			ChangeVisualState(true);
		}

		private void UpdatePanePlacement()
		{
			ChangeVisualState(true);
		}

		private void OpenPane()
		{
			ChangeVisualState(true);
		}

		private void ClosePane()
		{
			SplitViewPaneClosingEventArgs args = new SplitViewPaneClosingEventArgs();
			PaneClosing?.Invoke(this, args);
			if (args.Cancel)
			{
				//IsPaneOpen = true;
				return;
			}
			ChangeVisualState(true);
			PaneClosed?.Invoke(this, EventArgs.Empty);
		}

		private void ChangeVisualState(bool useTransitions)
		{
			if (!IsPaneOpen)
			{
				if(DisplayMode == SplitViewDisplayMode.CompactInline)
				{
					GoToState(useTransitions, "ClosedCompact" + PanePlacement.ToString());
				}
				else
					GoToState(useTransitions, "Closed");
			}
			else {
				switch(DisplayMode)
				{
					case SplitViewDisplayMode.Overlay:
						GoToState(useTransitions, "OpenOverlay" + PanePlacement.ToString()); break;
					case SplitViewDisplayMode.Inline:
						GoToState(useTransitions, "OpenInline" + PanePlacement.ToString()); break;
					case SplitViewDisplayMode.CompactOverlay:
						GoToState(useTransitions, "OpenCompactOverlay" + PanePlacement.ToString()); break;
				}
			}
        }

		private bool GoToState(bool useTransitions, string stateName)
		{
			return VisualStateManager.GoToState(this, stateName, useTransitions);
		}

		/// <summary>
		/// Occurs when the SplitView pane is closed.
		/// </summary>
		public event EventHandler PaneClosed;

		/// <summary>
		/// Occurs when the SplitView pane is closing.
		/// </summary>
		public event EventHandler<SplitViewPaneClosingEventArgs> PaneClosing;
	}
}
