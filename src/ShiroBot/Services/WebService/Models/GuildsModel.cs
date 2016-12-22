using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShiroBot.Services.WebService.Models
{
    public class GuildsModel
    {
        public bool owner { get; set; }
        public int permissions { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }
}
