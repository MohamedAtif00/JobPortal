namespace JobPortal.Helper
{
    public static class ImageHelper
    {
        public static async Task<string> SaveImageAsync(IFormFile imageFile, string rootPath, string folder, string fileName = null)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Image file cannot be null or empty", nameof(imageFile));
            }

            var uploadsFolder = Path.Combine(rootPath, folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileExtension = Path.GetExtension(imageFile.FileName);


            string? uniqueFileName = !string.IsNullOrWhiteSpace(fileName)? fileName + fileExtension :  Guid.NewGuid().ToString() + fileExtension;


            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                // Log exception (if logging is configured)
                // _logger.LogError(ex, "Error occurred while saving image file.");
                throw;
            }

            return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
        }

        public static string GetImageFilePath(string relativePathWithoutExtension, string webRootPath)
        {
            if (string.IsNullOrEmpty(relativePathWithoutExtension))
            {
                throw new ArgumentNullException(nameof(relativePathWithoutExtension));
            }

            // Construct the directory path
            var directoryPath = Path.Combine(webRootPath, Path.GetDirectoryName(relativePathWithoutExtension)?.Replace("/", Path.DirectorySeparatorChar.ToString()) ?? string.Empty);

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            // Get the base file name (without extension)
            var fileNameWithoutExtension = Path.GetFileName(relativePathWithoutExtension);

            // Search for files with matching base name and any extension
            var matchingFiles = Directory.GetFiles(directoryPath, $"{fileNameWithoutExtension}.*");

            if (matchingFiles.Length == 0)
            {
                throw new FileNotFoundException($"No file found matching: {fileNameWithoutExtension} in {directoryPath}");
            }

            // Return the first match (or customize this if needed)
            return matchingFiles.First();
        }


        public static bool DeleteImage(string relativePath, string webRootPath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            var absolutePath = Path.Combine(webRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            try
            {
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log exception (if logging is configured)
                // _logger.LogError(ex, "Error occurred while deleting image file.");
                throw;
            }
        }
    }
}
