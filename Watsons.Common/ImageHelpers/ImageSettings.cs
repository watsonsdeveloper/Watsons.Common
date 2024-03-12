using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watsons.Common.ImageHelpers
{
    public class ImageSettings
    {
        public string FilePath { get; set; }
        public string UrlPath { get; set; }
        public int ResizeWidth { get; set; }
        public int ResizeHeight { get; set; }
        public int MinImageUpload { get; set; }
        public int MaxImageUpload { get; set; }
    }
}
