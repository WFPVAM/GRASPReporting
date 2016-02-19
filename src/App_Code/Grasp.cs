﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

public partial class BindingContainer
{
    public BindingContainer()
    {
        this.FormFieldAndBinding = new HashSet<FormFieldAndBinding>();
    }

    public decimal id { get; set; }
    public string bType { get; set; }
    public Nullable<int> maxRange { get; set; }
    public Nullable<int> minRange { get; set; }
    public Nullable<byte> pushed { get; set; }
    public string value { get; set; }

    public virtual ICollection<FormFieldAndBinding> FormFieldAndBinding { get; set; }
}

public partial class BindingRules
{
    public decimal FormField_id { get; set; }
    public decimal id { get; set; }
    public string bType { get; set; }
    public string value { get; set; }
    public string label { get; set; }
    public string name { get; set; }
    public int positionIndex { get; set; }
    public string type { get; set; }
    public string x_form { get; set; }
    public Nullable<decimal> form_id { get; set; }
    public string bindingsPolicy { get; set; }
    public decimal Expr1 { get; set; }
    public Nullable<byte> calculated { get; set; }
    public string formula { get; set; }
    public Nullable<byte> required { get; set; }
}

public partial class ConstraintContainer
{
    public ConstraintContainer()
    {
        this.FormField_ConstraintContainer = new HashSet<FormField_ConstraintContainer>();
    }

    public decimal id { get; set; }
    public string cType { get; set; }
    public int maxRange { get; set; }
    public int minRange { get; set; }
    public Nullable<byte> pushed { get; set; }
    public string value { get; set; }
    public string cNumber { get; set; }

    public virtual ICollection<FormField_ConstraintContainer> FormField_ConstraintContainer { get; set; }
}

public partial class Contact
{
    public Contact()
    {
        this.GroupMembership = new HashSet<GroupMembership>();
    }

    public decimal contact_id { get; set; }
    public byte active { get; set; }
    public string emailAddress { get; set; }
    public string name { get; set; }
    public string notes { get; set; }
    public string otherPhoneNumber { get; set; }
    public string phoneNumber { get; set; }
    public Nullable<byte> pushed { get; set; }

    public virtual ICollection<GroupMembership> GroupMembership { get; set; }
}

public partial class DataTransfer
{
    public int id { get; set; }
    public string dbname { get; set; }
    public string dbpassword { get; set; }
    public string dbusername { get; set; }
    public Nullable<byte> manual { get; set; }
    public Nullable<byte> scheduled { get; set; }
    public Nullable<System.DateTime> scheduledtime { get; set; }
    public string servername { get; set; }
    public string serverport { get; set; }
    public Nullable<byte> sharable { get; set; }
}

public partial class Detail_Form_Response
{
    public decimal id { get; set; }
    public string name { get; set; }
}

public partial class Email
{
    public decimal id { get; set; }
    public string content { get; set; }
    public Nullable<decimal> date { get; set; }
    public string recipients { get; set; }
    public Nullable<int> status { get; set; }
    public string subject { get; set; }
    public Nullable<decimal> sender_id { get; set; }

    public virtual EmailAccount EmailAccount { get; set; }
}

public partial class EmailAccount
{
    public EmailAccount()
    {
        this.Email = new HashSet<Email>();
        this.keywordaction = new HashSet<keywordaction>();
        this.reminder = new HashSet<reminder>();
    }

    public decimal id { get; set; }
    public string accountName { get; set; }
    public string accountPassword { get; set; }
    public string accountServer { get; set; }
    public Nullable<int> accountServerPort { get; set; }
    public Nullable<byte> enabled { get; set; }
    public Nullable<byte> isForReceiving { get; set; }
    public Nullable<decimal> lastCheck { get; set; }
    public string protocol { get; set; }
    public byte useSsl { get; set; }

    public virtual ICollection<Email> Email { get; set; }
    public virtual ICollection<keywordaction> keywordaction { get; set; }
    public virtual ICollection<reminder> reminder { get; set; }
}

public partial class Form
{
    public Form()
    {
        this.IsDeleted = false;
        this.FormResponse = new HashSet<FormResponse>();
        this.Survey = new HashSet<Survey>();
        this.FormField = new HashSet<FormField>();
        this.FormField1 = new HashSet<FormField>();
    }

