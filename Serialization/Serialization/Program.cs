using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Serialization
{
    class Program
    {
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

        //adds object to list
        public static void TaskAdd(List<WritingMaterial> materials)
        {
            Console.WriteLine("Write the number of object type:");
            int index = 0;
            foreach (MaterialsInfoStruct materialinfo in MaterialsInfo)
            {
                Console.Write(index.ToString() +  " - " + materialinfo.Name + "  ");
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
            Console.WriteLine(materials[index].Name);
            Type materialtype = materials[index].GetType();
            FieldInfo[] fields = materialtype.GetFields();
            Console.WriteLine("index = " + index.ToString());
            Console.WriteLine("number = " + field_number.ToString());
            Console.WriteLine("value = " + value);

            Console.WriteLine(fields[field_number].Name);
            Console.WriteLine();

            try
            {
                fields[field_number].SetValue(materials[index], double.Parse(value));
            }
            catch 
            {
                try
                {
                    Console.WriteLine(fields[field_number].GetType());
                    fields[field_number].SetValue(materials[index], byte.Parse(value));
                }
                catch 
                {
                    try
                    {
                        fields[field_number].SetValue(materials[index], bool.Parse(value));
                    }
                    catch
                    {
                        try
                        {
                            fields[field_number].SetValue(materials[index], value);
                        }
                        catch
                        {
                            Console.WriteLine("CAN'T set such value!!!");
                        }
                    }
                }
            }

            foreach (FieldInfo field in fields)
            {
                Console.Write(field.Name + "  ");
                Console.WriteLine(field.GetValue(materials[index]));
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
                if(parsed)
                {
                    Console.WriteLine("write value");
                    string value = Console.ReadLine();
                    EditMaterial(materials, index, fieldNumber, value);
                }
            }
            else
                Console.WriteLine("enter NORMAL data!(please)");
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

        static void Main()
        {
            Console.WriteLine("hey");
             
            //getting constructors of materials
            foreach(var materialtype in MaterialTypes)
            {
                Type type = Type.GetType(materialtype.FullName);
                var constructor = type.GetConstructor(new Type[] { });
                MaterialsInfo.Add(new MaterialsInfoStruct(materialtype.Name, constructor));
                //Console.WriteLine(materialtype.Name);
            }
            
            //creating task list
            List<TasksStructure> tasks = new List<TasksStructure>();
            tasks.Add(new TasksStructure(tasks.Count + 1, "add", TaskAdd));
            tasks.Add(new TasksStructure(tasks.Count + 1, "remove", TaskRemove));
            tasks.Add(new TasksStructure(tasks.Count + 1, "edit", TaskEdit));
            tasks.Add(new TasksStructure(tasks.Count + 1, "show all", TaskShow));
            //tasks.Add(new TasksStructure(tasks.Count + 1, "serialize", TaskSerialize));
            //tasks.Add(new TasksStructure(tasks.Count + 1, "deserialize", TaskDeserialize));


            StringBuilder helloString = new StringBuilder();
            helloString.Append("Choose task and index: ");
            foreach (var task in tasks)
            {
                helloString.Append(task.number.ToString() + " - " + task.name + "  ");
            }
            
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
                else
                    Console.WriteLine("no such task!");

                Console.WriteLine();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Pencil));

        }
    }
}