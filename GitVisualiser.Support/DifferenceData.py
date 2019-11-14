import json
from collections import OrderedDict
import os
from threading import Lock

class DifferenceData:

   __checkDirectoryCreatedMutex = Lock()

   def __init__(self, numberOfContributors, 
         contributors, 
         touchedDirectoriesWithPercentages, 
         conflictData, 
         distance, 
         archipelagoName, 
         branchName, 
         lastMergeDate, 
         pullRequests):
      self.numberOfContributors = numberOfContributors
      self.contributors = contributors
      self.touchedDirectoriesWithPercentages = touchedDirectoriesWithPercentages
      self.conflictData = conflictData
      self.distance = distance
      self.archipelago = archipelagoName
      self.branch = branchName
      self.lastMergeDate = lastMergeDate
      self.pullRequests = pullRequests

   def saveToFile(self):
      # ordered just to make it a bit more human readable
      jsonDictionary = OrderedDict([
         ("releaseBranch", self.archipelago),
         ("archipelago", self.branch),
         ("distance", int(self.distance)),
         ("lastMergeDate", self.lastMergeDate),
         ("numberOfContributors", self.numberOfContributors),
         ("contributors", self.contributors),
         ("touchedDirectoriesWithPercentages", self.touchedDirectoriesWithPercentages),
         ("conflictData", self.conflictData),
         ("pullRequests", self.pullRequests)
      ])

      jsonData = json.dumps(jsonDictionary, indent=4, separators=(',', ': '), sort_keys=False)

      directoryToSaveTo = "json/"
      self.__checkDirectoryCreatedMutex.acquire()
      try:
         if not os.path.exists(directoryToSaveTo):
            os.makedirs(directoryToSaveTo)
      finally:
         self.__checkDirectoryCreatedMutex.release()

      fileName = self._getMergeDataFileName()
      pathToFile = directoryToSaveTo + fileName
      open(pathToFile, "a")  # force file creation
      file = open(pathToFile, "w")
      file.write(jsonData)
      file.close()

   def _getMergeDataFileName(self):
      archipelago = self.archipelago
      branch = self.branch

      fileName = archipelago + "_diff_" + branch + ".json"

      # we can't create a file with a slash in it
      fileName = fileName.replace('/', '-')
      fileName = fileName.replace('\\', '-')

      return fileName