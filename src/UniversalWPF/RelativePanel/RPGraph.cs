#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

// Based on port of https://github.com/microsoft/microsoft-ui-xaml/blob/7198c4e69019516fb08ecb3937492ffff5fe9702/dxaml/xcp/components/relativepanel/lib/RPGraph.cpp#L1 (WinUI 3 v1.4.2)
namespace UniversalWPF
{
    internal class RPGraph
    {
        private double m_minX = 0;
        private double m_maxX = 0;
        private double m_minY = 0;
        private double m_maxY = 0;
        private bool m_isMinCapped = false;
        private bool m_isMaxCapped = false;
        private Size m_availableSizeForNodeResolution;
        private List<RPNode> m_nodes = new List<RPNode>();

        public List<RPNode> Nodes => m_nodes;

        public void AddNodes(UIElementCollection uIElementCollection)
        {
            foreach (var item in uIElementCollection)
            {
                if (item is DependencyObject dpo)
                    m_nodes.Add(new RPNode(dpo));
            }
        }

        public void ResolveConstraints(DependencyObject parent)
        {
            foreach (var node in m_nodes)
            {
                object? value = node.GetLeftOfValue();
                if (value is not null)
                    node.SetLeftOfConstraint(GetNodeByValue(value, parent));

                value = node.GetAboveValue();
                if (value is not null)
                    node.SetAboveConstraint(GetNodeByValue(value, parent));

                value = node.GetRightOfValue();
                if (value is not null)
                    node.SetRightOfConstraint(GetNodeByValue(value, parent));

                value = node.GetBelowValue();
                if (value is not null)
                    node.SetBelowConstraint(GetNodeByValue(value, parent));

                value = node.GetAlignHorizontalCenterWithValue();
                if (value is not null)
                    node.SetAlignHorizontalCenterWithConstraint(GetNodeByValue(value, parent));

                value = node.GetAlignVerticalCenterWithValue();
                if (value is not null)
                    node.SetAlignVerticalCenterWithConstraint(GetNodeByValue(value, parent));

                value = node.GetAlignLeftWithValue();
                if (value is not null)
                    node.SetAlignLeftWithConstraint(GetNodeByValue(value, parent));

                value = node.GetAlignTopWithValue();
                if (value is not null)
                    node.SetAlignTopWithConstraint(GetNodeByValue(value, parent));

                value = node.GetAlignRightWithValue();
                if (value is not null)
                    node.SetAlignRightWithConstraint(GetNodeByValue(value, parent));

                value = node.GetAlignBottomWithValue();
                if (value is not null)
                    node.SetAlignBottomWithConstraint(GetNodeByValue(value, parent));

                node.SetAlignLeftWithPanelConstraint(node.GetAlignLeftWithPanelValue());
                node.SetAlignTopWithPanelConstraint(node.GetAlignTopWithPanelValue());
                node.SetAlignRightWithPanelConstraint(node.GetAlignRightWithPanelValue());
                node.SetAlignBottomWithPanelConstraint(node.GetAlignBottomWithPanelValue());
                node.SetAlignHorizontalCenterWithPanelConstraint(node.GetAlignHorizontalCenterWithPanelValue());
                node.SetAlignVerticalCenterWithPanelConstraint(node.GetAlignVerticalCenterWithPanelValue());
            }
        }

        public void MeasureNodes(Size availableSize)
        {
            foreach (var node in Nodes)
            {
                MeasureNode(node, availableSize);
            }
            m_availableSizeForNodeResolution = availableSize;
        }

        public void ArrangeNodes(Rect finalRect)
        {
            Size finalSize = finalRect.Size;

            // If the final size is the same as the available size that we used
            // to measure the nodes, this means that the pseudo-arrange pass  
            // that we did during the measure pass is, in fact, valid and the 
            // ArrangeRects that were calculated for each node are correct. In 
            // other words, we can just go ahead and call arrange on each
            // element. However, if the width and/or height of the final size
            // differs (e.g. when the element's HorizontalAlignment and/or
            // VerticalAlignment is something other than Stretch and thus the final
            // size corresponds to the desired size of the panel), we must first
            // recalculate the horizontal and/or vertical values of the ArrangeRects,
            // respectively.
            if (m_availableSizeForNodeResolution.Width != finalSize.Width)
            {
                foreach (RPNode node in m_nodes)
                {
                    node.SetArrangedHorizontally(false);
                }

                foreach (RPNode node in m_nodes)
                {
                    ArrangeNodeHorizontally(node, finalSize);
                }
            }

            if (m_availableSizeForNodeResolution.Height != finalSize.Height)
            {
                foreach (RPNode node in m_nodes)
                {
                    node.SetArrangedVertically(false);
                }

                foreach (RPNode node in m_nodes)
                {
                    ArrangeNodeVertically(node, finalSize);
                }
            }

            m_availableSizeForNodeResolution = finalSize;

            foreach (RPNode node in m_nodes)
            {
                Debug.Assert(node.IsArranged);

                Rect layoutSlot = new Rect(
                 x: Math.Max(node.m_arrangeRect.X + finalRect.X, 0.0),
                 y: Math.Max(node.m_arrangeRect.Y + finalRect.Y, 0.0),
                 width: Math.Max(node.m_arrangeRect.Width, 0.0),
                 height: Math.Max(node.m_arrangeRect.Height, 0.0));

                node.Arrange(layoutSlot);
            }
        }

