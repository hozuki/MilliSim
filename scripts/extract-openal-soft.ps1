param (
    [Parameter(Mandatory=$true)][string]$destination
)

echo "Downloading OpenAL-Soft (v1.18.2/Windows)..."
(New-Object Net.WebClient).DownloadFile('http://kcat.strangesoft.net/openal-binaries/openal-soft-1.18.2-bin.zip', 'C:\openal-soft.zip')

echo "Extracting OpenAL-Soft..."

$7zCommand = '7z e C:\openal-soft.zip -y -o"$destination" openal-soft-1.18.2-bin/bin/Win32/soft_oal.dll'
$block = [scriptblock]::Create($7zCommand)

Invoke-Command -ScriptBlock $block

echo "Renaming to openal32.dll..."

$openAlDllPath = [System.IO.Path]::Combine($destination, "soft_oal.dll")
$openAlDllNewName = "openal32.dll"

Rename-Item -Path $openAlDllPath -NewName $openAlDllNewName
