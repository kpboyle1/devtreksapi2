DevTreksStatsAPI (DTSA)

Version: beta1, June 28, 2018

Introduction
DTSA is a Web API application that exposes a simple server-side interface for 
communicating with statistical packages. Clients, such as HTML web applications, 
send requests to run statistical algorithms on remote statistical packages, often 
using data sciences machines. The server processes the request and sends the result 
of the statistical analysis back to the client for further processing. In the DevTreks 
web application, that processing involves the metadata summary of the statistical 
results in client side calculators including storing the URL to the statistical 
results (i.e. stored in an Indicator.MathExpression property). 

home site
https://www.devtreks.org

source code sites
https://github.com/kpboyle1/devtreksapi2.git (.NET Core 2.1)


What's New in Version beta1
1. This release upgraded the app to .NetCore 2.1. The previous release was tested 
on an Azure data sciences virtual machine but not this release. Current efforts 
are directed towards a lightweight version of DevTreks, focusing on Social 
Performance Analysis. 







