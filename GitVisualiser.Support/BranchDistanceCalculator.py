"""
Obviously, the actual 'distance' calculated is arbitrary in its metric

Even if we have no conflicting data and a distance of zero, we may still have "touchedDirectoriesWithPercentages", 
   with a load of zero percentages. These are directories that have files that have been automatically merged.
"""

import TimeDistance
import ActivityDetector
import DifferenceData
import ConfigurationFile
import Github

class BranchDistanceCalculator:

   def __init__(self, archipelagoName, branchName):

      self.differenceData = None

      self._distance = 0
      self._archipelagoName = archipelagoName
      self._comparisonBranchName = branchName
      self._pullRequests = []

      self._longTimeSinceMergeWithArchipelagoModifier = ConfigurationFile.longTimeSinceMergeWithArchipelagoModifier
      self._branchIsNotUpToDateInHoursWithArchipelagoModifier = ConfigurationFile.branchIsNotUpToDateInHoursWithArchipelagoModifier
      self._forEveryConflictModifier = ConfigurationFile.forEveryConflictModifier
      self._forEveryContributorModifier = ConfigurationFile.forEveryContributorModifier
      self._touchedWithAHighPercentageThreshold = ConfigurationFile.touchedWithAHighPercentageThreshold
      self._forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier = ConfigurationFile.forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier

      activityDetector = ActivityDetector.ActivityDetector(self._archipelagoName, self._comparisonBranchName)
      timeDistance = TimeDistance.TimeDistance(archipelagoName, branchName, self._longTimeSinceMergeWithArchipelagoModifier, self._branchIsNotUpToDateInHoursWithArchipelagoModifier)

      self._modifyDistanceWithConflictData(activityDetector.ConflictData)
      self._modifyDistanceWithNumberOfContributors(activityDetector.Contributors)
      self._modifyDistanceWithTimeDistance(timeDistance)
      self._modifyDistanceWithHighPercentageTouched(activityDetector.TouchedDirectoriesWithPercentages)
      
      # ask teamcity if the branch is green
      #teamCity = TeamCity(branch) 
      #print "is team city green: ", teamCity.isGreen

      # We can't use this to affect distance as we don't know if it is in this release or the 
      #  next. 
      self._pullRequests = Github.GithubWrapper.getPullRequests(branchName)

      self._storeAndSaveDifferenceData(self._archipelagoName, 
         self._comparisonBranchName, 
         activityDetector, 
         timeDistance.LastMergeDate,
         self._pullRequests)

   def _modifyDistanceWithTimeDistance(self, timeDistance):
      self._distance += timeDistance.Distance

   def _modifyDistanceWithConflictData(self, conflictData):
      conflictDistance = 0

      numberOfConflicts = len(conflictData['line'])
      conflictDistance = numberOfConflicts * self._forEveryConflictModifier

      self._distance += conflictDistance

   def _modifyDistanceWithNumberOfContributors(self, contributors):
      numberOfContributors = len(contributors)
      distanceModifier = numberOfContributors * self._forEveryContributorModifier
      self._distance += distanceModifier
      
   def _isThereAnyConflictingData(self, conflictData):
      # could have chosen any array in the dictionary
      length = len(conflictData['file'])
      anyItemsInArray = (length != 0)
      return anyItemsInArray

   def _modifyDistanceWithHighPercentageTouched(self, touchedDirectoriesWithPercentages):
      numberOfHighPercentageTouchedDirectories = 0

      for tuple in touchedDirectoriesWithPercentages:
         touchedDirectoryPercentage = tuple[0]
         touchedDirectoryPercentage = int(touchedDirectoryPercentage)
         if touchedDirectoryPercentage >= self._touchedWithAHighPercentageThreshold:
            numberOfHighPercentageTouchedDirectories += 1

      self._distance += numberOfHighPercentageTouchedDirectories * self._forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier

   def _storeAndSaveDifferenceData(self, archipelagoName, branchName, activityDetector, lastMergeDate, pullRequests):
      numberOfContributors = len(activityDetector.Contributors)
      contributors = activityDetector.Contributors
      touchedDirectoriesWithPercentages = activityDetector.TouchedDirectoriesWithPercentages
      conflictData = activityDetector.ConflictData
      distance = self._distance

      self.differenceData = DifferenceData.DifferenceData(numberOfContributors, 
         contributors, 
         touchedDirectoriesWithPercentages, 
         conflictData, 
         distance,
         archipelagoName, 
         branchName, 
         lastMergeDate, 
         pullRequests)

      self.differenceData.saveToFile()
      