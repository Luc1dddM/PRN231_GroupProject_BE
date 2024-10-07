namespace Catalog.API.Repository
{
    public interface IUploadImageRepository
    {
        public void UploadFile(IFormFile file, string Id);
    }
}
