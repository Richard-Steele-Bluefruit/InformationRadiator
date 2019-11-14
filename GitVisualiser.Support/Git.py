import subprocess
import os
import re
import tempfile
import TimeFormatter
import datetime
import time

# or could just specify that you need the git exe on your PATH env var
# if windows
__gitExe = "C:\Program Files (x86)\Git\cmd\git.exe"
__grepExe = "C:\\Program Files (x86)\\Git\\bin\\grep.exe"
# if linux ...

def __runCommand(commandType, commandParameters):
   output = None

   try:
      stderrSuppress = open(os.devnull, 'w')
      output = subprocess.check_output([__gitExe, commandType, commandParameters], stderr=stderrSuppress)
      stderrSuppress.close()
   except subprocess.CalledProcessError:
      output = False
   except WindowsError:
      print "Unable to find the git or grep exe"
      print "System is now throwing a hissy fit and stuck in a while true"
      while True:
         time.sleep(1)

   return output

def __runCommandAndPipeOutputToFile(commandType, commandParameters, tempFile):
   output = None
   try:
      #print __gitExe, commandType, commandParameters[0]
      stderrSuppress = open(os.devnull, 'w')
      ps = subprocess.Popen([__gitExe, commandType, commandParameters], stdout=tempFile, stderr=stderrSuppress)
      
      # wait for the process to terminate
      ps.communicate()

      stderrSuppress.close()
      output = True

   except subprocess.CalledProcessError:
      output = False
      print "Git.__runCommandAndPipeOutputToFile(...): subprocess.CalledProcessError"
      print

   tempFile.seek(0)

   return output

'''
subprocess.PIPE doesn't work with large amounts of data. See penultimate paragraph of:
http://thraxil.org/users/anders/posts/2008/03/13/Subprocess-Hanging-PIPE-is-your-enemy/
'''
def __runMultiplePipedCommands(entireCommandString):
   output = None
   commands = []
   commands = entireCommandString.split(" | ")

   # set up file names
   fileOutputNames = []
   fileOutputNamePrefix = "tempFileOutput"
   for fileNameCounter in range(len(commands)):
      fileNameCounterAsString = `fileNameCounter`
      fileOutputNames.append(fileOutputNamePrefix + fileNameCounterAsString + ".txt")


   # create files 
   for fileNameCounter in range(len(commands)):
      outputFileName = fileOutputNames[fileNameCounter]
      outputFile = open(outputFileName, "a")
      outputFile.close()

   # create and run processes
   for i in range(len(commands)):
      try:
         command = commands[i]
         commandType = command.split(" ")[0]
         if commandType == "grep":
            executableToRun = __grepExe
         else:
            executableToRun = __gitExe + " " + commandType  # e.g. "git branch"
         indexOfFirstParameter = command.find(" ") + 1
         commandParameters = command[indexOfFirstParameter:]

         commandToRun = executableToRun + " " + commandParameters

         if i == 0:
            outputFile = open(fileOutputNames[i], "w")

            try:
               ps = subprocess.Popen(commandToRun, stdout=outputFile)
            except WindowsError:
               print "Unable to find the git or grep exe"
               print "System is now throwing a hissy fit and stuck in a while true"
               print
               while True:
                  time.sleep(1)

            ps.wait()# i.e. popen runs automatically (without communicate and at creation) when doing it with files (and 
                     #  possibly pipes)
            outputFile.close()
         else:
            inputFile = open(fileOutputNames[i-1], "r")  # previous command's output
            outputFile = open(fileOutputNames[i], "w")

            try:
               ps = subprocess.Popen(commandToRun, stdin=inputFile, stdout=outputFile)
            except WindowsError:
               print "Unable to find the git or grep exe"
               print "System is now throwing a hissy fit and stuck in a while true"
               print
               while True:
                  time.sleep(1)
            ps.wait()

            inputFile.close()
            outputFile.close()
   
      except subprocess.CalledProcessError:
         output = False
         print "Git.__runMultiplePipedCommands(...): subprocess.CalledProcessError"
         print

   outputFile = open(fileOutputNames[-1], "r")
   output = outputFile.read()
   outputFile.close()

   # delete the temporary files
   for i in range(len(fileOutputNames)):
      try:
         os.remove(fileOutputNames[i])
      except OSError, e:
         print "Error in: Git.__runMultiplePipedCommands(...)"
         print "Unable to delete file:", e.filename," | ", e.strerror
         print

   return output

