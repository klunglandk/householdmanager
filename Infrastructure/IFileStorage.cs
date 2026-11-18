namespace HouseholdManager.Infrastructure
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(Stream content, AllowedFileKind allowedKind, CancellationToken cancellationToken);

        Stream? Open(string storedFileName);

        void Delete(string storedFileName);
    }
}
