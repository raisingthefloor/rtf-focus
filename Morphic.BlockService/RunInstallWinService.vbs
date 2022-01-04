Set WshShell = CreateObject( "WScript.Shell" )

dim programDir
programDir=  property("CustomActionData")
WshShell.CurrentDirectory = programDir

installcommand = programDir & "Blocking Service\installservice.bat"
WshShell.Run ("cmd /k " & """""" & installcommand & """""")

WshShell.Sleep 20000

startcommand = programDir & "Blocking Service\startservice.bat"
WshShell.Run ("cmd /k " & """""" & startcommand & """""")


Set WshShell = Nothing