        /// <summary>
        /// Starting at zero, we will traverse all the horizontal and vertical 
        /// chains in the graph one by one while moving a "cursor" up and down 
        /// as we go in jumps that are equivalent to the size of the element 
        /// associated to the node that we are currently visiting. Everytime
        /// that the cursor hits a new max or new min value, we cache it. At the
        /// end, the difference between the max and the min values corresponds
        /// to the desired size of that chain. The size of the biggest chain
        /// also corresponds to the desired size of the panel.
        /// </summary>
        /// <returns></returns>
        public Size CalculateDesiredSize()
        {
            Size maxDesiredSize = new Size(0,0);

            MarkHorizontalAndVerticalLeaves();

            foreach (RPNode node in m_nodes)
            {
                if (node.m_isHorizontalLeaf)
                {
                    m_minX = 0.0f;
                    m_maxX = 0.0f;
                    m_isMinCapped = false;
                    m_isMaxCapped = false;

                    AccumulatePositiveDesiredWidth(node, 0.0f);
                    maxDesiredSize.Width = Math.Max(maxDesiredSize.Width, m_maxX - m_minX);
                }

                if (node.m_isVerticalLeaf)
                {
                    m_minY = 0.0f;
                    m_maxY = 0.0f;
                    m_isMinCapped = false;
                    m_isMaxCapped = false;

                    AccumulatePositiveDesiredHeight(node, 0.0f);
                    maxDesiredSize.Height = Math.Max(maxDesiredSize.Height, m_maxY - m_minY);
                }
            }

            return maxDesiredSize;
        }

        private RPNode GetNodeByValue(object value, DependencyObject parent)
        {
            if (value is UIElement element)
            {
                foreach (var node in m_nodes)
                    if (node.Element == element)
                        return node;
            }
            throw new InvalidOperationException("Element {value} not found in RelativePanel"); //TODO: Better error here
        }

