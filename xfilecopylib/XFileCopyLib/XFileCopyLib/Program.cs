using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFileCopyLib
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var sourceFile = new FileInfo(@"C:\test\Alita.Battle.Angel.2019.ViE.1080p.BluRay.DTS-ES.x264-SPARKS.mkv");
                var destFile = new FileInfo(@"D:\test\Alita.Battle.Angel.2019.ViE.1080p.BluRay.DTS-ES.x264-SPARKS.mkv");
                Console.WriteLine($"start copy {sourceFile.FullName} to {destFile.FullName}...");
                if (!destFile.Directory.Exists) destFile.Directory.Create();
                FileCopier.MoveTo(sourceFile, destFile, true, (double progress) =>
                {
                    Console.WriteLine($"copy {destFile.FullName}: {progress}%");
                });
                Console.WriteLine($"copy {sourceFile.FullName} succeed...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();
        }
    }
}
