﻿using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    public class JsonParameter
    {
        public string Content { get; set; }
        public string ContentType { get; set; }
        public JsonParameter(object content) : this(JsonConvert.SerializeObject(content), "application/json") { }
        public JsonParameter(object content, string contenttype) : this(JsonConvert.SerializeObject(content), contenttype) { }
        public JsonParameter(string content, string contenttype)
        {
            Content = content;
            ContentType = contenttype;
        }
    }
}