        // Starting off with the space that is available to the entire panel
        // (a.k.a. available size), we will constrain this space little by 
        // little based on the ArrangeRects of the dependencies that the
        // node has. The end result corresponds to the MeasureRect of this node. 
        // Consider the following example: if an element is to the left of a 
        // sibling, that means that the space that is available to this element
        // in particular is now the available size minus the Width of the 
        // ArrangeRect associated with this sibling.
        void CalculateMeasureRectHorizontally(RPNode node, Size availableSize, out double x, out double width)
        {

            bool isHorizontallyCenteredFromLeft = false;
            bool isHorizontallyCenteredFromRight = false;

            // The initial values correspond to the entire available space. In
            // other words, the edges of the element are aligned to the edges
            // of the panel by default. We will now constrain each side of this
            // space as necessary.
            x = 0.0f;
            width = availableSize.Width;

            // If we have infinite available width, then the Width of the
            // MeasureRect is also infinite; we do not have to constrain it.
            if (!double.IsPositiveInfinity(availableSize.Width))
            {
                // Constrain the left side of the available space, i.e.
                // a) The child has its left edge aligned with the panel (default),
                // b) The child has its left edge aligned with the left edge of a sibling,
                // or c) The child is positioned to the right of a sibling.
                //
                //  |;;                 |               |                                                   
                //  |;;                 |               |                
                //  |;;                 |.......:|                       ;;......:;; 
                //  |;;                 |;             ;|       .               ;;             ;;
                //  |;;                 |;             ;|     .;;............   ;;             ;;
                //  |;;                 |;             ;|   .;;;;......   ;;             ;;
                //  |;;                 |;             ;|    ':;;......   ;;             ;;
                //  |;;                 |;             ;|      ':               ;;             ;;       
                //  |;;                 |.......:|                       ........:
                //  |;;                 |               |               
                //  |;;                 |               |
                //  AlignLeftWithPanel  AlignLeftWith   RightOf
                //
                if (!node.IsAlignLeftWithPanel)
                {
                    if (node.IsAlignLeftWith)
                    {
                        RPNode alignLeftWithNeighbor = node.m_alignLeftWithNode;
                        double restrictedHorizontalSpace = alignLeftWithNeighbor.m_arrangeRect.X;

                        x = restrictedHorizontalSpace;
                        width -= restrictedHorizontalSpace;
                    }
                    else if (node.IsAlignHorizontalCenterWith)
                    {
                        isHorizontallyCenteredFromLeft = true;
                    }
                    else if (node.IsRightOf)
                    {
                        RPNode rightOfNeighbor = node.m_rightOfNode;
                        double restrictedHorizontalSpace = rightOfNeighbor.m_arrangeRect.X + rightOfNeighbor.m_arrangeRect.Width;

                        x = restrictedHorizontalSpace;
                        width -= restrictedHorizontalSpace;
                    }
                }

                // Constrain the right side of the available space, i.e.
                // a) The child has its right edge aligned with the panel (default),
                // b) The child has its right edge aligned with the right edge of a sibling,
                // or c) The child is positioned to the left of a sibling.
                //  
                //                                          |               |                   ;;|
                //                                          |               |                   ;;|
                //  ;;......:;;                       |;......:;|                   ;;|
                //  ;;             ;;               .       |;             ;|                   ;;|
                //  ;;             ;;   ............;;.     |;             ;|                   ;;|
                //  ;;             ;;   ......;;;;.   |;             ;|                   ;;|
                //  ;;             ;;   ......;;:'    |;             ;|                   ;;|
                //  ;;             ;;               :'      |;             ;|                   ;;|
                //  ........:                       |.......:|                   ;;|
                //                                          |               |                   ;;|
                //                                          |               |                   ;;|
                //                                          LeftOf          AlignRightWith      AlignRightWithPanel
                //
                if (!node.IsAlignRightWithPanel)
                {
                    if (node.IsAlignRightWith)
                    {
                        RPNode alignRightWithNeighbor = node.m_alignRightWithNode;

                        width -= availableSize.Width - (alignRightWithNeighbor.m_arrangeRect.X + alignRightWithNeighbor.m_arrangeRect.Width);
                    }
                    else if (node.IsAlignHorizontalCenterWith)
                    {
                        isHorizontallyCenteredFromRight = true;
                    }
                    else if (node.IsLeftOf)
                    {
                        RPNode leftOfNeighbor = node.m_leftOfNode;

                        width -= availableSize.Width - leftOfNeighbor.m_arrangeRect.X;
                    }
                }

                if (isHorizontallyCenteredFromLeft && isHorizontallyCenteredFromRight)
                {
                    RPNode alignHorizontalCenterWithNeighbor = node.m_alignHorizontalCenterWithNode;
                    double centerOfNeighbor = alignHorizontalCenterWithNeighbor.m_arrangeRect.X + (alignHorizontalCenterWithNeighbor.m_arrangeRect.Width / 2.0f);
                    width = Math.Min(centerOfNeighbor, availableSize.Width - centerOfNeighbor) * 2.0f;
                    x = centerOfNeighbor - (width / 2.0f);
                }
            }
        }
        void CalculateMeasureRectVertically(RPNode node, Size availableSize, out double y, out double height)
        {

            bool isVerticallyCenteredFromTop = false;
            bool isVerticallyCenteredFromBottom = false;

            // The initial values correspond to the entire available space. In
            // other words, the edges of the element are aligned to the edges
            // of the panel by default. We will now constrain each side of this
            // space as necessary.
            y = 0.0f;
            height = availableSize.Height;

            // If we have infinite available height, then the Height of the
            // MeasureRect is also infinite; we do not have to constrain it.
            if (!double.IsPositiveInfinity(availableSize.Height))
            {
                // Constrain the top of the available space, i.e.
                // a) The child has its top edge aligned with the panel (default),
                // b) The child has its top edge aligned with the top edge of a sibling,
                // or c) The child is positioned to the below a sibling.
                //
                //  ================================== AlignTopWithPanel
                //  .................
                //
                //
                //
                //  --------;;=============;;--------- AlignTopWith
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //  --------.=============.--------- Below 
                //                  .
                //                .:;:.
                //              .:;;;;;:.
                //                ;;;;;
                //                ;;;;;
                //                ;;;;;
                //                ;;;;;
                //                ;;;;;
                //
                //          ;;......:;; 
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ........:
                //
                if (!node.IsAlignTopWithPanel)
                {
                    if (node.IsAlignTopWith)
                    {
                        RPNode alignTopWithNeighbor = node.m_alignTopWithNode;
                        double restrictedVerticalSpace = alignTopWithNeighbor.m_arrangeRect.Y;

                        y = restrictedVerticalSpace;
                        height -= restrictedVerticalSpace;
                    }
                    else if (node.IsAlignVerticalCenterWith)
                    {
                        isVerticallyCenteredFromTop = true;
                    }
                    else if (node.IsBelow)
                    {
                        RPNode belowNeighbor = node.m_belowNode;
                        double restrictedVerticalSpace = belowNeighbor.m_arrangeRect.Y + belowNeighbor.m_arrangeRect.Height;

                        y = restrictedVerticalSpace;
                        height -= restrictedVerticalSpace;
                    }
                }

                // Constrain the bottom of the available space, i.e.
                // a) The child has its bottom edge aligned with the panel (default),
                // b) The child has its bottom edge aligned with the bottom edge of a sibling,
                // or c) The child is positioned to the above a sibling.
                //
                //          ;;......:;; 
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ........:
                //
                //                ;;;;;
                //                ;;;;;
                //                ;;;;;
                //                ;;;;;
                //                ;;;;;
                //              ..;;;;;..
                //               '..:'
                //                 ':`
                //                  
                //  --------;;=============;;--------- Above 
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //          ;;             ;;
                //  --------.=============.--------- AlignBottomWith
                //
                // 
                //
                //  .................
                //  ================================== AlignBottomWithPanel
                //
                if (!node.IsAlignBottomWithPanel)
                {
                    if (node.IsAlignBottomWith)
                    {
                        RPNode alignBottomWithNeighbor = node.m_alignBottomWithNode;

                        height -= availableSize.Height - (alignBottomWithNeighbor.m_arrangeRect.Y + alignBottomWithNeighbor.m_arrangeRect.Height);
                    }
                    else if (node.IsAlignVerticalCenterWith)
                    {
                        isVerticallyCenteredFromBottom = true;
                    }
                    else if (node.IsAbove)
                    {
                        RPNode aboveNeighbor = node.m_aboveNode;

                        height -= availableSize.Height - aboveNeighbor.m_arrangeRect.Y;
                    }
                }

                if (isVerticallyCenteredFromTop && isVerticallyCenteredFromBottom)
                {
                    RPNode alignVerticalCenterWithNeighbor = node.m_alignVerticalCenterWithNode;
                    double centerOfNeighbor = alignVerticalCenterWithNeighbor.m_arrangeRect.Y + (alignVerticalCenterWithNeighbor.m_arrangeRect.Height / 2.0f);
                    height = Math.Min(centerOfNeighbor, availableSize.Height - centerOfNeighbor) * 2.0f;
                    y = centerOfNeighbor - (height / 2.0f);
                }
            }
        }

