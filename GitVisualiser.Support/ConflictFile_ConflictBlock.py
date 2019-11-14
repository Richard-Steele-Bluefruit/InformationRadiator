import mmap
import StringIO
import tempfile

import Git

class ConflictBlock(object):
   
   def __init__(self, start, end):
      self.start = start
      self.end = end

      # this is because this is used in memmap.find(...) to specify the range of indices to search
      self.numCharacters = start + (end - start)   

class ConflictFile(object):

   _StartOfOurBlockIdentifier = "+<<<<<<< .our"
   _EndOfTheirBlockIdentifier = "+>>>>>>> .their"
   _ConflictStartOfBlockIndentifier = "changed in both"
   _InBetweenOurAndTheirBlockIdentifier = "+======="

   def __init__(self, archipelagoName, branchName):
      self._memMap = None
      self.__indexOfEndOfTheLastConflict = 0

      mergeConflictTempFile = tempfile.TemporaryFile() # self == debug
      Git.getMergeConflictsAndSaveToFile(archipelagoName, branchName, mergeConflictTempFile)
      mergeConflictTempFile.seek(0)

      try:
         useCurrentSizeOfFile = 0
         self._memMap = mmap.mmap(mergeConflictTempFile.fileno(), useCurrentSizeOfFile, access = mmap.ACCESS_READ)
      except ValueError:
         raise RuntimeError("Error: in ConflictFile::__init__(...), could not create memmap to tempfile for conflicts")

   def read(self, aConflictBlock):
      self._memMap.seek(aConflictBlock.start)

      dataRead = StringIO.StringIO()
      #while self._memMap.tell() != aConflictBlock.end:
      #   dataRead.write(self._memMap.read_byte())
      dataRead.write(self._memMap.read(aConflictBlock.end - aConflictBlock.start))

      stringData = dataRead.getvalue()
      dataRead.close()

      return stringData

   def getNextConflictBlock(self):
      start = self._memMap.find(ConflictFile._ConflictStartOfBlockIndentifier, 
         self.__indexOfEndOfTheLastConflict, 
         self.endOfFile())

      couldNotFindConflict = -1
      if start == couldNotFindConflict:
         return None

      # sunny day scenario, there are just one or more blocks with .our and .their changes
      lineAfterStart = start + len(ConflictFile._ConflictStartOfBlockIndentifier)
      end = self._memMap.find(ConflictFile._ConflictStartOfBlockIndentifier, lineAfterStart, self.endOfFile())
      if end == couldNotFindConflict:
         end = self.endOfFile()

      # rainy day scenario, automatic merges mixed in
      indexOfNextChangedInBoth = end
      indexOfNextOur = self._memMap.find(ConflictFile._StartOfOurBlockIdentifier, start, self.endOfFile())
      
      noMoreConflicts = (indexOfNextChangedInBoth == couldNotFindConflict) or (indexOfNextOur == couldNotFindConflict)
      if noMoreConflicts:
         return None

      thereIsAnotherChangedInBothBeforeTheNextOur = (indexOfNextChangedInBoth < indexOfNextOur)
      if thereIsAnotherChangedInBothBeforeTheNextOur:
         self.__indexOfEndOfTheLastConflict = lineAfterStart   # move past current automerge
         return self.getNextConflictBlock()

      newConflictBlock = ConflictBlock(start, end)

      # set up for when we're asked for the next conflict block
      self.__indexOfEndOfTheLastConflict = end

      return newConflictBlock

   def endOfFile(self):
      return self._memMap.size() - 1

   def findInConflictBlock(self, blockNameType, conflictBlock):
      # line is a bit awkward, we go from the "our" identifier back up to the "changed in both" identifier
      if blockNameType == "line":
         startOfOurBlockIndex = self._getStartOfBlockIndex(conflictBlock, "our")
         lineConflictBlock = ConflictBlock(conflictBlock.start, startOfOurBlockIndex)
         startOfBlockIdentifier = "@@ "   # that space is important
         start = self._memMap.rfind(startOfBlockIdentifier, lineConflictBlock.start, lineConflictBlock.numCharacters) # note, _R_find
         start = start + len(startOfBlockIdentifier) # move the index to be after the identifier
         endOfBlockIdentifier = " @@"  # that space is important  # thankfully, there is never a space after the closing @@
         end = self._memMap.rfind(endOfBlockIdentifier, lineConflictBlock.start, lineConflictBlock.numCharacters) # note, _R_find

         return ConflictBlock(start, end)

      elif blockNameType == "between line number and our":
         startOfOurBlockIndex = self._getStartOfBlockIndex(conflictBlock, "our")
         lineConflictBlock = ConflictBlock(conflictBlock.start, startOfOurBlockIndex)
         startOfBlockIdentifier = " @@"
         start = self._memMap.rfind(startOfBlockIdentifier, lineConflictBlock.start, lineConflictBlock.numCharacters)
         end = startOfOurBlockIndex

         return ConflictBlock(start, end)

      start = self._getStartOfBlockIndex(conflictBlock, blockNameType);
      self._memMap.seek(start)   # unsure if necessary, doesn't hurt though

      if blockNameType == "our":
         endOfBlockString = ConflictFile._InBetweenOurAndTheirBlockIdentifier
      elif blockNameType == "their":
         endOfBlockString = ConflictFile._EndOfTheirBlockIdentifier
      elif blockNameType == "file name":
         endOfBlockString = '\n'  # we want just the whole line
         
      end = self._memMap.find(endOfBlockString, start, self.endOfFile())

      return ConflictBlock(start, end)

   def _getStartOfBlockIndex(self, conflictBlock, blockNameType):
      self._memMap.seek(conflictBlock.start)
      startOfBlockIdentifier = None

      if blockNameType == "our":
         startOfBlockIdentifier = ConflictFile._StartOfOurBlockIdentifier
      elif blockNameType == "their":
         startOfBlockIdentifier = ConflictFile._InBetweenOurAndTheirBlockIdentifier
      elif blockNameType == "file name":
         startOfBlockIdentifier = '\n' # we're interested in the line after "changed in both"

      lowestIndexOfStartOfBlock = self._memMap.find(startOfBlockIdentifier, conflictBlock.start, self.endOfFile())

      indexAfterStartOfBlockIdentifier = lowestIndexOfStartOfBlock + len(startOfBlockIdentifier)
      return indexAfterStartOfBlockIdentifier