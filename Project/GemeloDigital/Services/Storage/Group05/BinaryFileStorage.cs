using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal class BinaryFileStorage : Storage
    {
        List<string> list = new List<string>();

        internal override void Initialize()
        {
            //Console.WriteLine("BinaryFileStorage: Initializing");

            list = new List<string>();

            string scenesFile = "scenes.dat";

            if (!File.Exists(scenesFile))
            {
                // Creamos archivo vacío, desde el cual luego iremos modificando y añadiendo las nuevas scenes
                FileStream streamCreate = new FileStream(scenesFile, FileMode.Create, FileAccess.Write);
                
                streamCreate.Close();
            }

            // Si existe, lo cargamos
            FileStream streamLoad = new FileStream(scenesFile, FileMode.Open, FileAccess.Read);

            byte[] bytesInt = new byte[sizeof(int)];

            int infoLeft = streamLoad.Read(bytesInt); 

            while (infoLeft > 0)
            {
                int idSize = BitConverter.ToInt32(bytesInt);
                byte[] bytesStr = new byte[idSize];
                streamLoad.Read(bytesStr);

                string sceneName = Encoding.UTF8.GetString(bytesStr);
                list.Add(sceneName);
                                
                infoLeft = streamLoad.Read(bytesInt);
            }

            streamLoad.Close();
        }

        internal override void Finish()
        {
            //Console.WriteLine("BinaryFileStorage: Finish");
        }

        // Función que carga el archivo que contiene los datos de 'points' y lo vuelca a memoria en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre en UTF8 (int)
        // Nombre en UTF8
        // coordenadas en float (orden X,Y,Z)
        internal void LoadPointsFile(string storageId)
        {            
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;

            // variable que nos permite cargar el archivo exacto asociado al nombre de la escena
            string scenePointsName = storageId + "points.dat";
            FileStream streamPoints = new FileStream(scenePointsName, FileMode.Open, FileAccess.Read);                       
            
            int infoLeft = streamPoints.Read(bytesInt);

            while (infoLeft > 0)
            {
                int idSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[idSize];
                streamPoints.Read(bytesStr);
                string pointId = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamPoints.Read(bytesInt);
                int nameSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[nameSize];
                streamPoints.Read(bytesStr);
                string pointName = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamPoints.Read(bytesFloat);
                float x = BitConverter.ToSingle(bytesFloat);

                streamPoints.Read(bytesFloat);
                float y = BitConverter.ToSingle(bytesFloat);

                streamPoints.Read(bytesFloat);
                float z = BitConverter.ToSingle(bytesFloat);
               
                // Una vez tenemos los datos, creamos el punto con estos
                Point p = SimulatorCore.CreatePointWithId(pointId);
                p.Name = pointName;
                p.Position = new System.Numerics.Vector3((float)x, (float)y, (float)z);

                infoLeft = streamPoints.Read(bytesInt);
            }

            streamPoints.Close();
        }

        // Función que carga el archivo que contiene los datos de 'Facilities' y lo vuelca a memoria en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre en UTF8 (int)
        // Nombre en UTF8
        // Float power consumed total
        // Cantidad de puntos en la lista de entrada (int)--> Entrances.Count
        // cada una de esas entradas --> int tamaño del id, string id
        // Cantidad de puntos en la lista de salida (int)--> Exits.Count
        // cada una de esas entradas --> int tamaño del id, string id
        // IMPORTANTE: esta función carga la última entrada y salida de las listas, ya que el programa por el momento solo tiene una entrada y una salida
        internal void LoadFacilitiesFile(string storageId)
        {
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;
                        
            string sceneFacilitiesName = storageId + "facilities.dat";
            FileStream streamFacilities = new FileStream(sceneFacilitiesName, FileMode.Open, FileAccess.Read);
            
            int infoLeft = streamFacilities.Read(bytesInt);

            while (infoLeft > 0)
            {
                int idSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[idSize];
                streamFacilities.Read(bytesStr);
                string facilityID = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamFacilities.Read(bytesInt);
                int nameSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[nameSize];
                streamFacilities.Read(bytesStr);
                string facilityName = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamFacilities.Read(bytesFloat);
                float powerConsumed = BitConverter.ToSingle(bytesFloat);

                streamFacilities.Read(bytesInt);
                int numberOfEntrances = BitConverter.ToInt32(bytesInt);
                List<string> entrances = new List<string>();

                string entranceId = "";
                for (int i = 0; i < numberOfEntrances; i++)
                {
                    streamFacilities.Read(bytesInt);
                    int entranceIdSize = BitConverter.ToInt32(bytesInt);

                    bytesStr = new byte[entranceIdSize];
                    streamFacilities.Read(bytesStr);

                    entranceId = Encoding.UTF8.GetString(bytesStr);
                    entrances.Add(entranceId);
                }

                streamFacilities.Read(bytesInt);
                int numberOfExits = BitConverter.ToInt32(bytesInt);
                List<string> exits = new List<string>();

                string exitId = "";
                for (int i = 0; i < numberOfExits; i++)
                {
                    streamFacilities.Read(bytesInt);
                    int exitIdSize = BitConverter.ToInt32(bytesInt);

                    bytesStr = new byte[exitIdSize];
                    streamFacilities.Read(bytesStr);

                    exitId = Encoding.UTF8.GetString(bytesStr);
                    exits.Add(exitId);
                }

                // crear facility!!!
                // necesitamos el punto (que previamente hemos cargado) que encaje con la Id
                // primero generamos una lista de puntos con las funciones de simulatorCore, sacando primero la lista de objects
                List<SimulatedObject> objectsPoint = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Point);
                List<Point> points = new List<Point>();
                Point point;

                foreach (SimulatedObject o in objectsPoint)
                {
                    point = SimulatorCore.AsPoint(o);
                    points.Add(point);
                }

                Point entrancePoint = new Point();
                Point exitPoint = new Point();
                foreach (Point p in points)
                {
                    if (p.Id == entranceId)
                    {
                        entrancePoint = p;
                    }
                    if (p.Id == exitId)
                    {
                        exitPoint = p;
                    }
                }

                Facility f = SimulatorCore.CreateFacilityWithId(facilityID, entrancePoint, exitPoint);
                f.Name = facilityName;
                f.PowerConsumed = powerConsumed;

                infoLeft = streamFacilities.Read(bytesInt);
            }

            streamFacilities.Close();
        }

        // Función que carga el archivo que contiene los datos de 'paths' y lo vuelca a memoria en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre (int)
        // Nombre en UTF8
        // capacity persons (int)
        // espacio en bytes del Id del punto1 (int)
        // id del punto1
        // espacio en bytes del Id del punto2 (int)
        // id del punto2
        internal void LoadPathsFile(string storageId)
        {
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;

            
            string scenePathsName = storageId + "paths.dat";
            FileStream streamPaths = new FileStream(scenePathsName, FileMode.Open, FileAccess.Read);
                        
            int infoLeft = streamPaths.Read(bytesInt);

            while (infoLeft > 0)
            {
                int idSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[idSize];
                streamPaths.Read(bytesStr);
                string pathID = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamPaths.Read(bytesInt);
                int nameSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[nameSize];
                streamPaths.Read(bytesStr);
                string pathName = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamPaths.Read(bytesInt);
                int capacityPersons = BitConverter.ToInt32(bytesInt);

                streamPaths.Read(bytesInt);
                int point1Size = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[point1Size];
                streamPaths.Read(bytesStr);
                string pathPoint1Id = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamPaths.Read(bytesInt);
                int point2Size = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[point2Size];
                streamPaths.Read(bytesStr);
                string pathPoint2Id = System.Text.Encoding.UTF8.GetString(bytesStr);


                // crear path
                List<SimulatedObject> objectsPoint = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Point);
                List<Point> points = new List<Point>();
                Point point;

                foreach (SimulatedObject o in objectsPoint)
                {
                    point = SimulatorCore.AsPoint(o);
                    points.Add(point);
                }

                Point point1 = new Point();
                Point point2 = new Point();
                foreach (Point po in points)
                {
                    if (po.Id == pathPoint1Id)
                    {
                        point1 = po;
                    }
                    if (po.Id == pathPoint2Id)
                    {
                        point2 = po;
                    }
                }

                Path p = SimulatorCore.CreatePathWithId(pathID, point1, point2);
                p.Name = pathName;
                p.CapacityPersons = capacityPersons;

                infoLeft = streamPaths.Read(bytesInt);
            }

            streamPaths.Close();

        }

        // Función que carga el archivo que contiene los datos de 'persons' y lo vuelca a memoria en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre (int)
        // Nombre en UTF8
        // age (int)
        // height (float)
        // weight (float)
        // money (float)
        // is at facility (si es null el archivo previamente guardó "null" en UTF8)
        // is at path (si es null el archivo previamente guardó "null" en UTF8)
        internal void LoadPersonsFile(string storageId)
        {
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;
           
            string scenePersonsName = storageId + "persons.dat";
            FileStream streamPersons = new FileStream(scenePersonsName, FileMode.Open, FileAccess.Read);
                        
            int infoLeft = streamPersons.Read(bytesInt);

            while (infoLeft > 0)
            {
                int idSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[idSize];
                streamPersons.Read(bytesStr);
                string personID = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamPersons.Read(bytesInt);
                int nameSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[nameSize];
                streamPersons.Read(bytesStr);
                string personName = System.Text.Encoding.UTF8.GetString(bytesStr);

                streamPersons.Read(bytesInt);
                int personAge = BitConverter.ToInt32(bytesInt);

                streamPersons.Read(bytesFloat);
                float personHeight = BitConverter.ToSingle(bytesFloat);

                streamPersons.Read(bytesFloat);
                float personWeight = BitConverter.ToSingle(bytesFloat);

                streamPersons.Read(bytesFloat);
                float personMoney = BitConverter.ToSingle(bytesFloat);

                streamPersons.Read(bytesInt);
                int facilityNameSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[facilityNameSize];
                streamPersons.Read(bytesStr);
                string? facilityId = System.Text.Encoding.UTF8.GetString(bytesStr);
                if (facilityId == "null")
                {
                    facilityId = null;
                }

                streamPersons.Read(bytesInt);
                int pathNameSize = BitConverter.ToInt32(bytesInt);
                bytesStr = new byte[pathNameSize];
                streamPersons.Read(bytesStr);
                string? pathId = System.Text.Encoding.UTF8.GetString(bytesStr);
                if (pathId == "null")
                {
                    pathId = null;
                }

                // crear person
                // hay que sacar la lista de facilities y paths para encontrar aquel en el que pueda estar ubicada la person
                List<SimulatedObject> objectsFacility = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Facility);
                List<Facility> facilities = new List<Facility>();
                Facility facility;

                foreach (SimulatedObject o in objectsFacility)
                {
                    facility = SimulatorCore.AsFacility(o);
                    facilities.Add(facility);
                }

                // Contemplamos que pueda ser nulo y lo inicializamos así
                Facility isAtFacility = null;

                // En caso de que no lo sea, lo reasignamos
                if (facilityId != null)
                {
                    foreach (Facility f in facilities)
                    {
                        if (f.Id == facilityId)
                        {
                            isAtFacility = f;
                        }
                    }
                }

                List<SimulatedObject> objectsPaths = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Path);
                List<Path> paths = new List<Path>();
                Path path;

                foreach (SimulatedObject o in objectsPaths)
                {
                    path = SimulatorCore.AsPath(o);
                    paths.Add(path);
                }

                // Contemplamos que pueda ser nulo y lo inicializamos así
                Path isAtPath = null;

                // En caso de que no lo sea, lo reasignamos
                if (pathId != null)
                {
                    foreach (Path pa in paths)
                    {
                        if (pa.Id == pathId)
                        {
                            isAtPath = pa;
                        }
                    }
                }

                Person p = SimulatorCore.CreatePersonWithId(personID);
                p.Name = personName;
                p.Age = personAge;
                p.Height = personHeight;
                p.Weight = personWeight;
                p.Money = personMoney;
                p.IsAtFacility = isAtFacility;
                p.IsAtPath = isAtPath;

                infoLeft = streamPersons.Read(bytesInt);
            }

            streamPersons.Close();
        }

        // Función que guarda el archivo que contiene los datos de 'points' en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre (int)
        // Nombre en UTF8
        // coordenadas en float (orden X,Y,Z)
        internal void SavePointsFile(string storageId)
        {            
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;

            // esta variable nos permite que cada escena tenga el nombre distinto referente al num que le corresponde
            string scenePointsName = storageId + "points.dat";
            FileStream streamPoints = new FileStream(scenePointsName, FileMode.Create, FileAccess.Write);

            // Sacamos la lista de puntos (ojo! devuelve lista de simulated objetcs con type punto que hay convertir a puntos)

            List<SimulatedObject> objectsPoint = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Point);
            List<Point> points = new List<Point>();
            Point point;

            foreach (SimulatedObject o in objectsPoint)
            {
                point = SimulatorCore.AsPoint(o);
                points.Add(point);
            }

            
            foreach (Point p in points)
            {
                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Id);
                streamPoints.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPoints.Write(bytesStr);

                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Name);
                streamPoints.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPoints.Write(bytesStr);

                bytesFloat = BitConverter.GetBytes(p.Position.X);
                streamPoints.Write(bytesFloat);

                bytesFloat = BitConverter.GetBytes(p.Position.Y);
                streamPoints.Write(bytesFloat);

                bytesFloat = BitConverter.GetBytes(p.Position.Z);
                streamPoints.Write(bytesFloat);
            }

            streamPoints.Close();

        }

        // Función que guarda el archivo que contiene los datos de 'facilities' en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre (int)
        // Nombre en UTF8
        // Float power consumed total
        // Cantidad de puntos en la lista de entrada (int)--> Entrances.Count
        // cada una de esas entradas --> int tamaño del id, string id
        // Cantidad de puntos en la lista de salida (int)--> Exits.Count
        // cada una de esas entradas --> int tamaño del id, string id
        // IMPORTANTE: esta función guarda la lista de entradas y salidas, pero de momento el programa solo permite crear facilities con una entrada y una salida,
        // por lo que guarda dos lists de un elemento cada una
        internal void SaveFacilitiesFile(string storageId)
        {
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;

            
            string sceneFacilitiesName = storageId + "facilities.dat";
            FileStream streamFacilities = new FileStream(sceneFacilitiesName, FileMode.Create, FileAccess.Write);

            // Sacamos la lista de facilities

            List<SimulatedObject> objectsFacilty = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Facility);
            List<Facility> facilities = new List<Facility>();
            Facility facility;

            foreach (SimulatedObject o in objectsFacilty)
            {
                facility = SimulatorCore.AsFacility(o);
                facilities.Add(facility);
            }

            

            foreach (Facility f in facilities)
            {
                bytesStr = System.Text.Encoding.UTF8.GetBytes(f.Id);
                streamFacilities.Write(BitConverter.GetBytes(bytesStr.Length));
                streamFacilities.Write(bytesStr);

                bytesStr = System.Text.Encoding.UTF8.GetBytes(f.Name);
                streamFacilities.Write(BitConverter.GetBytes(bytesStr.Length));
                streamFacilities.Write(bytesStr);

                bytesFloat = BitConverter.GetBytes(f.PowerConsumed);
                streamFacilities.Write(bytesFloat);

                bytesInt = BitConverter.GetBytes(f.Entrances.Count());
                streamFacilities.Write(bytesInt);
                foreach (Point p in f.Entrances)
                {
                    bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Id);
                    streamFacilities.Write(BitConverter.GetBytes(bytesStr.Length));
                    streamFacilities.Write(bytesStr);
                }

                bytesInt = BitConverter.GetBytes(f.Exits.Count());
                streamFacilities.Write(bytesInt);
                foreach (Point p in f.Exits)
                {
                    bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Id);
                    streamFacilities.Write(BitConverter.GetBytes(bytesStr.Length));
                    streamFacilities.Write(bytesStr);
                }
            }

            streamFacilities.Close();

        }

        // Función que guarda el archivo que contiene los datos de 'facilities' en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre (int)
        // Nombre en UTF8
        // capacity persons (int)
        // espacio en bytes del Id del punto1 (int)
        // id del punto1 (str)
        // espacio en bytes del Id del punto2 (int)
        // id del punto2 (str)
        internal void SavePathsFile(string storageId)
        {
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;

            // Guardar Paths
            string scenePathsName = storageId + "paths.dat";
            FileStream streamPaths = new FileStream(scenePathsName, FileMode.Create, FileAccess.Write);

            // Sacamos la lista de paths

            List<SimulatedObject> objectsPath = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Path);
            List<Path> paths = new List<Path>();
            Path path;

            foreach (SimulatedObject o in objectsPath)
            {
                path = SimulatorCore.AsPath(o);
                paths.Add(path);
            }
                      
            
            foreach (Path p in paths)
            {
                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Id);
                streamPaths.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPaths.Write(bytesStr);

                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Name);
                streamPaths.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPaths.Write(bytesStr);

                bytesInt = BitConverter.GetBytes(p.CapacityPersons);
                streamPaths.Write(bytesInt);

                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Point1.Id);
                streamPaths.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPaths.Write(bytesStr);

                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Point2.Id);
                streamPaths.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPaths.Write(bytesStr);
            }

            streamPaths.Close();
        }

        // Función que guarda el archivo que contiene los datos de 'facilities' en un orden concreto
        // Espacio en bytes del Id (int)
        // Id (str)
        // Tamaño de bytes del nombre (int)
        // Nombre en UTF8 (str)
        // age (int)
        // height (float)
        // weight (float)
        // money (float)
        // Tamaño en bytes del id de la facility o la palabra "null" (int)
        // is at facility (? si es null ponemos LA PALABRA "null", si no es null id de la facility)
        // Tamaño en bytes del id del path o la palabra "null" (int)
        // is at path
        internal void SavePersonsFile(string storageId)
        {
            byte[] bytesInt = new byte[sizeof(int)];
            byte[] bytesFloat = new byte[sizeof(float)];
            byte[] bytesStr;
                        
            string scenePersonsName = storageId + "persons.dat";
            FileStream streamPersons = new FileStream(scenePersonsName, FileMode.Create, FileAccess.Write);

            // Sacamos la lista de personas

            List<SimulatedObject> objectsPerson = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Person);
            List<Person> persons = new List<Person>();
            Person person;

            foreach (SimulatedObject o in objectsPerson)
            {
                person = SimulatorCore.AsPerson(o);
                persons.Add(person);
            }                       
            

            foreach (Person p in persons)
            {
                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Id);
                streamPersons.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPersons.Write(bytesStr);

                bytesStr = System.Text.Encoding.UTF8.GetBytes(p.Name);
                streamPersons.Write(BitConverter.GetBytes(bytesStr.Length));
                streamPersons.Write(bytesStr);

                bytesInt = BitConverter.GetBytes(p.Age);
                streamPersons.Write(bytesInt);

                bytesFloat = BitConverter.GetBytes(p.Height);
                streamPersons.Write(bytesFloat);

                bytesFloat = BitConverter.GetBytes(p.Weight);
                streamPersons.Write(bytesFloat);

                bytesFloat = BitConverter.GetBytes(p.Money);
                streamPersons.Write(bytesFloat);

                if (p.IsAtFacility == null)
                {
                    string empty = "null";
                    bytesStr = System.Text.Encoding.UTF8.GetBytes(empty);
                    streamPersons.Write(BitConverter.GetBytes(bytesStr.Length));
                    streamPersons.Write(bytesStr);
                }
                else
                {
                    bytesStr = System.Text.Encoding.UTF8.GetBytes(p.IsAtFacility.Id);
                    streamPersons.Write(BitConverter.GetBytes(bytesStr.Length));
                    streamPersons.Write(bytesStr);
                }

                if (p.IsAtPath == null)
                {
                    string empty = "null";
                    bytesStr = System.Text.Encoding.UTF8.GetBytes(empty);
                    streamPersons.Write(BitConverter.GetBytes(bytesStr.Length));
                    streamPersons.Write(bytesStr);
                }
                else
                {
                    bytesStr = System.Text.Encoding.UTF8.GetBytes(p.IsAtPath.Id);
                    streamPersons.Write(BitConverter.GetBytes(bytesStr.Length));
                    streamPersons.Write(bytesStr);
                }
            }

            streamPersons.Close();
        }

        // Función que añade el nombre de la escena al final del archivo "scenes.dat"
        // Este archivo se crea vacío la primera vez que se inicializa el sistema de guardado
        // Formato: igual que lo anteriores-> tamaño en bytes del string (int) y string en UTF8
        internal void SaveSceneIdFile(string storageId)
        {            
            string scenesFile = "scenes.dat";

            // Primero actualizamos la lista en memoria
            list.Add(storageId);

            // Ahora añadimos el nuevo ID al final del archivo
            FileStream streamScenes = new FileStream(scenesFile, FileMode.Append, FileAccess.Write);

            byte[] bytesStrScene = Encoding.UTF8.GetBytes(storageId);
            streamScenes.Write(BitConverter.GetBytes(bytesStrScene.Length));
            streamScenes.Write(bytesStrScene);

            streamScenes.Close();
        }

        // Cargamos la serie de archivos que componen una escena, es importante el orden, ya que algunas estructuras dependen de otras.
        internal override void LoadScene(string storageId)
        {
            //Console.WriteLine("BinaryFileStorage: Load simulation" + storageId);
            
            LoadPointsFile(storageId);

            LoadFacilitiesFile(storageId);
            
            LoadPathsFile(storageId);

            LoadPersonsFile(storageId);

        }

        // Guardamos la serie de archivos que componen una escena
        // El nombre de cada archivo siempre se compone de "nombreEscena" + "points.dat", cambiando points por facilities, paths... (ver cada una de las funciones)
        internal override void SaveScene(string storageId)
        {
            //Console.WriteLine("BinaryFileStorage: Save simulation " + storageId);

            SavePointsFile(storageId);

            SaveFacilitiesFile(storageId);

            SavePathsFile(storageId);

            SavePersonsFile(storageId);

            SaveSceneIdFile(storageId);

        }


        internal override void DeleteScene(string storageId)
        {
            // Primero la quitamos de la lista en memoria
            list.Remove(storageId);

            // Ahora reescribimos el archivo entero basándonos el la lista que tiene todos los archivos de guardado menos el que hemos quitado
            string scenesFile = "scenes.dat";

            FileStream stream = new FileStream(scenesFile, FileMode.Create, FileAccess.Write);

            foreach (string id in list)
            {
                byte[] bytesStr = Encoding.UTF8.GetBytes(id);

                // Primero el tamaño
                stream.Write(BitConverter.GetBytes(bytesStr.Length));

                // Luego el string en UTF8
                stream.Write(bytesStr);
            }

            stream.Close();

            // eliminamos los archivos asociados a la escena eliminada siguiendo el mismo patrón de nombre
                        
            File.Delete(storageId + "points.dat");
            File.Delete(storageId + "persons.dat");
            File.Delete(storageId + "facilities.dat");
            File.Delete(storageId + "paths.dat");
        }

        internal override List<string> ListScenes()
        {
            return list;list = new List<string>();
        }


    }
}
