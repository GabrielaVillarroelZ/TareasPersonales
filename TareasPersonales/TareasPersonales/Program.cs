using System;
using System.Collections.Generic;
using System.IO;

namespace GestorTareas
{
    enum TipoTarea
    {
        Persona,
        Trabajo,
        Ocio
    }

    class Tarea
    {
        private static int contadorId = 1;
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public TipoTarea Tipo { get; set; }
        public bool Prioridad { get; set; }

        public Tarea(string nombre, string descripcion, TipoTarea tipo, bool prioridad)
        {
            Id = contadorId++;
            Nombre = nombre;
            Descripcion = descripcion;
            Tipo = tipo;
            Prioridad = prioridad;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Nombre: {Nombre}, Descripción: {Descripcion}, Tipo: {Tipo}, Prioridad: {(Prioridad ? "Alta" : "Normal")}";
        }

        public string ToFileString()
        {
            return $"{Id},{Nombre},{Descripcion},{Tipo},{Prioridad}";
        }

        public static Tarea FromFileString(string linea)
        {
            var partes = linea.Split(',');
            var tarea = new Tarea(
                partes[1],
                partes[2],
                Enum.Parse<TipoTarea>(partes[3]),
                bool.Parse(partes[4])
            );
            tarea.Id = int.Parse(partes[0]);
            if (tarea.Id >= contadorId)
            {
                contadorId = tarea.Id + 1;
            }
            return tarea;
        }
    }

    class Program
    {
        static List<Tarea> tareas = new List<Tarea>();

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("\n--- GESTOR DE TAREAS ---");
                Console.WriteLine("1. Crear tarea");
                Console.WriteLine("2. Buscar tareas por tipo");
                Console.WriteLine("3. Eliminar tarea");
                Console.WriteLine("4. Exportar tareas");
                Console.WriteLine("5. Importar tareas");
                Console.WriteLine("6. Salir");
                Console.Write("Selecciona una opción: ");
                string? opcion = Console.ReadLine();

                if (opcion == null) continue;

                switch (opcion)
                {
                    case "1":
                        CrearTarea();
                        break;
                    case "2":
                        BuscarPorTipo();
                        break;
                    case "3":
                        EliminarTarea();
                        break;
                    case "4":
                        ExportarTareas();
                        break;
                    case "5":
                        ImportarTareas();
                        break;
                    case "6":
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no válida, intenta de nuevo.");
                        break;
                }
            }
        }

        static void CrearTarea()
        {
            Console.Write("Nombre: ");
            string? nombre = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine("Nombre inválido.");
                return;
            }

            Console.Write("Descripción: ");
            string? descripcion = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                Console.WriteLine("Descripción inválida.");
                return;
            }

            Console.Write("Tipo (Persona, Trabajo, Ocio): ");
            string? tipoInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(tipoInput) || !Enum.TryParse<TipoTarea>(tipoInput, true, out var tipo))
            {
                Console.WriteLine("Tipo inválido.");
                return;
            }

            Console.Write("¿Es de alta prioridad? (true/false): ");
            string? prioridadInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(prioridadInput) || !bool.TryParse(prioridadInput, out bool prioridad))
            {
                Console.WriteLine("Prioridad inválida.");
                return;
            }

            Tarea nuevaTarea = new Tarea(nombre, descripcion, tipo, prioridad);
            tareas.Add(nuevaTarea);

            Console.WriteLine("¡Tarea creada con éxito!");
        }

        static void BuscarPorTipo()
        {
            Console.Write("Introduce el tipo de tarea a buscar (Persona, Trabajo, Ocio): ");
            string? tipoInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(tipoInput) || !Enum.TryParse<TipoTarea>(tipoInput, true, out var tipo))
            {
                Console.WriteLine("Tipo inválido.");
                return;
            }

            Console.WriteLine($"\nTareas de tipo {tipo}:");
            foreach (var tarea in tareas)
            {
                if (tarea.Tipo == tipo)
                {
                    Console.WriteLine(tarea);
                }
            }
        }

        static void EliminarTarea()
        {
            Console.Write("Introduce el ID de la tarea a eliminar: ");
            string? idInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(idInput) || !int.TryParse(idInput, out int id))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var tarea = tareas.Find(t => t.Id == id);
            if (tarea != null)
            {
                tareas.Remove(tarea);
                Console.WriteLine("Tarea eliminada correctamente.");
            }
            else
            {
                Console.WriteLine("No se encontró ninguna tarea con ese ID.");
            }
        }

        static void ExportarTareas()
        {
            using (StreamWriter writer = new StreamWriter("tareas.txt"))
            {
                foreach (var tarea in tareas)
                {
                    writer.WriteLine(tarea.ToFileString());
                }
            }
            Console.WriteLine("Tareas exportadas a tareas.txt");
        }

        static void ImportarTareas()
        {
            if (File.Exists("tareas.txt"))
            {
                tareas.Clear();
                var lineas = File.ReadAllLines("tareas.txt");
                foreach (var linea in lineas)
                {
                    tareas.Add(Tarea.FromFileString(linea));
                }
                Console.WriteLine("Tareas importadas correctamente.");
            }
            else
            {
                Console.WriteLine("No se encontró el archivo tareas.txt.");
            }
        }
    }
}
