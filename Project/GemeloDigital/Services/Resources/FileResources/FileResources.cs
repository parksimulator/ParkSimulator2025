using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal class FileResources : Resources
    {
        const string basePath = "Resources\\";

        internal override void Initialize()
        {
            Console.WriteLine("FileResources: Initializing");

        }

        internal override void Finish()
        {
            Console.WriteLine("FileResources: Finish");
        }

        internal override bool ExistsResource(string id)
        {
            return File.Exists(basePath + id);
        }

        internal override byte[] GetResource(string id)
        {
            return File.ReadAllBytes(basePath + id);
        }

    }}
