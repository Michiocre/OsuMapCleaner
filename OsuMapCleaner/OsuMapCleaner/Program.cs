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

        static string workingDirectory = Path.GetDirectoryName(Directory.GetCurrentDirectory()) + @"\Songs";
        static string osuFolder = Path.GetDirectoryName(workingDirectory);
        static string output = "";
        
        static List<string> imageList = new List<string>(new string[] { ".png", ".jpg", ".jpeg" });
        
        static List<string> musicList = new List<string>(new string[] { ".mp3", ".ogg" });

        static List<string> noBackground = new List<string>();

        static List<string> errorList = new List<string>();

        static List<string> completed = new List<string>();
        static List<string> oldCompleted = new List<string>();

        static string lastLine = "";

        static string keepMode = "k";

        static void Main(string[] args)
        {
            if (Path.GetFileName(Directory.GetCurrentDirectory()) != "OsuMapCleaner")
            {
                Console.WriteLine("You are not executing the programm from the OsuMapCleaner folder, this could delete files you dont want to delete.");
                Console.WriteLine("This executable should be locatet in: /osu!/OsuMapCleaner");
                Console.WriteLine("Execution terminated. Press ENTER to close.");
                Console.ReadLine();
                System.Environment.Exit(1);
            }
            if (!Directory.Exists(workingDirectory))
            {
                Console.WriteLine("The songs folder could not be found");
                Console.WriteLine("This executable should be locatet in: /osu!/OsuMapCleaner");
                Console.WriteLine("Execution terminated. Press ENTER to close.");
                Console.ReadLine();
                System.Environment.Exit(1);
            }

            Console.WriteLine("Type 'Config' to open customize the execution");
            string answer = Console.ReadLine();
            if (answer == "Config" || answer == "C" || answer == "c")
            {
                Console.WriteLine("Do you want to rescan the whole songs folder? (Y/N)");
                answer = Console.ReadLine();
                if (!(answer == "Y" || answer == "y"))
                {
                    string completedFile = Directory.GetCurrentDirectory() + @"\MapCleanerCompleted.txt";

                    if (File.Exists(completedFile))
                    {
                        oldCompleted = File.ReadAllText(completedFile).Split(new[] { ',' }).ToList<string>();
                    }
                }

                Console.WriteLine("Do you want to delete duplicate folder, keep noVideo or keep withVideo? (N/W)");
                answer = Console.ReadLine();
                if (answer == "N" || answer == "n")
                {
                    keepMode = "n";
                }
                else if (answer == "W" || answer == "w")
                {
                    keepMode = "w";
                }
            }
            

            List<string> beatmapFolders = Directory.EnumerateDirectories(workingDirectory).ToList<string>();

            int i = 0;
            foreach (string beatmapFolder in beatmapFolders)
            {
                string id = Path.GetFileName(beatmapFolder).Split(' ')[0];

                if (!oldCompleted.Contains(id))
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
                            string text = File.ReadAllText(file);
                            if (text != "")
                            {
                                text = text.Split(new[] { "AudioFilename: " }, StringSplitOptions.None)[1];
                                string audioFile = text.Split(new char[] { '\r', '\n' })[0];
                                saveFiles.Add(Path.GetFileName(audioFile).ToLower());

                                string[] textParts = text.Split(new[] { "Video," }, StringSplitOptions.None);

                                if (textParts.Length >= 2)
                                {
                                    try
                                    {
                                        text = textParts[0] + textParts[1].Split(new[] { '\r' }, StringSplitOptions.None)[1];
                                    }
                                    catch (Exception)
                                    {
                                        text = textParts[0] + textParts[1].Split(new[] { '\r', '\n' }, StringSplitOptions.None)[1];
                                    }
                                }

                                textParts = text.Split(new[] { ",\"" }, StringSplitOptions.None);
                                if (textParts.Length <= 1)
                                {
                                    noBackground.Add(beatmapFolder);
                                }
                                else
                                {
                                    string bg = textParts[1].Split(new[] { "\"" }, StringSplitOptions.None)[0];
                                    if (imageList.Contains(Path.GetExtension(bg).ToLower()))
                                    {
                                        saveFiles.Add(Path.GetFileName(bg).ToLower());
                                    }
                                    else
                                    {
                                        if (textParts.Length <= 2)
                                        {
                                            noBackground.Add(beatmapFolder);
                                        }
                                        else
                                        {
                                            bg = textParts[2].Split(new[] { "\"" }, StringSplitOptions.None)[0];
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
                    }

                    foreach (string file in files)
                    {
                        if (!saveFiles.Contains(Path.GetFileName(file).ToLower()))
                        {
                            lastLine = "[" + output + "%] ";
                            Console.Write("[" + output + "%]");
                            DeleteFile(file);
                            Console.WriteLine("");
                        }
                        else
                        {
                            if (lastLine != "[" + output + "%] ")
                            {
                                lastLine = "[" + output + "%] ";
                                Console.Write("[" + output + "%] ");
                                Console.WriteLine("");
                            }
                        }
                    }

                    foreach (string subFolder in subFolders)
                    {
                        DeleteFolder(subFolder, saveFiles);
                    }

                    i++;
                }
            }


            Console.WriteLine("##################################");
            PrintStatistics();
            Console.WriteLine("##################################");

            Console.WriteLine("Press ENTER to proceed to the after Test where it checks every Folder for missing Items");
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
                string id = Path.GetFileName(folder).Split(' ')[0];
                bool error = false;
                if (beatmapCounter <= 0)
                {
                    Console.WriteLine(TextBalancer(75,Path.GetFileName(folder), 2) + " Beatmaps");
                    error = true;
                }
                if (imageCounter <= 0 && !noBackground.Contains(folder))
                {
                    Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " Images");
                    error = true;
                }
                if (musicCounter <= 0)
                {
                    Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) +  " Music");
                    error = true;
                }
                if (otherCounter >= 1)
                {
                    Console.WriteLine(TextBalancer(75, Path.GetFileName(folder), 2) + " Other");
                    error = true;
                }
                if (error)
                {
                    errorList.Add(id);
                }
                else
                {
                    completed.Add(id);
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
                        DeleteDups(number, beatmapFolders);
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

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\MapCleanerErrors.txt", fileWrite);

            fileWrite = "";

            foreach (string text in completed)
            {
                fileWrite += text + ",";
            }

            fileWrite = fileWrite.Trim(','); //Deltes trailing ,

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\MapCleanerCompleted.txt", fileWrite);
        }

        public static void DeleteDups(int number, List<string> beatmapFolders)
        {
            if (keepMode == "w" || keepMode == "n")
            {
                List<string> dups = new List<string>();
                foreach (string folder in beatmapFolders)
                {
                    string id = Path.GetFileName(folder).Split(' ')[0];
                    if (id == number.ToString())
                    {
                        dups.Add(folder);
                    }
                }

                int keep = 0;

                if (keepMode == "n")
                {
                    for (int i = 0; i < dups.Count; i++)
                    {
                        if (dups[i].Contains("[no video]"))
                        {
                            keep = i;
                        }
                    }
                }

                if (keepMode == "w")
                {
                    for (int i = 0; i < dups.Count; i++)
                    {
                        if (!dups[i].Contains("[no video]"))
                        {
                            keep = i;
                        }
                    }
                }

                for (int i = 0; i < dups.Count; i++)
                {
                    if (i != keep)
                    {
                        DeleteFolder(dups[i], new List<string>());
                    }
                }
            }
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
