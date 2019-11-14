--------------------------------------------
INSTALLATION
--------------------------------------------
- install pygame
- install PyGithub
   |-> type "pip install PyGithub" at command line
- place into some folder within whichever repository you want measurements for
- you'll probably want to change 
"amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent" in config.txt to 
be something like 21. 1000 is just for testing/demoing
- you need to have git at "C:\Program Files (x86)\Git\cmd\git.exe" and you need 
to have grep at "C:\Program Files (x86)\Git\bin\grep.exe". See Git.py if you 
want to change it to wherever you have it.

--------------------------------------------
HOW TO USE
--------------------------------------------
- run main.py
- press F5 do a git fetch all and display the visualisation
- click on a circle to display its name and "distance" from the release 
archipelago
- once a circle is selected, press delete to get rid of it
- press F6 if you want to redraw all of the circles without a fetch or 
recalculation of the visualisation

--------------------------------------------
HOW TO RUN TESTS
--------------------------------------------
- install mock
   |-> type "pip install mock" at command line
- type the following to a terminal:
   - cd test
   - RUN-ALL-TESTS.bat

--------------------------------------------
NOTES
--------------------------------------------
- automatic repository updating probably wont work if it needs a password to 
access the server. It'll probably fall over or maybe just silently fail.
- see the config file (config.txt) for changing screen size or whether 
it automatically updates the repository (git fetch all) and various other 
things.

