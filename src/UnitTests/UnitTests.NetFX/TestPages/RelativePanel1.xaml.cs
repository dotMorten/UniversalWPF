#if NETFX_CORE
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace UnitTests.TestPages
{
    public sealed partial class RelativePanel1 : UserControl
    {
        public RelativePanel1()
        {
            this.InitializeComponent();
        }
    }
}
