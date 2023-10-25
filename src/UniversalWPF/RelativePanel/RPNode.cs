#nullable enable

using System;
using System.Diagnostics;
using System.Windows;

namespace UniversalWPF;

// Based on port of https://github.com/microsoft/microsoft-ui-xaml/blob/7198c4e69019516fb08ecb3937492ffff5fe9702/dxaml/xcp/components/relativepanel/lib/RPNode.cpp (WinUI 3 v1.4.2)
internal sealed class RPNode
{
    private readonly DependencyObject m_element;
    // Represents the space that's available to an element given its set of
    // constraints. The width and height of this rect is used to measure
    // a given element.
    public Rect m_measureRect;

    // Represents the exact space within the MeasureRect that will be used 
    // to arrange a given element.
    public Rect m_arrangeRect;

    RPState m_state = RPState.Unresolved;

    // Indicates if this is the last element in a dependency chain that is
    // formed by only connecting nodes horizontally.
    public bool m_isHorizontalLeaf = true;

    // Indicates if this is the last element in a dependency chain that is
    // formed by only connecting nodes vertically.
    public bool m_isVerticalLeaf = true;

    public RPConstraints m_constraints = RPConstraints.None;
    public RPNode? m_leftOfNode;
    public RPNode? m_aboveNode;
    public RPNode? m_rightOfNode;
    public RPNode? m_belowNode;
    public RPNode? m_alignHorizontalCenterWithNode;
    public RPNode? m_alignVerticalCenterWithNode;
    public RPNode? m_alignLeftWithNode;
    public RPNode? m_alignTopWithNode;
    public RPNode? m_alignRightWithNode;
    public RPNode? m_alignBottomWithNode;

    public RPNode(DependencyObject element)
    {
        m_element = element;
    }

    public DependencyObject Element => m_element;
    public double DesiredWidth => m_element is UIElement element ? element.DesiredSize.Width : 0;

    public double DesiredHeight => m_element is UIElement element ? element.DesiredSize.Height : 0;

    public void Measure(Size constrainedAvailableSize)
    {
        if (m_element is UIElement element)
        {
            element.Measure(constrainedAvailableSize);
            //element.EnsureLayoutStorage();
        }
    }
    public void Arrange(Rect finalRect)
    {
        if (m_element is UIElement element)
        {
            element.Arrange(finalRect);
        }
    }
    public object? GetLeftOfValue() => RelativePanel.GetLeftOf(m_element);
    public object? GetAboveValue() => RelativePanel.GetAbove(m_element);
    public object? GetRightOfValue() => RelativePanel.GetRightOf(m_element);
    public object? GetBelowValue() => RelativePanel.GetBelow(m_element);
    public object? GetAlignHorizontalCenterWithValue() => RelativePanel.GetAlignHorizontalCenterWith(m_element);
    public object? GetAlignVerticalCenterWithValue() => RelativePanel.GetAlignVerticalCenterWith(m_element);
    public object? GetAlignLeftWithValue() => RelativePanel.GetAlignLeftWith(m_element);
    public object? GetAlignTopWithValue() => RelativePanel.GetAlignTopWith(m_element);
    public object? GetAlignRightWithValue() => RelativePanel.GetAlignRightWith(m_element);
    public object? GetAlignBottomWithValue() => RelativePanel.GetAlignBottomWith(m_element);
    public bool GetAlignLeftWithPanelValue() => RelativePanel.GetAlignLeftWithPanel(m_element);
    public bool GetAlignTopWithPanelValue() => RelativePanel.GetAlignTopWithPanel(m_element);
    public bool GetAlignRightWithPanelValue() => RelativePanel.GetAlignRightWithPanel(m_element);
    public bool GetAlignBottomWithPanelValue() => RelativePanel.GetAlignBottomWithPanel(m_element);
    public bool GetAlignHorizontalCenterWithPanelValue() => RelativePanel.GetAlignHorizontalCenterWithPanel(m_element);
    public bool GetAlignVerticalCenterWithPanelValue() => RelativePanel.GetAlignVerticalCenterWithPanel(m_element);

