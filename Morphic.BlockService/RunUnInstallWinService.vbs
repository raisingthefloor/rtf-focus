Set WshShell = CreateObject( "WScript.Shell" )

dim programDir
programDir=  property("CustomActionData")
WshShell.CurrentDirectory = programDir

command = programDir & "uninstallservice.bat"

WshShell.Run ("cmd /c " & """""" & command & """""")
Set WshShell = Nothing