using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    [Serializable]
    public class Pen: WritingMaterial
    {
        public double line_width;
        public override byte Length { get; set; }
        public override string Name { get; set; }
        public Pen() 
        {
            Name = "Pen";  
            line_width = 0.4;
            Length = 7;
        }

        public override void Write()
        {
            Console.WriteLine("scratching with some pen..");
        }
    }
}
