<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Read.aspx.cs" Inherits="Scoop.Read" MasterPageFile="~/Layout/Default.master" %>

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
            <div class="article mb-5">
                <asp:Literal runat="server" ID="ArticleBody" />
            </div>
        </div>
    </section>

</asp:Content>