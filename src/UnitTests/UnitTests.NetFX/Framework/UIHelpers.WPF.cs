using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnitTests
{
    public partial class UIHelpers
    {
        public static void ApplySizeToView(System.Windows.FrameworkElement view, double width, double height, double scaleFactor)
        {
            view.Width = width;
            view.Height = height;
        }

        public static async Task<BitmapSource> RenderAsync(FrameworkElement visual, double scaleFactor)
        {
            TaskCompletionSource<object> tcs;
            if (!visual.IsLoaded)
            {
                tcs = new TaskCompletionSource<object>();
                visual.Loaded += (s, e) => tcs.TrySetResult(null);
                await tcs.Task;
            }

            Rect bounds = VisualTreeHelper.GetContentBounds(visual);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(visual.ActualWidth ),
                                                                (int)(visual.ActualHeight),
                                                                96, // * scaleFactor,
                                                                96, // * scaleFactor,
                                                                PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(visual);
                ctx.DrawRectangle(vb, null, new Rect(0, 0, visual.ActualWidth, visual.ActualHeight));
            }
            rtb.Render(dv);
            return rtb;
        }

        private static async Task RunUITest_Impl(Func<ContentControl, DisplayInfo, Task> action, [System.Runtime.CompilerServices.CallerFilePath] string testFilePath = null, [System.Runtime.CompilerServices.CallerMemberName] string testName = null)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            var w = ApplicationInitializer.window;
            await w.Dispatcher.Invoke(async () =>
            {
                var container = w.Content as ContentControl;
                container.Content = new ContentControl();
                w.Title = testFilePath + " :: " + testName + "()";
                DisplayInfo info = new DisplayInfo()
                {
                    Width = container.ActualWidth,
                    Height = container.ActualHeight
                };
                var visual = PresentationSource.FromVisual(container);
                if (visual != null)
                    info.ScaleFactor = visual.CompositionTarget.TransformToDevice.M11;

                try
                {
                    await action(container.Content as ContentControl, info);
                }
                finally
                {
                    container.Content = null;
                }
            });
        }
    }
}