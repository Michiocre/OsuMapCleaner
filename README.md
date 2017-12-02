# OsuMapCleaner
A simple c# console application that gets rid of files i dont need


I started this when my PC ran out of storage space and i realised how much Osu uses.
Since i always play with background dim 100%, no video or storyboard i thought i could get rid of those.
The videos alone cleared up a lot of space, but i thought to myself how far i could take this.

## Files i want to keep
```
  .osu      - else you wont have any beatmaps
  .mp3      - you could technically still play but thats i like them
  .jpg/png  - well not all of them only the backgrounds because i like the preview while browsing maps
```
So im gonna delete all the videos, storyboard elements, custom skins and hitsounds

Later i realised i also had to watch out for some rouge .ogg and .jpeg files

## How to use it

You just have to pull the .exe file into your songs folder -> start it and press ENTER

## How much will it delete

To benchmark it i downloaded all of the [Torrent Beatmappacks](https://osu.hiramiya.me/torrents.htm), this is just a bit above 100 Gb of maps. After i ran my programm it shrunk down to 50 Gb. Since these are Beatmaps from 2007 till 2017 its a pretty good baseline to say it will get rid of **50%** assuming you downloaded everymap with its video.

## The Error File

After deleting everything the programm checks that every Mapset still has at least one image and one music file. When it doesnt find any the map id is listed in the file.

## Autohotkey

After i deleted a lot of the files i wanted to keep while testing i wrote a AHK script that will automatically download all the maps listed in the error file. To use this you have to move the script into your maps folder. It will wait until you press CTRL+J, but for it to work correctly you first have to loggin into Osu in Google Chrome and tick the box to stay logged in. After pressing the Hotkey it will open a lot of new Tabs end everyone of them will download one file. You cant do anything else while its opening new Tabs but once it stops you can do whatever you want while waiting for the downloads to finish. But only close the Tabs when all dowloads are finished else it wont download all of the files.
* [Autohotkey](https://autohotkey.com/)

## Checking Files

At the end of the programm it will tell you to check some maps for pictures, or sound files. These are the maps the Programm isnt sure about, when you redownload them and there isnt a Background in the original .osz file then the programm will always throw this error.

### If you look at all the images in Test Doku sorted by date you will see how it went down
