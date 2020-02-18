using System.Windows;

namespace UniversalWPF
{
    internal struct DisplayRegionHelperInfo
    {
        private const int c_maxRegions = 2;

        public DisplayRegionHelperInfo(TwoPaneViewMode mode = TwoPaneViewMode.SinglePane)
        {
            Regions = new Rect[c_maxRegions];
            Mode = mode;
        }

        public TwoPaneViewMode Mode { get; set; }
        public Rect[] Regions { get; set; }
    }
}