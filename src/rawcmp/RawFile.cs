namespace Vurdalakov.RawCompare
{
    using System;
    using System.IO;

    public abstract class RawFile
    {
        public Boolean IsSupported(String fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    return IsSupported(binaryReader);
                }
            }
        }

        public void Read(String fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    Read(binaryReader);
                }
            }
        }

        protected abstract Boolean IsSupported(BinaryReader binaryReader);

        protected abstract void Read(BinaryReader binaryReader);

        public abstract Boolean IsEqualTo(RawFile rawFile);
    }
}
