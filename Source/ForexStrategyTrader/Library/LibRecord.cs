//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;

namespace ForexStrategyBuilder.Library
{
    public class LibRecord
    {
        public string SourceFileName { get; set; }
        public DateTime SourceLastWriteTime { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}",
                                 SourceFileName,
                                 SourceLastWriteTime.ToShortDateString());
        }
    }
}