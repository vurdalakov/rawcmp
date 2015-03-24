namespace Vurdalakov.RawCompare
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
        protected override Boolean IsSupported(BinaryReader binaryReader)
        {
            return true; // TODO
        }

        public ZipFileRecord[] FileRecords { get; private set; }

        protected override void Read(BinaryReader binaryReader)
        {
            if (binaryReader.GetLength() < 22)
            {
                throw new Exception("ZIP file can't be smaller than 22 bytes");
            }

            var records = new List<ZipFileRecord>();

            try
            {
                binaryReader.Seek(-17, SeekOrigin.End);
                do
                {
                    var endOfCentralDirectoryRecordOffset = binaryReader.Seek(-5, SeekOrigin.Current);
                    UInt32 signature = binaryReader.ReadUInt32();
                    if (0x06054b50 == signature)
                    {
                        binaryReader.Skip(6);

                        UInt16 entries = binaryReader.ReadUInt16();
                        Int32 centralSize = binaryReader.ReadInt32();
                        UInt32 centralDirOffset = binaryReader.ReadUInt32();
                        UInt16 commentSize = binaryReader.ReadUInt16();

                        // check if comment field is the very last data in file
                        if (binaryReader.BaseStream.Position + commentSize != binaryReader.BaseStream.Length)
                        {
                            throw new Exception("Invalid archive comment");
                        }

                        binaryReader.Seek(centralDirOffset, SeekOrigin.Begin);

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
                        return;
                    }
                }
                while (binaryReader.BaseStream.Position > 0);
            }
            catch { }

            throw new Exception("Central directory record not found or invalid");
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
