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

After the Rewrite i choose the instead of deleting everything i dont like, i should rather keep everything that i want and delete the rest, so the Programm searches through the .osu files and determins what to keep and what to delete.

## How to use it

You just have to pull the .exe file into your songs folder and start it

## How much will it delete

To benchmark it i downloaded all of the [Torrent Beatmappacks](https://osu.hiramiya.me/torrents.htm), this is just a bit above 100 Gb of maps. After i ran my programm it shrunk down to 50 Gb. Since these are Beatmaps from 2007 till 2017 its a pretty good baseline to say it will get rid of **50%** assuming you downloaded everymap with its video.

After the rewrite it will probably delete 1-5% more.

## The Error File

After the deletion process is finished it will quickly look over the Beatmap folders to make sure there are no maps without any sound files or without at least one picture. If this is the case it will ask you to check these folders and it will also write all the Beatmap Ids into the ErrorList.txt file. You should be to worried about it if some maps show up, older maps (especialy unranked) somtimes dont have an image while still including it in the .osu file. Feel free to look into those folders and .osu files to find out if something is wrong. If some files go missing that should be there (and are there when you redownload) let me know.

The programm will also let you know when a beatmap id apears more then once. This is the case when you downloaded a map twice and the installation folder got a differnet name, you can just delete the older of the two folders.

## Autohotkey -- This is only optional

After i deleted a lot of the files i wanted to keep while testing i wrote a AHK script that will automatically download all the maps listed in the error file. To use this you have to move the script into your maps folder. It will wait until you press CTRL+J, but for it to work correctly you first have to loggin into Osu in Google Chrome and tick the box to stay logged in. After pressing the Hotkey it will open a lot of new Tabs end everyone of them will download one file. You cant do anything else while its opening new Tabs but once it stops you can do whatever you want while waiting for the downloads to finish. But only close the Tabs when all dowloads are finished else it wont download all of the files.
* [Autohotkey](https://autohotkey.com/)

### If you look at all the images in Test Doku sorted by date you will see how it went down

### I wont compensate you for any files lost
