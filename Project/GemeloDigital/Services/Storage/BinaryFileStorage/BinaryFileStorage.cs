using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal class BinaryFileStorage : Storage
    {
        const string basePath = "Storage";
        const string extension = "park";

        internal override void Initialize()
        {
            if(!Directory.Exists(basePath)) { Directory.CreateDirectory(basePath); }

            Console.WriteLine("BinaryFileStorage: Init");
        }

        internal override void Finish()
        {
            Console.WriteLine("BinaryFileStorage: Finish");
        }
        
        internal override void LoadScene(string storageId)
        {
            SimulatorCore.NewScene();

            FileStream file = new FileStream(basePath + "\\" + storageId + "." + extension,
                                FileMode.Open, FileAccess.Read);

            // Textures

            int objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Texture t = SimulatorCore.CreateTextureWithId(id);
                t.Name = ReadString(file);
                t.ResourceId = ReadString(file);
            }

            // Shaders

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Shader s = SimulatorCore.CreateShaderWithId(id);
                s.Name = ReadString(file);
                s.ResourceId = ReadString(file);
            }

            // Materials

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Material m = SimulatorCore.CreateMaterialWithId(id);
                m.Name = ReadString(file);

                m.Color = ReadVector3(file);
                m.Texture = SimulatorCore.AsTexture(ReadReference(file));
                m.Shader = SimulatorCore.AsShader(ReadReference(file));
            }

            // Model

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Model m = SimulatorCore.CreateModelWithId(id);
                m.Name = ReadString(file);
                m.ResourceId = ReadString(file);
            }

            // Object 3D

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Object3D o3D = SimulatorCore.CreateObject3DWithId(id);
                o3D.Name = ReadString(file);

                o3D.Position = ReadVector3(file);
                o3D.Rotation = ReadVector3(file);
                o3D.Scale = ReadVector3(file);

                o3D.Material = SimulatorCore.AsMaterial(ReadReference(file));
                o3D.Model = SimulatorCore.AsModel(ReadReference(file));
            }

            // Camera 3D

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Camera3D c3D = SimulatorCore.CreateCamera3DWithId(id);
                c3D.Name = ReadString(file);

                c3D.Position = ReadVector3(file);
                c3D.Rotation = ReadVector3(file);

                c3D.FOV = ReadFloat(file);
                c3D.ZNear = ReadFloat(file);
                c3D.ZFar = ReadFloat(file);
            }

            // Points

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Point p = SimulatorCore.CreatePointWithId(id);
                p.Name = ReadString(file);

                p.Position = ReadVector3(file);
            }

            // Paths

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                
                string name = ReadString(file);

                Point point1 = SimulatorCore.AsPoint(ReadReference(file));
                Point point2 = SimulatorCore.AsPoint(ReadReference(file));

                int capacity = ReadInt(file);

                Path p = SimulatorCore.CreatePathWithId(id, point1, point2);
                p.Name = name;
                p.CapacityPersons = capacity;
            }

            // Facilities

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                string name = ReadString(file);
                List<Point> entrances = ObjectsToPoints(ReadReferenceList(file));
                List<Point> exits = ObjectsToPoints(ReadReferenceList(file));

                float power = ReadFloat(file);

                Facility f = SimulatorCore.CreateFacilityWithId(id, entrances[0], exits[0]);
                f.Name = name;
                f.Entrances = entrances;
                f.Exits = exits;
                f.PowerConsumed = power;


            }

            // Persons

            objCount = ReadInt(file);

            for(int i = 0; i < objCount; i++)
            {
                string id = ReadString(file);
                Person p = SimulatorCore.CreatePersonWithId(id);
                p.Name = ReadString(file);

                p.IsAtPath = SimulatorCore.AsPath(ReadReference(file));
                p.IsAtFacility= SimulatorCore.AsFacility(ReadReference(file));

                p.Age = ReadInt(file);
                p.Height = ReadFloat(file);
                p.Weight = ReadFloat(file);
                p.Money = ReadFloat(file);

            }

            file.Close();

        }

        internal override void SaveScene(string storageId)
        {
            FileStream file = new FileStream(basePath + "\\" + storageId + "." + extension,
                                FileMode.Create, FileAccess.Write);

            // Textures

            List<SimulatedObject> objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Texture);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Texture t = SimulatorCore.AsTexture(o);

                WriteString(t.Id, file);
                WriteString(t.Name, file);
                WriteString(t.ResourceId, file);
            }

            // Shaders

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Shader);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Shader s = SimulatorCore.AsShader(o);

                WriteString(s.Id, file);
                WriteString(s.Name, file);
                WriteString(s.ResourceId, file);
            }

            // Materials

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Material);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Material m = SimulatorCore.AsMaterial(o);

                WriteString(m.Id, file);
                WriteString(m.Name, file);
                WriteVector3(m.Color, file);
                WriteReference(m.Texture, file);
                WriteReference(m.Shader, file);
            }

            // Model

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Model);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Model m = SimulatorCore.AsModel(o);

                WriteString(m.Id, file);
                WriteString(m.Name, file);
                WriteString(m.ResourceId, file);
            }

            // Object 3D

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Object3D);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Object3D o3D = SimulatorCore.AsObject3D(o);

                WriteString(o3D.Id, file);
                WriteString(o3D.Name, file);

                WriteVector3(o3D.Position, file);
                WriteVector3(o3D.Rotation, file);
                WriteVector3(o3D.Scale, file);

                WriteReference(o3D.Material, file);
                WriteReference(o3D.Model, file);
            }

            // Camera 3D

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Camera3D);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Camera3D c3D = SimulatorCore.AsCamera3D(o);

                WriteString(c3D.Id, file);
                WriteString(c3D.Name, file);

                WriteVector3(c3D.Position, file);
                WriteVector3(c3D.Rotation, file);

                WriteFloat(c3D.FOV, file);
                WriteFloat(c3D.ZNear, file);
                WriteFloat(c3D.ZFar, file);
            }

            // Points

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Point);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Point p = SimulatorCore.AsPoint(o);

                WriteString(p.Id, file);
                WriteString(p.Name, file);

                WriteVector3(p.Position, file);
            }

            // Paths

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Path);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Path p = SimulatorCore.AsPath(o);

                WriteString(p.Id, file);
                WriteString(p.Name, file);

                WriteReference(p.Point1, file);
                WriteReference(p.Point2, file);

                WriteInt(p.CapacityPersons, file);
            }

            // Facilities

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Facility);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Facility f = SimulatorCore.AsFacility(o);

                WriteString(f.Id, file);
                WriteString(f.Name, file);

                WriteReferenceList(PointsToObjects(f.Entrances), file);
                WriteReferenceList(PointsToObjects(f.Exits), file);

                WriteFloat(f.PowerConsumed, file);
            }

            // Persons

            objects = SimulatorCore.FindObjectsOfType(SimulatedObjectType.Person);

            WriteInt(objects.Count, file);

            foreach(SimulatedObject o in objects)
            {
                Person p = SimulatorCore.AsPerson(o);

                WriteString(p.Id, file);
                WriteString(p.Name, file);

                WriteReference(p.IsAtPath, file);
                WriteReference(p.IsAtFacility, file);

                WriteInt(p.Age, file);
                WriteFloat(p.Height, file);
                WriteFloat(p.Weight, file);
                WriteFloat(p.Money, file);

            }
            
            file.Close();
        }

        internal override void DeleteScene(string storageId)
        {
            string path = basePath + "\\" + storageId + "." + extension;
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        internal override List<string> ListScenes()
        {
            List<string> paths = new List<string>(Directory.GetFiles(basePath, "*." + extension));
            List<string> ids = new List<string>();

            foreach(string p in paths)
            {
                ids.Add(System.IO.Path.GetFileNameWithoutExtension(p));
            }
            
            return ids;
        }

        internal List<SimulatedObject> PointsToObjects(List<Point> points)
        {
            List<SimulatedObject> result = new List<SimulatedObject>();
            foreach(Point p in points) { result.Add(p); }
            return result;
        }

        internal List<Point> ObjectsToPoints(List<SimulatedObject> objects)
        {
            List<Point> result = new List<Point>();
            foreach(SimulatedObject o in objects) { result.Add(SimulatorCore.AsPoint(o)); }
            return result;
        }


        internal void WriteReference(SimulatedObject obj, FileStream file)
        {
            if(obj == null) { WriteString("", file); }
            else { WriteString(obj.Id, file); }
        }

        internal void WriteReferenceList(List<SimulatedObject> objs, FileStream file)
        {
            WriteInt(objs.Count, file);
            foreach(SimulatedObject obj in objs)
            {
                WriteReference(obj, file);
            }
        }

        internal void WriteVector3(Vector3 data, FileStream file)
        {
            WriteFloat(data.X, file);
            WriteFloat(data.Y, file);
            WriteFloat(data.Z, file);
        }

        internal void WriteBool(bool data, FileStream file)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            file.Write(bytes);
        }

        internal void WriteInt(int data, FileStream file)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            file.Write(bytes);
        }

        internal void WriteFloat(float data, FileStream file)
        {
            byte[] bytes = BitConverter.GetBytes(data);
            file.Write(bytes);
        }

        internal void WriteString(string data, FileStream file)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            WriteInt(bytes.Length, file); 
            file.Write(bytes);
        }

        internal SimulatedObject ReadReference(FileStream file)
        {
            string id = ReadString(file);

            if(id == "") { return null; }
            else { return SimulatorCore.FindObjectById(id); }
        }

        internal List<SimulatedObject> ReadReferenceList(FileStream file)
        {
            List<SimulatedObject> result = new List<SimulatedObject>();

            int count = ReadInt(file);
            for(int i = 0; i < count; i++)
            {
                result.Add(ReadReference(file));
            }

            return result;
        }

        internal Vector3 ReadVector3(FileStream file)
        {
            Vector3 result = new Vector3();
            result.X = ReadFloat(file);
            result.Y = ReadFloat(file);
            result.Z = ReadFloat(file);
            return result;
        }

        internal bool ReadBool(FileStream file)
        {
            byte[] bytes = new byte[sizeof(bool)];
            file.Read(bytes);

            return BitConverter.ToBoolean(bytes);
        }

        internal int ReadInt(FileStream file)
        {
            byte[] bytes = new byte[sizeof(int)];
            file.Read(bytes);

            return BitConverter.ToInt32(bytes);
        }

        internal float ReadFloat(FileStream file)
        {
            byte[] bytes = new byte[sizeof(float)];
            file.Read(bytes);

            return BitConverter.ToSingle(bytes);
        }

        internal string ReadString(FileStream file)
        {
            int size = ReadInt(file);
            byte[] bytes = new byte[size];
            file.Read(bytes);

            return Encoding.UTF8.GetString(bytes);
        }

    }
}
