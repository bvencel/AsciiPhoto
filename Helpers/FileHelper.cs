using AsciiPhoto.Entities;
using AsciiPhoto.Enums;

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace AsciiPhoto.Helpers
{
    public static class FileHelper
    {
        public static bool IsFile(string path)
        {
            // Get the file attributes for file or directory
            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static List<BitmapWithMetadata>? LoadBitmapsOriginalSize(ConverterSettings settings, StringBuilder consoleContent)
        {
            if (settings is null)
            {
                return null;
            }

            if (settings.Source == InputSources.NotSet || settings.Source == InputSources.Screen)
            {
                return null;
            }

            List<BitmapWithMetadata> files = new();

            // Other sources than screen
            if (IsFile(settings.Path))
            {
                FileInfo fileInfo = new(settings.Path);

                using Bitmap loadedBitmap = new(fileInfo.FullName);
                Bitmap resizedBitmap = ImageHelper.ResizeLoadedBitmap(settings, out _, loadedBitmap);

                files.Add(new BitmapWithMetadata()
                {
                    LoadedBitmap = resizedBitmap,
                    LoadedHeight = loadedBitmap.Height,
                    LoadedWidth = loadedBitmap.Width,
                    Path = fileInfo.FullName,
                });

                if (consoleContent != null)
                {
                    consoleContent.AppendLine($"Processing single file '{settings.Path}'");
                }
            }
            else
            {
                DirectoryInfo filesOfFolder = new(settings.Path);

                List<FileInfo> fileList = new();
                fileList.AddRange(filesOfFolder.GetFiles("*.jpg", SearchOption.AllDirectories));
                fileList.AddRange(filesOfFolder.GetFiles("*.jpeg", SearchOption.AllDirectories));
                fileList.AddRange(filesOfFolder.GetFiles("*.png", SearchOption.AllDirectories));
                fileList.AddRange(filesOfFolder.GetFiles("*.gif", SearchOption.AllDirectories));
                fileList = fileList.OrderBy(s => s.Name).ToList();

                foreach (FileInfo fileInfo in fileList)
                {
                    using Bitmap loadedBitmap = new(fileInfo.FullName);
                    Bitmap resizedBitmap = ImageHelper.ResizeLoadedBitmap(settings, out _, loadedBitmap);

                    files.Add(new BitmapWithMetadata()
                    {
                        LoadedBitmap = resizedBitmap,
                        LoadedHeight = loadedBitmap.Height,
                        LoadedWidth = loadedBitmap.Width,
                        Path = fileInfo.FullName,
                    });
                }

                if (consoleContent != null)
                {
                    consoleContent.AppendLine($"Found {files.Count} pictures in '{settings.Path}' directory");
                }
            }

            return files;
        }
    }
}