        // The ArrageRect (a.k.a. layout slot) corresponds to the specific rect 
        // within the MeasureRect that will be given to an element for it to
        // position itself. The exact rect is calculated based on anchoring
        // and, unless anchoring dictates otherwise, the size of the
        // ArrangeRect is equal to the desired size of the element itself. 
        // Consider the following example: if the node is right-anchored, the 
        // right side of the ArrangeRect should overlap with the right side
        // of the MeasureRect; this same logic is applied to the other three
        // sides of the rect.
        void CalculateArrangeRectHorizontally(RPNode node, out double x, out double width)
        {
            Rect measureRect = node.m_measureRect;
            double desiredWidth = Math.Min(measureRect.Width, node.DesiredWidth);

            Debug.Assert(node.IsMeasured && !double.IsPositiveInfinity(measureRect.Width));

            // The initial values correspond to the left corner, using the 
            // desired size of element. If no attached properties were set, 
            // this means that the element will default to the left corner of
            // the panel.
            x = measureRect.X;
            width = desiredWidth;

            if (node.IsLeftAnchored)
            {
                if (node.IsRightAnchored)
                {
                    x = measureRect.X;
                    width = measureRect.Width;
                }
                else
                {
                    x = measureRect.X;
                    width = desiredWidth;
                }
            }
            else if (node.IsRightAnchored)
            {
                x = measureRect.X + measureRect.Width - desiredWidth;
                width = desiredWidth;
            }
            else if (node.IsHorizontalCenterAnchored)
            {
                x = measureRect.X + (measureRect.Width / 2.0f) - (desiredWidth / 2.0f);
                width = desiredWidth;
            }
        }
        void CalculateArrangeRectVertically(RPNode node, out double y, out double height)
        {

            Rect measureRect = node.m_measureRect;
            double desiredHeight = Math.Min(measureRect.Height, node.DesiredHeight);

            Debug.Assert(node.IsMeasured && !double.IsPositiveInfinity(measureRect.Height));

            // The initial values correspond to the top corner, using the 
            // desired size of element. If no attached properties were set, 
            // this means that the element will default to the top corner of
            // the panel.
            y = measureRect.Y;
            height = desiredHeight;

            if (node.IsTopAnchored)
            {
                if (node.IsBottomAnchored)
                {
                    y = measureRect.Y;
                    height = measureRect.Height;
                }
                else
                {
                    y = measureRect.Y;
                    height = desiredHeight;
                }
            }
            else if (node.IsBottomAnchored)
            {
                y = measureRect.Y + measureRect.Height - desiredHeight;
                height = desiredHeight;
            }
            else if (node.IsVerticalCenterAnchored)
            {
                y = measureRect.Y + (measureRect.Height / 2.0f) - (desiredHeight / 2.0f);
                height = desiredHeight;
            }
        }

