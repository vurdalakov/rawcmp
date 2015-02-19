namespace Vurdalakov.RawCompare
{
    using System;
    using System.IO;

    public class Application
    {
        private CommandLineParser _commandLineParser;
        private Boolean _silent;

        public Application(String[] args)
        {
            try
            {
                _commandLineParser = new CommandLineParser(args);

                if (_commandLineParser.FileNames.Length != 2)
                {
                    throw new Exception();
                }

                _silent = _commandLineParser.IsOptionSet("silent");
            }
            catch
            {
                Help();
            }
        }

        public void Run()
        {
            try
            {
                var fileName1 = _commandLineParser.FileNames[0];
                var fileName2 = _commandLineParser.FileNames[1];

                Print("Comparing files {0} and {1}", fileName1, fileName2);

                if (!Path.GetExtension(fileName1).Equals(".zip") || !Path.GetExtension(fileName2).Equals(".zip"))
                {
                    Print("Unsupported file format");
                    Environment.Exit(3);
                }

                var zipFile1 = new ZipFile();
                zipFile1.Read(fileName1);

                var zipFile2 = new ZipFile();
                zipFile2.Read(fileName2);

                var filesAreEqual = zipFile1.IsSameAs(zipFile2);

                Print(filesAreEqual ? "No differences encountered" : "Files are different");

                Environment.Exit(filesAreEqual ? 0 : 1);
            }
            catch (Exception ex)
            {
                Print("Error comparing files: {0}", ex.Message);
                Environment.Exit(2);
            }
        }

        private void Print(String format, params Object[] args)
        {
            if (!_silent)
            {
                Console.WriteLine(String.Format(format, args));
            }
        }

        private void Help()
        {
            Console.WriteLine("Raw File Compare 1.0 | (c) Vurdalakov | https://github.com/vurdalakov/rawcmp\n");
            Console.WriteLine("Compares raw content of two files, ignoring insignificant data and metadata\n(archive comments, MP3 tags, image thumbnails, etc.)\n");
            Console.WriteLine("Usage:\n\trawcmp file1 file2 [-silent]\n");
            Console.WriteLine("Exit codes:\n\t0 - files are equal\n\t1 - files are different\n\t2 - one or both files not found\n\t3 - unsupported file format\n\t-1 - invalid command line syntax\n");
            Console.WriteLine("Supported file formats: ZIP\n");
            Environment.Exit(-1);
        }
    }
}
