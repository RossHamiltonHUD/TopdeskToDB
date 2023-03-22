namespace TopdeskDataCache
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Item
    {
        [JsonProperty("item")]
        public KnowledgeItem[]? KnowledgeItems { get; set; }

        [JsonProperty("prev")]
        public Uri Prev { get; set; }

        [JsonProperty("next")]
        public Uri Next { get; set; }
    }

    public partial class KnowledgeItem
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("number")]
        public string? Number { get; set; }

        [JsonProperty("manager")]
        public NestedInfo? Manager { get; set; }

        [JsonProperty("creator")]
        public NestedInfo? Creator { get; set; }

        [JsonProperty("modifier")]
        public NestedInfo? Modifier { get; set; }

        [JsonProperty("status")]
        public NestedInfo? Status { get; set; }

        [JsonProperty("parent")]
        public NestedInfo Parent { get; set; }

        [JsonProperty("visibility")]
        public Visibility Visibility { get; set; }

        [JsonProperty("urls")]
        public Urls Urls { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("modificationDate")]
        public DateTime ModificationDate { get; set; }
    }

    public partial class Content
    {
        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("content")]
        public string? ContentContent { get; set; }

        [JsonProperty("commentsForOperators")]
        public string? CommentsForOperators { get; set; }
    }

    public partial class Urls
    {
        [JsonProperty("operator")]
        public string? Operator { get; set; }

        [JsonProperty("ssp")]
        public string? Ssp { get; set; }

        [JsonProperty("open")]
        public string? Open { get; set; }
    }

    public partial class Visibility
    {
        [JsonProperty("sspVisibility")]
        public string? SspVisibility { get; set; }

        [JsonProperty("sspVisibilityFilteredOnBranches")]
        public bool SspVisibilityFilteredOnBranches { get; set; }

        [JsonProperty("operatorVisibilityFilteredOnBranches")]
        public bool OperatorVisibilityFilteredOnBranches { get; set; }

        [JsonProperty("openKnowledgeItem")]
        public bool OpenKnowledgeItem { get; set; }
    }
}
