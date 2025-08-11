<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Scoop.HomePage" MasterPageFile="~/Layout/Default.master" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentBody">

    <!-- Hero Section -->
    <section class="hero-section" id="home">
        <div class="container">
            <div class="row align-items-center hero-content">
                <div class="col-lg-8">
                    <h1 class="hero-title">SCO Open Platform</h1>
                    <p class="hero-subtitle">Scoop is an affordable, open-source alternative to SCORM Cloud for managing and delivering training content.</p>
                    <div class="d-flex gap-3">
                        <asp:HyperLink runat="server" ID="DemoLink" CssClass="btn btn-light btn-lg px-4">
                            <i class="fas fa-play me-2"></i>Watch demo
                        </asp:HyperLink>
                        <asp:HyperLink runat="server" ID="MoreLink" CssClass="btn btn-outline-light btn-lg px-4">Learn more</asp:HyperLink>
                    </div>
                </div>
                <div class="col-lg-4">
                    <!-- Placeholder for hero image or illustration -->
                </div>
            </div>
        </div>
    </section>

    <!-- Features Section -->
    <section class="py-5 my-5" id="features">
        <div class="container">
            <div class="text-center mb-5">
                <h2 class="display-5 fw-bold mb-3">Features</h2>
                <p class="lead text-muted">Everything you need to manage and deliver training with Shareable Content Objects</p>
            </div>
            <div class="row g-4">
                <div class="col-lg-4">
                    <div class="feature-card">
                        <div class="feature-icon icon-primary">
                            <i class="fas fa-bolt"></i>
                        </div>
                        <h4 class="mb-3">Test and deliver e-learning</h4>
                        <p class="text-muted">
                            Scoop is an affordable, easy-to-use solution for delivering SCORM content and tracking learner progress. Compatible with SCORM 1.2, access the Scoop web platform directly for centralized learning management, or seamlessly integrate Scoop's capabilities into your own applications and learning mangement systems.</p>
                    </div>
                </div>
                <div class="col-lg-4">
                    <div class="feature-card">
                        <div class="feature-icon icon-success">
                            <i class="fas fa-rocket"></i>
                        </div>
                        <h4 class="mb-3">Launch SCORM from any LMS</h4>
                        <p class="text-muted">
                            Scoop enables you to deliver SCORM content to your learners from any learning management system - while maintaining centralized hosting in one place. With Scoop, you can publish updated versions of your content to multiple platforms simultaneously, revoke learner access when licenses expire, and aggregate usage analytics from all your learning platforms.
                        </p>
                    </div>
                </div>
                <div class="col-lg-4">
                    <div class="feature-card">
                        <div class="feature-icon icon-info">
                            <i class="fas fa-chart-line"></i>
                        </div>
                        <h4 class="mb-3">Report on learner progress</h4>
                        <p class="text-muted">
                          Scoop provides built-in reporting features that enable you to monitor course completion rates and examine detailed question-level analytics. Filter reports to show individual course or learner performance, summarize results per learning program, or run aggregate analysis across your entire learning library. Export the data for advanced analysis using your own external tools.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- Stats Section -->
    <section class="stats-section">
        <div class="container">
            <div class="row">
                <div class="col-md-3 col-6">
                    <div class="stat-card">
                        <div class="stat-number"><asp:Literal runat="server" ID="PackageCount" /></div>
                        <div class="stat-label">Packages</div>
                    </div>
                </div>
                <div class="col-md-3 col-6">
                    <div class="stat-card">
                        <div class="stat-number"><asp:Literal runat="server" ID="LearnerCount" /></div>
                        <div class="stat-label">Learners</div>
                    </div>
                </div>
                <div class="col-md-3 col-6">
                    <div class="stat-card">
                        <div class="stat-number"><asp:Literal runat="server" ID="EventCount" />K+</div>
                        <div class="stat-label">Learning Interactions</div>
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

    <!-- CTA Section -->
    <section class="cta-section text-center">
        <div class="container">
            <h2 class="display-5 fw-bold mb-4">Ready to get started?</h2>
            <p class="lead mb-4">Join the many great teams using our platform</p>
            <button class="btn btn-light btn-lg px-5 py-3">
                <i class="fas fa-play me-2"></i>Start a free trial
            </button>
        </div>
    </section>


</asp:Content>

