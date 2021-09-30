Set WshShell = CreateObject( "WScript.Shell" )

dim programDir
programDir=  property("CustomActionData")
WshShell.CurrentDirectory = programDir

installcommand = programDir & "Blocking Service\installservice.bat"
WshShell.Run ("cmd /c " & """""" & installcommand & """""")

startcommand = programDir & "Blocking Service\startservice.bat"
WshShell.Run ("cmd /c " & """""" & startcommand & """""")


Set WshShell = Nothing