Set WshShell = CreateObject( "WScript.Shell" )

dim programDir
programDir=  property("CustomActionData")
WshShell.CurrentDirectory = programDir

installandstartcommand = programDir & "installservice.bat"

WshShell.Run ("cmd /c " & """""" & installandstartcommand & """""")

Set WshShell = Nothing