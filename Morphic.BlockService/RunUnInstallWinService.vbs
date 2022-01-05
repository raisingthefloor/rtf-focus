Set WshShell = CreateObject( "WScript.Shell" )

dim programDir
programDir=  property("CustomActionData")
WshShell.CurrentDirectory = programDir

command = programDir & "Blocking Service\uninstallservice.bat"
msgbox command
WshShell.Run ("cmd /c " & """""" & command & """""")
Set WshShell = Nothing