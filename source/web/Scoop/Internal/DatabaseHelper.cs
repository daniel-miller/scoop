using System.Data.SqlClient;
using System.IO;

namespace Scoop
{
    public class DatabaseHelper
    {
        private static bool IsInitialized = false;

        public string ConnectionString { get; private set; }

        public DatabaseHelper()
        {
            ConnectionString = AppSettings.ConnectionString;

            if (!IsInitialized)
                Initialize();
        }

        public int ExecuteCount(string sql)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public void ExecuteScript(string sql)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Reset()
            => CreateTables();

        private void Initialize()
        {
            var query = @"
SELECT COUNT(*)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'scoop'
";

            if (ExecuteCount(query) != 4)
                CreateTables();

            IsInitialized = true;
        }

        private void CreateTables()
        {
            var script = @"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'scoop')
BEGIN
    EXEC('CREATE SCHEMA scoop AUTHORIZATION dbo');
END

DROP TABLE IF EXISTS scoop.ScoEvent;

DROP TABLE IF EXISTS scoop.ScoLearner;

DROP TABLE IF EXISTS scoop.ScoPackage;

DROP TABLE IF EXISTS scoop.ScoProgress;

CREATE TABLE scoop.ScoEvent (
    EventNumber   INT              NOT NULL IDENTITY PRIMARY KEY,
    EventTime     DATETIMEOFFSET   NOT NULL,
    PackageNumber INT              NOT NULL,
    LearnerId     UNIQUEIDENTIFIER NOT NULL,
    ProgressKey   VARCHAR(200)     NOT NULL,
    ProgressValue VARCHAR(MAX)
);

CREATE TABLE scoop.ScoLearner (
    LearnerNumber     INT              NOT NULL IDENTITY PRIMARY KEY,
    LearnerId         UNIQUEIDENTIFIER NOT NULL,
    LearnerEmail      VARCHAR(200)     NOT NULL,
    LastAuthenticated DATETIMEOFFSET
);

CREATE TABLE scoop.ScoPackage (
    OrganizationSlug   VARCHAR(50)     NOT NULL,
    PackageSlug        VARCHAR(50)     NOT NULL,
    PackageNumber      INT IDENTITY    NOT NULL PRIMARY KEY,
    PackageTitle       VARCHAR(200)    NOT NULL,
    PackageDescription VARCHAR(MAX),
    ScormVersion       VARCHAR(10)     NOT NULL,
    PackageUploaded    DATETIMEOFFSET  NOT NULL,
    PackageUploadedBy  VARCHAR(200)    NOT NULL,
    PackageIsActive    BIT DEFAULT (1) NOT NULL,
    CONSTRAINT UQ_ScoPackage_Key UNIQUE (OrganizationSlug, PackageSlug)
);

CREATE TABLE scoop.ScoProgress (
    ProgressNumber   INT              NOT NULL IDENTITY PRIMARY KEY,
    PackageNumber    NVARCHAR(50)     NOT NULL,
    LearnerId        UNIQUEIDENTIFIER NOT NULL,
    ProgressKey      VARCHAR(200)     NOT NULL,
    ProgressValue    VARCHAR(MAX),
    ProgressModified DATETIMEOFFSET   NOT NULL,
    CONSTRAINT UQ_ScormData_Key UNIQUE (PackageNumber, LearnerId, ProgressKey)
);
";

            ExecuteScript(script);

            var path = AppSettings.GetPhysicalPath(AppSettings.PackageFolder);
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
    }
}