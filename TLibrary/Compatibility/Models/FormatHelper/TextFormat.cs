﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavstal.TLibrary.Compatibility
{
    public class TextFormat
    {
        public string Key { get; set; }
        public string StartTag { get; set; }
        public string EndTag { get; set; }
        public bool isDecoration { get; set; }

        public TextFormat() { }

        public TextFormat(string key, string start, string end, bool isdeco)
        {
            Key = key;
            StartTag = start;
            EndTag = end;
            isDecoration = isdeco;
        }
    }
}