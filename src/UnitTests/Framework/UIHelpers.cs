using System;
using System.Threading.Tasks;

namespace UnitTests
{
    public class DisplayInfo
    {
        public double ScaleFactor { get; internal set; } = 1;
        public double Width { get; internal set; }
        public double Height { get; internal set; }
    }

    public partial class UIHelpers
    {
        public static Task RunUITest(Func<System.Windows.Controls.ContentControl, DisplayInfo, Task> action, [System.Runtime.CompilerServices.CallerFilePath] string testFilePath = null, [System.Runtime.CompilerServices.CallerMemberName] string testName = null)
        {
            return RunUITest_Impl(action, testFilePath, testName);
        }
    }
}
