Write-Output "Started";

$folderPath = $PSScriptRoot + "\Playtata\Context\";
$sourceFile = $folderPath + "ChromePlaytataContextBuilder.cs";
Write-Output "Source File=" $sourceFile;
$sourceContent = Get-Content $sourceFile;

New-Item ($folderPath + "FirefoxPlaytataContextBuilder.cs") -type file -force;
$sourceContent.
    replace("Chrome", "Firefox") |
    Set-Content ($folderPath + "FirefoxPlaytataContextBuilder.cs") -force;

New-Item ($folderPath + "EdgePlaytataContextBuilder.cs") -type file -force;
$sourceContent.
    replace("Chrome", "Edge") |
    Set-Content ($folderPath + "EdgePlaytataContextBuilder.cs") -force;

New-Item ($folderPath + "OperaPlaytataContextBuilder.cs") -type file -force;
$sourceContent.
    replace("Chrome", "Opera") |
    Set-Content ($folderPath + "OperaPlaytataContextBuilder.cs") -force;

New-Item ($folderPath + "SafariPlaytataContextBuilder.cs") -type file -force;
$sourceContent.
    replace("Chrome", "Safari") |
    Set-Content ($folderPath + "SafariPlaytataContextBuilder.cs") -force;

New-Item ($folderPath + "InternetExplorerPlaytataContextBuilder.cs") -type file -force;
$sourceContent.
    replace("OpenQA.Selenium.Chrome", "OpenQA.Selenium.IE").
    replace("Chrome", "InternetExplorer") |
    Set-Content ($folderPath + "InternetExplorerPlaytataContextBuilder.cs") -force;

Write-Output "Finished";
$host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")