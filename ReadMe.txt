The ApplyDBScripts Utility tool executes scripts, specified in a parameter supplied xml file, against a database which can be specified manually or within a web.config file

-s		Required Parameter - Absolute path to the settings XML file which details the scripts to apply.

-c 	Optional Parameter - Connection String - In the same format as what would be supplied in a typical web.config file.

-cp	Optional Parameter - Config Path - Path of  a config file where connection details are stored.

-cn	Optional Parameter - Connection Name - Dependant Upon -cp - If a value is supplied for -cp then a connection name is required to specify one of possible multiple connection strings in the one config file.

-t		Optional Parameter - Tail Program Path - The path of a shell program such as BareTail which will allow monitoring of output from ApplyDBScripts.

-e		Optional Parameter - Exit on Error - If set to True, the program will exit when an error occurs. i.e. if there is an error while checking svn properties or a sql error applying scripts to the database.

-l		Optional Parameter - Log File Distinct - If set to True, a distinct log file will be created for each day the program is executed, i.e. it appends todays date to logfile

-svn  Optional Parameter  - Takes in a comma seperated list of svn info xml files which will contain the svn info about scripts to be applied

Sample Command Line Parameter Configuration

-s 'D:\Work\Projects\ApplyDBScriptsSettings.xml'
 -cp 'D:\Work\Projects\Airline\CrewPay\trunk\CrewPay\Web.Config' 
 -cn 'AirlineCrewPayDBUpdater' 
 -t 'C:\Work\Projects\Airline\Common\trunk\baretail.exe'

Added a line to the read me file

Adding a line to the branch. This is getting added after a different line was added to master