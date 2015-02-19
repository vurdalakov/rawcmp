namespace Vurdalakov.RawCompare
{
    using System;
    using System.IO;

    public abstract class RawFile
    {
        public void Read(String fileName)
        {
            Read(new FileStream(fileName, FileMode.Open, FileAccess.Read));
        }

        public void Read(Stream stream)
        {
            Read(new BinaryReader(stream));
        }

        protected abstract void Read(BinaryReader binaryReader);

        public abstract Boolean IsSameAs(RawFile rawFile);
    }
}
