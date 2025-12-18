using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace GemeloDigital
{
    internal class Cargarguardar : Storage
    {
        List<string> list = new List<string>();

        internal override void Initialize()
        {
            list = new List<string>();
            string currentFolder = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(currentFolder, "*.dat");

            foreach (string file in files)
            {
                string cleanName = System.IO.Path.GetFileNameWithoutExtension(file);
                list.Add(cleanName);
            }

            Console.WriteLine("Sistema de guardado inicializado." + list.Count + "escenas encontradas.");
        }

        internal override void Finish()
        {
            Console.WriteLine("DummyStorage: Finish");
        }

        // --- MÉTODO AUXILIAR ---
        private string GetCorrectFilename(string storageId)
        {
            if (storageId.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
            {
                return storageId;
            }
            return storageId + ".dat";
        }

        internal override void LoadScene(string storageId)
        {
            Console.WriteLine("DummyStorage: Load simulation " + storageId);

            string filename = GetCorrectFilename(storageId);

            if (!File.Exists(filename))
            {
                Console.WriteLine("El archivo no existe en: " + System.IO.Path.GetFullPath(filename));
                return;
            }

            SimulatorCore.NewScene();

            FileStream fichero = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] bytes;

            // 1. LEER POINTS
            bytes = new byte[sizeof(int)]; fichero.Read(bytes);
            int pointCount = BitConverter.ToInt32(bytes);

            Dictionary<string, Point> pointsById = new Dictionary<string, Point>();

            for (int i = 0; i < pointCount; i++)
            {
                Point po = SimulatorCore.CreatePoint();
                // Id
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int idLen = BitConverter.ToInt32(bytes);
                bytes = new byte[idLen]; fichero.Read(bytes);
                string id = Encoding.UTF8.GetString(bytes);
                // Name
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int nameLen = BitConverter.ToInt32(bytes);
                bytes = new byte[nameLen];
                fichero.Read(bytes);
                po.Name = Encoding.UTF8.GetString(bytes);
                // Pos
                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float x = BitConverter.ToSingle(bytes);
                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float y = BitConverter.ToSingle(bytes);
                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float z = BitConverter.ToSingle(bytes);
                po.Position = new Vector3(x, y, z);
                pointsById[id] = po;
            }

            // 2. FACILITIES
            bytes = new byte[sizeof(int)]; fichero.Read(bytes);
            int facilityCount = BitConverter.ToInt32(bytes);
            Dictionary<string, Facility> facilitiesById = new Dictionary<string, Facility>();

            for (int i = 0; i < facilityCount; i++)
            {
                // Id
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int idLen = BitConverter.ToInt32(bytes);
                bytes = new byte[idLen];
                fichero.Read(bytes);
                string id = Encoding.UTF8.GetString(bytes);
                // Name
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int nameLen = BitConverter.ToInt32(bytes);
                bytes = new byte[nameLen];
                fichero.Read(bytes);
                string name = Encoding.UTF8.GetString(bytes);
                // Power
                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                float power = BitConverter.ToSingle(bytes);
                // Entrances
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int entCount = BitConverter.ToInt32(bytes);
                List<Point> entrances = new List<Point>();
                for (int j = 0; j < entCount; j++)
                {
                    bytes = new byte[sizeof(int)];
                    fichero.Read(bytes);
                    int pIdLen = BitConverter.ToInt32(bytes);
                    bytes = new byte[pIdLen];
                    fichero.Read(bytes);
                    string pid = Encoding.UTF8.GetString(bytes);
                    if (pointsById.ContainsKey(pid)) entrances.Add(pointsById[pid]);
                }
                // Exits
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int extCount = BitConverter.ToInt32(bytes);
                List<Point> exits = new List<Point>();
                for (int j = 0; j < extCount; j++)
                {
                    bytes = new byte[sizeof(int)];
                    fichero.Read(bytes);
                    int pIdLen = BitConverter.ToInt32(bytes);
                    bytes = new byte[pIdLen];
                    fichero.Read(bytes);
                    string pid = Encoding.UTF8.GetString(bytes);
                    if (pointsById.ContainsKey(pid)) exits.Add(pointsById[pid]);
                }

                if (entrances.Count > 0 && exits.Count > 0)
                {
                    Facility f = SimulatorCore.CreateFacility(entrances[0], exits[0]);
                    f.Name = name; f.PowerConsumed = power;
                    for (int j = 1; j < entrances.Count; j++) f.Entrances.Add(entrances[j]);
                    for (int j = 1; j < exits.Count; j++) f.Exits.Add(exits[j]);
                    facilitiesById[id] = f;
                }
            }

            // 3. PATHS
            bytes = new byte[sizeof(int)]; fichero.Read(bytes);
            int pathCount = BitConverter.ToInt32(bytes);
            Dictionary<string, Path> pathsById = new Dictionary<string, Path>();
            for (int i = 0; i < pathCount; i++)
            {
                // Id
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int idLen = BitConverter.ToInt32(bytes);
                bytes = new byte[idLen];
                fichero.Read(bytes);
                string id = Encoding.UTF8.GetString(bytes);
                // Name
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int nameLen = BitConverter.ToInt32(bytes);
                bytes = new byte[nameLen];
                fichero.Read(bytes);
                string name = Encoding.UTF8.GetString(bytes);
                // P1
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int p1Len = BitConverter.ToInt32(bytes);
                bytes = new byte[p1Len];
                fichero.Read(bytes);
                string p1Id = Encoding.UTF8.GetString(bytes);
                // P2
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int p2Len = BitConverter.ToInt32(bytes);
                bytes = new byte[p2Len];
                fichero.Read(bytes);
                string p2Id = Encoding.UTF8.GetString(bytes);
                // Cap
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int cap = BitConverter.ToInt32(bytes);

                if (pointsById.ContainsKey(p1Id) && pointsById.ContainsKey(p2Id))
                {
                    Path pa = SimulatorCore.CreatePath(pointsById[p1Id], pointsById[p2Id]);
                    pa.Name = name;
                    pa.CapacityPersons = cap;
                    pathsById[id] = pa;
                }
            }

            // 4. PERSONS
            bytes = new byte[sizeof(int)];
            fichero.Read(bytes);
            int personCount = BitConverter.ToInt32(bytes);
            for (int i = 0; i < personCount; i++)
            {
                Person p = SimulatorCore.CreatePerson();
                // Id
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int idLen = BitConverter.ToInt32(bytes);
                bytes = new byte[idLen];
                fichero.Read(bytes);
                // Name
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int nameLen = BitConverter.ToInt32(bytes);
                bytes = new byte[nameLen];
                fichero.Read(bytes);
                p.Name = Encoding.UTF8.GetString(bytes);
                // Stats
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                p.Age = BitConverter.ToInt32(bytes);
                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                p.Height = BitConverter.ToSingle(bytes);
                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                p.Weight = BitConverter.ToSingle(bytes);
                bytes = new byte[sizeof(float)];
                fichero.Read(bytes);
                p.Money = BitConverter.ToSingle(bytes);
                // Fac
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int fIdLen = BitConverter.ToInt32(bytes);
                if (fIdLen > 0)
                {
                    bytes = new byte[fIdLen];
                    fichero.Read(bytes);
                    string fid = Encoding.UTF8.GetString(bytes);
                    if (facilitiesById.ContainsKey(fid)) p.IsAtFacility = facilitiesById[fid];
                }
                // Path
                bytes = new byte[sizeof(int)];
                fichero.Read(bytes);
                int paIdLen = BitConverter.ToInt32(bytes);
                if (paIdLen > 0)
                {
                    bytes = new byte[paIdLen];
                    fichero.Read(bytes);
                    string paid = Encoding.UTF8.GetString(bytes);
                    if (pathsById.ContainsKey(paid)) p.IsAtPath = pathsById[paid];
                }
            }

            fichero.Close();
            Console.WriteLine("Escena cargada correctamente desde: " + System.IO.Path.GetFullPath(filename));
        }

        internal override void SaveScene(string storageId)
        {
            Console.WriteLine("Grupo 6: Save simulation " + storageId);
            string filename = GetCorrectFilename(storageId);
            Console.WriteLine("Guardando archivo en: " + System.IO.Path.GetFullPath(filename));

            FileStream fichero = new FileStream(filename, FileMode.Create, FileAccess.Write);
            byte[] bytes;

            List<SimulatedObject> allObjects = SimulatorCore.GetObjects();
            List<Point> points = new List<Point>();
            List<Facility> facilities = new List<Facility>();
            List<Path> paths = new List<Path>();
            List<Person> persons = new List<Person>();

            foreach (var obj in allObjects)
            {
                if (obj.Type == SimulatedObjectType.Point) points.Add(SimulatorCore.AsPoint(obj));
                else if (obj.Type == SimulatedObjectType.Facility) facilities.Add(SimulatorCore.AsFacility(obj));
                else if (obj.Type == SimulatedObjectType.Path) paths.Add(SimulatorCore.AsPath(obj));
                else if (obj.Type == SimulatedObjectType.Person) persons.Add(SimulatorCore.AsPerson(obj));
            }

            // Guardar Points
            bytes = BitConverter.GetBytes(points.Count); fichero.Write(bytes);
            foreach (var po in points)
            {
                byte[] idBytes = Encoding.UTF8.GetBytes(po.Id);
                fichero.Write(BitConverter.GetBytes(idBytes.Length));
                fichero.Write(idBytes);
                byte[] nameBytes = Encoding.UTF8.GetBytes(po.Name);
                fichero.Write(BitConverter.GetBytes(nameBytes.Length));
                fichero.Write(nameBytes);
                fichero.Write(BitConverter.GetBytes(po.Position.X));
                fichero.Write(BitConverter.GetBytes(po.Position.Y));
                fichero.Write(BitConverter.GetBytes(po.Position.Z));
            }
            // Guardar Facilities
            bytes = BitConverter.GetBytes(facilities.Count);
            fichero.Write(bytes);
            foreach (var f in facilities)
            {
                byte[] idBytes = Encoding.UTF8.GetBytes(f.Id);
                fichero.Write(BitConverter.GetBytes(idBytes.Length));
                fichero.Write(idBytes);
                byte[] nameBytes = Encoding.UTF8.GetBytes(f.Name);
                fichero.Write(BitConverter.GetBytes(nameBytes.Length));
                fichero.Write(nameBytes);
                fichero.Write(BitConverter.GetBytes(f.PowerConsumed));
                fichero.Write(BitConverter.GetBytes(f.Entrances.Count));
                foreach (var ent in f.Entrances)
                {
                    byte[] pid = Encoding.UTF8.GetBytes(ent.Id);
                    fichero.Write(BitConverter.GetBytes(pid.Length));
                    fichero.Write(pid);
                }
                fichero.Write(BitConverter.GetBytes(f.Exits.Count));
                foreach (var ext in f.Exits)
                {
                    byte[] pid = Encoding.UTF8.GetBytes(ext.Id);
                    fichero.Write(BitConverter.GetBytes(pid.Length));
                    fichero.Write(pid);
                }
            }
            // Guardar Paths
            bytes = BitConverter.GetBytes(paths.Count);
            fichero.Write(bytes);
            foreach (var pa in paths)
            {
                byte[] idBytes = Encoding.UTF8.GetBytes(pa.Id);
                fichero.Write(BitConverter.GetBytes(idBytes.Length));
                fichero.Write(idBytes);
                byte[] nameBytes = Encoding.UTF8.GetBytes(pa.Name);
                fichero.Write(BitConverter.GetBytes(nameBytes.Length));
                fichero.Write(nameBytes);
                byte[] p1 = Encoding.UTF8.GetBytes(pa.Point1.Id);
                fichero.Write(BitConverter.GetBytes(p1.Length));
                fichero.Write(p1);
                byte[] p2 = Encoding.UTF8.GetBytes(pa.Point2.Id);
                fichero.Write(BitConverter.GetBytes(p2.Length));
                fichero.Write(p2);
                fichero.Write(BitConverter.GetBytes(pa.CapacityPersons));
            }
            // Guardar Persons
            bytes = BitConverter.GetBytes(persons.Count);
            fichero.Write(bytes);
            foreach (var p in persons)
            {
                byte[] idBytes = Encoding.UTF8.GetBytes(p.Id);
                fichero.Write(BitConverter.GetBytes(idBytes.Length));
                fichero.Write(idBytes);
                byte[] nameBytes = Encoding.UTF8.GetBytes(p.Name);
                fichero.Write(BitConverter.GetBytes(nameBytes.Length));
                fichero.Write(nameBytes);
                fichero.Write(BitConverter.GetBytes(p.Age));
                fichero.Write(BitConverter.GetBytes(p.Height));
                fichero.Write(BitConverter.GetBytes(p.Weight));
                fichero.Write(BitConverter.GetBytes(p.Money));
                string fid = (p.IsAtFacility != null) ? p.IsAtFacility.Id : ""; byte[] fBytes = Encoding.UTF8.GetBytes(fid);
                fichero.Write(BitConverter.GetBytes(fBytes.Length));
                if (fBytes.Length > 0) fichero.Write(fBytes);
                string pid = (p.IsAtPath != null) ? p.IsAtPath.Id : ""; byte[] pBytes = Encoding.UTF8.GetBytes(pid);
                fichero.Write(BitConverter.GetBytes(pBytes.Length));
                if (pBytes.Length > 0) fichero.Write(pBytes);
            }

            fichero.Close();

            // Actualizamos la lista
            string cleanName = storageId;
            if (storageId.EndsWith(".dat")) cleanName = storageId.Substring(0, storageId.Length - 4);

            if (!list.Contains(cleanName))
            {
                list.Add(cleanName);
            }

            Console.WriteLine("Escena guardada correctamente");
        }

        internal override void DeleteScene(string storageId)
        {
            Console.WriteLine("Deleting simulation " + storageId);
            list.Remove(storageId);
            string filename = GetCorrectFilename(storageId);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        internal override List<string> ListScenes()
        {
            return list;
        }
    }
}