    // The node is said to be anchored when its ArrangeRect is expected to
    // align with its MeasureRect on one or more sides. For example, if the 
    // node is right-anchored, the right side of the ArrangeRect should overlap
    // with the right side of the MeasureRect. Anchoring is determined by
    // specific combinations of dependencies.
    public bool IsLeftAnchored => (IsAlignLeftWithPanel || IsAlignLeftWith || (IsRightOf && !IsAlignHorizontalCenterWith));
    public bool IsTopAnchored => (IsAlignTopWithPanel || IsAlignTopWith || (IsBelow && !IsAlignVerticalCenterWith));
    public bool IsRightAnchored => (IsAlignRightWithPanel || IsAlignRightWith || (IsLeftOf && !IsAlignHorizontalCenterWith));
    public bool IsBottomAnchored => (IsAlignBottomWithPanel || IsAlignBottomWith || (IsAbove && !IsAlignVerticalCenterWith));
    public bool IsHorizontalCenterAnchored => ((IsAlignHorizontalCenterWithPanel && !IsAlignLeftWithPanel && !IsAlignRightWithPanel && !IsAlignLeftWith && !IsAlignRightWith && !IsLeftOf && !IsRightOf)
         || (IsAlignHorizontalCenterWith && !IsAlignLeftWithPanel && !IsAlignRightWithPanel && !IsAlignLeftWith && !IsAlignRightWith));
    public bool IsVerticalCenterAnchored => ((IsAlignVerticalCenterWithPanel && !IsAlignTopWithPanel && !IsAlignBottomWithPanel && !IsAlignTopWith && !IsAlignBottomWith && !IsAbove && !IsBelow)
         || (IsAlignVerticalCenterWith && !IsAlignTopWithPanel && !IsAlignBottomWithPanel && !IsAlignTopWith && !IsAlignBottomWith));
     
    // RPState flag checks.
    public bool IsUnresolved => m_state == RPState.Unresolved;
    public bool IsPending => (m_state & RPState.Pending) == RPState.Pending;
    public bool IsMeasured => (m_state & RPState.Measured) == RPState.Measured;
    public bool IsArrangedHorizontally => (m_state & RPState.ArrangedHorizontally) == RPState.ArrangedHorizontally;
    public bool IsArrangedVertically => (m_state & RPState.ArrangedVertically) == RPState.ArrangedVertically;
    public bool IsArranged => (m_state & RPState.Arranged) == RPState.Arranged;
     
    public void SetPending(bool value)
    {
        if(value)
            m_state |= RPState.Pending;
        else
            m_state &= ~RPState.Pending;
    }

    public void SetMeasured(bool value)
    {
        if (value)
            m_state |= RPState.Measured;
        else
            m_state &= ~RPState.Measured;
    }
    public void SetArrangedHorizontally(bool value)
    {
        if (value)
            m_state |= RPState.ArrangedHorizontally;
        else
            m_state &= ~RPState.ArrangedHorizontally;
    }
    public void SetArrangedVertically(bool value)
    {
        if (value)
            m_state |= RPState.ArrangedVertically;
        else
            m_state &= ~RPState.ArrangedVertically;
    }

    // RPEdge flag checks.
    public bool IsLeftOf => (m_constraints & RPConstraints.LeftOf) == RPConstraints.LeftOf; 
    public bool IsAbove => (m_constraints & RPConstraints.Above) == RPConstraints.Above; 
    public bool IsRightOf => (m_constraints & RPConstraints.RightOf) == RPConstraints.RightOf; 
    public bool IsBelow => (m_constraints & RPConstraints.Below) == RPConstraints.Below; 
    public bool IsAlignHorizontalCenterWith => (m_constraints & RPConstraints.AlignHorizontalCenterWith) == RPConstraints.AlignHorizontalCenterWith; 
    public bool IsAlignVerticalCenterWith => (m_constraints & RPConstraints.AlignVerticalCenterWith) == RPConstraints.AlignVerticalCenterWith; 
    public bool IsAlignLeftWith => (m_constraints & RPConstraints.AlignLeftWith) == RPConstraints.AlignLeftWith; 
    public bool IsAlignTopWith => (m_constraints & RPConstraints.AlignTopWith) == RPConstraints.AlignTopWith; 
    public bool IsAlignRightWith => (m_constraints & RPConstraints.AlignRightWith) == RPConstraints.AlignRightWith; 
    public bool IsAlignBottomWith=>(m_constraints & RPConstraints.AlignBottomWith) == RPConstraints.AlignBottomWith;
    public bool IsAlignLeftWithPanel => (m_constraints & RPConstraints.AlignLeftWithPanel) == RPConstraints.AlignLeftWithPanel; 
    public bool IsAlignTopWithPanel=>(m_constraints & RPConstraints.AlignTopWithPanel) == RPConstraints.AlignTopWithPanel; 
    public bool IsAlignRightWithPanel => (m_constraints & RPConstraints.AlignRightWithPanel) == RPConstraints.AlignRightWithPanel; 
    public bool IsAlignBottomWithPanel=>(m_constraints & RPConstraints.AlignBottomWithPanel) == RPConstraints.AlignBottomWithPanel; 
    public bool IsAlignHorizontalCenterWithPanel=>(m_constraints & RPConstraints.AlignHorizontalCenterWithPanel) == RPConstraints.AlignHorizontalCenterWithPanel;
    public bool IsAlignVerticalCenterWithPanel =>(m_constraints & RPConstraints.AlignVerticalCenterWithPanel) == RPConstraints.AlignVerticalCenterWithPanel;

