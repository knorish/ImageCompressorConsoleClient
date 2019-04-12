using System;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace ImageCompressorConsole
{
    public class ImageCompressor
    {
        string _cwd;

        public ImageCompressor()
        {
            _cwd = AppDomain.CurrentDomain.BaseDirectory + "\\ImageCompression\\";
        }

        public ImageCompressionResult CompressFile(string fileName, bool lossy)
        {
            string targetFile = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(fileName));

            var start = new ProcessStartInfo("cmd")
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = _cwd,
                Arguments = GetArguments(fileName, targetFile, lossy),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var stopwatch = Stopwatch.StartNew();

            using (var process = Process.Start(start))
            {
                process.WaitForExit();
            }

            stopwatch.Stop();

            var result = new ImageCompressionResult(fileName, targetFile, stopwatch.Elapsed);

            HandleResult(result);

            return result;
        }

        private static string GetArguments(string sourceFile, string targetFile, bool lossy)
        {
            if (!Uri.IsWellFormedUriString(sourceFile, UriKind.RelativeOrAbsolute) && !File.Exists(sourceFile))
                return null;

            string ext;

            try
            {
                ext = Path.GetExtension(sourceFile).ToLowerInvariant();
            }
            catch (ArgumentException ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return null;
            }

            switch (ext)
            {
                case ".png":
                    File.Copy(sourceFile, targetFile);

                    if (lossy)
                        return string.Format(CultureInfo.CurrentCulture, "/c pingo -s8 -pngpalette=79 -q \"{0}\"", targetFile);
                    else
                        return string.Format(CultureInfo.CurrentCulture, "/c pingo -auto=100 -s8 -q \"{0}\"", targetFile);

                case ".jpg":
                case ".jpeg":
                    if (lossy)
                        return string.Format(CultureInfo.CurrentCulture, "/c cjpeg -quality 80,60 -dct float -smooth 5 -outfile \"{1}\" \"{0}\"", sourceFile, targetFile);
                    else
                        return string.Format(CultureInfo.CurrentCulture, "/c jpegtran -copy none -optimize -progressive -outfile \"{1}\" \"{0}\"", sourceFile, targetFile);

                case ".gif":
                    return string.Format(CultureInfo.CurrentCulture, "/c gifsicle -O3 \"{0}\" --output=\"{1}\"", sourceFile, targetFile);
            }

            return null;
        }

        void HandleResult(ImageCompressionResult result)
        {
            string name = Path.GetFileName(result.OriginalFileName);

            if (result.Saving > 0 && result.ResultFileSize > 0 && File.Exists(result.ResultFileName))
            {
                File.Copy(result.ResultFileName, result.OriginalFileName, true);
                File.Delete(result.ResultFileName);
            }
        }
    }
}