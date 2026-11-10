using System.ComponentModel.DataAnnotations;

namespace HouseholdManager.Infrastructure
{
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";

        public string RootPath { get; set; } = "uploads";

        [Range(1, int.MaxValue)]
        public int MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
    }
}
