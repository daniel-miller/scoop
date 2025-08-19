using System;

namespace Scoop
{
    public class PlatformStatistics
    {
        public int EventCount { get; set; }
        public int LearnerCount { get; set; }
        public int PackageCount { get; set; }
        public int RegistrationCount { get; set; }
    }

    public class OrganizationStatistics
    {
        public int EventCount { get; set; }
        public int LearnerCount { get; set; }
        public int PackageCount { get; set; }
        public int RegistrationCount { get; set; }
    }

    public class CourseInfo
    {
        public int PackageNumber { get; set; }

        public string OrganizationSlug { get; set; }
        public string PackageSlug { get; set; }
        public int PackageSizeInKB { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public string Score { get; set; }
        public string Duration { get; set; }
        public int Attempts { get; set; }

        public string PackageSize
        {
            get
            {
                if (PackageSizeInKB < 1000)
                    return $"{PackageSizeInKB:n0} KB";

                return $"{PackageSizeInKB / 1000:n0} MB";
            }
        }
    }

    public class RegistrationInfo
    {
        public int PackageNumber { get; set; }
        public string OrganizationSlug { get; set; }
        public Guid LearnerId { get; set; }
    }

    public class StatInfo
    {
        public int PackageCount { get; set; }
        public int LearnerCount { get; set; }
        public int RegistrationCount { get; set; }
    }
}