using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TopdeskDataCache
{
    public partial class Ticket
    {
        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("callerLocation")]
        public NestedInfo CallerLocation { get; set; }

        [JsonProperty("category")]
        public NestedInfo Category { get; set; }

        [JsonProperty("subcategory")]
        public NestedInfo Subcategory { get; set; }

        [JsonProperty("callType")]
        public NestedInfo CallType { get; set; }

        [JsonProperty("entryType")]
        public NestedInfo EntryType { get; set; }

        [JsonProperty("impact")]
        public NestedInfo Impact { get; set; }

        [JsonProperty("urgency")]
        public NestedInfo Urgency { get; set; }

        [JsonProperty("priority")]
        public NestedInfo Priority { get; set; }

        [JsonProperty("duration")]
        public NestedInfo Duration { get; set; }

        [JsonProperty("actualDuration")]
        public string ActualDuration { get; set; }

        [JsonProperty("targetDate")]
        public string TargetDate { get; set; }

        [JsonProperty("feedbackRating")]
        public string FeedbackRating { get; set; }

        [JsonProperty("operator")]
        public NestedInfo Operator { get; set; }

        [JsonProperty("operatorGroup")]
        public NestedInfo OperatorGroup { get; set; }

        [JsonProperty("supplier")]
        public NestedInfo Supplier { get; set; }

        [JsonProperty("processingStatus")]
        public NestedInfo ProcessingStatus { get; set; }

        [JsonProperty("responseDate")]
        public string ResponseDate { get; set; }

        [JsonProperty("completed")]
        public string Completed { get; set; }

        [JsonProperty("completedDate")]
        public string CompletedDate { get; set; }

        [JsonProperty("closed")]
        public string Closed { get; set; }

        [JsonProperty("closedDate")]
        public string ClosedDate { get; set; }

        [JsonProperty("closureCode")]
        public NestedInfo ClosureCode { get; set; }

        [JsonProperty("callDate")]
        public string CallDate { get; set; }

        [JsonProperty("creator")]
        public NestedInfo Creator { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("modifier")]
        public NestedInfo Modifier { get; set; }

        [JsonProperty("modificationDate")]
        public string ModificationDate { get; set; }

        [JsonProperty("optionalFields1")]
        public OptionalFields Escalation { get; set; }
    }

    public partial class NestedInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Caller
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public partial class OptionalFields
    {
        [JsonProperty("searchlist1")]
        public NestedInfo EscType { get; set; }
    }
}
