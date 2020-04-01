using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UnitTests
{
    // Method for finding pixels connected to each other. Great for finding UI Elements on the screen
    // based on pixel color filter.
    // A good explanation of the Connected Component Analysis can be seen here: https://www.youtube.com/watch?v=ticZclUYy88
    // Uses a 4-connectivity 2-pass Hoshen-Kopelman algorithm
    public static class ImageAnalysis
    {
        /// <summary>
        /// Finds a set of pixels that are connected to each other, Looks at any pixels that are not black and/or transparent
        /// </summary>
        /// <param name="image"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static IList<Blob> FindConnectedPixels(BitmapSource image, double scaleFactor)
        {
            return FindConnectedPixels(image, scaleFactor, (c) => (c.R > 0 || c.G > 0 || c.B > 0) && c.A > 0);
        }

        public static async Task<IList<Blob>> FindConnectedPixelsAsync(FrameworkElement element, double scaleFactor, Func<Color, bool> includePixelFunction)
        {
            var bitmap = await UIHelpers.RenderAsync(element, scaleFactor);
            return FindConnectedPixels(bitmap, scaleFactor, includePixelFunction);
        }

        public static IList<Blob> FindConnectedPixels(BitmapSource image, double scaleFactor, Color color)
        {
            return FindConnectedPixels(image, scaleFactor, c => c.ToArgb() == color.ToArgb());
        }

        public static IList<Blob> FindConnectedPixels(BitmapSource image, double scaleFactor, Func<Color, bool> includePixelFunction)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;
            bool[] pixels = new bool[width * height];

            int bitsPerPixel = 32;
            int stride = image.PixelWidth * bitsPerPixel / 8;
            byte[] pixelbuffer = new byte[image.PixelHeight * stride];
            image.CopyPixels(pixelbuffer, stride, 0);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    byte b = pixelbuffer[row * stride + col * 4];
                    byte g = pixelbuffer[row * stride + col * 4 + 1];
                    byte r = pixelbuffer[row * stride + col * 4 + 2];
                    byte a = pixelbuffer[row * stride + col * 4 + 3];

                    pixels[col + row * width] = includePixelFunction(Color.FromArgb(a, r, g, b));
                }
            }

            Func<Color, bool> ismatch = includePixelFunction;
            int[] labels = new int[pixels.Length];
            int currentLabel = 0;
            // First pass - Label pixels
            UnionFind<int> sets = new UnionFind<int>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var idx = j + i * width;
                    bool v = pixels[idx];
                    if (v)
                    {
                        var l1 = i == 0 ? 0 : labels[j + (i - 1) * width];
                        var l2 = j == 0 ? 0 : labels[j + i * width - 1];
                        if (l1 == 0 && l2 == 0)
                        {
                            //Assign new label
                            currentLabel++;
                            labels[idx] = currentLabel;
                            sets.MakeSet(currentLabel);
                        }
                        else if (l1 > 0 && l2 == 0)
                            labels[idx] = l1; //Copy label from neighbor
                        else if (l1 == 0 && l2 > 0)
                            labels[idx] = l2; //Copy label from neighbor
                        else
                        {
                            labels[idx] = l1 < l2 ? l1 : l2; // Both neighbors have values. Grab the smallest label
                            if (l1 != l2)
                                sets.Union(sets.Find(l1), sets.Find(l2)); //store L1 is equivalent to L2
                        }
                    }
                }
            }
            // Second pass: Update equivalent labels
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var idx = j + i * width;
                    var lbl = labels[idx];
                    if (lbl > 0)
                    {
                        var l = sets.Find(lbl);
                        labels[idx] = l.Value;
                    }
                }
            }
            // Generate blobs
            var blobs = new List<Blob>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var idx = j + i * width;
                    var lbl = labels[idx];
                    if (lbl > 0)
                    {
                        var blob = blobs.Where(b => b.Id == lbl).FirstOrDefault();
                        if (blob != null)
                        {
                            blob.MinColumn = Math.Min(blob.MinColumn, j);
                            blob.MaxColumn = Math.Max(blob.MaxColumn, j);
                            blob.MinRow = Math.Min(blob.MinRow, i);
                            blob.MaxRow = Math.Max(blob.MaxRow, i);
                        }
                        else
                        {
                            blob = new Blob() { Id = lbl, MinColumn = j, MaxColumn = j, MinRow = i, MaxRow = i };
                            blobs.Add(blob);
                        }
                        blob.Pixels.Add(new System.Drawing.Point(j, i));
                    }
                }
            }
            foreach(var b in blobs)
            {
                b.MinColumn /= scaleFactor;
                b.MaxColumn /= scaleFactor;
                b.MinRow /= scaleFactor;
                b.MaxRow /= scaleFactor;
            }
            return blobs;
        }

        private class UnionFind<T>
        {
            // A generic Union Find Data Structure 
            // Based on https://github.com/thomas-villagers/unionfind.cs
            private Dictionary<T, SetElement> dict;

            public class SetElement
            {
                public SetElement Parent { get; internal set; }
                public T Value { get; }
                public int Size { get; internal set; }
                public SetElement(T value)
                {
                    Value = value;
                    Parent = this;
                    Size = 1;
                }
                public override string ToString() => string.Format("{0}, size:{1}", Value, Size);
            }

            public UnionFind()
            {
                dict = new Dictionary<T, SetElement>();
            }

            public SetElement MakeSet(T value)
            {
                SetElement element = new SetElement(value);
                dict[value] = element;
                return element;
            }

            public SetElement Find(T value)
            {
                SetElement element = dict[value];
                SetElement root = element;
                while (root.Parent != root)
                {
                    root = root.Parent;
                }
                SetElement z = element;
                while (z.Parent != z)
                {
                    SetElement temp = z;
                    z = z.Parent;
                    temp.Parent = root;
                }
                return root;
            }

            public SetElement Union(SetElement root1, SetElement root2)
            {
                if (root2.Size > root1.Size)
                {
                    root2.Size += root1.Size;
                    root1.Parent = root2;
                    return root2;
                }
                else
                {
                    root1.Size += root2.Size;
                    root2.Parent = root1;
                    return root1;
                }
            }
        }

        public class Blob
        {
            public int Id;
            public double MinColumn;
            public double MaxColumn;
            public double MinRow;
            public double MaxRow;
            public double Width => MaxColumn - MinColumn + 1;
            public double Height => MaxRow - MinRow + 1;
            public double CenterRow => MinRow + Height / 2;
            public double CenterColumn => MinColumn + Width / 2;
            public List<System.Drawing.Point> Pixels { get; } = new List<System.Drawing.Point>();
        }

        public static Blob Union(this IEnumerable<Blob> blobs)
        {
            var b = blobs.FirstOrDefault();
            if(b != null)
                foreach(var bl in blobs.Skip(1))
                {
                    b.MinColumn = Math.Min(bl.MinColumn, b.MinColumn);
                    b.MinRow = Math.Min(bl.MinRow, b.MinRow);
                    b.MaxColumn = Math.Max(bl.MaxColumn, b.MaxColumn);
                    b.MaxRow = Math.Max(bl.MaxRow, b.MaxRow);
                }
            return b;
        }
    }
}
