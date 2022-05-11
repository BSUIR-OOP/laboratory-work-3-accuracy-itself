using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Serialization.XML
{
    internal class MyXmlSerializer
    {
        public Type type;
        public Type[]? extratypes; 
        StringBuilder DocumentBeginning, DocumentEnd;
        public string document;
        public const string title = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        public const string standardstring = " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
        public const string typestring = " xsi:type=";

        public MyXmlSerializer(Type type, Type[]? extratypes)
        {
            this.type = type;
            this.extratypes = extratypes;
            DocumentBeginning = new StringBuilder();
            DocumentEnd = new StringBuilder();
        }

        public string Serialize(object? o)
        {
            DocumentBeginning.Clear();
            DocumentEnd.Clear();
            DocumentBeginning.Append(title);
            
            Type objecttype = o.GetType();
            DocumentBeginning.Append(GetElementBegining(this.type.Name + standardstring + typestring + "\"" + objecttype.Name + "\""));
            DocumentEnd.Append(GetElementEnd(this.type.Name));
            FieldInfo[] fields = objecttype.GetFields();
            PropertyInfo[] properties = objecttype.GetProperties();



            Console.WriteLine("\n" + objecttype.BaseType.Name);



            foreach (FieldInfo field in fields)
            {
                string value = field.GetValue(o).ToString();
                Console.WriteLine("b" + value + "b");

                Console.WriteLine(field.FieldType.BaseType.Name);
                //Console.WriteLine(typeof(Boolean).Name);
                if(field.FieldType.Name == typeof(Boolean).Name)
                    value = value.ToLower();
                
                if(field.FieldType.Name != typeof(string).Name)
                    if (Double.TryParse(value, out double ch))
                        value = value.Replace(',', '.');
                DocumentBeginning.Append(GetToken(field.Name, value));
            }
            foreach (PropertyInfo property in properties)
            {
                //property.GetT
                string value = property.GetValue(o).ToString();
                if (property.PropertyType.Name == typeof(Boolean).Name)
                    value = value.ToLower();

                if (property.PropertyType.Name != typeof(string).Name)
                    if (Double.TryParse(value, out double ch))
                        value = value.Replace(',', '.');
                DocumentBeginning.Append(GetToken(property.Name, value));
            }
            document = DocumentBeginning.ToString() + DocumentEnd.ToString();
            return document;
        }

        public object? Deserialize(string doc)
        {
            document = doc;


            return document;
        }

        public void GetNextToken(ref string document)
        {

        }

        private string GetToken(string name, string value)
        {
            return (GetElementBegining(name) + value + GetElementEnd(name));
        }

        private string GetElementBegining(string name)
        {
            return '<' + name + '>';
        }

        private string GetElementEnd(string name)
        {
            return "</" + name + '>';
        }
    }
}
