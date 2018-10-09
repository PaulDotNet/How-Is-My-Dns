# How-Is-My-Dns

Checks for website connectivity issues using all IP addresses from DNS.

When DNS returns IP addresses for the host the returned list may contain some addresses that are not accessible. For example DNS may return IPv4 and IPv6 addresses when only IPv4 ones work. For some apps that prefer IPv6 such DNS behavior may cause connection problems. They could just hang there not responding or timeout for no obvious reason.

This app checks if local machine can access specified web page (or rest service) using all IP addresses that DNS returns for the specified host. It uses default timeout value of 15 seconds so if server is down or very very slow then it may appear as having connectivity problem.


This is C# .net core 2.1 app and to run it you need .net core SDK installed on the machine.
To run this app first clone this repo:
    
`git clone https://github.com/PaulDotNet/How-Is-My-Dns.git`

Then run it from command line like this:

`cd How-Is-My-Dns`

`dotnet run https://github.com`

App will print all IP addresses of the specified host and then will attempt to connect to each IP address starting with IPv4 ones first.