        void MarkHorizontalAndVerticalLeaves()
        {
            foreach (var node in m_nodes)
            {
                node.m_isHorizontalLeaf = true;
                node.m_isVerticalLeaf = true;
            }

            foreach (var node in m_nodes)
            {
                node.UnmarkNeighborsAsHorizontalOrVerticalLeaves();
            }
        }

        void AccumulatePositiveDesiredWidth(RPNode node, double x)
        {
            double initialX = x;
            bool isHorizontallyCenteredFromLeft = false;
            bool isHorizontallyCenteredFromRight = false;

            Debug.Assert(node.IsMeasured);

            // If we are going in the positive direction, move the cursor
            // right by the desired width of the node with which we are 
            // currently working and refresh the maximum positive value.
            x += node.DesiredWidth;
            m_maxX = Math.Max(m_maxX, x);

            if (node.IsAlignLeftWithPanel)
            {
                if (!m_isMaxCapped)
                {
                    m_maxX = x;
                    m_isMaxCapped = true;
                }
            }
            else if (node.IsAlignLeftWith)
            {
                // If the AlignLeftWithNode and AlignRightWithNode are the
                // same element, we can skip the former, since we will move 
                // through the latter later.
                if (node.m_alignLeftWithNode != node.m_alignRightWithNode)
                {
                    AccumulateNegativeDesiredWidth(node.m_alignLeftWithNode, x);
                }
            }
            else if (node.IsAlignHorizontalCenterWith)
            {
                isHorizontallyCenteredFromLeft = true;
            }
            else if (node.IsRightOf)
            {
                AccumulatePositiveDesiredWidth(node.m_rightOfNode, x);
            }

            if (node.IsAlignRightWithPanel)
            {
                if (m_isMinCapped)
                {
                    m_minX = Math.Min(m_minX, initialX);
                }
                else
                {
                    m_minX = initialX;
                    m_isMinCapped = true;
                }
            }
            else if (node.IsAlignRightWith)
            {
                // If this element's right is aligned to some other 
                // element's right, now we will be going in the positive
                // direction to that other element in order to continue the
                // traversal of the dependency chain. But first, since we 
                // arrived to the node where we currently are by going in
                // the positive direction, that means that we have already 
                // moved the cursor right to calculate the maximum positive 
                // value, so we will use the initial value of Y.
                AccumulatePositiveDesiredWidth(node.m_alignRightWithNode, initialX);
            }
            else if (node.IsAlignHorizontalCenterWith)
            {
                isHorizontallyCenteredFromRight = true;
            }
            else if (node.IsLeftOf)
            {
                // If this element is to the left of some other element,
                // now we will be going in the negative direction to that
                // other element in order to continue the traversal of the
                // dependency chain. But first, since we arrived to the
                // node where we currently are by going in the positive
                // direction, that means that we have already moved the 
                // cursor right to calculate the maximum positive value, so
                // we will use the initial value of X.
                AccumulateNegativeDesiredWidth(node.m_leftOfNode, initialX);
            }

            if (isHorizontallyCenteredFromLeft && isHorizontallyCenteredFromRight)
            {
                double centerX = x - (node.DesiredWidth / 2.0);
                double edgeX = centerX - (node.m_alignHorizontalCenterWithNode.DesiredWidth / 2.0);
                m_minX = Math.Min(m_minX, edgeX);
                AccumulatePositiveDesiredWidth(node.m_alignHorizontalCenterWithNode, edgeX);
            }
            else if (node.IsHorizontalCenterAnchored)
            {
                // If this node is horizontally anchored to the center, then it
                // means that it is the root of this dependency chain based on
                // the current definition of precedence for constraints: 
                // e.g. AlignLeftWithPanel 
                // > AlignLeftWith 
                // > RightOf
                // > AlignHorizontalCenterWithPanel    
                // Thus, we can report its width as twice the width of 
                // either the difference from center to left or the difference
                // from center to right, whichever is the greatest.
                double centerX = x - (node.DesiredWidth / 2.0);
                double upper = m_maxX - centerX;
                double lower = centerX - m_minX;
                m_maxX = Math.Max(upper, lower) * 2.0;
                m_minX = 0.0;
            }
        }
        void AccumulateNegativeDesiredWidth(RPNode node, double x)
        {

            double initialX = x;
            bool isHorizontallyCenteredFromLeft = false;
            bool isHorizontallyCenteredFromRight = false;

            Debug.Assert(node.IsMeasured);

            // If we are going in the negative direction, move the cursor
            // left by the desired width of the node with which we are 
            // currently working and refresh the minimum negative value.
            x -= node.DesiredWidth;
            m_minX = Math.Min(m_minX, x);

            if (node.IsAlignRightWithPanel)
            {
                if (!m_isMinCapped)
                {
                    m_minX = x;
                    m_isMinCapped = true;
                }
            }
            else if (node.IsAlignRightWith)
            {
                // If the AlignRightWithNode and AlignLeftWithNode are the
                // same element, we can skip the former, since we will move 
                // through the latter later.
                if (node.m_alignRightWithNode != node.m_alignLeftWithNode)
                {
                    AccumulatePositiveDesiredWidth(node.m_alignRightWithNode, x);
                }
            }
            else if (node.IsAlignHorizontalCenterWith)
            {
                isHorizontallyCenteredFromRight = true;
            }
            else if (node.IsLeftOf)
            {
                AccumulateNegativeDesiredWidth(node.m_leftOfNode, x);
            }

            if (node.IsAlignLeftWithPanel)
            {
                if (m_isMaxCapped)
                {
                    m_maxX = Math.Max(m_maxX, initialX);
                }
                else
                {
                    m_maxX = initialX;
                    m_isMaxCapped = true;
                }
            }
            else if (node.IsAlignLeftWith)
            {
                // If this element's left is aligned to some other element's
                // left, now we will be going in the negative direction to 
                // that other element in order to continue the traversal of
                // the dependency chain. But first, since we arrived to the
                // node where we currently are by going in the negative 
                // direction, that means that we have already moved the 
                // cursor left to calculate the minimum negative value,
                // so we will use the initial value of X.
                AccumulateNegativeDesiredWidth(node.m_alignLeftWithNode, initialX);
            }
            else if (node.IsAlignHorizontalCenterWith)
            {
                isHorizontallyCenteredFromLeft = true;
            }
            else if (node.IsRightOf)
            {
                // If this element is to the right of some other element,
                // now we will be going in the positive direction to that
                // other element in order to continue the traversal of the
                // dependency chain. But first, since we arrived to the
                // node where we currently are by going in the negative
                // direction, that means that we have already moved the 
                // cursor left to calculate the minimum negative value, so
                // we will use the initial value of X.
                AccumulatePositiveDesiredWidth(node.m_rightOfNode, initialX);
            }

            if (isHorizontallyCenteredFromLeft && isHorizontallyCenteredFromRight)
            {
                double centerX = x + (node.DesiredWidth / 2.0);
                double edgeX = centerX + (node.m_alignHorizontalCenterWithNode.DesiredWidth / 2.0);
                m_maxX = Math.Max(m_maxX, edgeX);
                AccumulateNegativeDesiredWidth(node.m_alignHorizontalCenterWithNode, edgeX);
            }
            else if (node.IsHorizontalCenterAnchored)
            {
                // If this node is horizontally anchored to the center, then it
                // means that it is the root of this dependency chain based on
                // the current definition of precedence for constraints: 
                // e.g. AlignLeftWithPanel 
                // > AlignLeftWith 
                // > RightOf
                // > AlignHorizontalCenterWithPanel    
                // Thus, we can report its width as twice the width of 
                // either the difference from center to left or the difference
                // from center to right, whichever is the greatest.
                double centerX = x + (node.DesiredWidth / 2.0);
                double upper = m_maxX - centerX;
                double lower = centerX - m_minX;
                m_maxX = Math.Max(upper, lower) * 2.0f;
                m_minX = 0.0f;
            }
        }
        void AccumulatePositiveDesiredHeight(RPNode node, double y)
        {

            double initialY = y;
            bool isVerticallyCenteredFromTop = false;
            bool isVerticallyCenteredFromBottom = false;

            Debug.Assert(node.IsMeasured);

            // If we are going in the positive direction, move the cursor
            // up by the desired height of the node with which we are 
            // currently working and refresh the maximum positive value.
            y += node.DesiredHeight;
            m_maxY = Math.Max(m_maxY, y);

            if (node.IsAlignTopWithPanel)
            {
                if (!m_isMaxCapped)
                {
                    m_maxY = y;
                    m_isMaxCapped = true;
                }
            }
            else if (node.IsAlignTopWith)
            {
                // If the AlignTopWithNode and AlignBottomWithNode are the
                // same element, we can skip the former, since we will move 
                // through the latter later.
                if (node.m_alignTopWithNode != node.m_alignBottomWithNode)
                {
                    AccumulateNegativeDesiredHeight(node.m_alignTopWithNode, y);
                }
            }
            else if (node.IsAlignVerticalCenterWith)
            {
                isVerticallyCenteredFromTop = true;
            }
            else if (node.IsBelow)
            {
                AccumulatePositiveDesiredHeight(node.m_belowNode, y);
            }

            if (node.IsAlignBottomWithPanel)
            {
                if (m_isMinCapped)
                {
                    m_minY = Math.Min(m_minY, initialY);
                }
                else
                {
                    m_minY = initialY;
                    m_isMinCapped = true;
                }
            }
            else if (node.IsAlignBottomWith)
            {
                // If this element's bottom is aligned to some other 
                // element's bottom, now we will be going in the positive
                // direction to that other element in order to continue the
                // traversal of the dependency chain. But first, since we 
                // arrived to the node where we currently are by going in
                // the positive direction, that means that we have already 
                // moved the cursor up to calculate the maximum positive 
                // value, so we will use the initial value of Y.
                AccumulatePositiveDesiredHeight(node.m_alignBottomWithNode, initialY);
            }
            else if (node.IsAlignVerticalCenterWith)
            {
                isVerticallyCenteredFromBottom = true;
            }
            else if (node.IsAbove)
            {
                // If this element is above some other element, now we will 
                // be going in the negative direction to that other element
                // in order to continue the traversal of the dependency  
                // chain. But first, since we arrived to the node where we 
                // currently are by going in the positive direction, that
                // means that we have already moved the cursor up to 
                // calculate the maximum positive value, so we will use
                // the initial value of Y.
                AccumulateNegativeDesiredHeight(node.m_aboveNode, initialY);
            }

            if (isVerticallyCenteredFromTop && isVerticallyCenteredFromBottom)
            {
                double centerY = y - (node.DesiredHeight / 2.0);
                double edgeY = centerY - (node.m_alignVerticalCenterWithNode.DesiredHeight / 2.0);
                m_minY = Math.Min(m_minY, edgeY);
                AccumulatePositiveDesiredHeight(node.m_alignVerticalCenterWithNode, edgeY);
            }
            else if (node.IsVerticalCenterAnchored)
            {
                // If this node is vertically anchored to the center, then it
                // means that it is the root of this dependency chain based on
                // the current definition of precedence for constraints: 
                // e.g. AlignTopWithPanel 
                // > AlignTopWith
                // > Below
                // > AlignVerticalCenterWithPanel 
                // Thus, we can report its height as twice the height of 
                // either the difference from center to top or the difference
                // from center to bottom, whichever is the greatest.
                double centerY = y - (node.DesiredHeight / 2.0);
                double upper = m_maxY - centerY;
                double lower = centerY - m_minY;
                m_maxY = Math.Max(upper, lower) * 2.0f;
                m_minY = 0.0f;
            }
        }
        void AccumulateNegativeDesiredHeight(RPNode node, double y)
        {
            double initialY = y;
            bool isVerticallyCenteredFromTop = false;
            bool isVerticallyCenteredFromBottom = false;

            Debug.Assert(node.IsMeasured);

            // If we are going in the negative direction, move the cursor
            // down by the desired height of the node with which we are 
            // currently working and refresh the minimum negative value.
            y -= node.DesiredHeight;
            m_minY = Math.Min(m_minY, y);

            if (node.IsAlignBottomWithPanel)
            {
                if (!m_isMinCapped)
                {
                    m_minY = y;
                    m_isMinCapped = true;
                }
            }
            else if (node.IsAlignBottomWith)
            {
                // If the AlignBottomWithNode and AlignTopWithNode are the
                // same element, we can skip the former, since we will move 
                // through the latter later.
                if (node.m_alignBottomWithNode != node.m_alignTopWithNode)
                {
                    AccumulatePositiveDesiredHeight(node.m_alignBottomWithNode, y);
                }
            }
            else if (node.IsAlignVerticalCenterWith)
            {
                isVerticallyCenteredFromBottom = true;
            }
            else if (node.IsAbove)
            {
                AccumulateNegativeDesiredHeight(node.m_aboveNode, y);
            }

            if (node.IsAlignTopWithPanel)
            {
                if (m_isMaxCapped)
                {
                    m_maxY = Math.Max(m_maxY, initialY);
                }
                else
                {
                    m_maxY = initialY;
                    m_isMaxCapped = true;
                }
            }
            else if (node.IsAlignTopWith)
            {
                // If this element's top is aligned to some other element's
                // top, now we will be going in the negative direction to 
                // that other element in order to continue the traversal of
                // the dependency chain. But first, since we arrived to the
                // node where we currently are by going in the negative 
                // direction, that means that we have already moved the 
                // cursor down to calculate the minimum negative value,
                // so we will use the initial value of Y.
                AccumulateNegativeDesiredHeight(node.m_alignTopWithNode, initialY);
            }
            else if (node.IsAlignVerticalCenterWith)
            {
                isVerticallyCenteredFromTop = true;
            }
            else if (node.IsBelow)
            {
                // If this element is below some other element, now we'll
                // be going in the positive direction to that other element  
                // in order to continue the traversal of the dependency
                // chain. But first, since we arrived to the node where we
                // currently are by going in the negative direction, that
                // means that we have already moved the cursor down to
                // calculate the minimum negative value, so we will use
                // the initial value of Y.
                AccumulatePositiveDesiredHeight(node.m_belowNode, initialY);
            }

            if (isVerticallyCenteredFromTop && isVerticallyCenteredFromBottom)
            {
                double centerY = y + (node.DesiredHeight / 2.0);
                double edgeY = centerY + (node.m_alignVerticalCenterWithNode.DesiredHeight / 2.0);
                m_maxY = Math.Max(m_maxY, edgeY);
                AccumulateNegativeDesiredHeight(node.m_alignVerticalCenterWithNode, edgeY);
            }
            else if (node.IsVerticalCenterAnchored)
            {
                // If this node is vertically anchored to the center, then it
                // means that it is the root of this dependency chain based on
                // the current definition of precedence for constraints: 
                // e.g. AlignTopWithPanel 
                // > AlignTopWith
                // > Below
                // > AlignVerticalCenterWithPanel 
                // Thus, we can report its height as twice the height of 
                // either the difference from center to top or the difference
                // from center to bottom, whichever is the greatest.
                double centerY = y + (node.DesiredHeight / 2.0);
                double upper = m_maxY - centerY;
                double lower = centerY - m_minY;
                m_maxY = Math.Max(upper, lower) * 2.0f;
                m_minY = 0.0f;
            }
        }

