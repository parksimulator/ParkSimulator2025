using System;
using System.Collections.Generic;
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
        List<string> list = new List<string>();
      

        public string nombreEscena;
        public string horaActual;

        internal override void Initialize()
        {
           Console.WriteLine("G4Storage: Initializing");
            list = new List<string>(); 


        }

        internal override void Finish()
        {
            Console.WriteLine("G4Storage: Finish");
        }

        internal override void LoadScene(string storageId)
        {
            Console.WriteLine("G4Storge: Load simulation" + storageId);

            
        }

        internal override void SaveScene(string storageId)
        {
            Console.WriteLine("G4Storge: Save simulation " + storageId);

            nombreEscena = storageId;

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

                writer.WriteLine("Coordenadas: " + pointList[i].Position.X + ", " + pointList[i].Position.Y + ", " + pointList[i].Position.Z);

            }

            writer.WriteLine("\n--- Caminos ---");

            for (int i = 0; i < pathList.Count; i++)
            {
                writer.WriteLine("ID: " + pathList[i].Id);
                writer.WriteLine("Camino: " + pathList[i].Name);
                writer.WriteLine("Id Punto1: " + pathList[i].Point1.Id);
                writer.WriteLine("Punto1: " + pathList[i].Point1);
                writer.WriteLine("Id Punto2: " + pathList[i].Point1.Id);
                writer.WriteLine("Punto2: " + pathList[i].Point2);

            }

            writer.WriteLine("\n--- Instalaciones ---");


           


            for (int i = 0; i < facilitiesList.Count; i++)
            {
                writer.WriteLine("ID: " + facilitiesList[i].Id);
                writer.WriteLine("Instalción: " + facilitiesList[i].Name);

                string exitsString = "";
                bool first = true;
                // for sobre las exits
                {
                    exitsString = exitsString + (first ? "" : ",") + id;
                    first = false;
                }

                string 

                writer.WriteLine("Entrada: " + exitsString);
  
                writer.WriteLine("Consumen: " + facilitiesList[i].PowerConsumed);
            }

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
            return list; list = new List<string>();
        }
    }
}
