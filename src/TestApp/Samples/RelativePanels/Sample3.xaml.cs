#if NETFX_CORE
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace TestApp.Samples.RelativePanels
{
    public sealed partial class Sample3 : UserControl
    {
        public Sample3()
        {
            this.InitializeComponent();
        }
    }
}