        // Calculates the MeasureRect of a node and then calls Measure on the
        // corresponding element by passing the Width and Height of this rect.
        // Given that the calculation of the MeasureRect requires the 
        // ArrangeRects of the dependencies, we call this method recursively on
        // said dependencies first and calculate both rects as we go. In other
        // words, this method is figuratively a combination of a measure pass 
        // plus a pseudo-arrange pass.
        void MeasureNode(RPNode? node, Size availableSize)
        {
            if (node is null)
                return;

            if (node.IsPending)
            {
                // If the node is already in the process of being resolved
                // but we tried to resolve it again, that means we are in the
                // middle of circular dependency and we must throw an 
                // InvalidOperationException. We will fail fast here and let
                // the CRelativePanel handle the rest.
                throw new InvalidOperationException("RelativePanel error: Circular dependency detected. Layout could not complete");
            }
            else if (node.IsUnresolved)
            {
                Size constrainedAvailableSize;

                // We must resolve the dependencies of this node first.
                // In the meantime, we will mark the state as pending.
                node.SetPending(true);

                MeasureNode(node.m_leftOfNode, availableSize);
                MeasureNode(node.m_aboveNode, availableSize);
                MeasureNode(node.m_rightOfNode, availableSize);
                MeasureNode(node.m_belowNode, availableSize);
                MeasureNode(node.m_alignLeftWithNode, availableSize);
                MeasureNode(node.m_alignTopWithNode, availableSize);
                MeasureNode(node.m_alignRightWithNode, availableSize);
                MeasureNode(node.m_alignBottomWithNode, availableSize);
                MeasureNode(node.m_alignHorizontalCenterWithNode, availableSize);
                MeasureNode(node.m_alignVerticalCenterWithNode, availableSize);

                node.SetPending(false);

                CalculateMeasureRectHorizontally(node, availableSize, out double x, out double width);
                node.m_measureRect.X = x;
                node.m_measureRect.Width = width;
                CalculateMeasureRectVertically(node, availableSize, out double y, out double height);
                node.m_measureRect.Y = y;
                node.m_measureRect.Height = height;

                constrainedAvailableSize = new Size(
                    width: Math.Max(node.m_measureRect.Width, 0.0f),
                    height: Math.Max(node.m_measureRect.Height, 0.0f));
                node.Measure(constrainedAvailableSize);
                node.SetMeasured(true);

                // (Pseudo-) Arranging against infinity does not make sense, so 
                // we will skip the calculations of the ArrangeRects if 
                // necessary. During the true arrange pass, we will be given a
                // non-infinite final size; we will do the necessary
                // calculations until then.
                if (!double.IsPositiveInfinity(availableSize.Width))
                {
                    CalculateArrangeRectHorizontally(node, out x, out width);
                    node.m_arrangeRect.X = x;
                    node.m_arrangeRect.Width = width;
                    node.SetArrangedHorizontally(true);
                }

                if (!double.IsPositiveInfinity(availableSize.Height))
                {
                    CalculateArrangeRectVertically(node, out y, out height);
                    node.m_arrangeRect.Y = y;
                    node.m_arrangeRect.Height = height;
                    node.SetArrangedVertically(true);
                }
            }
        }

        // Calculates the X and Width properties of the ArrangeRect of a node
        // as well as the X and Width properties of the MeasureRect (which is
        // necessary in order to calculate the former correctly). Given that 
        // the calculation of the MeasureRect requires the ArrangeRects of the
        // dependencies, we call this method recursively on said dependencies
        // first.
        void ArrangeNodeHorizontally(RPNode node, Size finalSize) => throw new NotImplementedException();

        // Calculates the Y and Height properties of the ArrangeRect of a node
        // as well as the Y and Height properties of the MeasureRect (which is
        // necessary in order to calculate the former correctly).Given that 
        // the calculation of the MeasureRect requires the ArrangeRects of the
        // dependencies, we call this method recursively on said dependencies
        // first.
        void ArrangeNodeVertically(RPNode node, Size finalSize) => throw new NotImplementedException();
    }
}
