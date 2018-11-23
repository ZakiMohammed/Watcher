using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watcher
{
    class Program
    {
        static DateTime lastWrite = new DateTime();

        static string directoryName = "watcher";
        static string fileName = "path.txt";
        static string dPath = "";
        static string mPath = "";

        static void Main(string[] args)
        {
            try
            {
                var exit = false;
                var option = string.Empty;

                Setting();

                do
                {
                    try
                    {
                        var initialPath = "";
                        var directoryNames = "";
                        var directoryInput = "";

                        Console.Write("Last modified date time: ");
                        lastWrite = Convert.ToDateTime(Console.ReadLine());

                        Console.Write("Main Path (d:Desktop/m:Mobile): ");
                        initialPath = Console.ReadLine();

                        if (initialPath == "d")
                            initialPath = dPath;
                        if (initialPath == "m")
                            initialPath = mPath;

                        Console.Write("Directories: ");
                        directoryInput = Console.ReadLine();

                        if (directoryInput == "\\")
                        {
                            initialPath = initialPath.TrimEnd('\\');
                            directoryNames = "";
                        }

                        Console.WriteLine("\nModified files: ");

                        foreach (var directoryName in directoryNames.Split(','))
                            Watch(initialPath + directoryName);
                    }
                    catch { }
                    finally
                    {
                        Console.Write("\n(1) Continue (2) Setting (3) Quit: ");
                        option = Console.ReadLine();

                        if (option == "2")
                            Setting(true);
                        else if (option == "3")
                            exit = true;

                        Console.WriteLine();
                    }

                } while (!exit);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError: " + ex.Message);
                Console.WriteLine(ex);
            }
        }

        static void Setting(bool isUpdate = false)
        {
            var drives = DriveInfo.GetDrives();
            var content = "";
            var path = "";
            var doSetting = false;

            if (drives.Length > 0)
            {
                path = drives[1].Name + directoryName + "\\" + fileName;

                if (!Directory.Exists(drives[1].Name + directoryName))
                {
                    Directory.CreateDirectory(drives[1].Name + directoryName);
                    doSetting = true;
                }
                else
                {
                    if (!File.Exists(path))
                        doSetting = true;
                }

                if (doSetting || isUpdate)
                {
                    Console.WriteLine("Setting");

                    Console.Write("Enter desktop folder path: ");
                    content = Console.ReadLine();

                    Console.Write("Enter mdot folder path: ");
                    content += "," + Console.ReadLine();

                    File.WriteAllText(path, content);
                }

                content = File.ReadAllText(path);
                if (content.Split(',').Length > 1)
                {
                    dPath = content.Split(',')[0];
                    mPath = content.Split(',')[1];
                }
            }
        }

        static void Watch(string directoryName)
        {
            var directory = new DirectoryInfo(directoryName);
            var found = false;

            foreach (var subDirectory in directory.GetDirectories())
                Watch(subDirectory.FullName);

            foreach (var file in directory.GetFiles())
            {
                var fileInfo = new FileInfo(file.FullName);

                if (fileInfo.Extension != ".scc")
                {
                    if (file.LastWriteTime >= lastWrite)
                    {
                        Console.WriteLine(fileInfo.FullName);
                        found = true;
                    }
                }
            }

            if (found)
                Console.WriteLine();
        }
    }
}
