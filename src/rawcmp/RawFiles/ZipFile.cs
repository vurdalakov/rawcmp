﻿namespace Vurdalakov.RawCompare
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ZipFileRecord
    {
        public String FileName { get; private set; }
        public UInt32 FileSize { get; private set; }
        public UInt32 FileCrc { get; private set; }

        public ZipFileRecord(String fileName, UInt32 fileSize, UInt32 fileCrc)
        {
            FileName = fileName;
            FileSize = fileSize;
            FileCrc = fileCrc;
        }

        public Boolean IsEqualTo(ZipFileRecord record)
        {
            return FileName.ToLower().Equals(record.FileName.ToLower()) && (FileSize == record.FileSize) && (FileCrc == record.FileCrc);
        }
    }

    public class ZipFile: RawFile
    {
        private const UInt32 _signature = 0x06054b50;

        private Int64 FindEndOfCentralDirectoryRecordOffset(Stream stream)
        {
            var start = Environment.TickCount;

            var length = stream.Length;

            if (length < 22)
            {
                return -1; // ZIP file can't be smaller than 22 bytes
            }

            var size = (Int32)Math.Min(length, 66560); // max comment size is 65536

            stream.Seek(-size, SeekOrigin.End);

            var buffer = new Byte[size];
            stream.Read(buffer, 0, size);

            var binaryReader = new BinaryReader(new MemoryStream(buffer));

            try
            {
                binaryReader.Seek(-17, SeekOrigin.End);
                do
                {
                    var endOfCentralDirectoryRecordOffset = binaryReader.Seek(-5, SeekOrigin.Current);
                    if (_signature == binaryReader.ReadUInt32())
                    {
                        binaryReader.Skip(16);

                        var commentSize = binaryReader.ReadUInt16();
                        return (binaryReader.GetPosition() + commentSize) == binaryReader.GetLength() ? length - size + endOfCentralDirectoryRecordOffset : -1;
                    }
                }
                while (binaryReader.GetPosition() > 4);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }

            Console.WriteLine("{0}", (double)Environment.TickCount - start);

            return -1;
        }

        protected override Boolean IsSupported(BinaryReader binaryReader)
        {
            return FindEndOfCentralDirectoryRecordOffset(binaryReader.BaseStream) > 0;
        }

        public ZipFileRecord[] FileRecords { get; private set; }

        protected override void Read(BinaryReader binaryReader)
        {
            var endOfCentralDirectoryRecordOffset = FindEndOfCentralDirectoryRecordOffset(binaryReader.BaseStream);
            if (endOfCentralDirectoryRecordOffset < 0)
            {
                throw new Exception("Invalid file format");
            }

            binaryReader.Seek(endOfCentralDirectoryRecordOffset, SeekOrigin.Begin);

            binaryReader.Skip(10);
            var entries = binaryReader.ReadUInt16();
            binaryReader.Skip(4);
            var centralDirOffset = binaryReader.ReadUInt32();

            binaryReader.Seek(centralDirOffset, SeekOrigin.Begin);

            var records = new List<ZipFileRecord>(entries);
            for (int i = 0; i < entries; i++)
            {
                var record = ReadEntry(binaryReader);
                records.Add(record);
            }

            if (binaryReader.GetPosition() != endOfCentralDirectoryRecordOffset)
            {
                throw new Exception("Invalid central directory record");
            }

            FileRecords = records.ToArray();
        }

        private ZipFileRecord ReadEntry(BinaryReader binaryReader)
        {
            UInt32 signature = binaryReader.ReadUInt32();
            if (signature != 0x02014b50)
            {
                throw new Exception("Invalid central directory file header");
            }

            binaryReader.Skip(4);
            var flags = binaryReader.ReadUInt16();

            binaryReader.Skip(6);
            var fileCrc = binaryReader.ReadUInt32();
            binaryReader.Skip(4);
            var fileSize = binaryReader.ReadUInt32();

            var fileNameLength = binaryReader.ReadUInt16();
            var extraFieldLength = binaryReader.ReadUInt16();
            var fileCommentLength = binaryReader.ReadUInt16();

            binaryReader.Skip(12);

            var encoding = 0x0800 == (flags & 0x0800) ? Encoding.UTF8 : Encoding.Default;
            var fileNameBytes = binaryReader.Read(0, fileNameLength);
            var fileName = encoding.GetString(fileNameBytes, 0, fileNameLength);

            binaryReader.Skip(extraFieldLength + fileCommentLength);

            return new ZipFileRecord(fileName, fileSize, fileCrc);
        }

        public override Boolean IsEqualTo(RawFile rawFile)
        {
            var zipFile = rawFile as ZipFile;
            if (null == zipFile)
            {
                throw new ArgumentException();
            }

            if (FileRecords.Length != zipFile.FileRecords.Length)
            {
                return false;
            }

            Comparison<ZipFileRecord> comparison =
                delegate(ZipFileRecord file1, ZipFileRecord file2)
                {
                    return file1.FileName.CompareTo(file2.FileName);
                };

            var records1 = new List<ZipFileRecord>(FileRecords);
            records1.Sort(comparison);

            var records2 = new List<ZipFileRecord>(zipFile.FileRecords);
            records2.Sort(comparison);

            for (int i = 0; i < records1.Count; i++)
            {
                if (!records1[i].IsEqualTo(records2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