def getLastCommitDateAndTimeString(commit):
   gitCommandType = "log"
   commandParameter = [commit + " " + "--format=%ai"]
   output = __runCommand(gitCommandType, commandParameter)

   if output == False:
      print "could not find date for commit:", commit
      print
      return False

   output = output.split("\n")[0]
   return output

def getLastMergeDateAndTimeString(archipelago, branch):
   # git merge-base mdodkins/Development MrBobArchipelago
   gitCommandType = "merge-base"
   commandParameters = [archipelago + " " + branch]
   mergebaseOutput = __runCommand(gitCommandType, commandParameters)
   if mergebaseOutput == False:
      print "Error: in git.getLastMergeDateAndTimeString(...), merge-base"
      print
      return False
   mergebaseOutput = mergebaseOutput.strip()

   # git log --reverse --ancestry-path 95f8b004f55982dc8a552133b70c7eaaa0accf8b^..MrBobArchipelago >> 123.txt
   existingCommitsAfterMergeFile = tempfile.TemporaryFile()
   gitCommandType = "log"
   commandParameters = ["--reverse --ancestry-path " + mergebaseOutput + "^.." + branch]
   output = __runCommandAndPipeOutputToFile(gitCommandType, commandParameters, existingCommitsAfterMergeFile)
   if output == False:
      print "Error: in git.getLastMergeDateAndTimeString(...), log"
      print
      return False

   existingCommitsAfterMerge = existingCommitsAfterMergeFile.read()
   existingCommitsAfterMerge = existingCommitsAfterMerge.split("commit ")
   someLengthThatIsLessThanACommitSHA = 5
   # remove empty items
   [x if not (x.isspace() or len(x) == 0) else existingCommitsAfterMerge.remove(x) for x in existingCommitsAfterMerge]

   if len(existingCommitsAfterMerge) == 0:
      print "Error: in git.getLastMergeDateAndTimeString(...), no commits in log"
      print
      return False

   # go and collect the names of all of the branches with their authors
   gitCommandType = "for-each-ref"
   # note: on windows, you must use double quotes( " ) and not single quotes ( ' ) for specifying parameters to --format
   commandParameters = ["""--format="%(authorname) %(refname)" """]
   allBranchesWithAuthors = __runCommand(gitCommandType, commandParameters)
   if allBranchesWithAuthors == False:
      print "Error: in git.getLastMergeDateAndTimeString(...), for-each-ref"
      print
      return False

   hasFoundMatch = re.search('.*{0}'.format(branch), allBranchesWithAuthors)
   if hasFoundMatch == None:
      print "Error: in git.getLastMergeDateAndTimeString(...), there is no author for this branch. Seriously, what?!"
      print
      return False
   nameOfAuthorWithBranch = hasFoundMatch.group()
   nameOfAuthor = ' '.join(nameOfAuthorWithBranch.split(' ')[:-1])   # -1 == the branch, :. the rest is the name
   nameOfAuthor.strip()

   shaOfMergeCommit = None
   for commit in existingCommitsAfterMerge:
      # now look for:
      #  Author: Dominic Roberts
      hasFound = re.search("Author: " + nameOfAuthor, commit)
      if hasFound:
         shaOfMergeCommit = commit.split('\n')[0].split(' ')[0].strip()
         break
   if shaOfMergeCommit == None:
      print "Error: in git.getLastMergeDateAndTimeString(...), " + branch + " has never been merged with " + archipelago
      print
      return False
   
   # now we can finally get the date using shaOfMergeCommit
   gitCommandType = "show"
   commandParameters = [shaOfMergeCommit + " " + "-s --format=%ai"]
   output = __runCommand(gitCommandType, commandParameters)
   
   if output == False:
      print "Could not do \"git show\" for", shaOfMergeCommit
      print
      return False
   
   return output

