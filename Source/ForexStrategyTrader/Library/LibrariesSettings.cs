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
    [Serializable]
    public class LibrariesSettings
    {
        private SerializableDictionary<string, LibRecord> records;

        public SerializableDictionary<string, LibRecord> Records
        {
            get { return records ?? (records = new SerializableDictionary<string, LibRecord>()); }
            set { records = value; }
        }
    }
}