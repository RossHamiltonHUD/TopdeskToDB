using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TopdeskToDB
{
    public partial class InputTicket
    {
        //Fields for unpacking NestedInfo
        public string CallerBranch;
        public string CallerLocation;
        public string Category;
        public string Subcategory;
        public string CallType;
        public string EntryType;
        public string Impact;
        public string Urgency;
        public string Priority;
        public string Duration;
        public string Operator;
        public string OperatorGroup;
        public string Supplier;
        public string ProcessingStatus;
        public string ClosureCode;
        public string Creator;
        public string Modifier;
        public string Escalation;

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("callerBranch")]
        public NestedInfo CallerBranchNest { get; set; }

        [JsonProperty("callerLocation")]
        public NestedInfo CallerLocationNest { get; set; }

        [JsonProperty("category")]
        public NestedInfo CategoryNest { get; set; }

        [JsonProperty("subcategory")]
        public NestedInfo SubcategoryNest { get; set; }

        [JsonProperty("callType")]
        public NestedInfo CallTypeNest { get; set; }

        [JsonProperty("entryType")]
        public NestedInfo EntryTypeNest { get; set; }

        [JsonProperty("impact")]
        public NestedInfo ImpactNest { get; set; }

        [JsonProperty("urgency")]
        public NestedInfo UrgencyNest { get; set; }

        [JsonProperty("priority")]
        public NestedInfo PriorityNest { get; set; }

        [JsonProperty("duration")]
        public NestedInfo DurationNest { get; set; }

        [JsonProperty("actualDuration")]
        public string ActualDuration { get; set; }

        [JsonProperty("targetDate")]
        public string TargetDate { get; set; }

        [JsonProperty("feedbackRating")]
        public string FeedbackRating { get; set; }

        [JsonProperty("operator")]
        public NestedInfo OperatorNest { get; set; }

        [JsonProperty("operatorGroup")]
        public NestedInfo OperatorGroupNest { get; set; }

        [JsonProperty("supplier")]
        public NestedInfo SupplierNest { get; set; }

        [JsonProperty("processingStatus")]
        public NestedInfo ProcessingStatusNest { get; set; }

        [JsonProperty("responded")]
        public string Responded { get; set; }

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
        public NestedInfo ClosureCodeNest { get; set; }

        [JsonProperty("callDate")]
        public string CallDate { get; set; }

        [JsonProperty("creator")]
        public NestedInfo CreatorNest { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("modifier")]
        public NestedInfo ModifierNest { get; set; }

        [JsonProperty("modificationDate")]
        public string ModificationDate { get; set; }

        [JsonProperty("majorCall")]
        public string MajorCall { get; set; }

        [JsonProperty("optionalFields1")]
        public OptionalFields EscalationNest { get; set; }  
    }

    public partial class NestedInfo
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Caller
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("dynamicName")]
        public string DynamicName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public partial class OptionalFields
    {
        [JsonProperty("searchlist1")]
        public NestedInfo EscType { get; set; }
    }
}
