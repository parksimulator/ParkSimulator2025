using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace GemeloDigital
{
    internal partial class Storage02 : Storage
    {
        internal override List<string> ListScenes()
        {
            // muestro el fichero donde estan todas las .sb por ejemplo saves
            // en mi lista ecenas tinee que devolver una LISTScenes()={ "A.sb, "B.sb" , "c.sb"}
            // cundo ya tenemos varias esenas creandas se creara varios ficheros y aqui deve mostrar cuantas hay mas no mostrar lo de 
            //dentro ya que si quiero ver lo que existe dentro del fichero de texto tendre que ir a LoadScenes no aqui 

               
            string[] archivos = Directory.GetFiles("saves", "*.sb");

            List<string> nombresArchivos = new List<string>();

            foreach (string archivo in archivos)
            {
                string nombreArchivo = System.IO.Path.GetFileNameWithoutExtension(archivo);
                nombresArchivos.Add(nombreArchivo);
            }

            return nombresArchivos;

        }

    }
}