    public decimal id { get; set; }
    public string Code_Form { get; set; }
    public string bindingsPolicy { get; set; }
    public string designerVersion { get; set; }
    public byte finalised { get; set; }
    public string id_flsmsId { get; set; }
    public string name { get; set; }
    public string owner { get; set; }
    public byte pushed { get; set; }
    public string permittedGroup_path { get; set; }
    public Nullable<System.DateTime> FormCreateDate { get; set; }
    public Nullable<int> isHidden { get; set; }
    public bool IsDeleted { get; set; }
    public Nullable<System.DateTime> DeletedDate { get; set; }
    public Nullable<byte> FormVersion { get; set; }
    public string PreviousPublishedName { get; set; }
    public string PreviousPublishedID { get; set; }

    public virtual frontline_group frontline_group { get; set; }
    public virtual ICollection<FormResponse> FormResponse { get; set; }
    public virtual ICollection<Survey> Survey { get; set; }
    public virtual ICollection<FormField> FormField { get; set; }
    public virtual ICollection<FormField> FormField1 { get; set; }
}

public partial class FormField
{
    public FormField()
    {
        this.FormField_FormFieldAndBinding = new HashSet<FormField_FormFieldAndBinding>();
        this.FormField_ConstraintContainer = new HashSet<FormField_ConstraintContainer>();
        this.FormField_FormField = new HashSet<FormField_FormField>();
        this.FormField_FormField1 = new HashSet<FormField_FormField>();
        this.FormFieldAndBinding = new HashSet<FormFieldAndBinding>();
        this.Form1 = new HashSet<Form>();
        this.FormResponse = new HashSet<FormResponse>();
        this.IndexFields = new HashSet<IndexField>();
    }

    public decimal id { get; set; }
    public string id_flsmsId { get; set; }
    public string label { get; set; }
    public string name { get; set; }
    public int positionIndex { get; set; }
    public Nullable<byte> pushed { get; set; }
    public Nullable<byte> required { get; set; }
    public string type { get; set; }
    public string x_form { get; set; }
    public Nullable<decimal> form_id { get; set; }
    public Nullable<decimal> survey_id { get; set; }
    public string bindingsPolicy { get; set; }
    public Nullable<byte> calculated { get; set; }
    public string constraintPolicy { get; set; }
    public string formula { get; set; }
    public Nullable<byte> isReadOnly { get; set; }
    public Nullable<int> numberOfRep { get; set; }
    public Nullable<System.DateTime> FFCreateDate { get; set; }

    public virtual Form Form { get; set; }
    public virtual ICollection<FormField_FormFieldAndBinding> FormField_FormFieldAndBinding { get; set; }
    public virtual ICollection<FormField_ConstraintContainer> FormField_ConstraintContainer { get; set; }
    public virtual ICollection<FormField_FormField> FormField_FormField { get; set; }
    public virtual ICollection<FormField_FormField> FormField_FormField1 { get; set; }
    public virtual ICollection<FormFieldAndBinding> FormFieldAndBinding { get; set; }
    public virtual Survey Survey { get; set; }
    public virtual ICollection<Form> Form1 { get; set; }
    public virtual ICollection<FormResponse> FormResponse { get; set; }
    public virtual ICollection<IndexField> IndexFields { get; set; }
}

public partial class FormField_ConstraintContainer
{
    public decimal FormField_id { get; set; }
    public decimal constraints_id { get; set; }
    public int id { get; set; }

    public virtual ConstraintContainer ConstraintContainer { get; set; }
    public virtual FormField FormField { get; set; }
}

public partial class FormField_FormField
{
    public decimal FormField_id { get; set; }
    public decimal repetableFormFields_id { get; set; }
    public int id { get; set; }

    public virtual FormField FormField { get; set; }
    public virtual FormField FormField1 { get; set; }
}

public partial class FormField_FormFieldAndBinding
{
    public decimal FormField_id { get; set; }
    public decimal bindingCouples_id { get; set; }
    public int id { get; set; }

    public virtual FormField FormField { get; set; }
    public virtual FormFieldAndBinding FormFieldAndBinding { get; set; }
}

public partial class FormFieldAndBinding
{
    public FormFieldAndBinding()
    {
        this.FormField_FormFieldAndBinding = new HashSet<FormField_FormFieldAndBinding>();
    }

