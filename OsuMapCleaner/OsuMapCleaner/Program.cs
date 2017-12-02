using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OsuMapCleaner
{
    class Program
    {
        static int x = 0;
        static long s = 0;

        static List<string> imageList = new List<string>(new string[] {".png", ".jpg", ".jpeg"});

        static List<string> musicWhiteList = new List<string>(new string[] { ".mp3", ".ogg"});

        static List<string> whiteList = new List<string>(new string[] {".osu", ".osz", ".osr"});
        
        static List<string> backgrounds;

        static List<string> numbers = new List<string>();

        static void Main(string[] args)
        {
            whiteList.AddRange(musicWhiteList);

            string[] folders = Directory.EnumerateDirectories(Directory.GetCurrentDirectory()).ToArray<string>();
            string input = Console.ReadLine();

            List<string> noBackgroundFolder = new List<string>();


            //Main Part

            double lastPercent = -1;

            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                double percent = Math.Round(i / (folders.Length / 100.0));
                List<string> files = Directory.EnumerateFiles(folder).ToList<string>();

                backgrounds = new List<string>();

                //Searching for the Backgrounds     
                string noBackground = "";
                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();

                    if (extension == ".osu")
                    {
                        string raw = File.ReadAllText(file);
                        try
                        {
                            string bgName = Regex.Split(Regex.Split(raw, "0,0,\"")[1], "\"")[0];

                            if (imageList.Contains(Path.GetExtension(bgName).ToLower()))
                            {
                                backgrounds.Add(bgName);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            noBackground = folder;
                        }
                    }
                }

                if (noBackground != "")
                {
                    noBackgroundFolder.Add(noBackground);
                }

                //Done With Searching

                foreach (string file in files)
                {
                    bool delete = true;
                    string extension = Path.GetExtension(file).ToLower();
                    if (whiteList.Contains(extension))
                    {
                        delete = false;
                    }
                    else
                    {
                        foreach (string bg in backgrounds)
                        {
                            if (Path.GetFileName(file) == bg || Path.GetFileName(file).ToLower() == bg.ToLower())
                            {
                                delete = false;
                            }
                        }
                    }

                    if (delete)
                    {
                        DeleteFile(file);
                    }
                }

                foreach (string dir in Directory.EnumerateDirectories(folder))
                {
                    DeleteFolder(dir);
                }

                if (percent > lastPercent)
                {
                    Console.WriteLine("Percent: " + percent + "%");
                    lastPercent = percent;
                }
            }

            Console.WriteLine(x + " Files were deleted");
            if (s >= 1000000000.0)
            {
                Console.WriteLine(s / 1000000000.0 + " GB were saved");
            }
            else if (s >= 1000000.0)
            {
                Console.WriteLine(s / 1000000.0 + " MB were saved");
            }
            else if (s >= 1000.0)
            {
                Console.WriteLine(s / 1000.0 + " KB were saved");
            }
            else
            {
                Console.WriteLine(s + " B were saved");
            }

            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\noBackground.txt", noBackgroundFolder);

            Console.WriteLine("#-----------------------#");
            Console.WriteLine("All the Files were deleted, now its searching for Errors (this will be much quicker)");
            Console.WriteLine("#-----------------------#");

            //Finding out if all the maps have at least one background (Exept when thats intended)
            string noBgPath = Directory.GetCurrentDirectory() + "\\noBackground.txt";
            List<string> noBackgrounds = new List<string>();
            if (File.Exists(noBgPath))
            {
                noBackgrounds.AddRange(File.ReadAllLines(noBgPath).ToList<string>());
            }
            foreach (string folder in folders)
            {
                int x = 0;
                if (noBackgrounds.Count > 0)
                {
                    if (noBackgrounds.Contains(folder))
                    {
                        x = 1;
                    }
                }

                List<string> subFolders = new List<string>();

                getSubFolders(folder, subFolders);

                subFolders.Add(folder);

                foreach (string subFolder in subFolders)
                {
                    List<string> files = Directory.EnumerateFiles(subFolder).ToList<string>();

                    foreach (string file in files)
                    {
                        if (imageList.Contains(Path.GetExtension(file).ToLower()))
                        {
                            x++;
                        }
                    }
                }

                if (x == 0)
                {
                    string outp = folder.Split('\\')[folder.Split('\\').Length - 1];

                    numbers.Add(outp.Split(' ')[0]);

                    Console.WriteLine("Check: " + outp + "   | for missing images");
                }
            }
            Console.WriteLine("#-----------------------#");
            Console.WriteLine("First iteration complete");
            Console.WriteLine("#-----------------------#");

            foreach (string folder in folders)
            {
                List<string> files = Directory.EnumerateFiles(folder).ToList<string>();
                int x = 0;
                foreach (string file in files)
                {
                    if (musicWhiteList.Contains(Path.GetExtension(file).ToLower()))
                    {
                        x++;
                    }
                }
                if (x == 0)
                {
                    string outp = folder.Split('\\')[folder.Split('\\').Length - 1];
                    string number = outp.Split(' ')[0];
                    if (!numbers.Contains(number))
                    {
                        numbers.Add(number);
                    }

                    Console.WriteLine("Check: " + outp + "   | for missing Audio");
                }
            }

            string fileWrite = "";

            foreach (string number in numbers)
            {
                try
                {
                    Convert.ToInt32(number);
                    fileWrite += number + ",";
                }
                catch (Exception)
                {

                }
            }

            fileWrite = fileWrite.Trim(',');

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\ErrorList.txt", fileWrite);
            Console.WriteLine("#-----------------------#");
            Console.WriteLine("And were done");
            Console.WriteLine("#-----------------------#");
            Console.ReadLine();
        }

        static public void DeleteFile(string file)
        {
            s += new FileInfo(file).Length;
            File.Delete(file);
            Console.WriteLine(file);
            x++;
        }

        static public void DeleteFolder(string folder) {
            foreach (string file in Directory.EnumerateFiles(folder))
            {
                int why = 0;
                foreach (string bg in backgrounds)
                {
                    if (Path.GetFileName(bg) == Path.GetFileName(file))
                    {
                        why++;
                    }
                }

                if (why == 0)
                {

                    DeleteFile(file);
                }
                else
                {
                    
                }
            }
            foreach (string dir in Directory.EnumerateDirectories(folder))
            {
                DeleteFolder(dir);
            }
            if (Directory.EnumerateFiles(folder).Count() + Directory.EnumerateDirectories(folder).Count() == 0)
            {
                Directory.Delete(folder);
            }
        }

        static public List<string> getSubFolders(string folder, List<string> addHere)
        {
            foreach (string subFolder in Directory.EnumerateDirectories(folder))
            {
                addHere.Add(subFolder);
                getSubFolders(subFolder, addHere);
            }

            return addHere;
        }
    }
       
}
