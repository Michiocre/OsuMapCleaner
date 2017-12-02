^j::
	FileRead, Content, %A_WorkingDir%\ErrorList.txt
	StringSplit, array, Content, `,
	array := Trim(array)
	Send, ^n
	Sleep, 100
	Loop, %array0%
	{
		this_number := array%a_index%
		Send, https://osu.ppy.sh/d/%this_number%n
		Send, {Enter}
		Send, ^t
	}
	Send, ^w
Return