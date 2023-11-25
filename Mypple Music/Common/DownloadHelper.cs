using System.Net.Http;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;

namespace Mypple_Music.Common
{
    public class DownloadHelper
    {
        public static Uri GetMusicPath(Uri audioUrl)
        {
            if (audioUrl == null)
                return null;
            string title = Path.GetFileName(audioUrl.ToString());
            string localPath = Path.Combine("music", title);
            if (File.Exists(Path.GetFullPath(localPath)))
            {
                return new Uri(Path.GetFullPath(localPath));
            }
            else
            {
                return audioUrl;
            }

        }
        public static async Task<string> GetMusicAsync(Uri audioUrl)
        {
            try
            {
                if (audioUrl == null)
                    return "";

                string title = Path.GetFileName(audioUrl.ToString());
                string localPath = Path.Combine("music", title);
                if (File.Exists(Path.GetFullPath(localPath)))
                {
                    return "File Exist";
                }
                else
                {
                    using (HttpClient httpClient = new HttpClient())
                    using (
                        FileStream fileStream = new FileStream(
                            localPath,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None
                        )
                    )
                    {
                        // 从picUrl获取一个流
                        Stream stream = await httpClient.GetStreamAsync(audioUrl);
                        // 将这个流复制到文件流中
                        await stream.CopyToAsync(fileStream);
                    }
                    return "Download Successful";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        public static async Task<string> GetImageAsync(Uri picUrl)
        {
            try
            {
                if (picUrl == null)
                    return "";

                string title = Path.GetFileName(picUrl.ToString());
                string localPath = Path.Combine("image", title);
                if (File.Exists(Path.GetFullPath(localPath)))
                {
                    return Path.GetFullPath(localPath);
                }
                else
                {
                    using (HttpClient httpClient = new HttpClient())
                    using (
                        FileStream fileStream = new FileStream(
                            localPath,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None
                        )
                    )
                    {
                        // 从picUrl获取一个流
                        Stream stream = await httpClient.GetStreamAsync(picUrl);
                        // 将这个流复制到文件流中
                        await stream.CopyToAsync(fileStream);
                    }
                    return Path.GetFullPath(localPath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        public static async Task<string> GetGaussianBlurImageAsync(Uri picUrl)
        {
            try
            {
                if (picUrl == null)
                    return "";

                string title = Path.GetFileName(picUrl.ToString());
                string localPath = Path.Combine("image", title);
                string blurPath = Path.Combine("image", $"(Blur){title}");
                if (File.Exists(Path.GetFullPath(blurPath)))
                {
                    return Path.GetFullPath(blurPath);
                }
                else
                {
                    if (!File.Exists(localPath))
                    {
                        using (HttpClient httpClient = new HttpClient())
                        using (
                            FileStream fileStream = new FileStream(
                                localPath,
                                FileMode.Create,
                                FileAccess.Write,
                                FileShare.None
                            )
                        )
                        {
                            // 从picUrl获取一个流
                            Stream stream = await httpClient.GetStreamAsync(picUrl);
                            // 将这个流复制到文件流中
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        using (Image image = Image.Load(localPath))
                        {
                            image.Mutate(x => x.GaussianBlur(30));
                            image.Save(blurPath); // save the new image
                        }
                    }

                    return Path.GetFullPath(blurPath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
