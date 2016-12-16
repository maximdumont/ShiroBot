using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShiroBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new ShiroBot().RunAndBlockAsync(args).GetAwaiter().GetResult();
        }
    }
}
