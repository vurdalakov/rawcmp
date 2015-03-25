﻿namespace Vurdalakov.RawCompare
{
    using System;
    using System.Collections.Generic;

    public class RawCompare
    {
        private List<Type> _types = new List<Type>();

        public void AddFormat(Type type)
        {
            _types.Add(type);
        }

        public Boolean AreEqual(String fileName1, String fileName2)
        {
            var rawFile1 = GetFile(fileName1);
            rawFile1.Read(fileName1);

            var rawFile2 = GetFile(fileName2);
            rawFile2.Read(fileName2);

            return rawFile1.IsEqualTo(rawFile2);
        }

        private RawFile GetFile(String fileName)
        {
            foreach (Type type in _types)
            {
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

            return null;
        }
    }
}