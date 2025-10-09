using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBox.Models
{
    public class OllamaRequest
    {
        public string model { get; set; } = "gpt-oss:20b";
        public string prompt { get; set; } = string.Empty;
        public bool stream { get; set; } = false;
    }
}
