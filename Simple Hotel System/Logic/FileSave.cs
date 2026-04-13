namespace Simple_Hotel_System.Logic
{
    public class FileSave
    {
        public static (bool bOk, string sMsg, string fileUrl) SaveImage(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return (false, "File not found.", null);

            string[] allowedTypes = { "image/jpeg", "image/png", "image/jpg" };
            if (!allowedTypes.Contains(file.ContentType))
                return (false, "Only JPG and PNG images allowed.", null);

            string uploadPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads",
                folderName
            );

            Directory.CreateDirectory(uploadPath);

            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string fileUrl = $"/uploads/{folderName}/{fileName}";
            return (true, "File uploaded successfully.", fileUrl);
        }

        public static void DeleteImage(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return;

            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                fileUrl.TrimStart('/')
            );

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}
