import sys
import time

from BranchDistanceCalculator import BranchDistanceCalculator
from GUI import GUI
import Git
from BranchDistanceCalculatorThread import BranchDistanceCalculatorThread
import ConfigurationFile
import Github
import ArchipelagoData

def main(configFileName):

   ConfigurationFile.read(configFileName)

   gui = GUI()

   hideInput = True
   githubPassword = gui.getUserInput("Enter Github Password", hideInput)
   
   gui.drawMessage("Updating internal gubbins, please wait")
   Github.GithubWrapper.init(githubPassword)
   _updateRepository()

   branches = []

   releaseArchipelagoName = ConfigurationFile.releaseArchipelago
   branches = Git.getArchipelagoNamesRecentlyActiveInBranch(releaseArchipelagoName, ConfigurationFile.amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)

   # The archipelago(s) which are never merged into anything and we :. can't rely on when they have been merged into the release archipelago. 
   #  Usually only the release archipelagos are merged into it except that there might be bug fixes so really any branch could go in from 
   #  anywhere. It could be called anything and there could be multiple instances of it (though usually not).
   blackList = ConfigurationFile.blackList 
   branches = _removeBlacklistedArchipelagosFromList(branches, blackList)

   archipelagoDataList = _setUpArchipelagoDataList(branches, releaseArchipelagoName, gui)

   gui.clearScreen()
   gui.drawMessage("Press F5 to refresh")

   # Git.RunAndStoreAllRepeatedCommands()

   keepRunning = True
   justStartedAndThereforeJustUpdatedRepository = True
   haveDrawnCirclesOnceAlready = False
   while keepRunning:
      gui.go()

      events = gui.getEvents()
      for event in events:
         if (event == GUI.WindowClosedEvent) or (event == GUI.EscapeKeyEvent):
            keepRunning = False
            return

         if (event == gui.F5KeyEvent):
            gui.drawMessage("Updating internal gubbins, please wait")
            if not justStartedAndThereforeJustUpdatedRepository:
               _updateRepository()
            justStartedAndThereforeJustUpdatedRepository = False
            needToDrawCirclesEveryTime = ConfigurationFile.automaticallyUpdateRepository
            if needToDrawCirclesEveryTime:
               _drawIslands(releaseArchipelagoName, gui, archipelagoDataList)
            elif haveDrawnCirclesOnceAlready:
               _redrawIslands(releaseArchipelagoName, gui, archipelagoDataList)
            else:
               _drawIslands(releaseArchipelagoName, gui, archipelagoDataList)
            haveDrawnCirclesOnceAlready = True

         if (event == gui.F6KeyEvent):
            if haveDrawnCirclesOnceAlready:
               _redrawIslands(releaseArchipelagoName, gui, archipelagoDataList)

         if (event == gui.MouseClickReleaseEvent):
            gui.displayNameOfIslandAtMouseLocation()

         if (event == gui.DeleteKeyEvent):
            gui.doNotRenderSelectedIsland()

def _drawIslands(releaseArchipelago, gui, archipelagoDataList):
   _setArchipelagoData(releaseArchipelago, archipelagoDataList)

   gui.drawIslandsWithNames(archipelagoDataList)

def _redrawIslands(releaseArchipelago, gui, archipelagoDataList):
   gui.drawIslandsWithNames(archipelagoDataList)

def _updateRepository():
   if ConfigurationFile.automaticallyUpdateRepository:
      print "updating repository"
      Git.updateLocalRepositoryData()
      print "finished updated repository"

def _setArchipelagoData(releaseArchipelagoName, archipelagoDataList):
   threadList = []
   
   print "getting branch distances"
   for conflictingArchipelago in archipelagoDataList:
      if conflictingArchipelago.isReleaseArchipelago:
         continue

      thread = BranchDistanceCalculatorThread(releaseArchipelagoName, conflictingArchipelago.name)
      tuple = (thread, conflictingArchipelago)
      threadList.append(tuple);
      thread.go()
   
   while (len(threadList) != 0):
      for tuple in threadList:
         thread = tuple[0]
         if thread.isFinished():
            differenceData = thread.get().differenceData
            conflictingArchipelago = tuple[1]
            conflictingArchipelago.differenceData = differenceData
            threadList.remove(tuple)
            branchName = conflictingArchipelago.name
            print "got branch distance for", branchName
      time.sleep(2)  # let's not hammer the cpu

def _removeBlacklistedArchipelagosFromList(branchList, blackList):
   for branchToRemove in blackList:
      for branch in branchList:
         branchName = branch.split("/")[1]
         removeBranchName = branchToRemove
         if branchName == removeBranchName:
            branchList.remove(branch)

   return branchList

def _setUpArchipelagoDataList(branches, releaseArchipelagoName, gui):

   numberOfBranches = len(branches) +1 # +1 for releaseArchipelago
   branchColours = gui.getColours(numberOfBranches)

   archipelagoDataList = []
   numberOfBranches = len(branches)
   for i in range(numberOfBranches):
      archipelago = ArchipelagoData.ArchipelagoData(branches[i], branchColours[i])
      archipelagoDataList.append(archipelago) 

   archipelago = ArchipelagoData.ArchipelagoData(releaseArchipelagoName)
   archipelago.isReleaseArchipelago = True
   archipelagoDataList.append(archipelago) 

   return archipelagoDataList

if __name__ == "__main__":
   print "start\n"

   configFileName = None
   args = sys.argv[1:]
   if len(args) == 0:
      configFileName = "config.txt"
   else:
      configFileName = args[0]

   main(configFileName)
   print "\nfinish"
