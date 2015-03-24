namespace Vurdalakov.RawCompare
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class Mp3File : RawFile
    {
        protected override Boolean IsSupported(BinaryReader binaryReader)
        {
            return false;
        }

        protected override void Read(BinaryReader binaryReader)
        {
        }

        public override Boolean IsEqualTo(RawFile rawFile)
        {
            return false;
        }
    }
}
