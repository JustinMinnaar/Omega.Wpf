using Jem.CommonLibrary22;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jem.ProfilingLibrary23
{
    public static class WriterExtensions
    {
        public static void AddStep(this IWriter writer, JProTemplate jTemplate, string msg)
        {
            writer.WriteLine($"{jTemplate} - {msg}");
        }
    }
}