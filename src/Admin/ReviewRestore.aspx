﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReviewRestore.aspx.cs" Inherits="Admin_ReviewRestore" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body { font-family: Arial; font-size:12px;}
        #msg { margin: 5px 0; padding: 5px; border: 1px solid #ccc;background:#fafafa; }
    </style>
</head>
<body>
    <form id="form1" runat="server">        
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
    <div>
        <asp:HiddenField ID="hdnFilterCount" runat="server" />
        <asp:Panel ID="filterSummary" runat="server" Visible="false" CssClass="filterSummary">
            <asp:Label ID="lblFilterSummary" runat="server" Text=""></asp:Label>
        </asp:Panel>
    </div>
    <div>
        <h2>Review Process Restore</h2>
        <div>
        <label>Username:</label>
        <telerik:RadComboBox ID="DdlUsers" runat="server" DataTextField="username" EmptyMessage="Select a User"
            DataValueField="user_id" Skin="Metro" ForeColor="#222" HighlightTemplatedItems="true" DropDownWidth="500px" Width="250px">
            <HeaderTemplate>
                <ul>
                    <li class="col1">User Name</li>
                    <li class="col2">Role</li>
                </ul>
            </HeaderTemplate>
            <ItemTemplate>
                <ul>
                    <li class="col1">
                        <%# Eval("username") %></li>
                    <li class="col2">
                        <%# Eval("supervisor") %></li>
                </ul>
            </ItemTemplate>
        </telerik:RadComboBox>

        <label>Review Status</label>
        <telerik:RadComboBox ID="DdlReviewStatus" runat="server" EnableLoadOnDemand="True" Skin="Metro" AppendDataBoundItems="true" BorderColor="#66AFE9" BackColor="White" DataSourceID="edsResponseStatus" DataTextField="ResponseStatusName" DataValueField="ResponseStatusID">
            <Items>
                <telerik:RadComboBoxItem Text="Any" Value="0" />
            </Items>
        </telerik:RadComboBox>
            </div>
        <div>
            <br />
        <label>Review Date</label>
        <telerik:RadComboBox ID="DdlDate" runat="server" Skin="Metro" OnSelectedIndexChanged="ddlDate_SelectedIndexChanged" AutoPostBack="true"></telerik:RadComboBox>
        <telerik:RadDatePicker ID="RdpStartDate" runat="server" Width="140px" AutoPostBack="true"
            DateInput-EmptyMessage="Select a start date" MinDate="01/01/1000" MaxDate="01/01/3000">
            <Calendar>
                <SpecialDays>
                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday">
                    </telerik:RadCalendarDay>
                </SpecialDays>
            </Calendar>
        </telerik:RadDatePicker>
        <telerik:RadDatePicker ID="RdpEndDate" runat="server" Width="140px" AutoPostBack="true"
            DateInput-EmptyMessage="Select a start date" MinDate="01/01/1000" MaxDate="01/01/3000">
            <Calendar>
                <SpecialDays>
                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday">
                    </telerik:RadCalendarDay>
                </SpecialDays>
            </Calendar>
        </telerik:RadDatePicker>
        </div>
    </div>

    <hr />
    <telerik:RadButton ID="BtnTest" runat="server" SingleClick="true" SingleClickText="Verifying... Please Wait" Text=" Verify " OnClick="BtnTest_Click"></telerik:RadButton>
     &nbsp;&nbsp;&nbsp;
        <telerik:RadButton ID="BtnRestoreReviews" runat="server" Enabled="false" SingleClick="true" SingleClickText="Restoring... Please Wait" Text=" Restore " OnClick="BtnRestoreReviews_Click"></telerik:RadButton>
   
        <div id="msg"><asp:Literal ID="litResult" runat="server"></asp:Literal></div>        


    <asp:EntityDataSource ID="edsResponseStatus" runat="server" ConnectionString="name=GRASPEntities" DefaultContainerName="GRASPEntities" EnableFlattening="False" EntitySetName="FormResponseStatus" Select="it.[ResponseStatusName], it.[ResponseStatusID]">
    </asp:EntityDataSource>

    </form>
</body>
</html>
