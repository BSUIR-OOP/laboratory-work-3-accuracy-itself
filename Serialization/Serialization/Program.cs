using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Serialization
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("hey");
            GetFileNames(FileNames, "filenames.txt");

            //getting constructors of materials
            foreach (var materialtype in MaterialTypes)
            {
                Type type = Type.GetType(materialtype.FullName);
                var constructor = type.GetConstructor(new Type[] { });
                MaterialsInfo.Add(new MaterialsInfoStruct(materialtype.Name, constructor));
            }

            //creating task list
            List<TasksStructure> tasks = new List<TasksStructure>();
            tasks.Add(new TasksStructure(tasks.Count + 1, "add", TaskAdd));
            tasks.Add(new TasksStructure(tasks.Count + 1, "remove", TaskRemove));
            tasks.Add(new TasksStructure(tasks.Count + 1, "edit", TaskEdit));
            tasks.Add(new TasksStructure(tasks.Count + 1, "show all", TaskShow));
            tasks.Add(new TasksStructure(tasks.Count + 1, "serialize", TaskSerialize));
            tasks.Add(new TasksStructure(tasks.Count + 1, "deserialize", TaskDeserialize));
            tasks.Add(new TasksStructure(tasks.Count + 1, "myserialize", TaskMySerializeAsync));
            tasks.Add(new TasksStructure(tasks.Count + 1, "mydeserialize", TaskMyDeserialize));

            StringBuilder helloString = new StringBuilder();
            helloString.Append("Choose task: ");
            foreach (var task in tasks)
            {
                helloString.Append(task.number.ToString() + " - " + task.name + "  ");
            }
            helloString.Append((tasks.Count + 1).ToString() + " - exit");

            //choosing task
            while (true)
            {
                Console.WriteLine(helloString.ToString());
                string s;
                s = Console.ReadLine();
                int taskNumber;
                bool parsed = int.TryParse(s, out taskNumber);
                if ((taskNumber > 0) && (taskNumber <= tasks.Count))
                {
                    tasks[taskNumber - 1].solver(Materials);
                }
                else if (taskNumber == tasks.Count + 1)
                {
                    Console.WriteLine("Goodbye, my friend!");
                    break;
                }
                else
                    Console.WriteLine("no such task!");

                Console.WriteLine();
            }

            SaveFileNames(FileNames, "filenames.txt");
        }


        //preparation for showing the list of available tasks
        delegate void TaskSolver(List<WritingMaterial> materials);
        struct TasksStructure
        {
            public int number;
            public string name;
            public TaskSolver solver;
            public TasksStructure(int number, string name, TaskSolver solver)
            {
                this.number = number;
                this.name = name;
                this.solver = solver;
            }
        }

        public static List<string> FileNames = new List<string>();

        //gets names of serialized files
        public static async void GetFileNames(List<string> file_names, string file)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    file_names.Add(line.Trim());
                }
            }
        }

        //saves names of serialized files
        public static async void SaveFileNames(List<string> file_names, string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                foreach (string filename in file_names)
                    await writer.WriteLineAsync(filename);
            }
        }

        //adds object to list
        public static void TaskAdd(List<WritingMaterial> materials)
        {
            Console.WriteLine("Write the number of object type:");
            int index = 0;
            foreach (MaterialsInfoStruct materialinfo in MaterialsInfo)
            {
                Console.Write(index.ToString() + " - " + materialinfo.Name + "  ");
                index++;
            }
            Console.WriteLine();

            string s;
            int objectTypeNumber;
            s = Console.ReadLine();
            bool parsed = int.TryParse(s, out objectTypeNumber);
            if ((objectTypeNumber > MaterialsInfo.Count - 1) || (objectTypeNumber < 0))
                parsed = false;
            if (!parsed)
            {
                Console.WriteLine("you should enter NORMAL data!");
                return;
            }

            WritingMaterial material = (WritingMaterial)MaterialsInfo[objectTypeNumber].Constructor.Invoke(new object[] { });
            materials.Add(material);
        }

        //shows the object list
        public static void TaskShow(List<WritingMaterial> materials)
        {
            int index = 0;
            foreach (WritingMaterial material in materials)
            {
                Console.WriteLine(index.ToString() + ". " + material.Name);
                Type type = material.GetType();
                FieldInfo[] fields = type.GetFields();

                foreach (FieldInfo field in fields)
                {
                    Console.Write(field.Name + "  ");
                    Console.WriteLine(field.GetValue(material));
                }

                Console.WriteLine();
                index++;
            }
        }

        //gets the index of object in list
        public static int GetObjectIndex(List<WritingMaterial> materials)
        {
            string s;
            s = Console.ReadLine();
            int index;
            bool parsed = int.TryParse(s, out index);
            if ((index < 0) || (index > materials.Count - 1))
                parsed = false;
            if (!parsed)
            {
                return -1;
            }
            return index;
        }

        //removes the object from list by index
        public static void TaskRemove(List<WritingMaterial> materials)
        {
            Console.WriteLine("enter your object index");
            int index = GetObjectIndex(materials);
            if (index >= 0)
                materials.RemoveAt(index);
            else
                Console.WriteLine("enter NORMAL data!(please)");
        }

        //edits the object
        public static bool EditMaterial(List<WritingMaterial> materials, int index, int field_number, string value)
        {
            Type materialtype = materials[index].GetType();
            FieldInfo[] fields = materialtype.GetFields();
            var field = fields[field_number];
            Type? fieldtype = field.FieldType;
            try
            {
                if (fieldtype == typeof(string))
                    field.SetValue(materials[index], value);
                else if (fieldtype == typeof(int))
                    field.SetValue(materials[index], int.Parse(value));
                else if (fieldtype == typeof(byte))
                    field.SetValue(materials[index], byte.Parse(value));
                else if (fieldtype == typeof(Boolean))
                    field.SetValue(materials[index], Convert.ToBoolean(value));
                else if (fieldtype == typeof(double))
                    field.SetValue(materials[index], Double.Parse(value.Replace('.', ',')));
                else return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        //gets data for editing an object by indexes
        public static void TaskEdit(List<WritingMaterial> materials)
        {
            Console.WriteLine("enter your object index");
            int index = GetObjectIndex(materials);
            if (index >= 0)
            {
                Console.WriteLine("enter your field number");
                string s = Console.ReadLine();
                bool parsed = int.TryParse(s, out int fieldNumber);
                if (parsed)
                {
                    Console.WriteLine("write value");
                    string value = Console.ReadLine();
                    bool edited = EditMaterial(materials, index, fieldNumber, value);
                    if (!edited)
                        Console.WriteLine("CAN'T set such value");
                }
            }
            else
                Console.WriteLine("enter NORMAL data!(please)");
        }

        //XML serializing
        public static void TaskSerialize(List<WritingMaterial> materials)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<WritingMaterial>),
              new Type[] { typeof(Pen), typeof(BallPen), typeof(GelPen), typeof(Brush), typeof(Pencil) });

            string fileName;
            int index = 0;
            FileNames.Clear();

            fileName = index.ToString() + ".xml";
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                serializer.Serialize(fileStream, materials);
                //Console.WriteLine(    );
            }

            FileNames.Add(fileName);
        }

        public static async void TaskMySerializeAsync(List<WritingMaterial> materials)
        {
            XML.MyXmlSerializer serializer = new XML.MyXmlSerializer(typeof(WritingMaterial),
                new Type[] { typeof(BallPen), typeof(GelPen), typeof(Brush), typeof(Pencil) });

            string fileName;
            int index = 0;
            FileNames.Clear();
            /*foreach (var material in materials)
            {
                fileName = index.ToString() + "my.xml";
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    string serialized = serializer.Serialize(material);
                    await writer.WriteLineAsync(serialized);
                    
                }
                FileNames.Add(fileName);
                index++;
            }*/
            fileName = index.ToString() + "my.xml";
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                string serialized = serializer.Serialize(materials);
                await writer.WriteLineAsync(serialized);
                FileNames.Add(fileName);
            }
        }

        public static void TaskMyDeserialize(List<WritingMaterial> materials)
        {
            materials?.Clear();
            XML.MyXmlSerializer deserializer = new XML.MyXmlSerializer(typeof(WritingMaterial),
                new Type[] { typeof(Pen), typeof(BallPen), typeof(GelPen), typeof(Brush), typeof(Pencil) });

            foreach (var filename in FileNames)
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string fileString = reader.ReadToEnd();
                    //Console.WriteLine(fileString);
                    List<WritingMaterial>? deserialisedMaterials = deserializer.Deserialize(fileString) as List<WritingMaterial>;
                    if (deserialisedMaterials != null)
                        foreach (var material in deserialisedMaterials)
                            materials?.Add(material);
                }
            }
        }

        //XML deserializing
        public static void TaskDeserialize(List<WritingMaterial>? materials)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<WritingMaterial>),
                new Type[] { typeof(Pen), typeof(BallPen), typeof(GelPen), typeof(Brush), typeof(Pencil) });
            materials?.Clear();

            foreach (var filename in FileNames)
            {
                using (FileStream fileStream = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    List<WritingMaterial>? deserialisedMaterials = deserializer.Deserialize(fileStream) as List<WritingMaterial>;
                    if (deserialisedMaterials != null)
                        foreach (var material in deserialisedMaterials)
                            materials?.Add(material);
                }
            }
        }

        //for gathering information about materials
        internal struct MaterialsInfoStruct
        {
            public System.Reflection.ConstructorInfo Constructor;
            public string Name;
            internal MaterialsInfoStruct(string name, System.Reflection.ConstructorInfo constructor)
            {
                Constructor = constructor;
                Name = name;
            }
        }

        internal static List<MaterialsInfoStruct> MaterialsInfo = new List<MaterialsInfoStruct>();
        static List<WritingMaterial> Materials = new List<WritingMaterial>();
        static IEnumerable<Type> MaterialTypes = typeof(WritingMaterial).Assembly.ExportedTypes.Where(t => typeof(WritingMaterial).IsAssignableFrom(t) && t != typeof(WritingMaterial));
    }
}