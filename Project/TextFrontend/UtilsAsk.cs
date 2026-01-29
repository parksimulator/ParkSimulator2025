using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GemeloDigital
{
    internal partial class Program
    {
        static string AskString(string prompt)
        {
            string line = "";
            bool done = false;

            while(!done)
            {
                Console.Write(prompt  + ">");
                line = Console.ReadLine();
                line = line.Trim();
                if(line.Length <= 0) { Console.WriteLine("Tienes que escribir un texto"); }
                else { done = true; }
            }

            return line;
        }

        static int AskInteger(string prompt)
        {
            int number = -1;
            string line;
            bool done = false;

            while(!done)
            {
                Console.Write(prompt + "> ");
                line = Console.ReadLine();
                if(!Int32.TryParse(line, out number))
                {
                    Console.WriteLine("Introduce un número entero ");
                }
                else { done = true; }
            }

            return number;
        }

        static int AskIntegerBetween(string prompt, int minimum, int maximum)
        {

            int number = -1;
            bool done = false;

            while(!done)
            {
                number = AskInteger(prompt + "[" + minimum + "-" + maximum + "]");
                if(number >= minimum && number <= maximum) { done = true; }
                else { Console.WriteLine("Introduce un número entre " + minimum + " y " + maximum); }
            }

            return number;
        }

        static float AskSingle(string prompt)
        {
            float number = -1;
            string line;
            bool done = false;

            while(!done)
            {
                Console.Write(prompt + "> ");
                line = Console.ReadLine();
                if(!Single.TryParse(line, out number))
                {
                    Console.WriteLine("Introduce un número real");
                }
                else { done = true; }
            }

            return number;
        }

        static float AskSingleBetween(string prompt, float minimum, float maximum)
        {

            float number = -1;
            bool done = false;

            while(!done)
            {
                number = AskSingle(prompt + "[" + minimum + "-" + maximum + "]");
                if(number >= minimum && number <= maximum) { done = true; }
                else { Console.WriteLine("Introduce un número entre " + minimum + " y " + maximum); }
            }

            return number;
        }

        static void AskPersonProperties(Person p)
        {
            p.Age = AskIntegerBetween("Edad", 12, 100);
            p.Height = AskSingleBetween("Altura", 120, 200);
            p.Weight = AskSingleBetween("Peso", 35, 120);
            p.Money = AskSingleBetween("Dinero", 10, 500);

            SimulatedObject obj = PickObjectOrNull("Instalación", "Instalaciones", SimulatedObjectType.Facility);
            Facility facility = SimulatorCore.AsFacility(obj);
            p.IsAtFacility = facility;

            if(facility != null)
            {
                p.IsAtPath = null;
            }
            else
            {
                obj = PickObjectOrNull("Camino", "Caminos", SimulatedObjectType.Path);
                Path path = SimulatorCore.AsPath(obj);
                p.IsAtPath = path;
            }

        }

        static void AskPointProperties(Point p)
        {
            Vector3 position;
            position.X = AskSingle("PosX");
            position.Y = AskSingle("PosY");
            position.Z = AskSingle("PosZ");

            p.Position = position;
        }

        static void AskFacilityProperties(Facility f, bool isCreate)
        {
            f.PowerConsumed = AskSingleBetween("Energía(KW/h)", 100, 1000);

            if(!isCreate)
            {
                if(SimulatorCore.CountObjectsOfType(SimulatedObjectType.Point) > 0)
                {
                    SimulatedObject o1 = PickObject("Entrada", "Puntos", SimulatedObjectType.Point);
                    SimulatedObject o2 = PickObject("Salida", "Puntos", SimulatedObjectType.Point);

                    Point p1 = SimulatorCore.AsPoint(o1);
                    Point p2 = SimulatorCore.AsPoint(o2); 

                    f.Entrances.Clear();
                    f.Entrances.Add(p1);
                    f.Exits.Clear();
                    f.Exits.Add(p2);
                }
                else
                {
                    Console.WriteLine("No hay puntos en la escena para usar como entrada o salida");
                    Thread.Sleep(messageWaitTime);
                }

            }

        }

        static void AskPathProperties(Path p)
        {
            bool done = false;

            Point p1 = null;
            Point p2 = null;

            while(!done)
            {
                SimulatedObject o1 = PickObject("Punto 1", "Puntos", SimulatedObjectType.Point);
                SimulatedObject o2 = PickObject("Punto 2", "Puntos", SimulatedObjectType.Point);

                p1 = SimulatorCore.AsPoint(o1);
                p2 = SimulatorCore.AsPoint(o2);

                if(p1 != p2) {  done = true; }
                else { Console.WriteLine("Los puntos deben ser distintos"); }
            }

            p.Point1 = p1;
            p.Point2 = p2;
        }

        static void AskCameraProperties(Camera c)
        {
            Vector3 position;
            position.X = AskSingle("PosX");
            position.Y = AskSingle("PosY");
            position.Z = AskSingle("PosZ");

            c.Position = position;

            Vector3 rotation;
            rotation.X = AskSingle("RotX");
            rotation.Y = AskSingle("RotY");
            rotation.Z = AskSingle("RotZ");

            c.Rotation = rotation;

            c.FOV = AskSingle("FOV");
            c.ZNear = AskSingle("ZNear");
            c.ZFar = AskSingle("ZFar");

        }

        static void AskModelProperties(Model m)
        {
            Vector3 position;
            position.X = AskSingle("PosX");
            position.Y = AskSingle("PosY");
            position.Z = AskSingle("PosZ");

            m.Position = position;

            Vector3 rotation;
            rotation.X = AskSingle("RotX");
            rotation.Y = AskSingle("RotY");
            rotation.Z = AskSingle("RotZ");

            m.Rotation = rotation;

            Vector3 scale;
            scale.X = AskSingle("ScaleX");
            scale.Y = AskSingle("ScaleY");
            scale.Z = AskSingle("ScaleZ");

            m.Scale = rotation;

            SimulatedObject obj = PickObjectOrNull("Material", "Materials", SimulatedObjectType.Material);
            m.material = SimulatorCore.AsMaterial(obj);
        }

        static void AskMaterialProperties(Material m)
        {
            Vector3 color;
            color.X = AskIntegerBetween("ColorR", 0, 255) / 255.0f;
            color.Y = AskIntegerBetween("ColorG", 0, 255) / 255.0f;
            color.Z = AskIntegerBetween("ColorB", 0, 255) / 255.0f;

            m.Color = color;

            SimulatedObject obj = PickObjectOrNull("Shader", "Shaders", SimulatedObjectType.Shader);
            m.Shader = SimulatorCore.AsShader(obj);

            obj = PickObjectOrNull("Textura", "Texturas", SimulatedObjectType.Texture);
            m.Texture = SimulatorCore.AsTexture(obj);
        }

        static void AskMeshProperties(Mesh m)
        {
            m.ResourceId = AskString("Identificador de recurso");
        }

        static void AskShaderProperties(Shader s)
        {
            s.ResourceId = AskString("Identificador de recurso");
        }

        static void AskTextureProperties(Texture t)
        {
            t.ResourceId = AskString("Identificador de recurso");
        }

        static void AskContinue()
        {
            Console.Write("pulsa [intro] para continuar");
        }

    }
}
