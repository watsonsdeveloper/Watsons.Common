using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Options;
using System.Drawing.Drawing2D;
using System.Net;
using System.IO;

namespace Watsons.Common.ImageHelpers
{
    public class ImageHelper : IImageHelper
    {
        private readonly ImageSettings? _imageSettings;

        private string? _base64;
        private string? _size;
        private ImageFormat? _extension;

        public string? ImagePath { get; set; }
        public string? Path { get; set; }
        public string? Name { get; set; }
        public ImageFormat? Extension
        {
            get { return _extension ?? ImageFormat.Png; }
            set { _extension = value; }
        }

        public string? Size
        {
            get
            {
                return _size ??= !string.IsNullOrEmpty(ImagePath) ? new FileInfo(ImagePath).Length.ToString() : null;
                //if (!string.IsNullOrEmpty(_size))
                //{
                //    return _size;
                //}
                //else if (!string.IsNullOrEmpty(ImagePath))
                //{
                //    return _size = new FileInfo(ImagePath).Length.ToString();
                //}
                //else
                //{
                //    return null;
                //}
            }
            set { _size = value; }
        }
        public string? Type { get; set; }
        public string? Base64
        {
            get
            {
                return _base64 ??= !string.IsNullOrEmpty(ImagePath) ? GetBase64Image(ImagePath) : null;
            }
            set { _base64 = value; }
        }
        public string? Url { get; set; }
        public string? Thumbnail { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? DeleteUrl { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        public ImageHelper() { }

        public ImageHelper(IOptions<ImageSettings> imageSettings)
        {
            _imageSettings = imageSettings.Value;
        }

        //public static ImageService Save(string filePath, string imageName, string extension)
        //{
        //    var imagePath = Path.Combine(filePath, imageName);

        //    if (!Directory.Exists(filePath))
        //    {
        //        Directory.CreateDirectory(filePath);
        //    }

        //    return new ImageService { IsSuccess = true };
        //}

        public static ImageHelper DownloadImageToLocal(string downloadUrl, string filePath, string imageName, bool overrideExist = false)
        {
            var imagePath = System.IO.Path.Combine(filePath, imageName);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            if (!overrideExist && File.Exists(imagePath))
            {
                return new ImageHelper()
                {
                    ImagePath = imagePath,
                    Name = imageName,

                };
            }

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(downloadUrl, imagePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return new()
            {
                ImagePath = imagePath,
                Name = imageName
            };
        }

        public static Bitmap ResizeWithHeightAspectRatio(string originalImagePath, string resizedImagePath, int resizedHeight, ImageFormat imageFormat)
        {
            try
            {
                using (Image originalImage = Image.FromFile(originalImagePath))
                {
                    int originalWidth = originalImage.Width;
                    int originalHeight = originalImage.Height;
                    int resizedWidth = (int)(originalWidth * ((float)resizedHeight / originalHeight));

                    // Create a new bitmap with the desired width and height
                    Bitmap resizedImage = new Bitmap(resizedWidth, resizedHeight);

                    // Create a graphics object to draw on the new bitmap
                    using (Graphics graphics = Graphics.FromImage(resizedImage))
                    {

                        // Configure the graphics object for high-quality image resizing
                        graphics.InterpolationMode = InterpolationMode.Low;
                        graphics.SmoothingMode = SmoothingMode.HighSpeed;
                        graphics.PixelOffsetMode = PixelOffsetMode.Half;
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;

                        // Draw the original image onto the new bitmap with the desired size
                        graphics.DrawImage(originalImage, 0, 0, resizedWidth, resizedHeight);
                        resizedImage.Save(resizedImagePath, imageFormat);

                        return resizedImage;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static bool DeleteImage(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        public static string GetBase64Image(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public static Image SaveBase64Image(string base64String, string imagePath, ImageFormat? extension)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (Image image = Image.FromStream(ms))
                    {
                        image.Save(imagePath, ImageFormat.Jpeg);
                        return image;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
