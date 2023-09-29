using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Techtonica_Mod_Loader
{
    public static class ImageCache
    {
        // Objects & Variables
        private static Dictionary<int, CachedImage> images = new Dictionary<int, CachedImage>();

        // Public Functions

        public static async void CacheImage(string url) {
            if (IsImageCached(url)) {
                // ToDo: Log warning
                return;
            }

            CachedImage image = new CachedImage() {
                id = GetNewID(),
                url = url,
            };
            images.Add(image.id, image);

            try {
                using (WebClient webClient = new WebClient()) {
                    byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(url));

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(imageData);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    using (FileStream stream = new FileStream(image.GetPath(), FileMode.Create)) {
                        encoder.Save(stream);
                    }

                    Save();
                }
            }
            catch (Exception ex) {
                string error = $"Error downloading image: {ex.Message}";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static bool IsImageCached(int id) {
            return images.ContainsKey(id);
        }

        public static bool IsImageCached(string url) {
            return images.Values.Where(image => image.url == url).Count() == 1;
        }

        public static string GetImagePath(int id) {
            if (IsImageCached(id)) {
                return images[id].GetPath();
            }
            else {
                string error = $"Tried to get image path for image that is not cached '{id}'";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return null;
            }
        }

        public static string GetImagePath(string url) {
            CachedImage image = GetCachedImageFromUrl(url);
            if (image != null) return image.GetPath();
            else return null;
        }

        public static void ClearCache() {
            foreach(CachedImage image in images.Values) {
                File.Delete(image.GetPath());
            }

            images.Clear();
            Save();
        }

        // Private Functions

        private static int GetNewID() {
            if (images.Count == 0) return 0;
            else return images.Keys.Max() + 1;
        }

        private static CachedImage GetCachedImageFromUrl(string url) {
            if (IsImageCached(url)) {
                return images.Values.Where(image => image.url == url).ToList()[0];
            }
            else {
                return null;
            }
        }

        // Data Functions

        public static void Save() {
            string json = JsonConvert.SerializeObject(images.Values.ToList());
            File.WriteAllText(ProgramData.Paths.imageCacheMapFile, json);
        }

        public static void Load() {
            if (!File.Exists(ProgramData.Paths.imageCacheMapFile)) return;

            string json = File.ReadAllText(ProgramData.Paths.imageCacheMapFile);
            List<CachedImage> cachedImages = JsonConvert.DeserializeObject<List<CachedImage>>(json);
            foreach(CachedImage image in cachedImages) {
                images.Add(image.id, image);
            }
        }
    }

    public class CachedImage {
        public int id;
        public string url;
        
        public string GetPath() {
            return $"{ProgramData.Paths.imageCache}/{id}.png";
        }
    }
}
