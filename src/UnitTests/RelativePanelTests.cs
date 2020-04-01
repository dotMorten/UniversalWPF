using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalWPF;

namespace UnitTests
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
                var blobs = await ImageAnalysis.FindConnectedPixelsAsync(panel, info.ScaleFactor, (c) => c.ToArgb() == System.Drawing.Color.Red.ToArgb());
                Assert.AreEqual(1, blobs.Count);
                Assert.AreEqual(100, blobs[0].Width, 1);
                Assert.AreEqual(75, blobs[0].Height, 1);
            });
        }

        [TestMethod]
        public async Task EmptyPanel_NoSize()
        {
            await UIHelpers.RunUITest(async (container, info) =>
            {
                var panel = new RelativePanel() { Background = new SolidColorBrush(Colors.Red), Margin = new Thickness(20) };
                container.Content = panel;
                var blobs = await ImageAnalysis.FindConnectedPixelsAsync(panel, info.ScaleFactor, (c) => c.ToArgb() == System.Drawing.Color.Red.ToArgb());
                Assert.AreEqual(1, blobs.Count);
                Assert.AreEqual(info.Width - 40, blobs[0].Width, 1);
                Assert.AreEqual(info.Height - 40, blobs[0].Height, 1);
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
                var redblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Red);
                var blueblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Blue);
                Assert.AreEqual(1, redblobs.Count);
                Assert.AreEqual(150, redblobs[0].Width, 1);
                Assert.AreEqual(100, redblobs[0].Height, 1);
                Assert.AreEqual(1, blueblobs.Count);
                Assert.AreEqual(150, blueblobs[0].Width, 1);
                Assert.AreEqual(100, blueblobs[0].Height, 1);
                Assert.AreEqual(redblobs[0].MaxColumn + 1, blueblobs[0].MinColumn, 1);
                Assert.AreEqual(0, blueblobs[0].MinRow, 1);
                Assert.AreEqual(100, redblobs[0].MinColumn, 1);
                Assert.AreEqual(0, redblobs[0].MinRow, 1);
                Assert.AreEqual(399, blueblobs[0].MaxColumn, 1);
            });
        }

        [TestMethod]
        public async Task RelativePanelXamlTest()
        {
            await UIHelpers.RunUITest(async (container, info) =>
            {
                var panel = new TestPages.RelativePanel1() { Width = 600, Height = 400 };
                container.Content = panel;
                var bitmap = await UIHelpers.RenderAsync(panel, info.ScaleFactor);
                var redblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Red);
                var blueblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Blue);
                var greenblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Green);
                var cyanblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Cyan);
                var orangeblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Orange);
                var purpleblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Purple);
                var yellowblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.Yellow);
                var textblobs = ImageAnalysis.FindConnectedPixels(bitmap, info.ScaleFactor, System.Drawing.Color.CornflowerBlue);
                Assert.AreEqual(1, redblobs.Count, "Red blob count");
                Assert.AreEqual(1, blueblobs.Count, "Blue blob count");
                Assert.AreEqual(1, greenblobs.Count, "Green blob count");
                Assert.AreEqual(1, cyanblobs.Count, "Cyan blob count");
                Assert.AreEqual(1, orangeblobs.Count, "Orange blob count");
                Assert.AreEqual(1, purpleblobs.Count, "Purple blob count");
                Assert.AreEqual(1, yellowblobs.Count, "Yellow blob count");
                Assert.IsTrue(textblobs.Count > 1, "Text blob count");
                var textblob = textblobs.Union(); //Generate bounding box of all text blobs
                Assert.IsTrue(textblob.MaxColumn < yellowblobs[0].MinColumn, "Text left of yellow");
                Assert.IsTrue(textblob.MinColumn < yellowblobs[0].MinRow, "Text below yellow top");

                Assert.AreEqual(0, redblobs[0].MinRow, 1, "Red top side");
                Assert.AreEqual(0, redblobs[0].MinColumn, 1, "Red left side");
                Assert.AreEqual(100, redblobs[0].Width, 1, "Red width");
                Assert.AreEqual(100, redblobs[0].Height, 1, "Red height");

                Assert.AreEqual(0, blueblobs[0].MinRow, 1, "Blue top side");
                Assert.AreEqual(100, blueblobs[0].MinColumn, 1, "Blue left side");
                Assert.AreEqual(100, blueblobs[0].Width, 1, "Blue width");
                Assert.AreEqual(100, blueblobs[0].Height, 1, "Blue height");

                Assert.AreEqual(105, greenblobs[0].MinRow, 1, "Green top side");
                Assert.AreEqual(0, greenblobs[0].MinColumn, 1, "Green left side");
                Assert.AreEqual(200, greenblobs[0].Width, 1, "Green width");
                Assert.AreEqual(100, greenblobs[0].Height, 1, "Green height");

                Assert.AreEqual(155, purpleblobs[0].MinRow, 1, "Purple top side");
                Assert.AreEqual(200, purpleblobs[0].MinColumn, 1, "Purple left side");
                Assert.AreEqual(400, purpleblobs[0].Width, 1, "Purple width");
                Assert.AreEqual(50, purpleblobs[0].Height, 1, "Purple height"); 
                
                Assert.AreEqual(100, orangeblobs[0].MinRow, 1, "Orange top side");
                Assert.AreEqual(600, orangeblobs[0].MaxColumn, 1, "Orange right side");
                Assert.AreEqual(50, orangeblobs[0].Width, 1, "Orange width");
                Assert.AreEqual(50, orangeblobs[0].Height, 1, "Orange height");

                Assert.AreEqual(50, cyanblobs[0].CenterRow, 1, "Cyan center row");
                Assert.AreEqual(400, cyanblobs[0].CenterColumn, 1, "Cyan center column");
                Assert.AreEqual(50, cyanblobs[0].Width, 1, "Cyan width");
                Assert.AreEqual(50, cyanblobs[0].Height, 1, "Cyan height");

                Assert.AreEqual(205, yellowblobs[0].MinRow, 1, "Yellow top side");
                Assert.AreEqual(100, yellowblobs[0].MinColumn, 1, "Yellow left side");
                Assert.AreEqual(500, yellowblobs[0].Width, 1, "Yellow width");
                Assert.AreEqual(195, yellowblobs[0].Height, 1, "Yellow height");
            });
        }
    }
}
