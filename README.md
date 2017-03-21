# Royal Holloway Local Exchange Trading System

![Royal Holloway Local Exchange Trading System](/readmeimages/home.PNG?raw=true "Royal Holloway Local Exchange Trading System")

**Local Exchange Trading System** or **LETS** is an internet based, not-for-profit inter-community trading network which provides its members with information service and the members can buy/sell goods and services among themselves using a local currency (LETS Credits) rather than the national currency. 

The services can also be viewed as favours which can mutually benefit the community and all its members. It not only enables the transfer of skills and services but also improves a person's or group's prospect, by providing them with skills such as learning a new language, or by simply providing them with an essential service such as gardening or plumbing.

## Motivation

My motivation behind doing this project was to explore new technologies and deepen my interest in web development by implementing a system involving using them. During my year in industry placement I had the opportunity to learn Web application development, but the project that I was working on was as a part of a team due to which I was not able to work on all the components of the system like backend database. This project gave me the opportunity to work from end-to-end, and also allowed me to explore and learn new technologies that I had never used before. I also got the opportunity to conduct my own research on the requirements and technologies and tackle problems that arised during the development process.

I also wanted to enhanced my software engineering skills by correctly designing and implementing the web application following the best practices, performing the appropriate tests and using appropriate coding conventions.

## Installation

The local exchange trading system that I have been working uses Microsoft ASP.NET MVC framework. To develop ASP.NET web application Microsoft requires the developers to use Visual Studio IDE. In this chapter I will explain the steps that developers needs to be followed to setup and run the project on their local machine.


### System Requirements
At the time of writing this report, Microsoft limits the development of ASP.NET web application to windows operating system, so please Check your operating system and apply latest Windows Updates. 

Visual Studio 2015 requires Windows 7 Service Pack 1 or newer so please make sure that at a minimum you have Service Pack 1 installed.

Visual Studio 2015 requires 6GB of disk space at the minimum for the installation.  


### Download and Install Visual Studio 2015
Microsoft was a free version of Visual Studio available for everyone which can be downloaded from [here](https://www.visualstudio.com/vs/community/).

After the visual studio installer has downloaded, run the executable and give the program admin rights to install the system packages and other dependencies.


### Download the Local Exchange Trading System Project
1.	After the successful installation of visual studio, unzip the local exchange trading system project.

2.	After the project has been unzipped, double click and navigate into the project folder named LETS.

3.	This folder contains many folders like LETS, LETS.Tests, LETSUserStories, UMLDiagrams, etc. You will also see a file called LETS with Microsoft Visual Studio Solution (.sln) file extension. Double click and run this file. Windows will automatically open and load the project in visual studio.

4.	Please agree to all the warning/popups that visual studio shows when opening the project. After opening the project visual studio will compile and configure the project for the local environment.

### Running the Project
By default, visual studio should show a green play button on the top in the taskbar with a browser name in it depending on the browser installed on the PC. Clicking on this button should run the project, open the browser and load the website in the browser. As the mongo database is hosted on the web and the project has been configured to connect to the database automatically there is no need to download or install or configure the database locally.

## Troubleshooting

There maybe some problems that you could encounter when trying to running the project on your local machine.

**There is no play button with the browser name in the taskbar**  
Some users might encounter this problem when running the project for the first time on their system. If you see a start button instead of a play button as shown in the below picture

![Start Button Instead of Play](/readmeimages/start.png?raw=true "Start Button Instead of Play")

The simple trick is to right click on the LETS project solution and select “**Set as StartUp Project**” from the options as shown in the below picture. 

![Set as Project](/readmeimages/setasproject.jpg?raw=true | width = 200 "Set as Project")

This will set the LETS project as the start-up project and the start button will change to a browser launch play button.

## License

A short snippet describing the license (MIT, Apache, etc.)