    public decimal id { get; set; }
    public Nullable<byte> pushed { get; set; }
    public Nullable<decimal> bContainer_id { get; set; }
    public Nullable<decimal> fField_id { get; set; }

    public virtual BindingContainer BindingContainer { get; set; }
    public virtual FormField FormField { get; set; }
    public virtual ICollection<FormField_FormFieldAndBinding> FormField_FormFieldAndBinding { get; set; }
}

public partial class FormFieldExport
{
    public decimal id { get; set; }
    public string label { get; set; }
    public string name { get; set; }
    public int positionIndex { get; set; }
    public Nullable<byte> required { get; set; }
    public string type { get; set; }
    public string x_form { get; set; }
    public Nullable<decimal> form_id { get; set; }
    public Nullable<decimal> survey_id { get; set; }
    public Nullable<byte> isReadOnly { get; set; }
    public Nullable<int> numberOfRep { get; set; }
    public Nullable<System.DateTime> FFCreateDate { get; set; }
    public Nullable<decimal> FormFieldParentID { get; set; }
    public Nullable<byte> calculated { get; set; }
    public string formula { get; set; }
}

public partial class FormFieldExt
{
    public int FormFieldExtID { get; set; }
    public decimal FormID { get; set; }
    public string FormFieldExtFormula { get; set; }
    public string FormFieldExtName { get; set; }
    public string FormFieldExtLabel { get; set; }
    public Nullable<int> PositionIndex { get; set; }
    public Nullable<decimal> FormFieldID { get; set; }
}

public partial class FormFieldExtDependencies
{
    public int FFEDID { get; set; }
    public decimal FormFieldID { get; set; }
    public int FormFieldExtID { get; set; }
    public Nullable<double> FormFieldDefaultValue { get; set; }
}

public partial class FormFieldResponses
{
    public decimal id { get; set; }
    public string value { get; set; }
    public Nullable<int> RVRepeatCount { get; set; }
    public Nullable<int> FormResponseID { get; set; }
    public Nullable<System.DateTime> RVCreateDate { get; set; }
    public Nullable<int> formFieldId { get; set; }
    public Nullable<decimal> parentForm_id { get; set; }
    public string label { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public Nullable<decimal> survey_id { get; set; }
    public Nullable<int> positionIndex { get; set; }
    public string senderMsisdn { get; set; }
    public Nullable<System.DateTime> FRCreateDate { get; set; }
    public int ResponseStatusID { get; set; }
    public Nullable<double> nvalue { get; set; }
    public Nullable<System.DateTime> dvalue { get; set; }
}

public partial class FormFieldResponsesReviews
{
    public decimal id { get; set; }
    public string value { get; set; }
    public Nullable<int> RVRepeatCount { get; set; }
    public Nullable<int> FormResponseID { get; set; }
    public Nullable<int> formFieldId { get; set; }
    public Nullable<decimal> parentForm_id { get; set; }
    public string label { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public Nullable<decimal> survey_id { get; set; }
    public int positionIndex { get; set; }
    public string senderMsisdn { get; set; }
    public Nullable<System.DateTime> FRCreateDate { get; set; }
    public int ResponseStatusID { get; set; }
    public Nullable<double> nvalue { get; set; }
    public Nullable<System.DateTime> dvalue { get; set; }
}

public partial class FormResponse
{
    public FormResponse()
    {
        this.FormField = new HashSet<FormField>();
        this.ResponseValue = new HashSet<ResponseValue>();
    }

    public decimal id { get; set; }
    public string clientVersion { get; set; }
    public string id_flsmsId { get; set; }
    public byte pushed { get; set; }
    public string senderMsisdn { get; set; }
    public Nullable<decimal> parentForm_id { get; set; }
    public string Code_Form { get; set; }
    public Nullable<byte> fromDataEntry { get; set; }
    public Nullable<System.DateTime> FRCreateDate { get; set; }
    public int ResponseStatusID { get; set; }
    public Nullable<System.DateTime> LastUpdatedDate { get; set; }

    public virtual Form Form { get; set; }
    public virtual ICollection<FormField> FormField { get; set; }
    public virtual ICollection<ResponseValue> ResponseValue { get; set; }
}

public partial class FormResponseCoords
{
    public int FRCoordID { get; set; }
    public Nullable<int> FormResponseID { get; set; }
    public string FRCoordText { get; set; }
    public System.Data.Entity.Spatial.DbGeography FRCoordGeo { get; set; }
    public string frXCoordText { get; set; }
    public System.Data.Entity.Spatial.DbGeometry FRCoordGeometry { get; set; }
}

public partial class FormResponseCoords_ResponseValue
{
    public decimal FormResponseCoords_FRCoordID { get; set; }
    public decimal formResponses_id { get; set; }

