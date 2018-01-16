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

        static int FileCount = 0;
        static long s = 0;

        static string workingDirectory = Directory.GetCurrentDirectory();
        static string osuFolder = Path.GetDirectoryName(workingDirectory);
        static string output = "";
        
        static List<string> imageList = new List<string>(new string[] { ".png", ".jpg", ".jpeg" });
        
        static List<string> musicList = new List<string>(new string[] { ".mp3", ".ogg" });

        static List<string> noBackground = new List<string>();

        static List<string> errorList = new List<string>();

        static void Main(string[] args)
        {
            List<string> beatmapFolders = Directory.EnumerateDirectories(workingDirectory).ToList<string>();

            int i = 0;
            foreach (string beatmapFolder in beatmapFolders)
            {
                string percent = (100 * i / beatmapFolders.Count).ToString();

                output = TextBalancer(3, percent, 1);
                

                List<string> files = Directory.EnumerateFiles(beatmapFolder).ToList<string>();
                List<string> subFolders = Directory.EnumerateDirectories(beatmapFolder).ToList<string>();

                List<string> saveFiles = new List<string>();

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".osu")
                    {
                        string lines = File.ReadAllText(file);
                        lines = lines.Split(new[] { "AudioFilename: " }, StringSplitOptions.None)[1];
                        string audioFile = lines.Split(new char[] { '\r', '\n'})[0];
                        saveFiles.Add(Path.GetFileName(audioFile).ToLower());

                        string[] lines1 = lines.Split(new[] { "Video," }, StringSplitOptions.None);
                        
                        if (lines1.Length >= 2)
                        {
                            lines = lines1[0] + lines1[1].Split(new[] { "\r" }, StringSplitOptions.None)[1];
                        }


                        string[] lines2 = lines.Split(new[] { ",\"" }, StringSplitOptions.None);
                        if (lines2.Length <= 1)
                        {
                            noBackground.Add(beatmapFolder);
                        }
                        else
                        {
                            string bg = lines2[1].Split(new[] { "\"" }, StringSplitOptions.None)[0];
                            if (imageList.Contains(Path.GetExtension(bg).ToLower()))
                            {
                                saveFiles.Add(Path.GetFileName(bg).ToLower());
                            }
                            else
                            {
                                if (lines2.Length <= 2)
                                {
                                    noBackground.Add(beatmapFolder);
                                }
                                else
                                {
                                    bg = lines2[2].Split(new[] { "\"" }, StringSplitOptions.None)[0];
                                    if (imageList.Contains(Path.GetExtension(bg).ToLower()))
                                    {
                                        saveFiles.Add(Path.GetFileName(bg).ToLower());
                                    }
                                    else
                                    {
                                        noBackground.Add(beatmapFolder);
                                    }
                                }
                            }
                        }
                        

                        saveFiles.Add(Path.GetFileName(file).ToLower());
                    }
                }

                foreach (string file in files)
                {
                    Console.Write("[" + output + "%] ");
                    if (!saveFiles.Contains(Path.GetFileName(file).ToLower()))
                    {

                        DeleteFile(file);
                    }
                    Console.WriteLine("");
                    
                }

                foreach (string subFolder in subFolders)
                {
                    DeleteFolder(subFolder, saveFiles);
                }

                i++;
            }


            Console.WriteLine("##################################");
            PrintStatistics();
            Console.WriteLine("##################################");

            Console.WriteLine("Press ENTER to proced to the after Test where it checks every Folder for missing Items");
            Console.ReadLine();

            foreach (string folder in beatmapFolders)
            {
                int beatmapCounter = 0;
                int imageCounter = 0;
                int musicCounter = 0;
                int otherCounter = 0;

                foreach (string file in GetVeryAllFiles(folder))
                {
                    if (Path.GetExtension(file).ToLower() == ".osu")
                    {
                        beatmapCounter++;
                    }
                    else if (musicList.Contains(Path.GetExtension(file).ToLower()))
                    {
                        musicCounter++;
                    }
                    else if(imageList.Contains(Path.GetExtension(file).ToLower()))
                    {
                        imageCounter++;
                    }
                    else
                    {
                        otherCounter++;
                    }
                }
                string id = "";
                if (beatmapCounter <= 0)
                {
                    Console.WriteLine(TextBalancer(75,Path.GetFileName(folder), 2) + " Beatmaps");
                    id = Path.GetFileName(folder).Split(' ')[0];
                }
                if (imageCounter <= 0 && !noBackground.Contains(folder))
                {
                    Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " Images");
                    id = Path.GetFileName(folder).Split(' ')[0];
                }
                if (musicCounter <= 0)
                {
                    Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) +  " Music");
                    id = Path.GetFileName(folder).Split(' ')[0];
                }
                if (otherCounter >= 1)
                {
                    Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " Other");
                    id = Path.GetFileName(folder).Split(' ')[0];
                }
                if (id != "")
                {
                    try
                    {
                        int number = Convert.ToInt32(id);
                        if (errorList.Contains(number.ToString()))
                        {
                            Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " is suspected to be duplicated");
                        }
                        else
                        {
                            errorList.Add(number.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        if (errorList.Contains(Path.GetFileName(folder)))
                        {
                            Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " is suspected to be duplicated");
                        }
                        else
                        {
                            errorList.Add(Path.GetFileName(folder));
                        }
                    }
                }
            }

            Console.WriteLine("Press ENTER to check for duplicate Beatmaps");
            Console.ReadLine();

            List<string> numbers = new List<string>();

            foreach (string folder in beatmapFolders)
            {
                string id = Path.GetFileName(folder).Split(' ')[0];
                try
                {
                    int number = Convert.ToInt32(id);
                    if (numbers.Contains(number.ToString()))
                    {
                        Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " is suspected to be duplicated");
                    }
                    else
                    {
                        numbers.Add(number.ToString());
                    }
                }
                catch (Exception)
                {
                    if (numbers.Contains(Path.GetFileName(folder)))
                    {
                        Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " is suspected to be duplicated");
                    }
                    else
                    {
                        numbers.Add(Path.GetFileName(folder));
                    }
                }
            }

            Console.WriteLine("Press ENTER to end");
            Console.ReadLine();

            string fileWrite = "";

            foreach (string text in errorList)
            {
                fileWrite += text + ",";
            }

            fileWrite = fileWrite.Trim(','); //Deltes trailing ,

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\ErrorList.txt", fileWrite);
        }

        public static List<string> GetVeryAllFiles(string folder)
        {
            List<string> allFiles = Directory.EnumerateFiles(folder).ToList<string>();
            foreach (string subFolder in Directory.EnumerateDirectories(folder))
            {
                allFiles.AddRange(GetVeryAllFiles(subFolder));
            }
            return allFiles;
        }

        public static string TextBalancer(int desiredLength, string text, int type)
        {
            string output = "";
            switch (type)
            {
                case 0:
                    int missing = desiredLength - text.Length;

                    if (missing <= 0)
                    {
                        return text;
                    }
                    else
                    {
                        int missing1 = missing / 2;
                        int missing2 = missing - missing1;
                        string output2 = "";

                        for (int i = 0; i < missing1; i++)
                        {
                            output += " ";
                        }

                        for (int i = 0; i < missing2; i++)
                        {
                            output2 += " ";
                        }

                        return output + text + output2;
                    }
                    
                case 1:
                    if (desiredLength - text.Length <= 0)
                    {
                        return text;
                    }
                    else
                    {
                        for (int i = desiredLength; i > text.Length; i--)
                        {
                            output += " ";
                        }

                        return output + text;
                    }

                case 2:
                    if (desiredLength-text.Length <= 0)
                    {
                        return text;
                    }
                    else
                    {
                        for (int i = text.Length; i < desiredLength; i++)
                        {
                            output += " ";
                        }

                        return text + output;
                    }
                default:
                    return text;
            }
            
        }
       

        //Deletes a File and adds its size to the static s
        static public void DeleteFile(string file)
        {
            s += new FileInfo(file).Length;
            File.Delete(file);
            Console.Write(TextBalancer(50, Path.GetFileName(file), 2) + " - Deleted");
            FileCount++;
        }

        //Function deletes a whole folder exept if there is a background in it -> then it will delte everything exept the background file
        static public void DeleteFolder(string folder, List<string> saveFiles) {
            foreach (string file in Directory.EnumerateFiles(folder))
            {
                Console.Write("[" + output + "%] ");
                if (!saveFiles.Contains(Path.GetFileName(file).ToLower()))
                {
                    DeleteFile(file);
                }
                Console.WriteLine("");
            }
            foreach (string dir in Directory.EnumerateDirectories(folder))
            {
                DeleteFolder(dir, saveFiles);
            }
            if (Directory.EnumerateFiles(folder).Count() + Directory.EnumerateDirectories(folder).Count() == 0) // After deleting everything exept background, if there are files left in the folder it gets spared
            {
                Directory.Delete(folder);
            }
        }

        //Function to get all subfolders of all subfolder of all subfolder ...
        static public List<string> GetSubFolders(string folder, List<string> addHere)
        {
            foreach (string subFolder in Directory.EnumerateDirectories(folder))
            {
                addHere.Add(subFolder);
                GetSubFolders(subFolder, addHere);
            }

            return addHere;
        }

        static public void PrintStatistics()
        {
            Console.WriteLine(FileCount + " Files were deleted");
            if (s >= 1000000000.0)
            {
                Console.WriteLine(Math.Round(s / 1000000000.0, 2) + " GB were saved"); //If its more then 1 gb it will say   x,xx GB
            }
            else if (s >= 1000000.0)
            {
                Console.WriteLine(Math.Round(s / 1000000.0, 2) + " MB were saved"); //If its more then 1 mb it will say   x,xx MB
            }
            else if (s >= 1000.0)
            {
                Console.WriteLine(Math.Round(s / 1000.0, 2) + " KB were saved"); //If its more then 1 kb it will say   x,xx KB
            }
            else
            {
                Console.WriteLine(s + " B were saved"); //Else its    x B
            }
        }
        
    }
       
}
