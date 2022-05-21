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
        public Type objecttype;
        public Type[]? extratypes;
        public const string title = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        public const string standardstring = " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
        public const string typestring = " xsi:type=";
        public const string arraystring = "ArrayOf";

        public MyXmlSerializer(Type type, Type[]? extratypes)
        {
            this.objecttype = type;
            this.extratypes = extratypes;
        }

        //start method
        public string Serialize(object? o)
        {
            StringBuilder serialized = new StringBuilder();
            serialized.Append(title);

            var enumerable = o as System.Collections.IEnumerable;
            if (enumerable != null)
                serialized.Append(SerializeEnumerable(o));
            else
                serialized.Append(SerializeObject(o));

            return serialized.ToString();
        }

        //Serializes one material
        public string SerializeObject(object o)
        {
            //create string for object
            StringBuilder DocumentBeginning = new StringBuilder();
            StringBuilder DocumentEnd = new StringBuilder();
            DocumentBeginning.Clear();
            DocumentEnd.Clear();

            //gather info about object
            Type objecttype = o.GetType();
            DocumentBeginning.Append(GetElementBegining(this.objecttype.Name + typestring + "\"" + objecttype.Name + "\""));
            DocumentEnd.Append(GetElementEnd(this.objecttype.Name));
            FieldInfo[] fields = objecttype.GetFields();
            PropertyInfo[] properties = objecttype.GetProperties();

            foreach (FieldInfo field in fields)
            {
                string value = field.GetValue(o).ToString();
                if (field.FieldType.Name == typeof(Boolean).Name)
                    value = value.ToLower();

                if (field.FieldType.Name != typeof(string).Name)
                    if (Double.TryParse(value, out double ch))
                        value = value.Replace(',', '.');
                DocumentBeginning.Append(MakeToken(field.Name, value));
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
                DocumentBeginning.Append(MakeToken(property.Name, value));
            }

            return DocumentBeginning.ToString() + DocumentEnd.ToString();
        }

        //serializing list
        public string SerializeEnumerable(object objectlist)
        {
            StringBuilder DocumentBeginning = new StringBuilder();
            StringBuilder DocumentEnd = new StringBuilder();
            DocumentBeginning.Append(GetElementBegining(arraystring + objecttype.Name + standardstring));
            DocumentEnd.Append(GetElementEnd(arraystring + objecttype.Name));

            //serealize every object
            var list = objectlist as List<WritingMaterial>;
            foreach (var listobject in list)
                DocumentBeginning.Append(SerializeObject(listobject));

            return DocumentBeginning.ToString() + DocumentEnd.ToString();
        }

        public object? Deserialize(string doc)
        {
            string document = doc;
            var materials = new List<WritingMaterial>();
            CutTitle(ref document);

            //cut arraystring
            GetTokenInfo(document, out string typename, out string value);
            document = value;

            while (document != "")
            {
                //get next object
                string token = GetNextToken(ref document);

                //get object info
                GetTokenInfo(token, out typename, out value);
                typename = GetObjectTypeName(token);
                token = value;

                //create object
                Type type = GetObjectType(typename);
                if (type == null)
                    throw new MyXmlSerializerException("Cannot serialize such things.");

                WritingMaterial material = Activator.CreateInstance(type) as WritingMaterial;

                //create dict for fields
                Dictionary<string, string> values = new Dictionary<string, string>();

                //go through field tokens
                while (token != "")
                {
                    Console.WriteLine("objtoken:" + token);
                    Console.WriteLine();
                    //get next field
                    string fieldtoken = GetNextToken(ref token);
                    Console.WriteLine("obj field: " + fieldtoken);
                    Console.WriteLine();
                    GetTokenInfo(fieldtoken, out typename, out value);
                    values.Add(typename, value);
                }

                //go through fields
                foreach (var field in type.GetFields())
                {
                    Console.WriteLine(field.Name);
                    Type fieldtype = field.FieldType;
                    if (fieldtype == typeof(string))
                        field.SetValue(material, values[field.Name]);
                    else if (fieldtype == typeof(int))
                        field.SetValue(material, int.Parse(values[field.Name]));
                    else if (fieldtype == typeof(byte))
                        field.SetValue(material, byte.Parse(values[field.Name]));
                    else if (fieldtype == typeof(Boolean))
                        field.SetValue(material, Convert.ToBoolean(values[field.Name]));
                    else if (fieldtype == typeof(double))
                        field.SetValue(material, Double.Parse(values[field.Name].Replace('.', ',')) );
                }

                //go through properties
                foreach (var property in type.GetProperties())
                {
                    Console.WriteLine(property.Name);
                    Type propertytype = property.PropertyType;
                    if (propertytype == typeof(string))
                        property.SetValue(material, values[property.Name]);
                    else if (propertytype == typeof(int))
                        property.SetValue(material, int.Parse(values[property.Name]));
                    else if (propertytype == typeof(byte))
                        property.SetValue(material, byte.Parse(values[property.Name]));
                    else if (propertytype == typeof(Boolean))
                        property.SetValue(material, Convert.ToBoolean(values[property.Name]));
                    else if (propertytype == typeof(double))
                        property.SetValue(material, Convert.ToDouble(values[property.Name].Replace('.', ',')) );
                }

                materials.Add(material);
            }

            return materials;
        }

        //changing document by cutting first token
        public string GetNextToken(ref string document)
        {
            string doc = document;
            int spaceindex = document.IndexOf(' '), bracketindex = document.IndexOf('>'), index;
            if (spaceindex < 0)
                index = bracketindex;
            else if (bracketindex < 0)
                index = spaceindex;
            else
                index = Math.Min(spaceindex, bracketindex);

            string typename = document.Substring(1, index - 1);
            document = document.Substring(document.IndexOf('/' + typename) + typename.Length + 2);
            return doc.Substring(0, doc.IndexOf('/' + typename) + typename.Length + 2);
        }

        private void CutTitle(ref string doc)
        {
            doc = doc.Substring(doc.IndexOf('>') + 1);
        }

        private string GetObjectTypeName(string document)
        {
            string type = document.Substring(document.IndexOf(typestring) + typestring.Length + 1);
            return type.Substring(0, type.IndexOf('\"'));
        }

        //looking for necessary type
        private Type GetObjectType(string typename)
        {
            foreach (Type type in extratypes)
            {
                if (type.Name == typename)
                    return type;
            }

            return null;
        }

        //"opening" token by getting name and value
        public void GetTokenInfo(string document, out string typename, out string value)
        {
            int spaceindex = document.IndexOf(' '), bracketindex = document.IndexOf('>'), index;
            if (spaceindex < 0)
                index = bracketindex;
            else if (bracketindex < 0)
                index = spaceindex;
            else
                index = Math.Min(spaceindex, bracketindex);

            typename = document.Substring(1, index - 1);
            value = document.Substring(document.IndexOf('>') + 1, document.LastIndexOf('<') - document.IndexOf('>') - 1);
        }

        private string MakeToken(string name, string value)
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
