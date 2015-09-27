#if NETFX_CORE
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace TestApp.Shared
{
    public sealed partial class Sample1 : UserControl
    {
        public Sample1()
        {
            this.InitializeComponent();
        }
    }
}
