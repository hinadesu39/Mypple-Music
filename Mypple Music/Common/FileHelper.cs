using Mypple_Music.Models;
using System.Net.Http;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System;
using System.Reflection.Metadata.Ecma335;

namespace Mypple_Music.Common
{
    public class FileHelper
    {
        public static async Task<string> GetMusicAsync(Uri picUrl)
        {
            if (picUrl == null)
                return "";
            byte[] data;
            string title = Path.GetFileName(picUrl.ToString());
            string localPath = $@"music/{title}";
            if (File.Exists(localPath))
            {
                return localPath;
            }
            else
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    data = await httpClient.GetByteArrayAsync(picUrl);
                    File.WriteAllBytes(localPath, data);
                }
                return localPath;
            }
        }
        public static async Task<string> GetImageAsync(Uri picUrl)
        {
            if (picUrl == null)
                return "";
            byte[] data;
            string title = Path.GetFileName(picUrl.ToString());
            string localPath = $@"image/{title}";
            if (File.Exists(localPath))
            {
                return localPath;
            }
            else
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    data = await httpClient.GetByteArrayAsync(picUrl);
                    File.WriteAllBytes(localPath, data);
                }
                return localPath;
            }
        }
        public static async Task<string> GetGaussianBlurImageAsync(Uri picUrl)
        {
            if (picUrl == null)
                return "";
            byte[] data;
            string title = Path.GetFileName(picUrl.ToString());
            string localPath = $@"image/{title}";
            string newPath = $@"image/(Blur){title}";

            //如果在本地有缓存直接读取没有再下载后缓存
            if (File.Exists(localPath))
            {
                data = await File.ReadAllBytesAsync(localPath);
            }
            else
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    data = await httpClient.GetByteArrayAsync(picUrl);
                    File.WriteAllBytes(localPath, data);
                }
            }

            //如果有则直接返回

            if (File.Exists(newPath))
            {
                return newPath;
            }
            using (Image image = Image.Load(localPath))
            {
                image.Mutate(x => x.GaussianBlur(30));
                image.Save(newPath); // save the new image
            }
            return newPath;
        }
    }
}
