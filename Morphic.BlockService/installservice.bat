taskkill /F /IM Morphic.BlockService.exe
SC CREATE "FocusService" start= auto binpath= "%cd%\Morphic.BlockService.exe" displayname= "Morphic Focus Web Blocking Service"
SC start FocusService
cd .