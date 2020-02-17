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
            //TODO
            return new DisplayRegionHelperInfo(TwoPaneViewMode.Wide);
        }
    }
}
