using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal partial class Storage02 : Storage
    {
        List<string> nombresArchivos = new List<string>();
        internal override void DeleteScene(string storageId)
        {
            Console.WriteLine("Deleting simulation " + storageId);

            string filePath = System.IO.Path.Combine("saves", storageId + ".sb");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine("File deleted.");
            }

            nombresArchivos.Remove(storageId);
            Console.WriteLine("Scene removed from the list.");
        }

    }
}
