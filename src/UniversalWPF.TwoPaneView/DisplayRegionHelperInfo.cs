using System.Windows;

namespace UniversalWPF
{
    internal class DisplayRegionHelperInfo
    {
        private TwoPaneViewMode wide;

        public DisplayRegionHelperInfo(TwoPaneViewMode mode)
        {
            Mode = wide;
        }

        public TwoPaneViewMode Mode { get; }
        public Rect[] Regions { get; private set; }
    }
}