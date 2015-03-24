namespace Vurdalakov.RawCompare
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class Mp3File : RawFile
    {
        protected override void Read(BinaryReader binaryReader)
        {
        }

        public override Boolean IsSameAs(RawFile rawFile)
        {
            return false;
        }
    }
}
