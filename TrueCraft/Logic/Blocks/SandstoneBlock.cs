using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SandstoneBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x18;
        
        public override byte ID { get { return 0x18; } }
        
        public override double BlastResistance { get { return 4; } }

        public override double Hardness { get { return 0.8; } }

        public override byte Luminance { get { return 0; } }
        
        public override string DisplayName { get { return "Sandstone"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 12);
        }
    }
}