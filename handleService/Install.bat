%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe notifyservice.exe
Net Start WarningSystem-HandleService
sc config WarningSystem-HandleService start= auto