using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    [Serializable]
    public class BallPen: Pen
    {
        //0 - ordinary, 1 - oil 
        public byte ink_type;
        public override string Name { get; set; }

        public BallPen()
        {
            Name = "Ball Pen";
            ink_type = 0;
            Length = 9;
        }

        public override void Write()
        {
            if (ink_type == 0)
                Console.WriteLine("writing ordinarily..");
            else
                Console.WriteLine("writing softly..");
        }
    }
}
