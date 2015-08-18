%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe healthyCheckService
Net Start WarningSystem-HealthyCheckService
sc config WarningSystem-HealthyCheckService start= auto