def getMergeConflictsAndSaveToFile(archipelagoName, branchName, mergeConflictTempFile):
   #git merge-base DO-NOT-MERGE-test-conflict-another-conflict DO-NOT-PUSH-test-conflict
   #// gives "ba5ab64223edb39b65c53b578d2307e34d1fffc3" as output
   #git merge-tree ba5ab64223edb39b65c53b578d2307e34d1fffc3 DO-NOT-MERGE-test-conflict-another-conflict DO-NOT-PUSH-test-conflict

   # get sha of merge between two
   gitCommandType = "merge-base"
   commandParameters = [archipelagoName + " " + branchName]
   mergeSHA = __runCommand(gitCommandType, commandParameters)
   if mergeSHA == False:
      print "Error: in git.getMergeConflictsAndSaveToFile(...).__runCommand"
      print
      return False
   mergeSHA = mergeSHA.strip()

   # get merge data and immediately create a file
   gitCommandType = "merge-tree"
   commandParameters = [mergeSHA + " " + archipelagoName + " " + branchName]
   output = None
   output = __runCommandAndPipeOutputToFile(gitCommandType, commandParameters, mergeConflictTempFile)

   if output == False:
      print "Error: in git.getMergeConflictsAndSaveToFile(...).__runCommandAndPipeOutputToFile"
      print
      return False

def getCommitsAndAuthorsSinceLastMergeWithArchipelago(archipelagoName, branchName):
   # git shortlog -s -n --since="2015-06-15 10:45:49"
   dateTimeString = getLastMergeDateAndTimeString(archipelagoName, branchName)
   if dateTimeString == False:
      print "Error: in git.getCommitsAndAuthorsSinceLastMergeWithArchipelago(...), git.getLastMergeDateAndTimeString(...)"
      print
      return False

   dateTimeString = dateTimeString.split(' ')[:2]
   dateTimeString = dateTimeString[0] + " " + dateTimeString[1]

   gitCommandType = "shortlog"
   commandParameters = ["-s -n --since=\"" + dateTimeString + "\""]
   output = __runCommand(gitCommandType, commandParameters)

   if output == False:
      print "Error: in git.getCommitsAndAuthorsSinceLastMergeWithArchipelago(...)"
      print
      return False

   output = output.splitlines()

   commitsAndAuthors = []
   for item in output:
      item = item.strip()
      numberOfCommits = int(re.findall('\d+', item)[0])
      author = re.findall('\s+\S+.*', item)[0].strip()
      commitsAndAuthors.append(numberOfCommits)
      commitsAndAuthors.append(author)

   return commitsAndAuthors

def getTouchedDirectoriesWithPercentageTouched(archipelagoName, branchName):
   gitCommandType = "diff"
   commandParameters = ["--dirstat=0" + " " + archipelagoName + " " + branchName]
   dirstatOutput = __runCommand(gitCommandType, commandParameters)

   if dirstatOutput == False:
      print "Error: in git.getTouchedDirectoriesWithAndByPercentageTouched(...)"
      print
      return False

   if len(dirstatOutput) == 0:
      return []

   dirstatOutput = dirstatOutput[:-1] # remove empty line

   # now take the dirstatOutput string and put it into a list of tuples that we can sort
   # we have to consider there being spaces in the directory name
   percentageAndDirectoryTupleList = []
   percentageAndDirectoryTupleList = dirstatOutput.splitlines()
   percentageAndDirectoryTupleList = [line.strip() for line in percentageAndDirectoryTupleList] # take out leading + trailing whitespace
   percentageAndDirectoryTupleList = [line.partition(' ')[0::2] for line in percentageAndDirectoryTupleList]   # partition also gives us the ' ' as a part of the tuple
   percentageAndDirectoryTupleList = [(float(line[0][:-1]), line[1]) for line in percentageAndDirectoryTupleList]   # remove the "%" characters

   return percentageAndDirectoryTupleList

