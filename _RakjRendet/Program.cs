using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Diagnostics;
namespace _RakjRendet
{
	class Program
	{
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.MoveTo(tempPath);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        static void Main(string[] args)
		{
			string ezakönyvtár = Directory.GetCurrentDirectory();
            string célkönyvtár = ezakönyvtár + "\\_rend";
            if (!Directory.EnumerateDirectories(ezakönyvtár).Any(x=>x==célkönyvtár))
                Directory.CreateDirectory(célkönyvtár);

			Process proc = new Process();

            Regex másolandópattern = new Regex(@"^[^_].*");

            string fájlnév;
            string kiterjesztés;
            string fájlcélkönyvtár;
            foreach (string path in Directory.EnumerateFiles(ezakönyvtár).Where(x => másolandópattern.IsMatch(x.Split('\\').Last())))
            {
                fájlnév = path.Split('\\').Last();
                kiterjesztés = fájlnév.Split('.').Last();
                fájlcélkönyvtár = célkönyvtár + "\\" + kiterjesztés;
                if (!Directory.EnumerateDirectories(célkönyvtár).Any(x => x.Split('\\').Last() == kiterjesztés))
                    Directory.CreateDirectory(fájlcélkönyvtár);
                File.Copy(path, fájlcélkönyvtár + "\\" + fájlnév, true);
                File.Delete(path);
            }

            foreach (string path in Directory.EnumerateDirectories(ezakönyvtár).Where(x=> másolandópattern.IsMatch(x.Split('\\').Last())))
			{
                DirectoryCopy(path, célkönyvtár + "\\" + path.Split('\\').Last(), true);
                Directory.Delete(path,true);
            }
        }
	}
}
