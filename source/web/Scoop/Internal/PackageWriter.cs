using System;
using System.Data.SqlClient;

namespace Scoop
{
    public class PackageWriter
    {
        public void SavePackageInfo(CourseInfo courseInfo, string managerEmail)
        {
            var db = new DatabaseHelper();

            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                var query = @"
INSERT INTO scoop.ScoPackage (OrganizationSlug, PackageSlug, PackageTitle, PackageDescription, ScormVersion,
                              PackageUploaded, PackageUploadedBy, PackageIsActive, PackageSizeInKB)
VALUES (@OrganizationSlug, @PackageSlug, @PackageTitle, @PackageDescription, @ScormVersion, GETUTCDATE(),
        @PackageUploadedBy, 1, @PackageSizeInKB)
";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@OrganizationSlug", courseInfo.OrganizationSlug);

                    cmd.Parameters.AddWithValue("@PackageSizeInKB", courseInfo.PackageSizeInKB);
                    cmd.Parameters.AddWithValue("@PackageSlug", courseInfo.PackageSlug);
                    cmd.Parameters.AddWithValue("@PackageTitle", courseInfo.Title ?? "Untitled Course");
                    cmd.Parameters.AddWithValue("@PackageDescription", courseInfo.Description ?? "(No description available)");
                    cmd.Parameters.AddWithValue("@PackageUploadedBy", managerEmail);

                    cmd.Parameters.AddWithValue("@ScormVersion", courseInfo.Version ?? "1.2");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(string organization, string package, string managerEmail)
        {
            var db = new DatabaseHelper();

            using (SqlConnection connection = new SqlConnection(db.ConnectionString))
            {
                connection.Open();

                var query = @"UPDATE scoop.ScoPackage 
SET PackageIsActive = 0,
    PackageDeleted = GETUTCDATE(),
    PackageDeletedBy = @PackageDeletedBy,
    PackageSlug = PackageSlug + @DeleteTimestamp
WHERE OrganizationSlug = @OrganizationSlug AND PackageSlug = @PackageSlug";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@OrganizationSlug", organization);

                    cmd.Parameters.AddWithValue("@PackageSlug", package);

                    cmd.Parameters.AddWithValue("@PackageDeletedBy", managerEmail);

                    cmd.Parameters.AddWithValue("@DeleteTimestamp", $"_DELETED_{DateTimeOffset.Now:O}");

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}