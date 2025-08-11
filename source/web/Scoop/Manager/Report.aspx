<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="Scoop.ReportPage" MasterPageFile="~/Layout/Default.master" %>


<asp:Content runat="server" ContentPlaceHolderID="ContentHead">

    <style>

        h1 { font-size: 3rem; font-weight: 700 !important; margin-bottom: 1rem !important; }
        h2 { margin-top: 3rem; }
        h3 { margin-top: 2rem; }
        p, ul li { font-size: 1.2rem; color: #333A44; }

    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="ContentBody">

    <section class="py-5 my-5">
        <div class="container">

            <h1>Report</h1>

            <p class="lead text-muted">Internal use only</p>

            <hr />

            <textarea runat="server" id="output" rows="40" cols="120">

</textarea>

        </div>
    </section>

</asp:Content>
