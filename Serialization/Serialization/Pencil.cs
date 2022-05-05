using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    [Serializable]
    public class Pencil: WritingMaterial
    {
        public byte pencil_shade;
        public override byte Length { get; set; }
        public override string Name { get; set; }

        public Pencil()
        {
            Name = "Pencil";
            pencil_shade = 1;
            Length = 1;
        }

        public override void Write()
        {
            if (pencil_shade < 5)
                Console.WriteLine("drawing softly..");
            else if (pencil_shade < 10)
                Console.WriteLine("drawing normally..");
            else
                Console.WriteLine("drawing not softly..");
        }

        public delegate void Pencil_del();
    }
}
