using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// Based on port of https://github.com/microsoft/microsoft-ui-xaml/blob/7198c4e69019516fb08ecb3937492ffff5fe9702/dxaml/xcp/core/core/elements/RelativePanel.cpp#L1 (WinUI 3 v1.4.2)
namespace UniversalWPF
{
    /// <summary>
    /// Defines an area within which you can position and align child objects in relation
    /// to each other or the parent panel.
    /// </summary>
    /// <remarks>
    /// <para><b>Default position</b></para>
    ///    <para>By default, any unconstrained element declared as a child of the RelativePanel is given the entire
    ///    available space and positioned at the(0, 0) coordinates(upper left corner) of the panel.So, if you
    /// are positioning a second element relative to an unconstrained element, keep in mind that the second
    /// element might get pushed out of the panel.
    /// </para>
    ///<para><b>Conflicting relationships</b></para>
    ///    <para>
    ///    If you set multiple relationships that target the same edge of an element, you might have conflicting
    /// relationships in your layout as a result.When this happens, the relationships are applied in the
    ///    following order of priority:
    ///      •   Panel alignment relationships (AlignTopWithPanel, AlignLeftWithPanel, …) are applied first.
    ///      •   Sibling alignment relationships(AlignTopWith, AlignLeftWith, …) are applied second.
    ///      •   Sibling positional relationships(Above, Below, RightOf, LeftOf) are applied last.
    /// </para>
    /// <para>
    /// The panel-center alignment properties(AlignVerticalCenterWith, AlignHorizontalCenterWithPanel, ...) are
    /// typically used independently of other constraints and are applied if there is no conflict.
    ///</para>
    /// <para>
    /// The HorizontalAlignment and VerticalAlignment properties on UI elements are applied after relationship
    /// properties are evaluated and applied. These properties control the placement of the element within the
    /// available size for the element, if the desired size is smaller than the available size.
    /// </para>
    /// </remarks>
    public partial class RelativePanel : Panel
    {
        private readonly RPGraph m_graph = new RPGraph();

        private void GenerateGraph()
        {
            m_graph.Nodes.Clear();
            if(Children.Count > 0)
            {
                m_graph.AddNodes(Children);
                m_graph.ResolveConstraints(this);
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for
        /// child elements and determines a size for the System.Windows.FrameworkElement-derived
        /// class.</summary>
        /// <param name="availableSize">
        /// The available size that this element can give to child elements. Infinity can
        /// be specified as a value to indicate that the element will size to whatever content
        /// is available.
        /// </param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations
        /// of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            GenerateGraph();
            m_graph.MeasureNodes(availableSize);
            // Now that the children have been measured, we can calculate
            // the desired size of the panel, which corresponds to the 
            // desired size of the children as a whole plus the size of
            // the border.
            return m_graph.CalculateDesiredSize();
        }

        /// <summary>
        ///  When overridden in a derived class, positions child elements and determines a
        ///  size for a System.Windows.FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself
        /// and its children.
        /// </param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            m_graph.ArrangeNodes(new Rect(0,0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
    }
}
