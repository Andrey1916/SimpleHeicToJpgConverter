using System.IO;
using System.Threading.Tasks;
using ImageMagick;

namespace SimpleHaicToJpgConverter
{
    internal static class Converter
    {
        public enum OperationResult
        {
            Nothing,
            Converted,
            Copied
        }

        public async static Task<OperationResult> HeicToJpgAsync(string source, string destination, bool copyOtherwise = false)
        {
            string pathToFile               = Path.GetFullPath(source);
            string filename                 = Path.GetFileName(source);
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(source);
            string fileExtension            = Path.GetExtension(source)
                                                  .ToLower();

            if (fileExtension == ".heic")
            {
                string destinationPath = Path.Combine(destination, filenameWithoutExtension + ".jpg");

                using (var image = new MagickImage(source))
                {
                    await image.WriteAsync(destinationPath);
                }

                return OperationResult.Converted;
            }
            else
            {
                if (copyOtherwise)
                {
                    File.Copy(source, Path.Combine(destination, filename));

                    return OperationResult.Copied;
                }

                return OperationResult.Nothing;
            }
        }

        public static OperationResult HeicToJpg(string source, string destination, bool copyOtherwise = false)
        {
            string pathToFile               = Path.GetFullPath(source);
            string filename                 = Path.GetFileName(source);
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(source);
            string fileExtension            = Path.GetExtension(source)
                                                  .ToLower();

            if (fileExtension == ".heic")
            {
                string destinationPath = Path.Combine(destination, filenameWithoutExtension + ".jpg");

                using (var image = new MagickImage(source))
                {
                    image.Write(destinationPath);
                }

                return OperationResult.Converted;
            }
            else
            {
                if (copyOtherwise)
                {
                    File.Copy(source, Path.Combine(destination, filename));

                    return OperationResult.Copied;
                }

                return OperationResult.Nothing;
            }
        }
    }
}