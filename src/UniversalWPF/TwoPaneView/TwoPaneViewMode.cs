﻿/*
Based on a port of TwoPaneView control in the WinUI Library: https://github.com/microsoft/microsoft-ui-xaml/
    MIT License

    Copyright (c) Microsoft Corporation. All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE
*/

namespace UniversalWPF
{
    /// <summary>
    /// Defines constants that specify how panes are shown in a <see cref="TwoPaneView"/>.
    /// </summary>
    /// <seealso cref="TwoPaneView"/>
    /// <seealso cref="TwoPaneView.Mode"/>
    public enum TwoPaneViewMode
    {
        /// <summary>
        /// Only one pane is shown.
        /// </summary>
        SinglePane = 0,

        /// <summary>
        /// Panes are shown side-by-side.
        /// </summary>
        Wide = 1,

        /// <summary>
        /// Panes are shown top-bottom.
        /// </summary>
        Tall = 2,
    }
}
