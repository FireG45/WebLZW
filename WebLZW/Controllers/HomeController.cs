using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using WebLZW.Models;

namespace WebLZW.Controllers {
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;
        IWebHostEnvironment _appEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment appEnvironment) {
            _logger = logger;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile, int mode) {
            string path = "";
            if (uploadedFile != null) {
                path = _appEnvironment.WebRootPath + "/Files/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(path, FileMode.Create)) {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                return RedirectToAction("Compress", new { path = path, mode = mode });
            }
            return RedirectToAction("Error");
        }

        public IActionResult Compress(string path, int mode) {
            LZW lzw = new();
            Task task;
            string end_file_path = "";
            string typeFile = "";
            StreamWriter end_file;
            RedirectToActionResult action = RedirectToAction("Error");

            if (path != null && System.IO.File.Exists(path)) {
                if (mode == 0) {
                    if (Path.GetExtension(path).ToUpper() == ".TXT") {
                        typeFile = "Сжатый";
                        var file = System.IO.File.OpenRead(path);
                        end_file_path = _appEnvironment.WebRootPath + "/CompressedFiles/" + Path.GetFileName(path) + ".lzw";
                        end_file = new StreamWriter(System.IO.File.Create(end_file_path));
                        task = new(() => lzw.Compress(file, end_file));
                        task.Start();
                        task.Wait();
                        action = RedirectToAction("Download", new { path = end_file_path, typeFile = typeFile });
                    }
                }
                if (mode == 1) {
                    if (Path.GetExtension(path).ToUpper() == ".LZW") {
                        typeFile = "Распакованный";
                        var file = new StreamReader(path);
                        var filename = Path.GetFileName(path);
                        end_file_path = _appEnvironment.WebRootPath + "/DecompressedFiles/" + filename.Remove(filename.Length - 4);
                        end_file = new StreamWriter(System.IO.File.Create(end_file_path));
                        task = new(() => lzw.Decompress(file, end_file));
                        task.Start();
                        task.Wait();
                        action = RedirectToAction("Download", new { path = end_file_path, typeFile = typeFile });
                    }
                }
            } 
            
            return action;
        }

        public IActionResult Download(string path, string typeFile) {
            ViewData["Path"] = path;
            ViewData["TypeFile"] = typeFile;
            return View();
        }

        public IActionResult DownloadFile(string path) {

            var memory = DownloadSinghFile(path);

            return File(memory.ToArray(), "text/plain", Path.GetFileName(path));
        }

        private MemoryStream DownloadSinghFile(string path) {

            var memory = new MemoryStream();
            if (System.IO.File.Exists(path)) {
                var net = new System.Net.WebClient();

                var data = net.DownloadData(path);

                var content = new System.IO.MemoryStream(data);

                memory = content;
                System.IO.File.Delete(path);
            }
            memory.Position = 0;

            return memory;
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}