    public virtual ResponseValue ResponseValue { get; set; }
}

public partial class FormResponseReviews
{
    public int FormResponseReviewID { get; set; }
    public decimal FormResponseID { get; set; }
    public string FRRUserName { get; set; }
    public System.DateTime FormResponseReviewDate { get; set; }
    public int FormResponsePreviousStatusID { get; set; }
    public int FormResponseCurrentStatusID { get; set; }
    public string FormResponseReviewDetail { get; set; }
    public Nullable<int> FormResponseReviewSeqNo { get; set; }
}

public partial class FormResponseServerStatus
{
    public decimal Id { get; set; }
    public string InstanceUniqueIdentifier { get; set; }
    public bool IsSavedToServer { get; set; }
}

public partial class FormResponseStatus
{
    public int ResponseStatusID { get; set; }
    public string ResponseStatusName { get; set; }
    public Nullable<int> ResponseStatusIndex { get; set; }
    public Nullable<int> ResponseStatusDependency { get; set; }
}

public partial class frontline_group
{
    public frontline_group()
    {
        this.Form = new HashSet<Form>();
        this.keywordaction = new HashSet<keywordaction>();
        this.GroupMembership = new HashSet<GroupMembership>();
    }

    public string path { get; set; }
    public string id_flsmsId { get; set; }
    public string parentPath { get; set; }
    public byte pushed { get; set; }

    public virtual ICollection<Form> Form { get; set; }
    public virtual ICollection<keywordaction> keywordaction { get; set; }
    public virtual ICollection<GroupMembership> GroupMembership { get; set; }
}

public partial class FrontlineMultimediaMessagePart
{
    public FrontlineMultimediaMessagePart()
    {
        this.message = new HashSet<message>();
    }

    public decimal id { get; set; }
    public Nullable<byte> binaryData { get; set; }
    public string content { get; set; }

    public virtual ICollection<message> message { get; set; }
}

public partial class GroupMembership
{
    public decimal id { get; set; }
    public Nullable<byte> pushed { get; set; }
    public decimal contact_contact_id { get; set; }
    public string group_path { get; set; }

    public virtual Contact Contact { get; set; }
    public virtual frontline_group frontline_group { get; set; }
}

public partial class Index
{
    public int IndexID { get; set; }
    public string IndexName { get; set; }
    public Nullable<System.DateTime> IndexCreateDate { get; set; }
    public int formID { get; set; }
    public Nullable<System.DateTime> IndexLastUpdateDate { get; set; }
    public string IndexLastUpdateUserName { get; set; }
    public string IndexAlgorithm { get; set; }
}

public partial class IndexField
{
    public int IndexFieldID { get; set; }
    public int IndexID { get; set; }
    public decimal FormFieldID { get; set; }

    public virtual FormField FormField { get; set; }
    public virtual IndexField IndexFields1 { get; set; }
    public virtual IndexField IndexField1 { get; set; }
}

public partial class IndexHASH
{
    public int IndexHASHID { get; set; }
    public string IndexHASHString { get; set; }
    public int IndexID { get; set; }
    public int FormResponseID { get; set; }
}

public partial class Keyword
{
    public Keyword()
    {
        this.keywordaction = new HashSet<keywordaction>();
    }

    public decimal id { get; set; }
    public string description { get; set; }
    public string keyword1 { get; set; }

    public virtual ICollection<keywordaction> keywordaction { get; set; }
}

public partial class keywordaction
{
    public decimal id { get; set; }
    public decimal commandInteger { get; set; }
    public string commandString { get; set; }
    public decimal counter { get; set; }
    public string emailRecipients { get; set; }
    public string emailSubject { get; set; }
    public decimal endDate { get; set; }
    public string externalCommand { get; set; }
    public Nullable<decimal> externalCommandResponseActionType { get; set; }
    public Nullable<decimal> externalCommandResponseType { get; set; }
    public Nullable<decimal> externalCommandType { get; set; }
    public decimal startDate { get; set; }
    public Nullable<decimal> type { get; set; }
    public Nullable<decimal> emailAccount_id { get; set; }
    public string group_path { get; set; }
    public decimal keyword_id { get; set; }

