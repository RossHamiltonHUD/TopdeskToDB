namespace TopdeskDataCache
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ChangeList
    {
        [JsonProperty("results")]
        public List<Change> Results { get; set; }

        [JsonProperty("next")]
        public Uri Next { get; set; }
    }

    public partial class Change
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("modifier")]
        public NestedInfo Modifier { get; set; }

        [JsonProperty("submitDate")]
        public string SubmitDate { get; set; }

        [JsonProperty("simple")]
        public Simple Simple { get; set; }

        [JsonProperty("type")]
        public NestedInfo Type { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("requestDate")]
        public string RequestDate { get; set; }

        [JsonProperty("urgent")]
        public bool Urgent { get; set; }

        [JsonProperty("phases")]
        public Phases Phases { get; set; }

        [JsonProperty("requester")]
        public Requester Requester { get; set; }

        [JsonProperty("creator")]
        public NestedInfo Creator { get; set; }

        [JsonProperty("impact")]
        public NestedInfo Impact { get; set; }

        [JsonProperty("changeType")]
        public string ChangeType { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("canceled")]
        public Canceled Canceled { get; set; }

        [JsonProperty("processingStatus")]
        public string ProcessingStatus { get; set; }

        [JsonProperty("subcategory")]
        public NestedInfo Subcategory { get; set; }

        [JsonProperty("category")]
        public NestedInfo Category { get; set; }

        [JsonProperty("briefDescription")]
        public string BriefDescription { get; set; }

        [JsonProperty("status")]
        public NestedInfo Status { get; set; }
    }

    public partial class Canceled
    {
        [JsonProperty("cancelDate")]
        public string CancelDate { get; set; }
    }

    public partial class Phases
    {
        [JsonProperty("evaluation")]
        public Evaluation Evaluation { get; set; }

        [JsonProperty("progress")]
        public Evaluation Progress { get; set; }

        [JsonProperty("prfc")]
        public Rfc Prfc { get; set; }

        [JsonProperty("rfc")]
        public Rfc Rfc { get; set; }
    }

    public partial class Evaluation
    {
        [JsonProperty("authorizer")]
        public Assignee Authorizer { get; set; }

        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        [JsonProperty("noGoDate")]
        public string NoGoDate { get; set; }

        [JsonProperty("plannedEndDate")]
        public string PlannedEndDate { get; set; }
    }

    public partial class Assignee
    {
        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Rfc
    {
        [JsonProperty("authorizer")]
        public Assignee Authorizer { get; set; }

        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        [JsonProperty("rejectedDate")]
        public string RejectedDate { get; set; }

        [JsonProperty("plannedEndDate", NullValueHandling = NullValueHandling.Ignore)]
        public string PlannedEndDate { get; set; }
    }

    public partial class Requester
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("department")]
        public NestedInfo Department { get; set; }
    }

    public partial class Simple
    {
        [JsonProperty("plannedImplementationDate")]
        public string PlannedImplementationDate { get; set; }

        [JsonProperty("implementationDate")]
        public string ImplementationDate { get; set; }

        [JsonProperty("plannedStartDate")]
        public string PlannedStartDate { get; set; }

        [JsonProperty("closedDate")]
        public string ClosedDate { get; set; }

        [JsonProperty("assignee")]
        public Assignee Assignee { get; set; }

        [JsonProperty("startDate")]
        public string StartDate { get; set; }
    }
}
