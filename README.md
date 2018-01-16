# OsuMapCleaner
A simple c# console application that gets rid of files i dont need


I started this when my PC ran out of storage space and i realised how much space Osu uses.
Since i always play with background dim 100%, no video and no storyboard i thought i could get rid of those.
The videos alone cleared up a lot of space, but i was intrigued of how far i could take this.

## Files i want to keep
```
  .osu      - else you wont have any beatmaps
  .mp3      - you could technically still play but thats i like them
  .jpg/png  - well not all of them only the backgrounds because i like the preview while browsing maps
```
This means a normal beatmap folder should in the best case have: 1 Background Image, 1 Music File, x  amount of .osu files.

In my first development cycle i created whitelists and blacklists for example: .wav file was blacklisted because only hitsounds use .wav, or .mp3 was whitelisted because almost all of the music is in mp3.

After the rewrite i choose that instead of deleting everything i dont like, i should rather keep everything that i want and delete the rest, so the Programm searches through the .osu files and determins what to keep and what to delete.
Meaning everything gets deleted except its an .osu file or it is specified inside the .osu file (only Backgrounds and Music)

## How to use it

Just place the .exe file inside of your Songs folder (Probably at C:\Users\USER\AppData\Local\osu!\Songs replace USER with your username) and start it, it will instantly beginn deleting files. 

## How much will it delete

To benchmark it i downloaded all of the [Torrent Beatmappacks](https://osu.hiramiya.me/torrents.htm), this is just a bit above 100 Gb of maps. After i ran my programm it shrunk down to 50 Gb. Since these are Beatmaps from 2007 untill 2017 its a pretty good baseline to say it will get rid of **50%** assuming you downloaded everymap with its video.

After the rewrite it will probably delete 1-5% more.

## The Error File

After the deletion process is finished it will quickly look over the Beatmap folders to make sure that every folder has at least one music, background and .osu file. If there is a problem it will ask the user to check these folders and it will also write all the Beatmap Ids into the ErrorList.txt file. You should not be to worried about it if some maps show up, older maps (especialy unranked) somtimes dont have an image while still including it in the .osu file. Feel free to look into those folders and .osu files to find out if something is wrong. If some files go missing that should be there (and are there when you redownload) let me know.

The programm will also let you know when a beatmap id apears more then once. This is the case when you downloaded a map twice and the installation folder got a differnet name, you can just delete the older of the two folders.

## Autohotkey -- This is only optional

After i deleted a lot of the files i wanted to keep while testing i wrote a AHK script that will automatically download all the maps listed in the error file. To use this you have to move the script into your maps folder. It will wait until you press CTRL+J, but for it to work correctly you first have to login into Osu using Google Chrome and tick the box to stay logged in. After pressing the Hotkey (CTRL+J) it will open a lot of new Tabs end everyone of them will download one file. You cant do anything else while its opening new Tabs but once it stops you can do whatever you want while waiting for the downloads to finish. But only close the Tabs when all dowloads are finished else it will not finish those that are still going.
* [Autohotkey](https://autohotkey.com/)

### If you look at all the images in Test Doku sorted by date you will see how it went down

### I wont compensate you for any files lost
