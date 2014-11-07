<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="IndexManagement.aspx.cs" Inherits="Admin_IndexManagement" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        /** Columns */
        .rcbHeader ul,
        .rcbFooter ul,
        .rcbItem ul,
        .rcbHovered ul,
        .rcbDisabled ul { margin: 0; padding: 0; width: 100%; display: inline-block; list-style-type: none; }
        .rcbHovered { background: #bbeaf3; }
        .col1,
        .col2,
        .col3 { margin: 0; padding: 0 3px 0 0; width: 110px; line-height: 13px; float: left; }
        .col2 { width: 300px; }
        #indexUpdateDetails { float: left; width: 350px; border-bottom: 1px solid #ccc; padding: 3px 0; }
        .pnlIndexDetails { margin-top: 50px; margin-left: 5px; }
        .help { border: 1px solid #666; background: #bbeaf3; font-size: 12px; padding: 6px; margin: 6px 0; color: #222; }
        .pnlGridFields { border: 2px solid #ccc; padding: 5px; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" runat="Server">
    <%--    <telerik:RadAjaxManager ID="ajaxManager" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdIndex">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdIndex" LoadingPanelID="ajaxLoadingPanel" />                    
                    <telerik:AjaxUpdatedControl ControlID="grdIndexFields" LoadingPanelID="ajaxLoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>--%>
    <telerik:RadAjaxLoadingPanel ID="ajaxLoadingPanel" runat="server">Loading...</telerik:RadAjaxLoadingPanel>
    <h1>Indexes Management</h1>
    <div class="help">
        Indices are necessary for the Check Duplicates function. An index is a routine test that the database will make to check for duplicates. Inside the index you must define which field or fields will be checked for duplicates. If you define more than one field, then it will search for records that have the same values in ALL of the fields you have specified.
    </div>
    Form:
    <telerik:RadComboBox ID="ddlForms" runat="server" DataSourceID="edsForms" DataTextField="name" AutoPostBack="true" EmptyMessage="Select a form" DataValueField="id" Skin="Metro" OnSelectedIndexChanged="ddlForms_SelectedIndexChanged"></telerik:RadComboBox>

    <asp:Panel runat="server" ID="pnlNewIndexHelp" Visible="false">
        <div class="help">
            To start click on <strong>Add New Index</strong>, give it a name, then proceed to select one or more fields that you want checked for duplicates. 
        </div>
    </asp:Panel>

    <telerik:RadGrid ID="grdIndex" runat="server" AllowPaging="True" DataSourceID="edsIndex" Skin="MetroTouch" PageSize="3"
        CellSpacing="0" GridLines="None" ForeColor="#0058B1" BorderColor="White"
        AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White"
        OnItemCreated="grdIndex_ItemCreated" OnDeleteCommand="grdIndex_DeleteCommand" OnInsertCommand="grdIndex_InsertCommand"
        OnItemInserted="grdIndex_ItemInserted" OnPreRender="grdIndex_PreRender" OnSelectedIndexChanged="grdIndex_SelectedIndexChanged">
        <PagerStyle Mode="NextPrevAndNumeric" />
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="IndexID" DataSourceID="edsIndex" CommandItemDisplay="Top">
            <CommandItemSettings AddNewRecordText="Add New Index" />
            <Columns>
                <telerik:GridButtonColumn Text="View Details" CommandName="Select">
                </telerik:GridButtonColumn>
                <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn">
                </telerik:GridEditCommandColumn>
                <telerik:GridBoundColumn DataField="IndexID" DataType="System.Int32" FilterControlAltText="Filter IndexID column" HeaderText="IndexID" ReadOnly="True" SortExpression="IndexID" UniqueName="IndexID" Visible="false">
                    <ColumnValidationSettings>
                        <ModelErrorMessage Text="" />
                    </ColumnValidationSettings>
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IndexName" FilterControlAltText="Filter IndexName column" HeaderText="Name" SortExpression="Name" UniqueName="IndexName">
                    <ColumnValidationSettings>
                        <ModelErrorMessage Text="" />
                    </ColumnValidationSettings>
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="IndexCreateDate" DataType="System.DateTime" FilterControlAltText="Filter IndexCreateDate column" HeaderText="Date" SortExpression="IndexCreateDate" ReadOnly="true" UniqueName="IndexCreateDate">
                    <ColumnValidationSettings>
                        <ModelErrorMessage Text="" />
                    </ColumnValidationSettings>
                </telerik:GridBoundColumn>
                <telerik:GridButtonColumn Text="Delete" CommandName="Delete" ButtonType="ImageButton" />
            </Columns>
            <EditFormSettings>
                <EditColumn ButtonType="ImageButton" />
            </EditFormSettings>
        </MasterTableView>
    </telerik:RadGrid>
    <br />
    <br />
    <asp:Panel runat="server" ID="pnlGridFields" Visible="false" CssClass="pnlGridFields">
    <telerik:RadGrid ID="grdIndexFields" runat="server" Skin="Metro" ForeColor="#0058B1" BorderColor="White"
        AlternatingItemStyle-BackColor="#CCE6FF" HeaderStyle-BackColor="#0058B1" HeaderStyle-ForeColor="White"
        OnInsertCommand="grdIndexFields_InsertCommand" AutoGenerateColumns="False" OnItemDataBound="grdIndexFields_ItemDataBound"
        OnUpdateCommand="grdIndexFields_UpdateCommand" AllowPaging="True" DataSourceID="edsIndexFields" PageSize="5">
        <PagerStyle Mode="NextPrevAndNumeric" />
        <ExportSettings>
            <Pdf PageWidth="">
            </Pdf>
        </ExportSettings>
        <AlternatingItemStyle BackColor="#CCE6FF"></AlternatingItemStyle>
        <MasterTableView DataKeyNames="IndexFieldID" CommandItemDisplay="Top" TableLayout="Fixed" DataSourceID="edsIndexFields">
            <CommandItemSettings AddNewRecordText="Add New Field" />
            <Columns>
                <telerik:GridBoundColumn DataField="IndexFieldID" DataType="System.Int32" FilterControlAltText="Filter IndexFieldID column" HeaderText="ID" ReadOnly="True" SortExpression="IndexFieldID" UniqueName="IndexFieldID" Visible="false">
                    <ColumnValidationSettings>
                        <ModelErrorMessage Text="" />
                    </ColumnValidationSettings>
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="FormFieldID" DataType="System.Int32" FilterControlAltText="Filter FormFieldID column" HeaderText="Field Name" SortExpression="FormFieldID" UniqueName="FormFieldID">
                    <EditItemTemplate>
                        <telerik:RadComboBox ID="ddlFormFields" runat="server" DataTextField="name" EmptyMessage="Select a form field"
                            DataValueField="id" Skin="Metro" ForeColor="#222" HighlightTemplatedItems="true" DropDownWidth="600px">
                            <HeaderTemplate>
                                <ul>
                                    <li class="col1">Name</li>
                                    <li class="col2">Label</li>
                                    <li class="col3">Type</li>
                                </ul>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <ul>
                                    <li class="col1">
                                        <%# Eval("name") %></li>
                                    <li class="col2">
                                        <%# Eval("label") %></li>
                                    <li class="col3">
                                        <%# Eval("type") %></li>
                                </ul>
                            </ItemTemplate>
                        </telerik:RadComboBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblFormFieldName" runat="server" Text='<%# Eval("FormField.name") %>'></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn DataField="FormField.label" FilterControlAltText="Filter FormField.label column" HeaderText="Field Label" ReadOnly="true" UniqueName="label">
                    <ItemTemplate>
                        <asp:Label ID="lblFormFieldLabel" runat="server" Text='<%# Eval("FormField.label") %>'></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn Text="Delete" CommandName="Delete" ButtonType="ImageButton" />
            </Columns>
            <EditFormSettings>
                <EditColumn ButtonType="ImageButton" />
            </EditFormSettings>
        </MasterTableView>

        <HeaderStyle BackColor="#0058B1" ForeColor="White"></HeaderStyle>
    </telerik:RadGrid>
    </asp:Panel>


    <asp:Panel ID="pnlIndexDetails" runat="server" CssClass="pnlIndexDetails">
        <div class="help">
            Click on Generate Index. This may take a few seconds depending by the amount of data to index and CPU power.
From now on the index will be automatically updated with any new data that arrives.
Later, when you use the Check for Duplicates function, you must select the index that was generated for the fields that you are checking.
        </div>
        <div id="indexUpdateDetails">
            <asp:Literal ID="litLastUpdate" runat="server"></asp:Literal></div>
        <telerik:RadButton ID="btnGenerateHASH" runat="server" SingleClick="true"
            SingleClickText="Generating Index... Please Wait" Text="GENERATE INDEXES" Skin="MetroTouch" OnClick="btnGenerateHASH_Click">
        </telerik:RadButton>
    </asp:Panel>

    <telerik:RadToolTip runat="server" ID="rttFormSelect" Width="200px" ShowEvent="onmouseover"
        RelativeTo="Element" Animation="Fade" TargetControlID="ddlForms" IsClientID="false"
        HideEvent="LeaveTargetAndToolTip" Position="BottomRight" Text="Select a form on which you want to build an index...">
    </telerik:RadToolTip>


    <asp:EntityDataSource ID="edsIndexFields" runat="server" AutoGenerateWhereClause="true" Include="FormField" ConnectionString="name=GRASPEntities" DefaultContainerName="GRASPEntities" EnableDelete="True" EnableFlattening="False" EnableInsert="True" EnableUpdate="True" EntitySetName="IndexFields"></asp:EntityDataSource>
    <asp:EntityDataSource ID="edsForms" runat="server" ConnectionString="name=GRASPEntities" DefaultContainerName="GRASPEntities" EnableFlattening="False" EntitySetName="Form" EntityTypeFilter="Form" Select="it.[name], it.[id]" Where="it.[isHidden]=0 AND it.[finalised]=1">
    </asp:EntityDataSource>
    <asp:EntityDataSource ID="edsIndex" runat="server" ConnectionString="name=GRASPEntities" DefaultContainerName="GRASPEntities"
        EnableDelete="True" EnableFlattening="False" EnableInsert="True" EnableUpdate="True" EntitySetName="Indexes" Where="it.[formID] == @id">
        <WhereParameters>
            <asp:ControlParameter DbType="Int32" Name="id" ControlID="ddlForms"
                PropertyName="SelectedValue" />
        </WhereParameters>
    </asp:EntityDataSource>
</asp:Content>

