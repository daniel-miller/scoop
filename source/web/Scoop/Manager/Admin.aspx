<%@ Page Language="C#" CodeBehind="Admin.aspx.cs" Inherits="Scoop.AdminPage" MasterPageFile="~/Layout/Default.master" %>

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

            <h1>System Administration</h1>

            <p class="lead text-muted">Internal use only</p>

            <hr />

            <div class="alert bg-danger">
                <h2 class="text-light mt-0">Danger Zone</h2>
                <p class="text-light">
                    Delete everything, including all uploaded packages and all recorded learner progress.
                </p>
                <asp:LinkButton runat="server" ID="PurgeButton" CssClass="btn btn-light">
                    <i class="fas fa-radiation me-2"></i>Purge
                </asp:LinkButton>
            </div>

        </div>
    </section>

</asp:Content>
