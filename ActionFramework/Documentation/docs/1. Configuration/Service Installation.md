1. Open command prompt (as administrator)
2. type: cd "C:\Windows\Microsoft.NET\Framework64\v4.0.30319" and press enter
3. Type: installutil "C:\actionframework\agents\33333\ActionFramework.Agent.exe" and press enter. Results should look like the image below.

![Installation complete](service-installation-1.png)

1. Open services in windows, if it does not show, press refresh button and the Agent should appear in the list like the image below.

![Agent added to Windows Services.](service-installation-2.png)

1. Start agent service manually by pressing the "start" link-button.
2. Test the agent by browsing to one of it´s endpoints from a web browser.
3. Open Google Chrome or Windows Explorer and type in the url of the agent: http://localhost:1011/api/agent/run
