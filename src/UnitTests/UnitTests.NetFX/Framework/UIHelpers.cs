using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NETFX_CORE
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XPoint = Windows.Foundation.Point;
using ContainerView = Windows.UI.Xaml.Controls.Frame;
#elif __ANDROID__
using UIElement = global::Android.Views.View;
using XPoint = global::Android.Graphics.PointF;
using System.Drawing;
using ContainerView = global::Android.Widget.FrameLayout;
#elif __IOS__
using System.Drawing;
using ContainerView = UIKit.UIView;
#else
using System.Windows;
using System.Windows.Media;
using XPoint = System.Drawing.Point;
using ContainerView = System.Windows.Controls.ContentControl;
#endif
using System;
using System.Threading.Tasks;

namespace UnitTests
{
    public class DisplayInfo
    {
        public double ScaleFactor { get; internal set; } = 1;
        public double ScreenPixelFactor =>
#if __ANDROID__
    ScaleFactor;
#else
    1;
#endif
        public double Width { get; internal set; }
        public double Height { get; internal set; }
    }

    public partial class UIHelpers
    {
        public static Task RunUITest(Func<ContainerView, DisplayInfo, Task> action, [System.Runtime.CompilerServices.CallerFilePath] string testFilePath = null, [System.Runtime.CompilerServices.CallerMemberName] string testName = null)
        {
            return RunUITest_Impl(action, testFilePath, testName);
        }
    }
}
