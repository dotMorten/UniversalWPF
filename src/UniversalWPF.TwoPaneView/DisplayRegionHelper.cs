using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UniversalWPF
{
    internal class DisplayRegionHelper
    {
        public static Rect WindowRect => Rect.Empty; //TODO

        internal static DisplayRegionHelperInfo GetRegionInfo()
        {
            DisplayRegionHelperInfo info = new DisplayRegionHelperInfo(TwoPaneViewMode.SinglePane);
            /*
            if (view != null && view.ViewMode() == (Windows.UI.ViewManagement.ApplicationViewMode)c_ApplicationViewModeSpanning)
            {
                if (var appView = view.try_as<winrt.IApplicationViewSpanningRects>())
                {
                    List<Rect> rects = appView.GetSpanningRects();

                    if (rects.Count == 2)
                    {
                        info.Regions[0] = rects[0];
                        info.Regions[1] = rects[1];

                        // Determine orientation. If neither of these are true, default to doing nothing.
                        if (info.Regions[0].X < info.Regions[1].X && info.Regions[0].Y == info.Regions[1].Y)
                        {
                            // Double portrait
                            info.Mode = TwoPaneViewMode.Wide;
                        }
                        else if (info.Regions[0].X == info.Regions[1].X && info.Regions[0].Y < info.Regions[1].Y)
                        {
                            // Double landscape
                            info.Mode = TwoPaneViewMode.Tall;
                        }
                    }
                }
            }
            */
            return info;
        }
    }
}
