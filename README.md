This is the initial version of my tool to collect incident data from Topdesk, for reporting in PowerBI. It uses the Topdesk Incident API to collect the data, on an automated schedule in the background, then stores it in .json files for use with Power BI. Included is a script to set up the application, and a Power BI template which has prebuilt queries to access and format the data nicely.

To set up the tool:

1. Extract the .zip file to the directory where you would like to store the data
2. Rename the directory now if you wish to
3. Run Setup.ps1 with Powershell
4. Use both option 1 & option 2 to set up the scheduled task and your credentials
5. Copy the filepath from Setup.ps1
6. Open the PowerBI template "TopdeskReport.pbit", and paste the file path into the TopdeskDataLocation parameter. Leave the other parameter set to "Sample File"
7. Save your report in a useful location, and you can begin building visualisations

Refreshing the report should now import the latest data collected by the tool, according to your scheduled task.
