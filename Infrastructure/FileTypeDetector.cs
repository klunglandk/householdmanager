namespace HouseholdManager.Infrastructure
{
    public record DetectedFileType(string Extension, string ContentType, bool IsImage);

    public static class FileTypeDetector
    {
        private static readonly DetectedFileType Jpeg = new(".jpg", "image/jpeg", true);
        private static readonly DetectedFileType Png = new(".png", "image/png", true);
        private static readonly DetectedFileType WebP = new(".webp", "image/webp", true);
        private static readonly DetectedFileType Pdf = new(".pdf", "application/pdf", false);

        private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
        private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

        public static DetectedFileType? Detect(ReadOnlySpan<byte> header)
        {
            if (header.StartsWith(JpegSignature)) return Jpeg;
            if (header.StartsWith(PngSignature)) return Png;
            if (header.StartsWith("%PDF-"u8)) return Pdf;
            if (header.Length >= 12 && header[..4].SequenceEqual("RIFF"u8) && header.Slice(8, 4).SequenceEqual("WEBP"u8)) return WebP;
            return null;
        }

        public static string GetContentType(string storedFileName)
        {
            return Path.GetExtension(storedFileName).ToLowerInvariant() switch
            {
                ".jpg" => Jpeg.ContentType,
                ".png" => Png.ContentType,
                ".webp" => WebP.ContentType,
                ".pdf" => Pdf.ContentType,
                _ => "application/octet-stream"
            };
        }
    }
}
