using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal class piggieStorage : Storage
    {
        List<string> list = new List<string>();
        List<SimulatedObject> objects;

        public string currentDirectory = Directory.GetCurrentDirectory();
        

        internal override void Initialize()
        {
            

        }
        internal override void Finish()
        {
            
        }
        internal override void newScene()
        {

        }
        internal override void LoadScene(string storageId)
        {


            List<Path> paths = new List<Path>();
            List<Point> points = new List<Point>();
            List<Facility> facilities = new List<Facility>();
            List<Person> persons = new List<Person>();

            FileStream fichero = new FileStream(storageId + "\\Personas", FileMode.Create, FileAccess.Write);

            byte[] bytes = new byte[sizeof(int)];

            int vueltasPersonas = BitConverter.ToInt32(bytes);

            for (int o = 0; o < vueltasPersonas; o++)
            {

                fichero.Read(bytes);
                int cantidadDeLetras = BitConverter.ToInt32(bytes);

                bytes = new byte[cantidadDeLetras];
                fichero.Read(bytes);
                string nombre = Encoding.ASCII.GetString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int age = BitConverter.ToInt32(bytes);

                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float height = BitConverter.ToSingle(bytes);

                fichero.Read(bytes);
                float weight = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(bool)];
                fichero.Read(bytes);
                bool isAtFacility = BitConverter.ToBoolean(bytes);
                string? idFacility = null;

                if (isAtFacility)
                {
                    bytes = new byte[sizeof(int)];
                    fichero.Read(bytes);
                    int letrasCant = BitConverter.ToInt32(bytes);

                    bytes = new byte[letrasCant];
                    fichero.Read(bytes);
                    idFacility = Encoding.ASCII.GetString(bytes);
                }

                bytes = new byte[sizeof(bool)];
                fichero.Read(bytes);
                bool isAtPath = BitConverter.ToBoolean(bytes);
                string? idPath = null;

                if (isAtPath)
                {
                    bytes = new byte[sizeof(int)];
                    fichero.Read(bytes);
                    int letrasCant = BitConverter.ToInt32(bytes);

                    bytes = new byte[letrasCant];
                    fichero.Read(bytes);
                    idPath = Encoding.ASCII.GetString(bytes);
                }

                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float money = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int length = BitConverter.ToInt32(bytes);

                fichero.Read(bytes);
                int cantLetras = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetras];
                fichero.Read(bytes);
                string idPersona = Encoding.ASCII.GetString(bytes);

                fichero.Close();

                Person temp = SimulatorCore.CreatePersonWithId(idPersona);
                temp.Name = nombre;
                temp.Age = age;
                temp.Height = height;
                temp.Weight = weight;
                temp.IsAtFacility.Id = idFacility;
                temp.IsAtPath.Id = idPath;
                temp.Money = money;

                persons.Add(temp);
            }

            fichero = new FileStream(storageId + "\\Puntos", FileMode.Create, FileAccess.Write);

            bytes = new byte[sizeof(int)];
            fichero.Read(bytes);
            int vueltasPuntos = BitConverter.ToInt32(bytes);

            for (int o = 0; o < vueltasPuntos; o++)
            {

                fichero.Read(bytes);
                int cantLetrasNombrePuntos = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasNombrePuntos];
                fichero.Read(bytes);
                string nombrePuntos = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cantLetrasIdPuntos = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasNombrePuntos];
                fichero.Read(bytes);
                string idPuntos = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float posX = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float posY = BitConverter.ToSingle(bytes);

                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float posZ = BitConverter.ToSingle(bytes);

                Point point = SimulatorCore.CreatePointWithId(idPuntos);
                point.Name = nombrePuntos;
                Vector3 posicion = new Vector3();

                posicion.X = posX;
                posicion.Y = posY;
                posicion.Z = posZ;

                point.Position = posicion;

                points.Add(point);

                fichero.Close();
            }

            fichero = new FileStream(storageId + "\\Paths", FileMode.Create, FileAccess.Write);

            bytes = new byte[sizeof(int)];
            fichero.Read(bytes);
            int vueltasPaths = BitConverter.ToInt32(bytes);

            for (int i = 0; i < vueltasPaths; i++)
            {
                fichero.Read(bytes);
                int cantLetrasNombrePaths = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasNombrePaths];
                fichero.Read(bytes);
                string nombrePaths = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cantLetrasIdPaths = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasIdPaths];
                fichero.Read(bytes);
                string idPaths = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int CapacityPersons = BitConverter.ToInt32(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cantLetrasNombrePoint1 = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasNombrePoint1];
                fichero.Read(bytes);
                string idPoint1 = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cantLetrasNombrePoint2 = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasNombrePoint2];
                fichero.Read(bytes);
                string idPoint2 = BitConverter.ToString(bytes);

                int idPunto1FromList = points.FindIndex(x => x.Id == idPoint1);
                int idPunto2FromList = points.FindIndex(x => x.Id == idPoint2);

                Path temp = SimulatorCore.CreatePath(points[idPunto1FromList], points[idPunto2FromList]);

                temp.Name = nombrePaths;
                temp.CapacityPersons = CapacityPersons;
                temp.Id = idPaths;
                
                paths.Add(temp);
            }

            fichero.Close();

            fichero = new FileStream(storageId + "\\Facilities", FileMode.Create, FileAccess.Write);

            bytes = new byte[sizeof(int)];
            fichero.Read(bytes);
            int vueltasFacilities = BitConverter.ToInt32(bytes); 

            for (int i = 0; i < vueltasFacilities; i++) 
            {
                fichero.Read(bytes);
                int cantLetrasNombreFacility = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasNombreFacility];
                fichero.Read(bytes);
                string nombreFacility = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cantLetrasIdFacility = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasIdFacility];
                fichero.Read(bytes);
                string IdFacility = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cantLetrasIdFacilityPoint1 = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasIdFacilityPoint1];
                fichero.Read(bytes);
                string IdFacilityPoint1 = BitConverter.ToString(bytes);

                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cantLetrasIdFacilityPoint2 = BitConverter.ToInt32(bytes);

                bytes = new byte[cantLetrasIdFacilityPoint2];
                fichero.Read(bytes);
                string IdFacilityPoint2 = BitConverter.ToString(bytes);

                int idPunto1FromList = points.FindIndex(x => x.Id == IdFacilityPoint1);
                int idPunto2FromList = points.FindIndex(x => x.Id == IdFacilityPoint2);
                
                Facility temp = SimulatorCore.CreateFacilityWithId(IdFacility, points[idPunto1FromList], points[idPunto2FromList]);

                temp.Name = nombreFacility;

                facilities.Add(temp);
            }

            fichero.Close();

        }

        internal override void SaveScene(string storageId)
        {
            List<Path> paths = new List<Path>();
            List<Point> points = new List<Point>();
            List<Facility> facilities = new List<Facility>();
            List<Person> persons = new List<Person>();

            objects = SimulatorCore.GetObjects();
            Directory.CreateDirectory(currentDirectory + "\\" + storageId);

            for (int i = 0; i < objects.Count; i++) 
            {
                if (objects[i].Type == SimulatedObjectType.Facility) 
                {
                    Facility facility;
                    Point pointTemp = new Point();
                    Facility temp = new Facility(pointTemp, pointTemp);

                    facility = SimulatorCore.AsFacility(objects[i]);

                    temp.Id = facility.Id;
                    temp.Name = facility.Name;
                    temp.Entrances = facility.Entrances;
                    temp.Exits = facility.Exits;
                    temp.PowerConsumed = facility.PowerConsumed;

                    facilities.Add(temp);

                }
                else if (objects[i].Type == SimulatedObjectType.Person)
                {
                    Person person;
                    Person temp = new Person();

                    person = SimulatorCore.AsPerson(objects[i]);

                    temp.Name = person.Name;
                    temp.Age = person.Age;
                    temp.Height = person.Height;
                    temp.Weight = person.Weight;
                    temp.IsAtFacility = person.IsAtFacility;
                    temp.IsAtPath = person.IsAtPath;
                    temp.Money = person.Money;
                    temp.Id = person.Id;

                    persons.Add(temp);

                }
                else if (objects[i].Type == SimulatedObjectType.Point)
                {
                    Point point;
                    Point temp = new Point();

                    point = SimulatorCore.AsPoint(objects[i]);

                    temp.Name = point.Name;
                    temp.Id = point.Id;
                    temp.Position = point.Position;

                    points.Add(temp);

                }
                else if (objects[i].Type == SimulatedObjectType.Path)
                {
                    Path path;
                    Point pointTemp = new Point();
                    Path temp = new Path(pointTemp, pointTemp);

                    path = SimulatorCore.AsPath(objects[i]);

                    temp.Name = path.Name;
                    temp.Id = path.Id;
                    temp.CapacityPersons = path.CapacityPersons;
                    temp.Point1.Id = path.Point1.Id;
                    temp.Point2.Id = path.Point2.Id;

                    paths.Add(temp);
                }

            }

            FileStream fichero = new FileStream(storageId + "\\Personas", FileMode.Create, FileAccess.Write);

            for (int i = 0; i < persons.Count; i++) 
            {
                byte[] bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(persons.Count);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                int length = persons[i].Name.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(persons[i].Name);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(persons[i].Age);
                fichero.Write(bytes);

                bytes = new byte[sizeof(float)];
                bytes = BitConverter.GetBytes(persons[i].Height);
                fichero.Write(bytes);

                bytes = new byte[sizeof(float)];
                bytes = BitConverter.GetBytes(persons[i].Weight);
                fichero.Write(bytes);

                bool isAtFacilityBool;
                if (persons[i].IsAtFacility != null)
                {
                    isAtFacilityBool = true;
                    bytes = new byte[sizeof(bool)];
                    bytes = BitConverter.GetBytes(isAtFacilityBool);
                    fichero.Write(bytes);

                    bytes = new byte[sizeof(int)];
                    length = persons[i].IsAtFacility.Id.Length;
                    bytes = BitConverter.GetBytes(length);
                    fichero.Write(bytes);

                    bytes = new byte[length];
                    bytes = Encoding.ASCII.GetBytes(persons[i].IsAtFacility.Id);
                    fichero.Write(bytes);
                }
                else 
                {
                    isAtFacilityBool = false;
                    bytes = new byte[sizeof(bool)];
                    bytes = BitConverter.GetBytes(isAtFacilityBool);
                    fichero.Write(bytes);
                }

                if (persons[i].IsAtPath != null)
                {
                    isAtFacilityBool = true;
                    bytes = new byte[sizeof(bool)];
                    bytes = BitConverter.GetBytes(isAtFacilityBool);
                    fichero.Write(bytes);

                    bytes = new byte[sizeof(int)];
                    length = persons[i].IsAtPath.Id.Length;
                    bytes = BitConverter.GetBytes(length);
                    fichero.Write(bytes);

                    bytes = new byte[length];
                    bytes = Encoding.ASCII.GetBytes(persons[i].IsAtPath.Id);
                    fichero.Write(bytes);
                }
                else 
                {
                    isAtFacilityBool = false;
                    bytes = new byte[sizeof(bool)];
                    bytes = BitConverter.GetBytes(isAtFacilityBool);
                    fichero.Write(bytes);
                }

                bytes = new byte[sizeof(float)];
                bytes = BitConverter.GetBytes(persons[i].Money);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                length = persons[i].Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(persons[i].Id);
                fichero.Write(bytes);
            }

            fichero.Close();

            fichero = new FileStream(storageId + "\\Puntos", FileMode.Create, FileAccess.Write);

            for (int i = 0; i < points.Count; i++) 
            {
                byte[] bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(points.Count);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                int length = points[i].Name.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(points[i].Name);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                length = points[i].Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(points[i].Id);
                fichero.Write(bytes);

                bytes = new byte[sizeof(float)];
                bytes = BitConverter.GetBytes(points[i].Position.X);
                fichero.Write(bytes);

                bytes = new byte[sizeof(float)];
                bytes = BitConverter.GetBytes(points[i].Position.Y);
                fichero.Write(bytes);

                bytes = new byte[sizeof(float)];
                bytes = BitConverter.GetBytes(points[i].Position.Z);
                fichero.Write(bytes);

            }

            fichero.Close();

            

            fichero = new FileStream(storageId + "\\Paths", FileMode.Create, FileAccess.Write);

            for (int i = 0; i< paths.Count; i++)
            {
                byte[] bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(paths.Count);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                int length = paths[i].Name.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(paths[i].Name);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                length = paths[i].Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(paths[i].Id);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(paths[i].CapacityPersons);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                length = paths[i].Point1.Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(paths[i].Point1.Id);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                length = paths[i].Point2.Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(paths[i].Point2.Id);
                fichero.Write(bytes);
            }

            fichero.Close();

            fichero = new FileStream(storageId + "\\Facilities", FileMode.Create, FileAccess.Write);

            for (int i = 0; i < facilities.Count; i++) 
            {
                byte[] bytes = new byte[sizeof(int)];
                bytes = BitConverter.GetBytes(facilities.Count);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                int length = facilities[i].Name.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(facilities[i].Name);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                length = facilities[i].Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(facilities[i].Id);
                fichero.Write(bytes);


                bytes = new byte[sizeof(int)];
                int tamaño = facilities[i].Exits.Count;
                length = facilities[i].Exits[tamaño - 1].Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(facilities[i].Exits[tamaño - 1].Id);
                fichero.Write(bytes);

                bytes = new byte[sizeof(int)];
                tamaño = facilities[i].Entrances.Count;
                length = facilities[i].Entrances[tamaño - 1].Id.Length;
                bytes = BitConverter.GetBytes(length);
                fichero.Write(bytes);

                bytes = new byte[length];
                bytes = Encoding.ASCII.GetBytes(facilities[i].Entrances[tamaño - 1].Id);
                fichero.Write(bytes);
            }

            fichero.Close();

            list.Add(storageId);
        }

        internal override void DeleteScene(string storageId)
        {
            list.Remove(storageId);
        }

        internal override List<string> ListScenes()
        {
            string[] nombres = Directory.GetDirectories(currentDirectory);
            foreach(string item in nombres) 
            {
                string[] parts = item.Split("\\");
                list.Add(parts[parts.Length -1]);
            }
            return list;
        }

    }
}
