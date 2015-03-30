namespace Vurdalakov.RawCompare
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class RawCompare
    {
        private List<Type> _types = new List<Type>();

        public RawCompare()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsClass && type.IsSubclassOf(typeof(RawFile)))
                {
                    _types.Add(type);
                }
            }
        }

        public Boolean AreEqual(String fileName1, String fileName2)
        {
            var rawFile1 = GetFile(fileName1);
            var rawFile2 = GetFile(fileName2);

            if (rawFile1.GetType() != rawFile2.GetType())
            {
                throw new RawCompareException(4, "File formats are different");
            }

            rawFile1.Read(fileName1);
            rawFile2.Read(fileName2);

            return rawFile1.IsEqualTo(rawFile2);
        }

        private RawFile GetFile(String fileName)
        {
            foreach (Type type in _types)
            {
#if !DEBUG
                if (type.HasAttribute<IncompleteAttribute>())
                {
                    continue;
                }
#endif

                var rawFile = Activator.CreateInstance(type) as RawFile;
                if (null == rawFile)
                {
                    throw new Exception();
                }

                if (rawFile.IsSupported(fileName))
                {
                    return rawFile;
                }
            }

            throw new RawCompareException(3, "Unsupported file format: {0}", fileName);
        }
    }
}