    public virtual EmailAccount EmailAccount { get; set; }
    public virtual frontline_group frontline_group { get; set; }
    public virtual Keyword Keyword { get; set; }
}

public partial class message
{
    public message()
    {
        this.FrontlineMultimediaMessagePart = new HashSet<FrontlineMultimediaMessagePart>();
    }

    public decimal id { get; set; }
    public byte[] binaryMessageContent { get; set; }
    public decimal date { get; set; }
    public string dtype { get; set; }
    public string recipientMsisdn { get; set; }
    public int recipientSmsPort { get; set; }
    public int retriesRemaining { get; set; }
    public string senderMsisdn { get; set; }
    public int smsPartsCount { get; set; }
    public Nullable<int> smscReference { get; set; }
    public Nullable<int> status { get; set; }
    public string textContent { get; set; }
    public Nullable<int> type { get; set; }
    public string subject { get; set; }

    public virtual ICollection<FrontlineMultimediaMessagePart> FrontlineMultimediaMessagePart { get; set; }
}

public partial class Permissions
{
    public Permissions()
    {
        this.RolePermission = new HashSet<Role_Permissions>();
    }

    public int id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public virtual ICollection<Role_Permissions> RolePermission { get; set; }
}

public partial class reminder
{
    public string occurrence { get; set; }
    public decimal id { get; set; }
    public string content { get; set; }
    public Nullable<decimal> enddate { get; set; }
    public string recipients { get; set; }
    public Nullable<decimal> startdate { get; set; }
    public Nullable<int> status { get; set; }
    public string subject { get; set; }
    public Nullable<int> type { get; set; }
    public Nullable<decimal> emailAccount_id { get; set; }

    public virtual EmailAccount EmailAccount { get; set; }
}

public partial class Report
{
    public int ReportID { get; set; }
    public string ReportName { get; set; }
    public string ReportDescription { get; set; }
    public decimal FormID { get; set; }
    public Nullable<System.DateTime> ReportCreateDate { get; set; }
    public string Filters { get; set; }
    public string FiltersSummary { get; set; }
    public Nullable<int> FiltersCount { get; set; }
}

public partial class ReportField
{
    public int ReportFieldID { get; set; }
    public int ReportID { get; set; }
    public string ReportFieldLabel { get; set; }
    public decimal FormFieldID { get; set; }
    public Nullable<System.DateTime> ReportFieldCreateDate { get; set; }
    public string ChartType { get; set; }
    public string ReportFieldValueLabel { get; set; }
    public Nullable<decimal> ValueFormFieldID { get; set; }
    public string ReportFieldAggregate { get; set; }
    public Nullable<int> ReportFieldLegend { get; set; }
    public Nullable<int> ReportFieldTableData { get; set; }
    public string ReportFieldTitle { get; set; }
    public string ReportFieldNote { get; set; }
    public Nullable<int> ReportFieldOrder { get; set; }
}

public partial class ResponseDetails
{
    public decimal id { get; set; }
    public Nullable<System.DateTime> FRCreateDate { get; set; }
    public Nullable<byte> fromDataEntry { get; set; }
    public Nullable<decimal> parentForm_id { get; set; }
    public string Date { get; set; }
    public string enumerator { get; set; }
}

public partial class ResponseMapping
{
    public string FormName { get; set; }
    public Nullable<decimal> FormID { get; set; }
    public decimal ResponseID { get; set; }
    public Nullable<System.DateTime> FRCreateDate { get; set; }
    public System.Data.Entity.Spatial.DbGeometry FRCoordGeometry { get; set; }
}

public partial class ResponseRepeatable
{
    public decimal id { get; set; }
    public string value { get; set; }
    public Nullable<int> FormResponseID { get; set; }
    public Nullable<int> RVRepeatCount { get; set; }
    public Nullable<int> formFieldId { get; set; }
    public Nullable<decimal> ParentFormFieldID { get; set; }
    public Nullable<decimal> parentForm_id { get; set; }
    public string label { get; set; }
    public string name { get; set; }
    public Nullable<decimal> survey_id { get; set; }
    public Nullable<int> positionIndex { get; set; }
    public Nullable<int> SurveyElementIndex { get; set; }
    public string type { get; set; }
    public int ResponseStatusID { get; set; }
    public Nullable<double> nvalue { get; set; }
    public Nullable<System.DateTime> dvalue { get; set; }
}

public partial class ResponseRepeatableReviews
{
    public decimal id { get; set; }
    public string value { get; set; }
    public Nullable<int> FormResponseID { get; set; }
    public Nullable<int> RVRepeatCount { get; set; }
    public Nullable<int> formFieldId { get; set; }
    public Nullable<decimal> ParentFormFieldID { get; set; }
    public Nullable<decimal> parentForm_id { get; set; }
    public string label { get; set; }
    public string name { get; set; }
    public Nullable<decimal> survey_id { get; set; }
    public int positionIndex { get; set; }
    public Nullable<int> SurveyElementIndex { get; set; }
    public string type { get; set; }
    public int ResponseStatusID { get; set; }
    public Nullable<double> nvalue { get; set; }
    public Nullable<System.DateTime> dvalue { get; set; }
}

public partial class ResponseValue
{
    public ResponseValue()
    {
        this.FormResponseCoords_ResponseValue = new HashSet<FormResponseCoords_ResponseValue>();
        this.FormResponse = new HashSet<FormResponse>();
        this.ResponseValue1 = new HashSet<ResponseValue>();
        this.ResponseValue2 = new HashSet<ResponseValue>();
    }

