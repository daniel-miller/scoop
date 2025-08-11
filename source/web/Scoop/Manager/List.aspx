<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Scoop.PackageList" MasterPageFile="~/Layout/Default.master" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentHead">

    <style>

        .upload-section {
            background: white;
            border-radius: 10px;
            padding: 30px;
            margin-bottom: 30px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
        }

        .upload-area {
            border: 2px dashed #ddd;
            border-radius: 8px;
            padding: 30px;
            text-align: center;
            transition: all 0.3s;
        }

            .upload-area:hover {
                border-color: #667eea;
                background: #f8f9ff;
            }

        .feature-card:hover {
            background: #F8FAFC;
        }

        .feature-card {
            display: flex;
            flex-direction: column;
            height: 100%; /* Make sure the card takes full height of its container */
            min-height: 200px; /* Optional: set a minimum height */
        }

        .course-actions {
            margin-top: auto; /* This pushes the actions to the bottom */
            padding-top: 20px; /* Optional: add some spacing */
        }

    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="ContentBody">

    <!-- Hero Section -->
    <section class="hero-section" id="home">
        <div class="container">
            <div class="row align-items-center hero-content">
                <div class="col-lg-6">
                    <h1 runat="server" id="LibraryHeading" class="hero-title mb-0">Organization</h1>
                    <div class="hero-title mt-0 pt-0">SCO Library</div>
                    <p class="hero-subtitle">Scoop is an affordable, open-source alternative to SCORM Cloud for managing and delivering training content.</p>
                    <div class="d-flex gap-3">
                        <a runat="server" id="LearnLink" href="#" class="btn btn-outline-light btn-lg px-4">Learn More</a>
                    </div>
                </div>
                <div class="col-lg-6">

                    <div class="upload-section">
                        <div class="mb-2">Select a SCORM 1.2 package on your computer:</div>
                        <div class="mb-2 upload-area">
                            <asp:FileUpload ID="fileUpload" runat="server" CssClass="file-input" accept=".zip" />
                            <asp:Label ID="lblUploadStatus" runat="server"></asp:Label>
                        </div>
                        <asp:Button ID="btnUpload" runat="server" Text="Add package to library" OnClick="btnUpload_Click" CssClass="upload-btn btn btn-lg btn-secondary" Style="width: 100%;" />
                    </div>


                </div>
            </div>
        </div>
    </section>

    <!-- Packages section -->
    <section class="py-5 my-5" id="packages">

        <div class="container">

            <div class="row g-4">

                <asp:Repeater ID="rptCourses" runat="server">
                    <ItemTemplate>

                        <div class="col-lg-4">
                            <div class="feature-card">
                                <div class="text-muted pb-2" style="font-size: 0.8rem"><%# Eval("PackageSlug") %></div>
                                <h4 class="mb-3"><%# Eval("Title") %></h4>
                                <p class="text-muted"><%# Eval("Description") %></p>
                                <div class="course-actions">
                                    <asp:HyperLink ID="btnLaunch" runat="server" Text="Launch"
                                        CssClass="btn btn-success"><i class="fas fa-rocket me-2"></i>Launch</asp:HyperLink>
                                    <asp:HyperLink ID="btnReport" runat="server" Text="Report"
                                        CssClass="btn btn-light"><i class="fas fa-magnifying-glass me-2"></i>Report</asp:HyperLink>
                                    <asp:LinkButton ID="btnDelete" runat="server" Text="Delete"
                                        CommandArgument='<%# Eval("PackageSlug") %>'
                                        OnClick="btnDelete_Click" CssClass="btn btn-danger"
                                        OnClientClick="return confirm('Are you sure you want to delete this package?');"><i class="fas fa-trash"></i></asp:LinkButton>
                                </div>

                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </div>

        </div>

    </section>

    <!-- Stats Section -->
    <section class="stats-section">
        <div class="container">
            <div class="row">
                <div class="col-md-3 col-6">
                    <div class="stat-card">
                        <div class="stat-number">
                            <asp:Literal runat="server" ID="PackageCount" Text="0" />
                        </div>
                        <div class="stat-label">Packages</div>
                    </div>
                </div>
                <div class="col-md-3 col-6">
                    <div class="stat-card">
                        <div class="stat-number">
                            <asp:Literal runat="server" ID="LearnerCount" Text="0" />
                        </div>
                        <div class="stat-label">Learners</div>
                    </div>
                </div>
                <div class="col-md-3 col-6">
                    <div class="stat-card">
                        <div class="stat-number">
                            <asp:Literal runat="server" ID="RegistrationCount" Text="0" />
                        </div>
                        <div class="stat-label">Registrations</div>
                    </div>
                </div>
                <div class="col-md-3 col-6">
                    <div class="stat-card">
                        <div class="stat-number-static">99.9%</div>
                        <div class="stat-label">Uptime</div>
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
