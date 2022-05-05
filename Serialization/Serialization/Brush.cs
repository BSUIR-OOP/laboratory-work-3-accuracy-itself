using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    [Serializable]
    public class Brush : WritingMaterial
    {
        public byte brush_size;
        public bool is_paint;
        public enum Brush_Material
        {
            Fur,
            Synthetics
        }

        public Brush_Material material;
        public override byte Length { get; set;}
        public override string Name { get; set; }

        public Brush()
        {
            Name = "Brush";
            is_paint = false;
            Length = 13;
            brush_size = 1;
            material = Brush_Material.Fur;
        }

        public override void Write()
        {
            if (is_paint)
                Console.WriteLine("brushing..");
            else
                Console.WriteLine("not brushing..");
        }

    }
}
