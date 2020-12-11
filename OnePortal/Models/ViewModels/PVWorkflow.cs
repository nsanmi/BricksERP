using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlow.DAL.Data;

namespace OnePortal.Models.ViewModels
{
    public class PVWorkflow
    {
        public string VendorAccountNumber { set; get; }
        public string VendorBank { set; get; }
        public string AccountName { set; get; }
        public string Purpose { set; get; }
        public decimal Total { set; get; }
        public decimal? ConversionRate { set; get; }
        public string Recipient { set; get; }
        public Guid WorkflowId { set; get; }
        public string title { set; get; }
        public bpm_workflow workflow {set;get;}
    }

    internal enum ProcurementTestProcessTag
    {
        //ToDo the approval steps are almost the samething; consider using one for the switch statement
        
        ProcurementRequest = 1,
        //UnitHeadApproval = 2,
        //DirectorApproval = 3,
        //ProcurementLeadApproval = 4,
        //DfaApproval = 5,
        CountryDirectorApproval = 6,
        //AssignOfficer = 7,
        AssignProcurementOfficer = 7,
        //FirstComplianceReview = 7,
        RequestForQuotation = 8,
        UploadQuotation = 9,
        BidComparism = 10,
        HqApproval = 11,
        PurchaseOrder = 12,
        SecondProcurementLeadApproval = 13,
        ComplianceReview = 14,
        ProgramApproval = 15,
        DfaPoApproval = 16,
        SecondCountryDirectorApproval = 17,
        JobCompletion = 18,
        VendorPaymentRequestForm = 19,
        ComplianceRerviewAndApproval = 20,
        ThirdCountryDirectorApproval = 21,
        Complete = 22
    }

    internal enum NewProcurementProcessTag
    {
        ProcurementRequest = 1,
        CountryDirectorApproval = 6,
        AssignProcurementOfficer = 7,
        RequestForQuotation = 8,
        UploadQuotation = 9,
        BidComparism = 10,
        HqApproval = 11,
        PurchaseOrder = 12,
        SecondProcurementLeadApproval = 13,
        ComplianceReview = 14,
        ProgramApproval = 15,
        DfaPoApproval = 16,
        SecondCountryDirectorApproval = 17,
        JobCompletion = 18,
        VendorPaymentRequestForm = 19,
        ComplianceRerviewAndApproval = 20,
        ThirdCountryDirectorApproval = 21,
        Complete = 22
    }

    internal enum ProcurementAdminTag
    {
        ProcurementRequest = 1,
        CountryDirectorApproval = 5,
        UpdateCost= 6,
        FirstComplianceReview = 7,
        RequestForQuotation = 9,
        UploadQuotation = 10,
        BidComparism = 11,
        HqApproval = 12,
        PurchaseOrder = 13,
        SecondProcurementLeadApproval = 14,
        ComplianceReview = 15,
        ProgramApproval = 16,
        DfaPoApproval = 17,
        SecondCountryDirectorApproval = 18,
        JobCompletion = 19,
        VendorPaymentRequestForm = 20,
        ComplianceRerviewAndApproval = 21,
        ThirdCountryDirectorApproval = 22,
        Complete = 23
    }

    internal enum NewTravelAdvanceProcessTag
    {
        TravelAuthorization = 1,
        UnitHeadApproval = 2,
        TravelAdvance = 3,
        ComplianceReview = 4,
        UnitHeadTravelAdvanceApproval = 5,
        Complete = 6
    }

    internal enum ReimbursementProcessTag
    {
        Authorization = 1,
        UnitHeadApproval = 2,
        Reimbursement = 3,
        ComplianceReview = 4,
        UnitHeadTravelAdvanceApproval = 5,
        CountryDirectorApproval = 6,
        ProgramDirectorApproval = 7,
        Complete = 8
    }

    internal enum AdvanceProcessTag
    {
        AdvanceRequest = 1,
        UnitHeadApproval = 2,
        Advance = 3,
        ComplianceReview = 4,
        UnitHeadTravelAdvanceApproval = 5,
        CountryDirectorApproval = 6,
        ProgramDirectorApproval = 7,
        Complete = 8
    }

    internal enum MailType
    {
        Approval,
        Assignment,
        Attention
    }
}