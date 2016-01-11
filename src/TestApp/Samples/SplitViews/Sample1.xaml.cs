#if NETFX_CORE
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using UniversalWPF;
#endif

namespace TestApp.Samples.SplitViews
{
    public sealed partial class Sample1 : UserControl
    {
        public Sample1()
        {
            this.InitializeComponent();
        }

		private void splitview_PaneClosing(object sender, SplitViewPaneClosingEventArgs e)
		{
			e.Cancel = cancelClose.IsChecked.Value;
        }
    }
}
