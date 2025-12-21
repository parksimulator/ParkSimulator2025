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
using System.Globalization;



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

            string nombreEscena = storageId + ".txt";

            FileStream file = new FileStream(nombreEscena, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file, Encoding.UTF8);

            //Listas
            List<Point> listPoint = new List<Point>();
            List<Path> listPath = new List<Path>();
            List<Facility> listFacility = new List<Facility>();
            List<Person> listPerson = new List<Person>();

            // Para unir caminos por los IDs
            Dictionary<string, Point> pointsId = new Dictionary<string, Point>();

          
            // Variables
            string line;
            string apartado = ""; // Para decir si es caminos/puntos/instlaciones/personas

            line = reader.ReadLine();
            // Saltar cabecera hasta INFO
            while (line != null && line.Trim() != "*** INFO ***")
            {
                line = reader.ReadLine();
            }

            // Leer la siguiente línea después de INFO
            line = reader.ReadLine();


            while (line != null && line.Trim() != "*** FIN ***")
            {
               
                if (line.Trim().Length > 2)
                {
                    if (line.Contains("---"))
                    {
                        apartado = line.Trim('-', ' ').Trim();
                    }
                }

                if (apartado == "Puntos")
                {
                    // Formato:
                    // ID: X
                    // Nombre: Y
                    // Coordenada: x, y, z

                    if (line.Trim().StartsWith("ID:"))
                    {
                        // es una manera de ahorrarse hacer un arrays para separar los contenidos
                        string id = line.Split(": ")[1].Trim();

                        string nombreLine = reader.ReadLine();
                        string coordLine = reader.ReadLine();

                        string nombre = nombreLine.Split(":")[1].Trim();

                        string coordText = coordLine.Split(":")[1];
                        string[] coords = coordText.Split(",");

                        float x = float.Parse(coords[0].Trim(), CultureInfo.InvariantCulture);
                        float y = float.Parse(coords[1].Trim(), CultureInfo.InvariantCulture);
                        float z = float.Parse(coords[2].Trim(), CultureInfo.InvariantCulture);

                        Point p = SimulatorCore.CreatePoint(); // creamos la estructura de Point para guardar la info

                        p.Id = id;
                        p.Name = nombre;
                        p.Position = new Vector3(x, y, z);

                        listPoint.Add(p);
                        pointsId[id] = p;

                    }

                }
                else if (line.Trim() == "FIN PUNTOS")
                {
                    apartado = "";
                }
                else if (apartado == "Caminos")
                {
                    // Formato:
                    // ID: X
                    // Camino: Y
                    // Id Punto1: A
                    // Id Punto2: B

                    if (line.Trim().StartsWith("ID:"))
                    {
                        string id = line.Split(":")[1].Trim();

                        string caminoLine = reader.ReadLine();
                        string p1Line = reader.ReadLine();
                        string p2Line = reader.ReadLine();

                        string nombreCamino = caminoLine.Split(":")[1].Trim();
                        string idP1 = p1Line.Split(": ")[1].Trim();
                        string idP2 = p2Line.Split(": ")[1].Trim();

                        if (pointsId.ContainsKey(idP1) && pointsId.ContainsKey(idP2))
                        {
                            Path path = SimulatorCore.CreatePath(pointsId[idP1], pointsId[idP2]);
                            path.Id = id;
                            path.Name = nombreCamino;

                            listPath.Add(path);

                        }

                    }
                }
                else if (line.Trim() == "FIN CAMINOS")
                {
                    apartado = "";
                }

                else if (apartado == "Instalaciones")
                {
                    // Formato:
                    // ID: X
                    // Instalción: Y
                    // Entrada: id id id
                    // Salida: id id id
                    // Consumen: Z

                    if (line.Trim().StartsWith("ID:"))
                    {
                        string id = line.Split(":")[1].Trim();

                        string nombreLine = reader.ReadLine();
                        string entradaLine = reader.ReadLine();
                        string salidaLine = reader.ReadLine();
                        string consumoLine = reader.ReadLine();

                        string nombre = nombreLine.Split(": ")[1];

                        string textoEntradas = entradaLine.Split(":")[1].Trim();
                        string[] partesEntradas = textoEntradas.Split(" ");

                        List<string> entradasIds = new List<string>();
                        foreach (string s in partesEntradas)
                        {
                            if (s != "")
                                entradasIds.Add(s);
                        }

                        string textoSalidas = salidaLine.Split(":")[1].Trim();
                        string[] partesSalidas = textoSalidas.Split(" ");

                        List<string> salidasIds = new List<string>();
                        foreach (string s in partesSalidas)
                        {
                            if (s != "")
                                salidasIds.Add(s);
                        }


                        float consumo = float.Parse(consumoLine.Split(":")[1], CultureInfo.InvariantCulture);

                        Facility facility = SimulatorCore.CreateFacility(pointsId[entradasIds[0]], pointsId[salidasIds[0]]);

                        facility.Id = id;
                        facility.Name = nombre.Trim();
                        facility.PowerConsumed = consumo;


                        // iniciamos en 1 porque cuando creamos la facility le asignamos los primeros ids
                        for (int i = 1; i < entradasIds.Count; i++)
                        {
                            if (pointsId.ContainsKey(entradasIds[i]))
                            {
                                facility.Entrances.Add(pointsId[entradasIds[i]]);
                            }
                        }

                        for (int i = 1; i < salidasIds.Count; i++)
                        {
                            if (pointsId.ContainsKey(salidasIds[i]))
                            {
                                facility.Exits.Add(pointsId[salidasIds[i]]);

                            }
                        }

                        listFacility.Add(facility);

                    }

                }
                else if (line.Trim() == "FIN INSTALACIONES")
                {
                    apartado = "";
                }

                else if (apartado == "Personas")
                {
                    // Formato:
                    // ID: X
                    // Nombre: Y
                    // Edad: N
                    // Altura: N
                    // Peso: N
                    // Dinero: N

                    if (line.Trim().StartsWith("ID:"))
                    {

                        string id = line.Split(":")[1].Trim();

                        string nombreLine = reader.ReadLine();
                        string edadLine = reader.ReadLine();
                        string alturaLine = reader.ReadLine();
                        string pesoLine = reader.ReadLine();
                        string dineroLine = reader.ReadLine();

                        Person persona = SimulatorCore.CreatePerson();

                        persona.Id = id;
                        persona.Name = nombreLine.Split(":")[1].Trim();
                        persona.Age = Int32.Parse(edadLine.Split(":")[1].Trim());
                        persona.Height = float.Parse(alturaLine.Split(":")[1]);
                        persona.Weight = float.Parse(pesoLine.Split(":")[1]);
                        persona.Money = float.Parse(dineroLine.Split(":")[1]);

                        listPerson.Add(persona);
                    }

                }
                else if (line.Trim() == "FIN PERSONAS")
                {
                    apartado = "";
                }

                line = reader.ReadLine();
            }

            reader.Close();
            file.Close();
 
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
                writer.WriteLine("Salida: " + pointsExits);
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
            string[] files = Directory.GetFiles(direc, "*.txt");

            List<string> result = new List<string>();

            foreach (string file in files)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(file);
                result.Add(sceneName);
            }
            return result; 
        }
    }
}
