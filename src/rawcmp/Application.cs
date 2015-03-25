namespace Vurdalakov.RawCompare
{
    using System;
    using System.IO;

    public class Application : DosToolsApplication
    {
        public Application(String[] args) : base(args)
        {
            if (_commandLineParser.FileNames.Length != 2)
            {
                Help();
            }
        }

        protected override Int32 Execute()
        {
            try
            {
                var fileName1 = _commandLineParser.FileNames[0];
                var fileName2 = _commandLineParser.FileNames[1];

                Print("Comparing files {0} and {1}", fileName1, fileName2);

                var rawCompare = new RawCompare();
                rawCompare.AddFormat(typeof(ZipFile));

                var filesAreEqual = rawCompare.AreEqual(fileName1, fileName2);

                Print(filesAreEqual ? "No differences encountered" : "Files are different");

                return filesAreEqual ? 0 : 1;
            }
            catch (RawCompareException ex)
            {
                Print("Error comparing files: {0}", ex.Message);
                return ex.ExitCode;
            }
            catch (Exception ex)
            {
                Print("Error comparing files: {0}", ex.Message);
                return 9;
            }
        }

        protected override void Help()
        {
            Console.WriteLine("Raw File Compare 1.0 | (c) Vurdalakov | https://github.com/vurdalakov/rawcmp\n");
            Console.WriteLine("Compares raw content of two files, ignoring insignificant data and metadata\n(archive comments, MP3 tags, image thumbnails, etc.)\n");
            Console.WriteLine("Usage:\n\trawcmp file1 file2 [-silent]\n");
            Console.WriteLine("Exit codes:\n\t0 - files are equal\n\t1 - files are different\n\tN - an error occurred; visit homepage for a list\n\t-1 - invalid command line syntax\n");
            Console.WriteLine("Supported file formats: ZIP\n");
            Environment.Exit(-1);
        }
    }
}
