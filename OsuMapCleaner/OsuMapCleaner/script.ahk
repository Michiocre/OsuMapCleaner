^j::
	FileRead, Content, %A_WorkingDir%\ErrorList.txt
	StringSplit, array, Content, `, ;Splits at ','
	array := Trim(array)
	Send, ^n ;Firstly opens a new browserwindow
	Sleep, 100 ;Waiting for the window
	Loop, %array0%
	{
		this_number := array%a_index%
		Send, https://osu.ppy.sh/d/%this_number%n ;Types the direct downloadling (the trailing n is for no Video)
		Send, {Enter}
		Send, ^t ;Open new Tab
	}
	Send, ^w ;Closes the last tab
Return