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

        //All the Extensions Counting as images
        static List<string> imageList = new List<string>(new string[] {".png", ".jpg", ".jpeg"});

        //All the files that count as music files who will be whitelisted
        static List<string> musicWhiteList = new List<string>(new string[] { ".mp3", ".ogg"});

        //All the other files that will be whitelisted
        static List<string> whiteList = new List<string>(new string[] {".osu", ".osz", ".osr"});

        //Everything that is whitelisted will not get deleted no matter what

        //Initialisation of some generaly used Lists
        static List<string> numbers = new List<string>();
        static List<string> noBackgroundList = new List<string>();

        static void Main(string[] args)
        {
            whiteList.AddRange(musicWhiteList);

            string[] beatmapFolders = Directory.EnumerateDirectories(Directory.GetCurrentDirectory()).ToArray<string>();
            string input = Console.ReadLine(); //Waiting for the first enter
            

            //Main Part of the Code here the files will either get deleted or let through

            //Variables used to calculate how far through the folders we are
            double lastPercent = -1;
            double percent;

            for (int i = 0; i < beatmapFolders.Length; i++)
            {
                string currentFolder = beatmapFolders[i];
                percent = Math.Round(i / (beatmapFolders.Length / 100.0));
                List<string> files = Directory.EnumerateFiles(currentFolder).ToList<string>();

                //Searching for the Backgrounds   
                List<string> backgrounds = new List<string>();
                string noBackground = "";
                
                //During the first iteration it will read all the .osu files and search for all the used background files
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".osu")
                    {
                        string raw = File.ReadAllText(file);
                        try
                        {
                            string bgName = Regex.Split(Regex.Split(raw, "0,0,\"")[1], "\"")[0]; //The backgroundname should be between the first two quotes

                            if (imageList.Contains(Path.GetExtension(bgName).ToLower())) //If it isnt an image it will try again between the next two quotes
                            {
                                if (!backgrounds.Contains(bgName))
                                {
                                    backgrounds.Add(bgName);
                                }
                            }
                            else
                            {
                                bgName = Regex.Split(Regex.Split(raw, "0,0,\"")[3], "\"")[2];
                                if (imageList.Contains(Path.GetExtension(bgName).ToLower())) //If it still isnt an image there is no background
                                {
                                    if (!backgrounds.Contains(bgName))
                                    {
                                        backgrounds.Add(bgName);
                                    }
                                }
                            }
                        }
                        catch (IndexOutOfRangeException) //If there are no quotations in the whole file there cant be a background -> these will get written in the noBackground.txt
                        {
                            noBackground = currentFolder;
                        }
                    }
                }

                if (noBackground != "")
                {
                    noBackgroundList.Add(noBackground);
                }

                //Now we have all the background filenames, and now which files dont have a background

                foreach (string file in files)
                {
                    bool delete = true;
                    string extension = Path.GetExtension(file).ToLower(); //This is very important as sometimes there will be: mp3, MP3, Mp3
                    if (whiteList.Contains(extension)) //If its whitelisted
                    {
                        delete = false;
                    }
                    else
                    {
                        foreach (string bg in backgrounds)
                        {
                            if (Path.GetFileName(file) == bg || Path.GetFileName(file).ToLower() == bg.ToLower()) //Sometimes in the file its BG.png, but the file is called bg.png
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

                foreach (string dir in Directory.EnumerateDirectories(currentFolder)) //Deletes all the folders inside the Beatmap folder -> Mostly SB (Storyboard folder)
                {
                    DeleteFolder(dir, backgrounds);
                }

                if (percent > lastPercent) //If we have gone up by one percent it will print again
                {
                    Console.WriteLine("Percent: " + percent + "%");
                    lastPercent = percent;
                }
            }

            //s is the amount of storage cleard in bytes

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


            Console.WriteLine("#-----------------------#");
            Console.WriteLine("Checking all the Folders for missing Background");
            Console.WriteLine("#-----------------------#");

            //Finding out if all the maps have at least one background (Exept when thats intended)
            foreach (string folder in beatmapFolders)
            {
                bool hasBackground = false;
                if (noBackgroundList.Count > 0)
                {
                    if (noBackgroundList.Contains(folder)) //If the beatmap doesnt have a background (as read from the .osu file)
                    {
                        hasBackground = true;
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
                            hasBackground = true;
                        }
                    }
                }

                if (!hasBackground)
                {
                    string outp = folder.Split('\\')[folder.Split('\\').Length - 1];
                    string number = outp.Split(' ')[0];

                    if (!numbers.Contains(number))
                    {
                        numbers.Add(number);        //Lists the beatmap ids when its missing
                    }

                    Console.WriteLine("Check: " + outp);
                }
            }
            Console.WriteLine("#-----------------------#");
            Console.WriteLine("Checking all Folders for missing Audio");
            Console.WriteLine("#-----------------------#");


            //Finding out if all the maps have at least one music file
            foreach (string folder in beatmapFolders)
            {
                List<string> files = Directory.EnumerateFiles(folder).ToList<string>();
                bool hasMusic = false;
                foreach (string file in files)
                {
                    if (musicWhiteList.Contains(Path.GetExtension(file).ToLower()))
                    {
                        hasMusic = true;
                    }
                }
                if (!hasMusic)
                {
                    string outp = folder.Split('\\')[folder.Split('\\').Length - 1];
                    string number = outp.Split(' ')[0];
                    if (!numbers.Contains(number))
                    {
                        numbers.Add(number);        //Lists the beatmap ids when its missing
                    }

                    Console.WriteLine("Check: " + outp);
                }
            }


            Console.WriteLine("#-----------------------#");
            Console.WriteLine("Checking all Folders for missing Osu Files");
            Console.WriteLine("#-----------------------#");

            foreach (string folder in beatmapFolders)
            {
                List<string> files = Directory.EnumerateFiles(folder).ToList<string>();
                bool hasBeatmap = false;
                foreach (string file in files)
                {
                    if (Path.GetExtension(file).ToLower() == ".osu")
                    {
                        hasBeatmap = true;
                    }
                }
                if (!hasBeatmap)
                {
                    string outp = folder.Split('\\')[folder.Split('\\').Length - 1];
                    string number = outp.Split(' ')[0];
                    if (!numbers.Contains(number))
                    {
                        numbers.Add(number);        //Lists the beatmap ids when its missing
                    }

                    Console.WriteLine("Check: " + outp);
                }
            }


            //Creates the Text for the error file
            string fileWrite = "";

            foreach (string number in numbers)
            {
                try
                {
                    Convert.ToInt32(number); // This sorts out all the Beatmaps without ids
                    fileWrite += number + ",";
                }
                catch (Exception)
                {

                }
            }

            fileWrite = fileWrite.Trim(','); //Deltes trailing ,

            Console.WriteLine("#-----------------------#");
            Console.WriteLine("Checking all Folders for Duplicates");
            Console.WriteLine("#-----------------------#");


            List<string> allThemNumbers = new List<string>();

            foreach (string folder in beatmapFolders)
            {
                string outp = folder.Split('\\')[folder.Split('\\').Length - 1];
                string number = outp.Split(' ')[0];
                if (!allThemNumbers.Contains(number))
                {
                    try
                    {
                        Convert.ToInt32(number); // This sorts out all the Beatmaps without ids
                        allThemNumbers.Add(number);
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    Console.WriteLine("Check " + outp);
                }

            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "\\ErrorList.txt", fileWrite);
            Console.WriteLine("#-----------------------#");
            Console.WriteLine("And were done");
            Console.WriteLine("#-----------------------#");
            Console.ReadLine();
        }

        //Deletes a File and adds its size to the static s
        static public void DeleteFile(string file)
        {
            s += new FileInfo(file).Length;
            File.Delete(file);
            Console.WriteLine(file);
            FileCount++;
        }

        //Function deletes a whole folder exept if there is a background in it -> then it will delte everything exept the background file
        static public void DeleteFolder(string folder, List<string> backgrounds) {
            foreach (string file in Directory.EnumerateFiles(folder))
            {
                bool isBackground = false;
                foreach (string bg in backgrounds)
                {
                    if (Path.GetFileName(bg) == Path.GetFileName(file) || Path.GetFileName(bg) == Path.GetFileName(file).ToLower())
                    {
                        isBackground = true;
                    }
                }

                if (!isBackground)
                {
                    DeleteFile(file);
                }
            }
            foreach (string dir in Directory.EnumerateDirectories(folder))
            {
                DeleteFolder(dir, backgrounds);
            }
            if (Directory.EnumerateFiles(folder).Count() + Directory.EnumerateDirectories(folder).Count() == 0) // After deleting everything exept background, if there are files left in the folder it gets spared
            {
                Directory.Delete(folder);
            }
        }

        //Function to get all subfolders of all subfolder of all subfolder ...
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
