<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Play.aspx.cs" Inherits="Scoop.PackagePlay" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Responsive iFrame with Floating Div</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        html, body {
            width: 100%;
            height: 100%;
            overflow: hidden;
        }

        .iframe-container {
            position: relative;
            width: 100%;
            height: 100vh;
        }

        iframe {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            border: none;
        }

        .floating-status {
            position: fixed;
            top: 5px;
            background-color: rgba(0, 0, 0, 0.6);
            color: white;
            padding: 5px;
            border-radius: 5px;
            z-index: 1000;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
            font-family: Arial;
            font-size: 0.9rem;
            transition: all 0.3s ease;
            min-width: 40px;
        }

        .floating-status.collapsed {
            width: 48px;
            height: 43px;
            padding: 10px;
            overflow: hidden;
        }
        
        .floating-status.expanded {
            padding: 10px 20px;
            width: auto;
            height: auto;
        }

        .floating-status #content { color: antiquewhite; }

        .floating-status .status-title { color: white; }

        .floating-status button { padding: 2px 5px 2px 5px; font-weight: bold; }

        .floating-status #sessionTime { padding: 0 5px 0 5px; color: white; }

        .toggle-btn {
            cursor: pointer;
            float: right;
            margin-left: 10px;
            font-size: 0.9rem;
            user-select: none;
            transition: transform 0.3s ease;
        }
        
        .floating-status.collapsed .toggle-btn {
            transform: rotate(180deg);
            margin: 10px;
            float: none;
        }
        
        .content {
            display: inline-block;
        }
        
        .floating-status.collapsed .content {
            display: none;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="iframe-container">
            <iframe id="contentFrame" name="contentFrame" class="content-frame" src=""></iframe>
            <div id="floatingStatus" class="floating-status expanded">
                <span class="toggle-btn" id="toggleBtn">◀</span>
                <div class="content" id="content">
                    <asp:Label ID="lblCourseTitle" runat="server" Text="SCORM Course" CssClass="status-title"></asp:Label>
                    (<span id="statusIndicator" class="status-indicator"></span><span id="lessonStatus"></span>)
                    <span id="sessionTime">00:00:00</span>
                    <button type="button" onclick="exitCourse()">Exit</button>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hdnManifestPath" runat="server" />
        <asp:HiddenField ID="hdnOrganizationSlug" runat="server" />
        <asp:HiddenField ID="hdnPackageRoot" runat="server" />
        <asp:HiddenField ID="hdnPackageSlug" runat="server" />
        <asp:HiddenField ID="hdnPackageNumber" runat="server" />
        <asp:HiddenField ID="hdnPackageHref" runat="server" />
        <asp:HiddenField ID="hdnLearnerId" runat="server" />

    </form>

    <script type="text/javascript">
        var appRoot = '<%= ResolveUrl("~/") %>';
    </script>
    <script src="<%= ResolveUrl("~/Learner/ScormAPI.js") %>"></script>

    <script>

        var exitUrl = '<%= ExitUrl %>';
        
        var manifestPath = '<%= hdnManifestPath.Value %>';

        var organizationSlug = '<%= hdnOrganizationSlug.Value %>';

        var packageRoot = '<%= hdnPackageRoot.Value %>';
        var packageSlug = '<%= hdnPackageSlug.Value %>';
        var packageNumber = '<%= hdnPackageNumber.Value %>';
        var packageHref = '<%= hdnPackageHref.Value %>';

        var learnerId = '<%= hdnLearnerId.Value %>';
        
        // Initialize SCORM API
        window.API = new ScormAPI(packageNumber, learnerId);
        
        // Load initial content
        window.onload = function() {
            loadSCO(0);
            startTimer();
        };
        
        var currentSCOIndex = 0;
        // var scoList = []; // This should be populated from manifest
        
        function loadSCO(index) {
            // This should load the appropriate SCO based on the manifest
            var contentPath = appRoot + packageRoot + '/' + organizationSlug + '/' + packageSlug + '/' + packageHref;
            document.getElementById('contentFrame').src = contentPath;
        }
        
        function exitCourse() {
            if (window.API) {
                window.API.LMSFinish('');
            }
            window.top.location.href = exitUrl;
        }
        
        var startTime = new Date();
        function startTimer() {
            setInterval(function() {
                var now = new Date();
                var elapsed = Math.floor((now - startTime) / 1000);
                var hours = Math.floor(elapsed / 3600);
                var minutes = Math.floor((elapsed % 3600) / 60);
                var seconds = elapsed % 60;
                
                var timeStr = 
                    String(hours).padStart(2, '0') + ':' +
                    String(minutes).padStart(2, '0') + ':' +
                    String(seconds).padStart(2, '0');
                    
                document.getElementById('sessionTime').textContent = timeStr;
            }, 1000);
        }

        // Get elements
        const floatingDiv = document.getElementById('floatingStatus');
        const toggleBtn = document.getElementById('toggleBtn');

        // Track state
        let isExpanded = true;

        // Toggle function
        function toggleCollapse() {
            isExpanded = !isExpanded;

            if (isExpanded) {
                floatingDiv.classList.remove('collapsed');
                floatingDiv.classList.add('expanded');
                toggleBtn.innerHTML = '◀';
            } else {
                floatingDiv.classList.remove('expanded');
                floatingDiv.classList.add('collapsed');
                toggleBtn.innerHTML = '▶';
            }
        }

        // Add click event listener
        toggleBtn.addEventListener('click', toggleCollapse);

        // Optional: Add keyboard shortcut (Escape key)
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                toggleCollapse();
            }
        });

    </script>

</body>
</html>
