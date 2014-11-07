<%@ Page Title="" Language="C#" MasterPageFile="~/_master/AdminBlank.master" AutoEventWireup="true" CodeFile="CompareResponses.aspx.cs" Inherits="Admin_CompareResponses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
#result table { border-collapse:collapse;border:1px solid #333; }
#result table th { white-space: nowrap; background:#0058B1;color:#fff;text-align:center;border-bottom:2px solid #666;}
#result table td { white-space: nowrap; border:1px solid #ccc; }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" Runat="Server">

    <div id="result">
    <asp:Literal ID="litTable" runat="server" EnableViewState="false"></asp:Literal>
</div>
</asp:Content>

