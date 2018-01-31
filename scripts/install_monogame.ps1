# http://community.monogame.net/t/solved-build-monogame-3-6-application-on-appveyor-ci/8840/5
# Install MonoGame
(New-Object Net.WebClient).DownloadFile('http://www.monogame.net/releases/v3.6/MonoGameSetup.exe', 'C:\MonoGameSetup.exe')
Invoke-Command -ScriptBlock {C:\MonoGameSetup.exe /S /v/qn}
