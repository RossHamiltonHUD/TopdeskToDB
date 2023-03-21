function Register-Task
{
	cls
	Write-Host("The requests made to Topdesk aren't huge, but in the interests of minimising API calls, please")
	Write-Host("choose an update frequency that gives you the longest refresh period that works for your purposes.")
	Write-Host("Please select:`n")

	Write-Host("1. Run once a week (best for month-to-month reporting)")
	Write-Host("2. Run once a day (good for keeping track of KPIs as the month progresses)")
	Write-Host("3. Run every hour")
	Write-Host("4. Run every ten minutes (great for live dashboards)")
	Write-Host("5. Back")

	Write-Host("`nThe tool will only fetch historic data once, so only the current month gets updated on this schedule`n")

	$selection = Read-Host ("Your selection")

	Switch ($selection)
	{
		1 {	$Trigger= New-ScheduledTaskTrigger -Daily -DaysInterval 7 -At 10am	}
		2 {	$Trigger= New-ScheduledTaskTrigger -Daily -At 10am	}
		3 {	$Trigger= New-ScheduledTaskTrigger -Once -At (Get-Date) -RepetitionInterval (New-Timespan -Hours 1)}
		4 {	$Trigger= New-ScheduledTaskTrigger -Once -At (Get-Date) -RepetitionInterval (New-Timespan -Minutes 10)}
		5 { break } 
	}

	$Action= New-ScheduledTaskAction -Execute ((Get-Location).ToString()+"\TopdeskDataCache.exe") -WorkingDirectory ($here.ToString()+"\")
	$null = Register-ScheduledTask -TaskName "Topdesk Data Cache" -Trigger $Trigger -Action $Action –Force # Specify the name of the task
}

function Store-Topdesk-Creds($config)
{
    cls
	$email = Read-Host("Please enter your Topdesk email address").Trim()
	$app_password = Read-Host("Please paste in your Topdesk application password").Trim()

	$config.configuration.appSettings.add[0].value = $email.ToString()
	$config.configuration.appSettings.add[1].value = $app_password.ToString()

	$config.Save($config_file)
}

cls

$here = Convert-Path(Get-Location)

$exit = $false
$config_file = Convert-Path ($here.ToString() + "\TopdeskDataCache.dll.config")
#[xml]$config = Get-Content ($config_file)
$config_email = $config.configuration.appSettings.add[0].value
$config_password = $config.configuration.appSettings.add[1].value


while(!$exit)
{
	cls

    [xml]$config = Get-Content ($config_file)
    $config_email = $config.configuration.appSettings.add[0].value
    $config_password = $config.configuration.appSettings.add[1].value
	
	Write-Host ("Your data location for Power BI is: ")-ForegroundColor Black -BackgroundColor White
	Write-Host ($here.ToString() + "\topdeskData\") -ForegroundColor Black -BackgroundColor White

	if ($config_email -eq '' -or $config_password -eq '')
	{
		Write-Host "Topdesk credentials not stored, no data will be stored" -ForegroundColor Red
	}
	else
	{
		Write-Host ("Topdesk credentials appear to be stored: '"+$config_email+"':'"+$config_password+"'") -ForegroundColor Green
	}

	try
	{
		$null = Get-ScheduledTask "Topdesk Data Cache" -ErrorAction Stop
		Write-Host "Scheduled task found" -ForegroundColor Green
	}
	catch
	{
		Write-Host "Scheduled task was not found" -ForegroundColor Red
	}

	
	Write-Host("Please select from the following setup tasks:")
	Write-Host("1. Register/re-register a scheduled task")
	Write-Host("2. Store Topdesk credentials")
	Write-Host("3. Exit")
	$selection = Read-Host
	
	Switch($selection)
	{
		1 {Register-Task}
		2 {Store-Topdesk-Creds($config)}
		3 {
			$exit = $true
			exit
		}
	}
}