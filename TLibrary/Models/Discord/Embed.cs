﻿using System;
using Newtonsoft.Json;

namespace Tavstal.TLibrary.Models.Discord
{
    public class Embed
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("timestamp")]
        public  DateTime TimeStamp { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
        [JsonProperty("footer")]
        public EmbedFooter Footer { get; set; }
        [JsonProperty("image")]
        public EmbedImage Image { get; set; }
        [JsonProperty("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }
        [JsonProperty("video")]
        public EmbedVideo Video { get; set; }
        [JsonProperty("author")]
        public EmbedAuthor Author { get; set; }
        [JsonProperty("fields")]
        public EmbedField[] Fields { get; set; }

        public Embed() { }

        public Embed(string title = null, string type = null, string description = null, string url = null, DateTime timeStamp = default, int color = 0, EmbedFooter footer = null, EmbedImage image = null, EmbedThumbnail thumbnail = null, EmbedVideo video = null, EmbedAuthor author = null, EmbedField[] fields = null)
        {
            Title = title;
            Type = type;
            Description = description;
            Url = url;
            TimeStamp = timeStamp;
            Color = color;
            Footer = footer;
            Image = image;
            Thumbnail = thumbnail;
            Video = video;
            Author = author;
            Fields = fields;
        }

        public string GetIsoDateTime()
        {
            return TimeStamp.ToString("s");
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
