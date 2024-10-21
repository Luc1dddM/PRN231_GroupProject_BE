using Identity.Application.File.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Identity.Infrastructure.FileUtils.Services
{
    public class FileService : IFileSerivce
    {
        private IWebHostEnvironment environment;
        public FileService(IWebHostEnvironment env)
        {
            environment = env;
        }

        public Tuple<int, string> SaveImage(IFormFile imageFile)
        {
            try
            {
                var contentPath = environment.ContentRootPath;
                // path = "c://projects/productminiapi/uploads" ,not exactly something like that
                var path = Path.Combine(contentPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // Check the allowed extenstions
                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext))
                {
                    string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, msg);
                }
                string uniqueString = Guid.NewGuid().ToString();
                // we are trying to create a unique filename here
                var newFileName = path + "\\" + uniqueString + ext;
                using (var stream = new FileStream(newFileName, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                return new Tuple<int, string>(1, newFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, "Error has occured");
            }
        }

        public async Task DeleteImage(string imageFileName)
        {
            if (File.Exists(imageFileName))
                File.Delete(imageFileName);
        }

    }
}
