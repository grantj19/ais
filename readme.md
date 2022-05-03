# AISApi

AISapi is an API for interacting with vessel AIS positioning data.

## Installation

Download AISData.mysql from our Canvas submission.
Create a database named 'AISData' then run the following bash command:

```bash
mysql -u [username] -p AISData < AISData.mysql
```

Download AIS_API_Linux_x64.tar.gz from our Canvas submission.
Extract the file using the following command and note the location of the created folder

```bash
tar -xzvf AIS_API_Linux_x64.tar.gz
```

## Usage

This software is self containing, meaning that this package contains it's own runtimes (specifically Linux-x64). There is no need to install dotnet outside of this package to run the software.

Update the connection string in the appsettings.json file inside the /linux-x64/publish directory with the credentials for your local MySQL Server

Run the API with the following command using the proper permissions

```bash
cd /pathToFolder/linux-x64/publish
./AISapi
```

To view the API documentation and use the API, using a web browser, navigate to http://localhost:5000/swagger/index.html

## Testing

To run the API Test project, dotnet will need to be installed.

Follow the documentation [here](https://docs.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual) to install dotnet on linux machine.

Once dotnet is installed, navigate to the source code provided in the archive file downloaded from our Canvas submission.

Run the commands to execute the project tests

```bash
cd /pathToFolder/source/AISTests
dotnet test
```

## Contact

Any questions or comments may be directed to [Grant Gillespie](mailto:gjgillespie@bsu.edu) or [Connor Pflederer](mailto:cjpflederer@bsu.edu).