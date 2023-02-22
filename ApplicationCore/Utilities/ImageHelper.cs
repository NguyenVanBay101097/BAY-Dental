using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ApplicationCore.Utilities
{
    public static class ImageHelper
    {
        public static string ImageResizeImage(byte[] bytes_source, int? width = 1024, int? height = 1024,
           string encoding = "base64", ImageFormat filetype = null, bool avoid_if_small = false)
        {
            var base64_source = System.Convert.ToBase64String(bytes_source);
            return ImageResizeImage(base64_source, width: width, height: height, encoding: encoding, filetype: filetype, avoid_if_small: avoid_if_small);
        }

        public static string ImageResizeImage(string base64_source, int? width = 1024, int? height = 1024,
            string encoding = "base64", ImageFormat filetype = null, bool avoid_if_small = false)
        {
            if (string.IsNullOrEmpty(base64_source) || !(IsBase64String(base64_source)))
                return null;
            if (width == null && height == null)
                return null;

            using (var imageStream = new MemoryStream(System.Convert.FromBase64String(base64_source)))
            {
                var image = new Bitmap(imageStream);
                filetype = (filetype ?? image.RawFormat);
                if (filetype == ImageFormat.Bmp)
                    filetype = ImageFormat.Png;
                var asked_width = width;
                var asked_height = height;
                if (asked_width == null)
                    asked_width = (int)(image.Size.Width * ((float)asked_height / image.Size.Height));
                if (asked_height == null)
                    asked_height = (int)(image.Size.Height * ((float)asked_width / image.Size.Width));

                var size = new Size(asked_width ?? 0, asked_height ?? 0);
                //check image size: do not create a thumbnail if avoiding smaller images
                if (avoid_if_small && image.Size.Width <= size.Width && image.Size.Height <= size.Height)
                    return base64_source;

                if (image.Size != size)
                {
                    image = ImageResizeAndSharpen(image, size);
                }

                var backgroundStream = new MemoryStream();
                image.Save(backgroundStream, filetype);
                return System.Convert.ToBase64String(backgroundStream.ToArray());
            }
        }

        public static bool IsBase64String(this string s)
        {
            if (!string.IsNullOrEmpty(s))
                s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static bool IsBinSize(string base64_source)
        {
            if (string.IsNullOrEmpty(base64_source))
                return false;
            return Regex.IsMatch(base64_source, @"^\d+(\.\d*)? \w+$", RegexOptions.None);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                //graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Figure out the ratio
                double ratioX = (double)width / image.Width;
                double ratioY = (double)height / image.Height;
                // use whichever multiplier is smaller
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                // now we can get the new height and width
                int newHeight = (int)(image.Height * ratio);
                int newWidth = (int)(image.Width * ratio);

                // Now calculate the X,Y position of the upper-left corner 
                // (one of these will always be zero)
                int posX = (width - newWidth) / 2;
                int posY = (height - newHeight) / 2;

                graphics.DrawImage(image, posX, posY, newWidth, newHeight);
            }

            return destImage;
        }

        private static Bitmap ImageResizeAndSharpen(Bitmap image, Size size)
        {
            var resized = ResizeImage(image, size.Width, size.Height);
            //resized = Sharpen(resized);
            var img = NewBitmap(size.Width, size.Height);
            var res = img.Paste(resized, (size.Width - resized.Width) / 2, (size.Height - resized.Height) / 2);
            return res;
        }

        public static Bitmap NewBitmap(int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, width, height);
                graph.FillRectangle(Brushes.White, ImageSize);
            }
            return bmp;
        }

        public static Bitmap Paste(this Bitmap dest, Bitmap src, int x, int y)
        {
            using (var graphics = Graphics.FromImage(dest))
            {
                graphics.DrawImage(src, x, y, src.Width, src.Height);
            }

            return dest;
        }

        private static Bitmap ConvertRGBA(Bitmap c)
        {
            Bitmap d = new Bitmap(c.Width, c.Height);

            for (int i = 0; i < c.Width; i++)
            {
                for (int x = 0; x < c.Height; x++)
                {
                    Color oc = c.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    d.SetPixel(i, x, nc);
                }
            }

            return d;
        }

        public static Bitmap Sharpen(Bitmap image)
        {
            Bitmap sharpenImage = new Bitmap(image.Width, image.Height);

            int filterWidth = 3;
            int filterHeight = 3;
            int w = image.Width;
            int h = image.Height;

            double[,] filter = new double[filterWidth, filterHeight];

            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;

            double factor = 1.0;
            double bias = 0.0;

            Color[,] result = new Color[image.Width, image.Height];

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    //=====[REMOVE LINES]========================================================
                    // Color must be read per filter entry, not per image pixel.
                    Color imageColor = image.GetPixel(x, y);
                    //===========================================================================

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + w) % w;
                            int imageY = (y - filterHeight / 2 + filterY + h) % h;

                            //=====[INSERT LINES]========================================================
                            // Get the color here - once per fiter entry and image pixel.
                            imageColor = image.GetPixel(imageX, imageY);
                            //===========================================================================

                            red += imageColor.R * filter[filterX, filterY];
                            green += imageColor.G * filter[filterX, filterY];
                            blue += imageColor.B * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    sharpenImage.SetPixel(i, j, result[i, j]);
                }
            }
            return sharpenImage;
        }

        public static string DecodeBase64(this System.Text.Encoding encoding, string encodedText)
        {
            if (encodedText == null)
            {
                return null;
            }

            byte[] textAsBytes = System.Convert.FromBase64String(encodedText);
            return encoding.GetString(textAsBytes);
        }

        public static IDictionary<string, string> ImageGetResizedImages(string base64_source, bool return_big = false,
            bool return_medium = true, bool return_small = true, string big_name = "image", string medium_name = "image_medium",
            string small_name = "image_small", bool avoid_resize_big = true, bool avoid_resize_medium = false,
            bool avoid_resize_small = false)
        {
            var return_dict = new Dictionary<string, string>();
            if (return_big)
                return_dict.Add(big_name, ImageResizeImageBig(base64_source, avoid_if_small: avoid_resize_big));
            if (return_medium)
                return_dict.Add(medium_name, ImageResizeImageMedium(base64_source, avoid_if_small: avoid_resize_medium));
            if (return_small)
                return_dict.Add(small_name, ImageResizeImageSmall(base64_source, avoid_if_small: avoid_resize_small));
            return return_dict;
        }

        private static string ImageResizeImageBig(string base64_source, int? width = 1024, int? height = 1024, bool avoid_if_small = true)
        {
            return ImageResizeImage(base64_source, width: width, height: height, avoid_if_small: avoid_if_small);
        }

        private static string ImageResizeImageMedium(string base64_source, int? width = 128, int? height = 128, bool avoid_if_small = true)
        {
            return ImageResizeImage(base64_source, width: width, height: height, avoid_if_small: avoid_if_small);
        }

        private static string ImageResizeImageSmall(string base64_source, int? width = 64, int? height = 64, bool avoid_if_small = true)
        {
            return ImageResizeImage(base64_source, width: width, height: height, avoid_if_small: avoid_if_small);
        }

        public static Bitmap GetBitmapFromBase64(string base64_source)
        {
            using (var stream = new MemoryStream(System.Convert.FromBase64String(base64_source)))
            {
                var bitmap = new Bitmap(stream);
                return bitmap;
            }
        }

        public static string GetMineTypeBitmap(Bitmap bitmap)
        {
            var dict = new Dictionary<ImageFormat, string>();
            dict.Add(ImageFormat.Gif, "image/gif");
            dict.Add(ImageFormat.Jpeg, "image/jpeg");
            dict.Add(ImageFormat.Png, "image/png");

            return dict.ContainsKey(bitmap.RawFormat) ? dict[bitmap.RawFormat] : "";
        }

        public static string GetImageExtension(string base64)
        {
            switch(base64[0])
            {
                case '/':
                    return "jpeg";
                case 'i':
                    return "png";
                case 'R':
                    return "gif";
                case 'U':
                    return "webp";
                default:
                    throw new Exception("Unknown extension");
            }
        }

        public static string CropImage(string data, string type = "top", int? width = null, int? height = null,
            int w_ratio = 1, int h_ratio = 1, ImageFormat image_format = null)
        {
            // Used for cropping image and create thumbnail
            //param data: base64 data of image.
            //:param type: Used for cropping position possible
            //  Possible Values : 'top', 'center', 'bottom'
            //:param ratio: Cropping ratio
            //   e.g for (4, 3), (16, 9), (16, 10) etc
            //     send ratio(1, 1) to generate square image
            //:param size: Resize image to size
            //   e.g(200, 200)
            //  after crop resize to 200x200 thumbnail
            //:param image_format: return image format PNG, JPEG etc

            if (string.IsNullOrEmpty(data))
                return null;
            using (var imageStream = new MemoryStream(System.Convert.FromBase64String(data)))
            using (var output_stream = new MemoryStream())
            {
                using (var image = new Bitmap(imageStream))
                {
                    var w = image.Width;
                    var h = image.Height;
                    var new_w = w;
                    var new_h = h;

                    new_h = (w * h_ratio) / w_ratio;
                    new_w = w;
                    if (new_h > h)
                    {
                        new_h = h;
                        new_w = (h * w_ratio) / h_ratio;
                    }

                    image_format = image_format ?? image.RawFormat ?? ImageFormat.Jpeg;
                    if (type == "top")
                    {
                        var resized = new Bitmap(new_w, new_h);
                        RectangleF destinationRect = new RectangleF(0, 0, new_w, new_h);
                        RectangleF sourceRect = new RectangleF(0, 0, new_w, new_h);
                        using (var graphics = Graphics.FromImage(resized))
                        {
                            graphics.DrawImage(image, sourceRect, destinationRect, GraphicsUnit.Pixel);
                            resized.Save(output_stream, image_format);
                        }
                    }
                    else if (type == "center")
                    {
                        var resized = new Bitmap(new_w, new_h);
                        RectangleF destinationRect = new RectangleF((w - new_w) / 2, (h - new_h) / 2, new_w, new_h);
                        RectangleF sourceRect = new RectangleF(0, 0, new_w, new_h);
                        using (var graphics = Graphics.FromImage(resized))
                        {
                            graphics.DrawImage(image, sourceRect, destinationRect, GraphicsUnit.Pixel);
                            resized.Save(output_stream, image_format);
                        }
                    }
                    else if (type == "bottom")
                    {
                        var resized = new Bitmap(new_w, new_h);
                        RectangleF destinationRect = new RectangleF(0, h - new_h, new_w, new_h);
                        RectangleF sourceRect = new RectangleF(0, 0, new_w, new_h);
                        using (var graphics = Graphics.FromImage(resized))
                        {
                            graphics.DrawImage(image, sourceRect, destinationRect, GraphicsUnit.Pixel);
                            resized.Save(output_stream, image_format);
                        }
                    }
                    else
                    {
                        throw new Exception("ERROR: invalid value for crop_type");
                    }
                }

                if (width.HasValue || height.HasValue)
                {
                    using (var output_stream2 = new MemoryStream())
                    using (var image2 = new Bitmap(output_stream))
                    {
                        var thumbnail = image2.GetThumbnailImage(width ?? 0, height ?? 0, () => false, IntPtr.Zero);
                        thumbnail.Save(output_stream2, image_format);
                        return Convert.ToBase64String(output_stream2.ToArray());
                    }
                }

                return Convert.ToBase64String(output_stream.ToArray());
            }
        }
    }
}
