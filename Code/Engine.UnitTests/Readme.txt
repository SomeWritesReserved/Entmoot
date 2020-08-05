------------------------
 Unit tests for Engine
------------------------

These are the unit tests for the Engine proper and nothing else.


How to run tests:
----
It is easiest to setup Visual Studio to run the NUnit console runner targeting the built unit test library:
	1. Open the project's properties and go to the Debug tab.
	2. For "Start Action" choose "Start extern program" and browse to the NUnit console runner executable.
		a. Usually under "Extern\NUnit\Runner\nunit3-console.exe".
	3. For "Command line arguments" enter the assembly's name .
		a. Usually "Entmoot.Engine.UnitTests.dll --wait".
		b. You can enter in any NUnit console command line arguments which is useful to run only a subset of tests (--where).
		c. The --wait argument is useful as it allows you to see the NUnit output without the console immediately closing.
	4. For "Working directory" leave this blank since the default will be the "bin" directory which is what we want.