    public decimal id { get; set; }
    public string id_flsmsId { get; set; }
    public byte pushed { get; set; }
    public string value { get; set; }
    public Nullable<int> RVRepeatCount { get; set; }
    public Nullable<int> FormResponseID { get; set; }
    public Nullable<System.DateTime> RVCreateDate { get; set; }
    public Nullable<int> formFieldId { get; set; }
    public Nullable<int> positionIndex { get; set; }
    public Nullable<double> nvalue { get; set; }
    public Nullable<System.DateTime> dvalue { get; set; }

    public virtual ICollection<FormResponseCoords_ResponseValue> FormResponseCoords_ResponseValue { get; set; }
    public virtual ICollection<FormResponse> FormResponse { get; set; }
    public virtual ICollection<ResponseValue> ResponseValue1 { get; set; }
    public virtual ICollection<ResponseValue> ResponseValue2 { get; set; }
}

public partial class ResponseValueExt
{
    public int RespValueExtID { get; set; }
    public int FormResponseID { get; set; }
    public int FormFieldExtID { get; set; }
    public Nullable<double> nvalue { get; set; }
    public Nullable<int> FormFieldID { get; set; }
    public Nullable<int> PositionIndex { get; set; }
}

public partial class ResponseValueReviews
{
    public decimal id { get; set; }
    public string value { get; set; }
    public Nullable<int> RVRepeatCount { get; set; }
    public Nullable<int> FormResponseID { get; set; }
    public Nullable<int> formFieldId { get; set; }
    public Nullable<int> positionIndex { get; set; }
    public Nullable<double> nvalue { get; set; }
    public Nullable<System.DateTime> dvalue { get; set; }
    public Nullable<int> FormResponseReviewID { get; set; }
}

public partial class Role_Permissions
{
    public int id { get; set; }
    public int RoleID { get; set; }
    public int PermissionID { get; set; }

    public virtual Roles Roles { get; set; }
    public virtual Permissions Permissions { get; set; }
}

public partial class Roles
{
    public Roles()
    {
        this.User_Credential = new HashSet<User_Credential>();
        this.RolePermission = new HashSet<Role_Permissions>();
    }

    public int id { get; set; }
    public string description { get; set; }

    public virtual ICollection<User_Credential> User_Credential { get; set; }
    public virtual ICollection<Role_Permissions> RolePermission { get; set; }
}

public partial class RolesToResponseStatus
{
    public int RoleID { get; set; }
    public int ResponseStatusID { get; set; }
    public int RoleToRespStatusID { get; set; }
    public int RoleToRespStatusTypeID { get; set; }
}

public partial class SmsInternetServiceSettings
{
    public SmsInternetServiceSettings()
    {
        this.SmsInternetServiceSettings_SmsInternetServiceSettingValue = new HashSet<SmsInternetServiceSettings_SmsInternetServiceSettingValue>();
    }

    public decimal id { get; set; }
    public string serviceClassName { get; set; }

