# SCO Open Platform

Scoop is a lightweight web application that supports SCORM 1.2 compliant e-learning packages. Scoop provides a simple yet effective platform for hosting, managing, and tracking SCORM-based training and education content.

## Features

- **SCORM 1.2 Compliance**: Supports the SCORM 1.2 specification
- **Multi-Organization Support**: Host content for multiple organizations with separate access control
- **Package Management**: Upload, organize, and manage SCORM content packages
- **Progress Tracking**: Real-time tracking of learner progress and scores
- **Reporting**: Comprehensive reports on learner performance and course completion
- **Secure Authentication**: Cookie-based authentication with encryption support
- **Responsive Design**: Works across desktop and mobile devices

## Technology Stack

- **Framework**: ASP.NET Web Forms (.NET Framework 4.8)
- **Language**: C#
- **Database**: SQL Server
- **Front-end**: HTML5, CSS3, JavaScript
- **Dependencies**:
  - Markdig (Markdown processing)
  - Newtonsoft.Json (JSON serialization)
  - System.IO.Compression (ZIP file handling)

## Project Structure

```
scoop/
├── build/                    # Build scripts and configuration
│   ├── Build.ps1            # PowerShell build script
│   ├── Scoop.proj          # MSBuild project file
│   └── Scripts/            # Database scripts
├── source/
│   └── web/
│       └── Scoop/          # Main application
│           ├── Internal/  # Core business logic
│           ├── Layout/    # Master pages and templates
│           ├── Learner/   # Learner-facing pages
│           └── Manager/   # Management pages
└── LICENSE                 # MIT License
```

## Installation

### Prerequisites

- Windows Server with IIS 8.0 or later
- .NET Framework 4.8
- SQL Server 2016 or later
- Visual Studio 2019 or later (for development)

### Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone https://github.com/daniel-miller/scoop.git
   cd scoop
   ```

2. **Database Setup**
   - Create a new SQL Server database
   - Run the database initialization scripts from `build/Scripts/`
   - Update connection string in `Web.config`

3. **Configure Application Settings**
   - Copy `appsettings.local.config.example` to `appsettings.local.config`
   - Update the following settings:
     ```xml
     <add key="Scoop:ConnectionString" value="Your Connection String" />
     <add key="Scoop:CookieName" value="Your Cookie Name" />
     <add key="Scoop:CookieSecret" value="Your Secret Key" />
     <add key="Scoop:LoginUrl" value="Your Login URL" />
     ```

4. **Build the Application**
   ```powershell
   .\build\Build.ps1
   ```

5. **Deploy to IIS**
   - Create a new IIS website pointing to the `source/web/Scoop` directory
   - Ensure the application pool is set to .NET Framework 4.8
   - Set appropriate permissions for the `tmp/sco` directory (read/write)

## Usage

### For Administrators/Managers

1. **Access the Admin Panel**: Navigate to `/admin`
2. **Upload SCORM Packages**: 
   - Click "Upload Package"
   - Select a SCORM 1.2 compliant ZIP file
   - The system will automatically extract and parse the manifest
3. **View Reports**: Access detailed reports at `/{organization}/{package}/report`
4. **Manage Learners**: Track learner progress and completion status

### For Learners

1. **Access Course Library**: Navigate to `/{organization}`
2. **Launch a Course**: Click on any available course to start learning
3. **Track Progress**: The system automatically saves progress as you navigate through the course
4. **View Completion Status**: See your scores and completion status in real-time

## API Endpoints

### SCORM Progress API
- **Load Progress**: `GET /sco-progress?action=load&packageNumber={id}&learnerId={id}`
- **Save Progress**: `POST /sco-progress?action=save&packageNumber={id}&learnerId={id}`

## URL Structure

The application uses URL rewriting for clean URLs:

- **Home**: `/`
- **Organization Library**: `/{organization}`
- **Play Course**: `/{organization}/{package}`
- **View Report**: `/{organization}/{package}/report`
- **Admin Panel**: `/admin`

## SCORM Implementation

Scoop implements the SCORM 1.2 Runtime Environment, supporting:

- `LMSInitialize()` - Initialize communication
- `LMSFinish()` - Terminate communication
- `LMSGetValue()` - Retrieve data model values
- `LMSSetValue()` - Set data model values
- `LMSCommit()` - Persist data to the server
- `LMSGetLastError()` - Get the last error code
- `LMSGetErrorString()` - Get error description
- `LMSGetDiagnostic()` - Get diagnostic information

### Supported Data Model Elements

- Core elements (lesson_status, score, location, etc.)
- Student data (name, id, preferences)
- Objectives tracking
- Interactions tracking
- Suspend data for resuming sessions

## Security Features

- Encrypted authentication cookies
- SQL injection prevention
- XSS protection
- Configurable IP whitelist
- Secure file upload validation

## Configuration

Key configuration options in `Web.config`:

```xml
<appSettings>
  <add key="Scoop:PackageFolder" value="tmp/sco" />
  <add key="Scoop:CookieEncryption" value="Enabled|Disabled" />
  <add key="Scoop:Whitelist" value="IP addresses" />
</appSettings>
```

## Development

### Building from Source

1. Open `source/web/Scoop.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution (F6)
4. Run with IIS Express (F5)

### Database Scripts

Located in `build/Scripts/`:
- `Archive.sql` - Archive old data
- `Insert Product Category.sql` - Initial data setup
- `Count All Rows.sql` - Database statistics

## Troubleshooting

### Common Issues

1. **Package Upload Fails**
   - Ensure the `tmp/sco` directory has write permissions
   - Verify the ZIP file is SCORM 1.2 compliant
   - Check the manifest file exists at the root level

2. **Progress Not Saving**
   - Verify database connectivity
   - Check browser console for JavaScript errors
   - Ensure cookies are enabled

3. **Courses Not Launching**
   - Verify the launch file specified in the manifest exists
   - Check for cross-origin issues if hosting content externally
   - Ensure JavaScript is enabled in the browser

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Daniel Miller

## Support

For issues and questions:
- Create an issue on [GitHub](https://github.com/daniel-miller/scoop/issues)
- Check existing documentation and issues first

## Acknowledgments

- SCORM 1.2 specification by ADL
- The e-learning community for standards and best practices
- Contributors and testers who helped improve the platform