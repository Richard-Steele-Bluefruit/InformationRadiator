"""
look at number of people who are active in branch since last merge with archipelago
compare the files and directories that they're active in
"""

import MergeConflictDetector 
import Git

class ActivityDetector(object):

   @property
   def ConflictData(self):
      return self._conflictData

   @property
   def Contributors(self):
      return self._contributors

   @property
   def TouchedDirectoriesWithPercentages(self):
      return self._touchedDirectoriesWithPercentages

   def __init__(self, archipelagoName, branchName):
      self.numberOfCommitsByContributors = 0
      self._contributors = []
      self._touchedDirectoriesWithPercentages = []
      self._conflictData = {}

      self.__getContributors(archipelagoName, branchName)
      self.__getTouchedDirectories(archipelagoName, branchName)
      self.__getConflictData(archipelagoName, branchName)

   def __getContributors(self, archipelagoName, branchName):
      numberOfCommitsAndAuthors = Git.getCommitsAndAuthorsSinceLastMergeWithArchipelago(archipelagoName, branchName)
      
      isNumberOfCommits = True   # used as a toggle, the first item in the array will be the number of commits
      for item in numberOfCommitsAndAuthors:
         if isNumberOfCommits == True:
            isNumberOfCommits = False
            self.numberOfCommitsByContributors = self.numberOfCommitsByContributors + int(item)
         else:
            self._contributors.append(item)
            isNumberOfCommits = True

   def __getTouchedDirectories(self, archipelagoName, branchName):
      self._touchedDirectoriesWithPercentages = Git.getTouchedDirectoriesWithPercentageTouched(archipelagoName, branchName)
      self._touchedDirectoriesWithPercentages.sort(reverse=True)   # highest percentage first

   def __getConflictData(self, archipelagoName, branchName):
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector(archipelagoName, branchName)
      self._conflictData = mergeConflictDetector.ConflictData
