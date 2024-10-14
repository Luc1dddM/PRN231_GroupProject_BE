using System.Text;

namespace Catalog.API.Repository
{
    public class UploadImageRepository: IUploadImageRepository
    {
        private const string ApiBaseUrl = "https://api.github.com";
        private const string repo = "tesst";
        private const string owner = "Hvuthai";

        public async void UploadFile(IFormFile file, string Id)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            IConfiguration configuration = configurationBuilder.AddUserSecrets<UploadImageRepository>().Build();
            string githubToken = configuration.GetSection("github")["accessToken"];
            Id = Id.Contains(".jpg") ? Id : Id + ".jpg";

            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"token {githubToken}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ImageUploaderApp");  // Tên app của bạn

            // Đọc file từ IFormFile và chuyển sang Base64
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();
            string base64FileContent = Convert.ToBase64String(fileBytes);

            // URL để kiểm tra file có tồn tại trong repository hay không
            string fileUrl = $"{ApiBaseUrl}/repos/{owner}/{repo}/contents/{Id}";

            string sha = null;
            bool fileExists = false;

            // Kiểm tra xem file đã tồn tại trong repo hay chưa
            var getFileResponse = await _httpClient.GetAsync(fileUrl);
            if (getFileResponse.IsSuccessStatusCode)
            {
                fileExists = true;
                var fileContent = await getFileResponse.Content.ReadAsStringAsync();
                var fileInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(fileContent);
                sha = fileInfo.sha;  // Lấy SHA nếu file đã tồn tại


                // Tạo nội dung request PUT để upload hoặc update file
                var requestBody = new
                {
                    message = "commitMessage",
                    content = base64FileContent,
                    sha = fileExists ? sha : null,  // Chỉ cung cấp SHA nếu file đã tồn tại
                    branch = "main"
                };

                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                // Gửi yêu cầu PUT để upload file lên GitHub
                var putResponse = await _httpClient.PutAsync(fileUrl, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

                if (!putResponse.IsSuccessStatusCode)
                {
                    string errorMessage = await putResponse.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to upload file: {errorMessage}");
                }
            }
            else
            {

                // Cấu trúc dữ liệu gửi lên GitHub API
                var requestBody = new
                {
                    message = "commitMessage",        // Thông điệp commit
                    content = base64FileContent,    // Nội dung mã hóa Base64
                    branch = "main"                 // Tên nhánh để commit
                };

                // Chuyển dữ liệu sang JSON
                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                // Đường dẫn API Endpoint, file sẽ được upload vào repo theo tên file
                string apiUrl = $"{ApiBaseUrl}/repos/{owner}/{repo}/contents/{Id}";

                // Gửi request lên GitHub API
                var response = await _httpClient.PutAsync(apiUrl, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

                // Xử lý kết quả phản hồi từ GitHub
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to upload file: {errorMessage}");
                }
            }
        }
    }
}