"""
git branch -a | grep "remotes/*" | grep "Archipelago" | grep -vi "revert" | grep -vi "tmp" | grep -vi "merge"
"""
def getArchipelagoNames():
   commandParameters = "branch -a | grep \"remotes/*\" | grep -i \"Archipelago\""

   unallowedStrings = []
   unallowedStrings.append("revert")   # git prepends "revert" when creating a branch for reverting
   unallowedStrings.append("tmp")   # Andi has a script that has a "/tmp/" structure
   unallowedStrings.append("merge")   # At least Dom and Matthew (possibly others) create branches with "merge" in it for merging archipelgos into their own
   for string in unallowedStrings:
      # "-v" removes entries, "-i" ignores case
      commandParameters+= " | " + "grep -vi" + " " + "\"" + string + "\""
   
   output = __runMultiplePipedCommands(commandParameters)

   if output == False:
      print "Error: in git.getArchipelagoNames(...)"
      print
      return False

   # sanitise the output into a list
   output = output.strip()
   output = output.split("remotes/")   # we don't need the remotes part
   [x if (len(x) != 0) else output.remove(x) for x in output] # remove empty items
   output = [x[0:-3] if x != output[-1] else x for x in output] # remove two spaces and newline (the last one has had them trimmed off by output.strip())

   return output

def getArchipelagoNamesRecentlyActiveInBranch(releaseArchipelago, amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent):
   listOfAllArchipelagos = getArchipelagoNames()
   if listOfAllArchipelagos == False:
      print "Error: in getArchipelagoNamesRecentlyActiveInBranch(...)"
      print
      return False

   for x in listOfAllArchipelagos:
      if x == releaseArchipelago:
         listOfAllArchipelagos.remove(x)

   recentlyActiveBranches = []
   for branch in listOfAllArchipelagos:
      dateTimeStringOfLastMerge = getLastMergeDateAndTimeString(releaseArchipelago, branch)
      if dateTimeStringOfLastMerge == False:
         # the git command failed to find the branch or we've never merged with this branch before
         # so we don't care about it
         continue

      dateTimeOfLastMerge = TimeFormatter.convertDateTimeFromGitISO8601Format(dateTimeStringOfLastMerge)

      currentDateTime = datetime.datetime.now()
      timeDifference = currentDateTime - dateTimeOfLastMerge
      timeDifferenceInSeconds = timeDifference.total_seconds()
      timeDifferenceInMinutes = int(timeDifferenceInSeconds / 60)
      timeDifferenceInHours = int(timeDifferenceInMinutes / 60)
      timeDifferenceInDays = int(timeDifferenceInHours / 24)

      if timeDifferenceInDays < amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent:
         recentlyActiveBranches.append(branch)

   return recentlyActiveBranches

def updateLocalRepositoryData():
   gitFetchCommand = "fetch"
   commandParameters = ["--all -p"] # "--all" == all remotes, "-p" == get rid of deleted branches
   fetchOutput = __runCommand(gitFetchCommand, commandParameters)
   
   if fetchOutput == False or len(fetchOutput) == 0: # "or len(fetchOutput) == 0" is untested!!!
      print "Error: in git.updateLocalRepositoryData()"
      print "Unable to run git fetch"
      print

   return fetchOutput

def getURLForRemote(localRemoteName):
   gitCommand = "config"
   commandParameters = ["remote." + localRemoteName + ".url"]
   output = __runCommand(gitCommand, commandParameters)
   
   if output == False or len(output) == 0: # "or len(fetchOutput) == 0" is untested!!!
      print "Error: in git.getURLForRemote()"
      print
      return False

   return output