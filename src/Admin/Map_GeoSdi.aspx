<%@ Page Title="" Language="C#" MasterPageFile="~/_master/Admin.master" AutoEventWireup="true" CodeFile="Map_GeoSdi.aspx.cs" Inherits="Admin_Map_GeoSdi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .ifrmMap { width: 100%; height: 1200px; border: none; margin-top:100px;}

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody1" Runat="Server">

    <iframe class="ifrmMap" src="https://grasp-mapping.wfppal.org/maplite/?mapID=3173-2050&x=34.921141316276355&y=31.960704222695195&zoom=10"></iframe>
    
</asp:Content>

