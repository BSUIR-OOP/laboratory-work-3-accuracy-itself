using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    [Serializable]
    public class GelPen : Pen
    {
        public byte glitter_colour;
        public override string Name { get; set; }
        public GelPen()
        {
            Name = "Gel Pen";
            glitter_colour = 0;
            Length = 7;
        }

        public override void Write()
        {
            if (glitter_colour == 0)
                base.Write();
            else
                Console.WriteLine("writing and shining..");
        }
    }
}
