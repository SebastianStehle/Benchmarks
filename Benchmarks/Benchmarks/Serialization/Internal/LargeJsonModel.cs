// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

#pragma warning disable MA0048 // File name must match type name

using System.Text.Json.Serialization;

namespace Benchmarks.Serialization.Internal;

public class AuditReportClassification : BaseAuditReport
{
    public Dictionary<string, List<string>> CustomerHandbooks { get; set; }

    public Dictionary<string, List<HandbookSection>> HandbookSections { get; set; }
}

public class BaseAuditReport
{
    public Dictionary<string, string> AuditReportId { get; set; }

    public Dictionary<string, string> ViperGuid { get; set; }

    public Dictionary<string, string> AssessmentId { get; set; }

    public Dictionary<string, string> AssessmentName { get; set; }

    public Dictionary<string, string> CompanyId { get; set; }

    public Dictionary<string, string> CompanyName { get; set; }

    public Dictionary<string, ReportStatus> Status { get; set; }
}

public class HandbookSection
{
    public bool? CustomerReviewCompleted { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? Matched { get; set; }

    public ClassificationPolicyType PolicyType { get; set; }

    public DateTime? LastUpdated { get; set; }

    public int Index { get; set; }

    public List<string> CustomerSelectedEHBPolicyId { get; set; }

    public List<string> CustomerSelectedPolicyCategory { get; set; }

    public List<string> EHBPolicyId { get; set; }

    public List<string> HandbookId { get; set; }

    public string Detail { get; set; }

    public string HandbookName { get; set; }

    public string LastUpdatedBy { get; set; }

    public string SectionId { get; set; }

    public string Title { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReportStatus
{
    Submitted,
    Complete,
    InProgress
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClassificationPolicyType
{
    Recommended,
    Required,
    RequiredInHandbook,
    Multiple,
    NotApplicable
}
