# Royal Holloway Local Exchange Trading System

![Royal Holloway Local Exchange Trading System](/readmeimages/home.PNG?raw=true "Royal Holloway Local Exchange Trading System")

The local exchange trading system that I have been working uses Microsoft ASP.NET MVC framework. To develop ASP.NET web application Microsoft requires the developers to use Visual Studio IDE. In this chapter I will explain the steps that developers needs to be followed to setup and run the project on their local machine.

System Requirements
At the time of writing this report, Microsoft limits the development of ASP.NET web application to windows operating system, so please Check your operating system and apply latest Windows Updates. 
Visual Studio 2015 requires Windows 7 Service Pack 1 or newer so please make sure that at a minimum you have Service Pack 1 installed.
Visual Studio 2015 requires 6GB of disk space at the minimum for the installation.

Download and install Visual Studio 2015
Microsoft was a free version of Visual Studio available for everyone which can be downloaded from here.
After the visual studio installer has been downloaded, run the executable and give the program admin rights to install the system packages and other dependencies.

Download the Local Exchange Trading System project
1.	After the successful installation of visual studio, unzip the local exchange trading system project.
2.	After the project has been unzipped, double click and navigate into the project folder named LETS.
3.	This folder contains many folders like LETS, LETS.Tests, LETSUserStories, UMLDiagrams, etc. You will also see a file called LETS with Microsoft Visual Studio Solution (.sln) file extension. Double click and run this file. Windows will automatically open and load the project in visual studio.
4.	Please agree to all the warning/popups that visual studio shows when opening the project. After opening the project visual studio will compile and configure the project for the local environment.

Run the project
By default, visual studio should show a green play button on the top in the taskbar with a browser name in it depending on the browser installed on the PC. Clicking on this button should run the project, open the browser and load the website in the browser. As the mongo database is hosted on the web and the project has been configured to connect to the database automatically there is no need to download or install or configure the database locally.
