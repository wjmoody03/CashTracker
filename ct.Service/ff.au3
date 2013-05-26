AutoItSetOption("WinTitleMatchMode", "2")
 
WinWait("Opening")
$title = WinGetTitle("Opening")
WinActivate($title)
WinWaitActive($title)
Send("!s")
Send("{ENTER}")
 
