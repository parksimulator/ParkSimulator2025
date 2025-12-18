using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal partial class Program
    {
        static string? PickScene()
        {
            string? result = null;
            List<string> scenes = SimulatorCore.ListScenes();

            if(scenes.Count > 0)
            {
                int i = 0;
                Console.WriteLine("\nPiggieStorage/");
                for (int z = 0; z < scenes.Count; z++) 
                {
                    string valor = i != scenes.Count - 1 ? "├── " : "└── ";
                    Console.WriteLine(valor + i  + " --> " + scenes[z]);
                    i++;
                }

                int option = AskIntegerBetween("Escena", 0, scenes.Count - 1);

                result = scenes[option];
            }

            return result;
        }
    }
}