    public virtual ICollection<SmsInternetServiceSettings_SmsInternetServiceSettingValue> SmsInternetServiceSettings_SmsInternetServiceSettingValue { get; set; }
}

public partial class SmsInternetServiceSettings_SmsInternetServiceSettingValue
{
    public decimal SmsInternetServiceSettings_id { get; set; }
    public decimal properties_id { get; set; }
    public string mapkey { get; set; }

    public virtual SmsInternetServiceSettings SmsInternetServiceSettings { get; set; }
    public virtual SmsInternetServiceSettingValue SmsInternetServiceSettingValue { get; set; }
}

public partial class SmsInternetServiceSettingValue
{
    public SmsInternetServiceSettingValue()
    {
        this.SmsInternetServiceSettings_SmsInternetServiceSettingValue = new HashSet<SmsInternetServiceSettings_SmsInternetServiceSettingValue>();
    }

    public decimal id { get; set; }
    public string value { get; set; }

    public virtual ICollection<SmsInternetServiceSettings_SmsInternetServiceSettingValue> SmsInternetServiceSettings_SmsInternetServiceSettingValue { get; set; }
}

public partial class SmsModemSettings
{
    public decimal id { get; set; }
    public byte deleteMessagesAfterReceiving { get; set; }
    public string manufacturer { get; set; }
    public string model { get; set; }
    public string serial { get; set; }
    public string simPin { get; set; }
    public string smscNumber { get; set; }
    public Nullable<byte> supportingReceive { get; set; }
    public byte useDeliveryReports { get; set; }
    public byte useForReceiving { get; set; }
    public byte useForSending { get; set; }
}

public partial class StatisticsTable
{
    public decimal id { get; set; }
    public Nullable<decimal> chartType { get; set; }
    public Nullable<decimal> form_id { get; set; }
    public string query { get; set; }
    public string title { get; set; }
}

public partial class Survey
{
    public Survey()
    {
        this.FormField = new HashSet<FormField>();
        this.Survey_SurveyElement = new HashSet<Survey_SurveyElement>();
    }

    public decimal id { get; set; }
    public string name { get; set; }
    public Nullable<decimal> owner_id { get; set; }

    public virtual Form Form { get; set; }
    public virtual ICollection<FormField> FormField { get; set; }
    public virtual ICollection<Survey_SurveyElement> Survey_SurveyElement { get; set; }
}

public partial class Survey_SurveyElement
{
    public decimal Survey_id { get; set; }
    public decimal values_id { get; set; }
    public int id { get; set; }

    public virtual Survey Survey { get; set; }
    public virtual SurveyElement SurveyElement { get; set; }
}

public partial class SurveyElement
{
    public SurveyElement()
    {
        this.Survey_SurveyElement = new HashSet<Survey_SurveyElement>();
    }

    public decimal id { get; set; }
    public Nullable<int> position { get; set; }
    public string survey { get; set; }
    public Nullable<int> positionIndex { get; set; }
    public string value { get; set; }
    public byte defaultValue { get; set; }

    public virtual ICollection<Survey_SurveyElement> Survey_SurveyElement { get; set; }
}

public partial class SurveyListAPI
{
    public decimal id { get; set; }
    public string name { get; set; }
    public string value { get; set; }
    public Nullable<int> positionIndex { get; set; }
}

public partial class User_Credential
{
    public int user_id { get; set; }
    public string email { get; set; }
    public string frontlinesms_id { get; set; }
    public string name { get; set; }
    public string password { get; set; }
    public string phone_number { get; set; }
    public Nullable<byte> pushed { get; set; }
    public string supervisor { get; set; }
    public string surname { get; set; }
    public string username { get; set; }
    public Nullable<int> roles_id { get; set; }
    public Nullable<System.DateTime> UserDeleteDate { get; set; }
    public string UserResponseFilter { get; set; }

    public virtual Roles Roles { get; set; }
}

public partial class UserFilters
{
    public int UserFilterID { get; set; }
    public int userID { get; set; }
    public string UserFilterString { get; set; }
    public decimal formID { get; set; }
    public System.DateTime UserFilterCreateDate { get; set; }
    public int UserFilterIsEnabled { get; set; }
    public string UserFilterDescription { get; set; }
}

public partial class UserToFormResponses
{
    public int UserToFormResponsesID { get; set; }
    public int userID { get; set; }
    public decimal formResponseID { get; set; }
    public decimal formID { get; set; }
}
