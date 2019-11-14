"""
For giving the user info on exactly what the conflicts are
"""

from ConflictFile_ConflictBlock import ConflictFile, ConflictBlock

def rreplace(s, old, new, occurrence):
   li = s.rsplit(old, occurrence)
   return new.join(li)

class MergeConflictDetector(object):

   @property
   def ConflictData(self):
      return self._conflictData

   def __init__(self, archipelagoName, branchName):
      self._conflictData = {}

      self._setUpConflictDataDictionary()

      thereAreDifferencesBetweenTheBranches = None
      try:
         useCurrentSizeOfFile = 0
         self._conflictFile = ConflictFile(archipelagoName, branchName)
         thereAreDifferencesBetweenTheBranches = True
      except RuntimeError:
         thereAreDifferencesBetweenTheBranches = False

      if thereAreDifferencesBetweenTheBranches:
         self._findConflictingCode()
         
   def _setUpConflictDataDictionary(self):
      self._conflictData['ourCode'] = []
      self._conflictData['theirCode'] = []
      self._conflictData['file'] = []
      self._conflictData['line'] = []

   def _findConflictingCode(self):
      hasFoundAConflict = self._findNextConflictData()

      while hasFoundAConflict:
         hasFoundAConflict = self._findNextConflictData()

   def _findNextConflictData(self):
      conflictBlock = self._conflictFile.getNextConflictBlock()

      couldNotFindAConflict = (conflictBlock == None)
      if couldNotFindAConflict:
         return False

      self._getOurCode(conflictBlock)
      self._getTheirCode(conflictBlock)
      self._getConflictFileName(conflictBlock)
      self._getConflicingLineNumber(conflictBlock)
      
      return True

   def _getOurCode(self, conflictBlock):
      ourConflictBlock = self._conflictFile.findInConflictBlock("our", conflictBlock)
      ourText = self._conflictFile.read(ourConflictBlock)

      ourText = rreplace(ourText, '\n', '', 1)  # remove \n from end
      ourText = ourText.replace('\n', '', 1)  # remove \n from front
      self._conflictData['ourCode'].append(ourText)

   def _getTheirCode(self, conflictBlock):
      theirConflictBlock = self._conflictFile.findInConflictBlock("their", conflictBlock)
      theirText = self._conflictFile.read(theirConflictBlock)
   
      theirText = rreplace(theirText, '\n', '', 1)  # remove \n from end
      theirText = theirText.replace('\n', '', 1)  # remove \n from front
      self._conflictData['theirCode'].append(theirText)
   
   def _getConflictFileName(self, conflictBlock):
      fileNameConflictBlock = self._conflictFile.findInConflictBlock("file name", conflictBlock)
      fileNameLine = self._conflictFile.read(fileNameConflictBlock)
   
      mergeConflictFile = fileNameLine.split(' ')[-1]
   
      mergeConflictFile = rreplace(mergeConflictFile, '\n', '', 1)  # remove \n from end
      mergeConflictFile = mergeConflictFile.replace('\n', '', 1)  # remove \n from front
      
      self._conflictData['file'].append(mergeConflictFile)
   
   def _getConflicingLineNumber(self, conflictBlock):
      lineConflictBlock = self._conflictFile.findInConflictBlock("line", conflictBlock)
      lineWithDataOnCollisionLocation = self._conflictFile.read(lineConflictBlock)
   
      mergeConflictLineNumber = self._getLineNumberFromDataOnCollision(lineWithDataOnCollisionLocation)
      betweenLineNumberAndOurConflictBlock = self._conflictFile.findInConflictBlock("between line number and our", conflictBlock)
      rawText = self._conflictFile.read(betweenLineNumberAndOurConflictBlock)
      numberOfLinesInBetweenLineNumberAndConflictBlocks = rawText.count('\n') -1 # we're also counting the one on the last line
      mergeConflictLineNumber = mergeConflictLineNumber + numberOfLinesInBetweenLineNumberAndConflictBlocks

      self._conflictData['line'].append(mergeConflictLineNumber)
   
   def _getLineNumberFromDataOnCollision(self, lineWithDataOnCollisionLocation):
      mergeConflictLineNumber = lineWithDataOnCollisionLocation.split(',')
      mergeConflictLineNumber = mergeConflictLineNumber[0]
      mergeConflictLineNumber = mergeConflictLineNumber[1:] # remove minus
      return int(mergeConflictLineNumber)
