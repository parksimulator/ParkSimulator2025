using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;



namespace GemeloDigital
{
    internal class G4Storage : Storage
    {

        public string nombreEscena;
        public string horaActual;

        internal override void Initialize()
        {
           Console.WriteLine("G4Storage: Initializing");


        }

        internal override void Finish()
        {
            Console.WriteLine("G4Storage: Finish");
        }

        internal override void LoadScene(string storageId)
        {
            Console.WriteLine("G4Storge: Load simulation" + storageId);



            FileStream file = new FileStream(nombreEscena, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file, Encoding.UTF8);

            //Listas
            List<Point> listPoint = new List<Point>();
            List<Path> listPath = new List<Path>();
            List<Facility> listFacility = new List<Facility>();
            List<Person> listPerson = new List<Person>();
            
            // Variables
            string line;

            line = reader.ReadLine();
            while (line != " *** FIN *** ")
            {
                string[] parte = line.Split(": ");
                //saltos de linea

                bool skip = false;
                if (line.Trim().Length == 0) { skip = true; }

                else if (line.Trim().Length > 2)
                {
                     string apartado = line.Trim('-', ' ');
                }
                line =  reader.ReadLine();
            }
 
        }

        internal override void SaveScene(string storageId)
        {
            Console.WriteLine("G4Storge: Save simulation " + storageId);

            nombreEscena = storageId + ".txt";

            FileStream file = new FileStream(nombreEscena, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file, Encoding.UTF8);

            DateTime thisDay = DateTime.Now; // <-- Sacamos la fecha.

            // List<SimulatedObject> nombreLista = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Objeto);

            List<SimulatedObject> listPointsObj = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Point);
            List<SimulatedObject> listFacilityObj = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Facility);
            List<SimulatedObject> listPathObj = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Path);
            List<SimulatedObject> listPersonObj = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Person);

            // Creamos las listas 
            List<Point> pointList = new List<Point>();
            Point p;
            foreach (SimulatedObject obj in listPointsObj)
            {
                p = SimulatorCore.AsPoint(obj);
                pointList.Add(p);
            }

            List<Facility> facilitiesList = new List<Facility>();
            Facility f;
            foreach (SimulatedObject obj in listFacilityObj)
            {
                f = SimulatorCore.AsFacility(obj);
                facilitiesList.Add(f);
            }

            List<Path> pathList = new List<Path>();
            Path pt;
            foreach (SimulatedObject obj in listPathObj)
            {
                pt = SimulatorCore.AsPath(obj);
                pathList.Add(pt);
            }

            List<Person> personList = new List<Person>();
            Person person;
            foreach (SimulatedObject obj in listPersonObj)
            {
                person = SimulatorCore.AsPerson(obj);
                personList.Add(person);
            }


            // Variables
            string iDPoint = "";


            writer.WriteLine("******** " +  nombreEscena + " ******** ");
            writer.WriteLine("Nombre Equipo: " + Environment.MachineName); // <-- Sacamos el nombre del equipo.
            writer.WriteLine("Fecha: " + thisDay);

            writer.WriteLine("\n *** INFO ***");


            writer.WriteLine("\n --- Puntos ---");

            for (int i = 0; i < pointList.Count; i++)
            {
                writer.WriteLine("ID: " + pointList[i].Id);
                writer.WriteLine("Nombre: " + pointList[i].Name);
                writer.WriteLine("Coordenada: " + pointList[i].Position.X + ", " + pointList[i].Position.Y + ", " + pointList[i].Position.Z);
            }

            writer.WriteLine("FIN PUNTOS");

            writer.WriteLine("\n--- Caminos ---");

            for (int i = 0; i < pathList.Count; i++)
            {
                writer.WriteLine("ID: " + pathList[i].Id);
                writer.WriteLine("Camino: " + pathList[i].Name);
                writer.WriteLine("Id Punto1: " + pathList[i].Point1.Id);
                writer.WriteLine("Id Punto2: " + pathList[i].Point2.Id);
            }

            writer.WriteLine("FIN CAMINOS");

            writer.WriteLine("\n--- Instalaciones ---");

            for (int i = 0; i < facilitiesList.Count; i++)
            {
                writer.WriteLine("ID: " + facilitiesList[i].Id);
                writer.WriteLine("Instalción: " + facilitiesList[i].Name);

                string pointsEntrances = "";
                string pointsExits     = "";

                // Creamos un punto y lo recorremos dependiendo a las entradas que hay y guardamos el ID
                    foreach (Point point in facilitiesList[i].Entrances)
                    {
                        pointsEntrances += " " + point.Id;
                    }
                    foreach (Point point in facilitiesList[i].Exits)
                    {
                        pointsExits += " " + point.Id;
                    }
                
                writer.WriteLine("Entrada: " + pointsEntrances);
                writer.WriteLine("Entrada: " + pointsExits);
                writer.WriteLine("Consumen: " + facilitiesList[i].PowerConsumed);
            }
            writer.WriteLine("FIN INSTALACIONES");


            writer.WriteLine("\n --- Personas ---");

            for (int i = 0; i < personList.Count; i++)
            {
                writer.WriteLine("ID: " + personList[i].Id);
                writer.WriteLine("Nombre: " + personList[i].Name);
                writer.WriteLine("Edad: " + personList[i].Age);
                writer.WriteLine("Altura: " + personList[i].Height);
                writer.WriteLine("Peso: " + personList[i].Weight);
                writer.WriteLine("Dinero: " + personList[i].Money);
            }

            writer.WriteLine("FIN PERSONAS");
            writer.WriteLine("\n *** FIN *** ");

            writer.Close();
            file.Close();

            
        }
        
        internal override void DeleteScene(string storageId)
        {
            //Console.WriteLine("Deleting simulation " + storageId);
        }

        internal override List<string> ListScenes()
        {

            string direc = Directory.GetCurrentDirectory();
            string[] list = Directory.GetDirectories(direc);

            List<string> result = new List<string>();

            for (int i = 0; i < list.Length; i++) 
            {
                result.Add(list[i]);
            }

            /*
            try
            {
                var textFile = Directory.GetFiles();
            
            }
            catch { }
            string[] partes = nombreEscena.Split(".");
            string escena = partes[1];
            result.Add(escena);
            */
            return result; 
        }
    }
}
