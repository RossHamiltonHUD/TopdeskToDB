namespace TopdeskDataCache.schema
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Action
    {
        [JsonProperty("entryDate")]
        public string EntryDate { get; set; }

        [JsonProperty("operator")]
        public NestedInfo Operator { get; set; }
    }
}