    public void SetLeftOfConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_leftOfNode = neighbor;
            m_constraints |= RPConstraints.LeftOf;
        }
        else
        {
            m_leftOfNode = null;
            m_constraints &= ~RPConstraints.LeftOf;
        }
    }
    public void SetAboveConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_aboveNode = neighbor;
            m_constraints |= RPConstraints.Above;
        }
        else
        {
            m_aboveNode = null;
            m_constraints &= ~RPConstraints.Above;
        }
    }

    public void SetRightOfConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_rightOfNode = neighbor;
            m_constraints |= RPConstraints.RightOf;
        }
        else
        {
            m_rightOfNode = null;
            m_constraints &= ~RPConstraints.RightOf;
        }
    }
    public void SetBelowConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_belowNode = neighbor;
            m_constraints |= RPConstraints.Below;
        }
        else
        {
            m_belowNode = null;
            m_constraints &= ~RPConstraints.Below;
        }
    }

    public void SetAlignHorizontalCenterWithConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_alignHorizontalCenterWithNode = neighbor;
            m_constraints |= RPConstraints.AlignHorizontalCenterWith;
        }
        else
        {
            m_alignHorizontalCenterWithNode = null;
            m_constraints &= ~RPConstraints.AlignHorizontalCenterWith;
        }
    }

    public void SetAlignVerticalCenterWithConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_alignVerticalCenterWithNode = neighbor;
            m_constraints |= RPConstraints.AlignVerticalCenterWith;
        }
        else
        {
            m_alignVerticalCenterWithNode = null;
            m_constraints &= ~RPConstraints.AlignVerticalCenterWith;
        }
    }

    public void SetAlignLeftWithConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_alignLeftWithNode = neighbor;
            m_constraints |= RPConstraints.AlignLeftWith;
        }
        else
        {
            m_alignLeftWithNode = null;
            m_constraints &= ~RPConstraints.AlignLeftWith;
        }
    }
    public void SetAlignTopWithConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_alignTopWithNode = neighbor;
            m_constraints |= RPConstraints.AlignTopWith;
        }
        else
        {
            m_alignTopWithNode = null;
            m_constraints &= ~RPConstraints.AlignTopWith;
        }
    }
    public void SetAlignRightWithConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_alignRightWithNode = neighbor;
            m_constraints |= RPConstraints.AlignRightWith;
        }
        else
        {
            m_alignRightWithNode = null;
            m_constraints &= ~RPConstraints.AlignRightWith;
        }
    }
    public void SetAlignBottomWithConstraint(RPNode? neighbor)
    {
        if (neighbor is not null)
        {
            m_alignBottomWithNode = neighbor;
            m_constraints |= RPConstraints.AlignBottomWith;
        }
        else
        {
            m_alignBottomWithNode = null;
            m_constraints &= ~RPConstraints.AlignBottomWith;
        }
    }
    public void SetAlignLeftWithPanelConstraint( bool value)
    {
        if (value)
        {
            m_constraints |= RPConstraints.AlignLeftWithPanel;
        }
        else
        {
            m_constraints &= ~RPConstraints.AlignLeftWithPanel;
        }
    }

    public void SetAlignTopWithPanelConstraint( bool value)
    {
        if (value)
        {
            m_constraints |= RPConstraints.AlignTopWithPanel;
        }
        else
        {
            m_constraints &= ~RPConstraints.AlignTopWithPanel;
        }
    }
    public void SetAlignRightWithPanelConstraint( bool value)
    {
        if (value)
        {
            m_constraints |= RPConstraints.AlignRightWithPanel;
        }
        else
        {
            m_constraints &= ~RPConstraints.AlignRightWithPanel;
        }
    }
    public void SetAlignBottomWithPanelConstraint( bool value)
    {
        if (value)
        {
            m_constraints |= RPConstraints.AlignBottomWithPanel;
        }
        else
        {
            m_constraints &= ~RPConstraints.AlignBottomWithPanel;
        }
    }
    public void SetAlignHorizontalCenterWithPanelConstraint( bool value)
    {
        if (value)
        {
            m_constraints |= RPConstraints.AlignHorizontalCenterWithPanel;
        }
        else
        {
            m_constraints &= ~RPConstraints.AlignHorizontalCenterWithPanel;
        }
    }
    public void SetAlignVerticalCenterWithPanelConstraint( bool value)
    {
        if (value)
        {
            m_constraints |= RPConstraints.AlignVerticalCenterWithPanel;
        }
        else
        {
            m_constraints &= ~RPConstraints.AlignVerticalCenterWithPanel;
        }
    }

    public void UnmarkNeighborsAsHorizontalOrVerticalLeaves()
    {
        bool isHorizontallyCenteredFromLeft = false;
        bool isHorizontallyCenteredFromRight = false;
        bool isVerticallyCenteredFromTop = false;
        bool isVerticallyCenteredFromBottom = false;

        if (!IsAlignLeftWithPanel)
        {
            if (IsAlignLeftWith)
            {
                Debug.Assert(m_alignLeftWithNode != null);
                m_alignLeftWithNode.m_isHorizontalLeaf = false;
            }
            else if (IsAlignHorizontalCenterWith)
            {
                isHorizontallyCenteredFromLeft = true;
            }
            else if (IsRightOf)
            {
                Debug.Assert(m_rightOfNode != null);
                m_rightOfNode.m_isHorizontalLeaf = false;
            }
        }

        if (!IsAlignTopWithPanel)
        {
            if (IsAlignTopWith)
            {
                Debug.Assert(m_alignTopWithNode != null);
                m_alignTopWithNode.m_isVerticalLeaf = false;
            }
            else if (IsAlignVerticalCenterWith)
            {
                isVerticallyCenteredFromTop = true;
            }
            else if (IsBelow)
            {
                Debug.Assert(m_belowNode != null);
                m_belowNode.m_isVerticalLeaf = false;
            }
        }

        if (!IsAlignRightWithPanel)
        {
            if (IsAlignRightWith)
            {
                Debug.Assert(m_alignRightWithNode != null);
                m_alignRightWithNode.m_isHorizontalLeaf = false;
            }
            else if (IsAlignHorizontalCenterWith)
            {
                isHorizontallyCenteredFromRight = true;
            }
            else if (IsLeftOf)
            {
                Debug.Assert(m_leftOfNode != null);
                m_leftOfNode.m_isHorizontalLeaf = false;
            }
        }

        if (!IsAlignBottomWithPanel)
        {
            if (IsAlignBottomWith)
            {
                Debug.Assert(m_alignBottomWithNode != null);
                m_alignBottomWithNode.m_isVerticalLeaf = false;
            }
            else if (IsAlignVerticalCenterWith)
            {
                isVerticallyCenteredFromBottom = true;
            }
            else if (IsAbove)
            {
                Debug.Assert(m_aboveNode != null);
                m_aboveNode.m_isVerticalLeaf = false;
            }
        }

        if (isHorizontallyCenteredFromLeft && isHorizontallyCenteredFromRight)
        {
            Debug.Assert(m_alignHorizontalCenterWithNode != null);
            m_alignHorizontalCenterWithNode.m_isHorizontalLeaf = false;
        }

        if (isVerticallyCenteredFromTop && isVerticallyCenteredFromBottom)
        {
            Debug.Assert(m_alignVerticalCenterWithNode != null);
            m_alignVerticalCenterWithNode.m_isVerticalLeaf = false;
        }
    }
}

[Flags]
enum RPConstraints
{
    None = 0x00000,
    LeftOf = 0x00001,
    Above = 0x00002,
    RightOf = 0x00004,
    Below = 0x00008,
    AlignHorizontalCenterWith = 0x00010,
    AlignVerticalCenterWith = 0x00020,
    AlignLeftWith = 0x00040,
    AlignTopWith = 0x00080,
    AlignRightWith = 0x00100,
    AlignBottomWith = 0x00200,
    AlignLeftWithPanel = 0x00400,
    AlignTopWithPanel = 0x00800,
    AlignRightWithPanel = 0x01000,
    AlignBottomWithPanel = 0x02000,
    AlignHorizontalCenterWithPanel = 0x04000,
    AlignVerticalCenterWithPanel = 0x08000
}

[Flags]
enum RPState
{
    Unresolved = 0x00,
    Pending = 0x01,
    Measured = 0x02,
    ArrangedHorizontally = 0x04,
    ArrangedVertically = 0x08,
    Arranged = 0x0C
}