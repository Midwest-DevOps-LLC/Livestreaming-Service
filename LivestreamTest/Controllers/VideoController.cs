using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LivestreamTest.Controllers
{
    public class VideoController : Controller
    {
        [HttpGet("[controller]/GetStream")]
        public FileResult GetStream()
        {
            var directory = new DirectoryInfo(@"C:\Users\Mark\source\repos\LivestreamTest\LivestreamTest\wwwroot\vods");

            var filePath = "";

            foreach (var file in directory.GetFiles().OrderByDescending(f => f.LastWriteTime).ToList())
            {
                if (file.Extension == ".m3u8")
                {
                    filePath = file.FullName;
                    break;
                }
            }

            return PhysicalFile(filePath, "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpGet("[controller]/{tsFileName}")]
        public ActionResult GetTsFile(string tsFileName)
        {
            if (string.IsNullOrEmpty(tsFileName))
                return new EmptyResult();

            return PhysicalFile(@"C:\Users\Mark\source\repos\LivestreamTest\LivestreamTest\wwwroot\vods\" + tsFileName, "application/octet-stream", enableRangeProcessing: true);
        }
    }
}
