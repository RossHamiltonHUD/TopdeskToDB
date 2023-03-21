This is the initial version of my tool to collect incident data from Topdesk, for reporting in PowerBI

To set up the tool:

    Extract the .zip file to the directory where you would like to store the data
    Rename the directory now if you wish to
    Run Setup.ps1 with Powershell
    Use both option 1 & option 2 to set up the scheduled task and your credentials
    Copy the filepath from Setup.ps1
    Open the PowerBI template "TopdeskReport.pbit", and paste the file path into the TopdeskDataLocation parameter. Leave the other parameter set to "Sample File"
    Save your report in a useful location, and you can begin building visualisations

Refreshing the report should now import the latest data collected by the tool, according to your scheduled task.