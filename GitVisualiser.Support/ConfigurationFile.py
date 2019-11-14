import re
import sys

githubUsername = None
repositoryName = None
amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent = None
releaseArchipelago = None
longTimeSinceMergeWithArchipelagoModifier = None
branchIsNotUpToDateInHoursWithArchipelagoModifier = None
forEveryConflictModifier = None
forEveryContributorModifier = None
touchedWithAHighPercentageThreshold = None
forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier = None
textSize = None
maxSizeOfDrawableToStopCollidingWithCentre = None
maxDistanceFromCentreForScreenSize = None
minDistanceFromCentreForScreenSize = None
minSizeOfDrawableToBeVisible = None
windowName = None
screenWidth = None
screenHeight = None
automaticallyUpdateRepository = None
minimumPullRequestNumberFontSizePixels = None

def read(fileName):

   try:
      configFile = open(fileName, "r")
   except IOError:
      print "Could not find", fileName
      _exitProgram()

   wholeFileText = configFile.read()
   configFile.close()

   global githubUsername
   global repositoryName
   global amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent
   global releaseArchipelago
   global longTimeSinceMergeWithArchipelagoModifier
   global branchIsNotUpToDateInHoursWithArchipelagoModifier
   global forEveryConflictModifier
   global forEveryContributorModifier
   global touchedWithAHighPercentageThreshold
   global forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier
   global textSize
   global maxSizeOfDrawableToStopCollidingWithCentre
   global maxDistanceFromCentreForScreenSize
   global minDistanceFromCentreForScreenSize
   global minSizeOfDrawableToBeVisible
   global windowName
   global screenWidth
   global screenHeight
   global automaticallyUpdateRepository
   global blackList
   global minimumPullRequestNumberFontSizePixels

   githubUsername = _getValueFromFile(wholeFileText, "githubUsername")
   repositoryName = _getValueFromFile(wholeFileText, "repositoryName")
   amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent = _getValueFromFile(wholeFileText, "amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent")
   releaseArchipelago = _getValueFromFile(wholeFileText, "releaseArchipelago")
   longTimeSinceMergeWithArchipelagoModifier = _getValueFromFile(wholeFileText, "longTimeSinceMergeWithArchipelagoModifier")
   branchIsNotUpToDateInHoursWithArchipelagoModifier = _getValueFromFile(wholeFileText, "branchIsNotUpToDateInHoursWithArchipelagoModifier")
   forEveryConflictModifier = _getValueFromFile(wholeFileText, "forEveryConflictModifier")
   forEveryContributorModifier = _getValueFromFile(wholeFileText, "forEveryContributorModifier")
   touchedWithAHighPercentageThreshold = _getValueFromFile(wholeFileText, "touchedWithAHighPercentageThreshold")
   forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier = _getValueFromFile(wholeFileText, "forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier")
   textSize = _getValueFromFile(wholeFileText, "textSize")
   maxSizeOfDrawableToStopCollidingWithCentre = _getValueFromFile(wholeFileText, "maxSizeOfDrawableToStopCollidingWithCentre")
   maxDistanceFromCentreForScreenSize = _getValueFromFile(wholeFileText, "maxDistanceFromCentreForScreenSize")
   minDistanceFromCentreForScreenSize = _getValueFromFile(wholeFileText, "minDistanceFromCentreForScreenSize")
   minSizeOfDrawableToBeVisible = _getValueFromFile(wholeFileText, "minSizeOfDrawableToBeVisible")
   windowName = _getValueFromFile(wholeFileText, "windowName")
   screenWidth = _getValueFromFile(wholeFileText, "screenWidth")
   screenHeight = _getValueFromFile(wholeFileText, "screenHeight")
   automaticallyUpdateRepository = _getValueFromFile(wholeFileText, "automaticallyUpdateRepository")
   blackList = _getValueFromFile(wholeFileText, "blackList")
   minimumPullRequestNumberFontSizePixels = _getValueFromFile(wholeFileText, "minimumPullRequestNumberFontSizePixels")

def _getValueFromFile(wholeFileText, key):
   pattern = key + "\s\S+ *" # e.g. "mykey = 123 "
   matches = re.findall(pattern, wholeFileText)
   if len(matches) == 0:
      print "Could not find a match for", key, "in the config file"
      _exitProgram()
      return   # only gets here when doing testing
   
   match = matches[0]
   keyValuePair = match.split()
   value = keyValuePair[1]
   value = value.strip()

   valueIsAnInteger = _valueIsAnInteger(value)
   if valueIsAnInteger:
      value = int(value)
      return value

   valueAsABoolean = _valueAsABoolean(value)
   if valueAsABoolean != None:
      value = valueAsABoolean
      return value

   valueAsAList = _valueAsAList(value)
   if valueAsAList != None:
      value = valueAsAList
      return value

   # string
   return value

def _exitProgram():
   print "Exiting....."
   sys.exit(1)

def _valueIsAnInteger(value):
   try:
      int(value)
      return True
   except ValueError:
      return False

def _valueAsABoolean(value):
   if value == "true":
      return True
   if value == "false":
      return False
   return None

def _valueAsAList(value):
   if '[' in value:
      value = value[1:-1]  # take off those square brackets
      value = value.split(",")
      return value
   return None