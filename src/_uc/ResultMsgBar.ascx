<%--<author>Saad Mansour</author>--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ResultMsgBar.ascx.cs" Inherits="_uc_ResultMsgBar" %>

<div id="successMsg" class="row" runat="server" visible="false">
    <div class="col-lg-12">
        <div class="alert alert-dismissable alert-success">
            <button type="button" class="close" data-dismiss="alert">&times;</button>
            <asp:Literal ID="litSuccess" runat="server" Text="<%$ Resources:GeneralMessages, SavedSuccessfully %>"></asp:Literal>
        </div>
    </div>
</div>
<div id="errorMsg" class="row" runat="server" visible="false">
    <div class="col-lg-12">
        <div class="alert alert-dismissable alert-danger">
            <button type="button" class="close" data-dismiss="alert">&times;</button> <%--fileread="currForm.imgTest"--%>
            <asp:Literal ID="litFail" runat="server" Text="<%$ Resources:GeneralMessages, SavedFailed %>"></asp:Literal>
        </div>
    </div>
</div>
