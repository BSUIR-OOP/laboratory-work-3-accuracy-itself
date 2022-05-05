using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    [Serializable]
    public abstract class WritingMaterial
    {
        public abstract byte Length { get; set; }
        public abstract string Name { get; set; }
        public abstract void Write();
    }
}
