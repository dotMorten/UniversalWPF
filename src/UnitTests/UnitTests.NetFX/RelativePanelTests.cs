using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalWPF;

namespace UnitTests.NetFX
{
    [TestClass]
    public class RelativePanelTests
    {
        [TestMethod]
        public async Task EmptyPanel_FixedSize()
        {
            await UIHelpers.RunUITest(async (container, info) =>
            {
                var panel = new RelativePanel() { Width = 100, Height = 75, Background = new SolidColorBrush(Colors.Red) };
                container.Content = panel;
                var blobs = await ImageAnalysis.FindConnectedPixelsAsync(container, info.ScaleFactor, (c) => c.ToArgb() == System.Drawing.Color.Red.ToArgb());
                Assert.AreEqual(1, blobs.Count);
                Assert.AreEqual(100, blobs[0].Width);
                Assert.AreEqual(75, blobs[0].Height);
            });
        }
        [TestMethod]
        public async Task EmptyPanel_NoSize()
        {
            await UIHelpers.RunUITest(async (container, info) =>
            {
                var panel = new RelativePanel() { Background = new SolidColorBrush(Colors.Red) };
                container.Content = panel;
                var blobs = await ImageAnalysis.FindConnectedPixelsAsync(container, info.ScaleFactor, (c) => c.ToArgb() == System.Drawing.Color.Red.ToArgb());
                Assert.AreEqual(1, blobs.Count);
                Assert.AreEqual(info.Width, blobs[0].Width, 1);
                Assert.AreEqual(info.Height, blobs[0].Height, 1);
            });
        }

        [TestMethod]
        [WorkItem(13)]
        public async Task LeftOfRightAligned()
        {
            /*
            <RelativePanel>
                <Border x:Name="BlueRect" Background="Blue" Height="100" Width="150" RelativePanel.AlignRightWithPanel="True"/>
                <Border x:Name="RedRect" Background="Red" Height="100" Width="150" RelativePanel.LeftOf="BlueRect" Tag="Red"/>
            </RelativePanel>
            */
            await UIHelpers.RunUITest(async (container, info) =>
            {
                container.Width = 400;
                container.Height = 300;
                var panel = new RelativePanel();
                panel.Background = new SolidColorBrush(Colors.Green);
                var blueRect = new Border() { Width = 150, Height = 100, Background = new SolidColorBrush(Colors.Blue) };
                RelativePanel.SetAlignRightWithPanel(blueRect, true);
                panel.Children.Add(blueRect);

                var redRect = new Border() { Width = 150, Height = 100, Background = new SolidColorBrush(Colors.Red) };
                RelativePanel.SetLeftOf(redRect, blueRect);
                panel.Children.Add(redRect);

                container.Content = panel;
                var bitmap = await UIHelpers.RenderAsync(container, info.ScaleFactor);
                var redblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, (c) => c.ToArgb() == System.Drawing.Color.Red.ToArgb());
                var blueblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, (c) => c.ToArgb() == System.Drawing.Color.Blue.ToArgb());
                Assert.AreEqual(1, redblobs.Count);
                Assert.AreEqual(150, redblobs[0].Width);
                Assert.AreEqual(100, redblobs[0].Height);
                Assert.AreEqual(1, blueblobs.Count);
                Assert.AreEqual(150, blueblobs[0].Width);
                Assert.AreEqual(100, blueblobs[0].Height);
                Assert.AreEqual(redblobs[0].MaxColumn + 1, blueblobs[0].MinColumn);
                Assert.AreEqual(0, blueblobs[0].MinRow);
                Assert.AreEqual(100, redblobs[0].MinColumn);
                Assert.AreEqual(0, redblobs[0].MinRow);
                Assert.AreEqual(399, blueblobs[0].MaxColumn);
            });
        }
    }
}
