import QuickThread
import Queue
import BranchDistanceCalculator
from traceback import format_exc

class BranchDistanceCalculatorThread(QuickThread.QuickThread):

   def __init__(self, releaseArchipelago, branchToCompare):
      self._dataToReturnQueue = Queue.Queue()
      self.__parent = super(BranchDistanceCalculatorThread, self)
      
      arguments = (releaseArchipelago, branchToCompare)
      self.__parent.__init__(self._get, arguments)

   def go(self):
      try:
         self.__parent._start()
      except RuntimeError as error:
         errorMessage = str(error)
         if errorMessage == "threads can only be started once":
            self.__parent._restart()
         else:
            raise error


   def _get(self, archipelago, branchToCompare):
      try:
         branchDistanceCalculator = BranchDistanceCalculator.BranchDistanceCalculator(archipelago, branchToCompare)
      except Exception:
         print format_exc()   # print exception message with stack trace
         print
         print "ERROR during running thread. Attempting to stop thread."
         print
         self.__parent._stop()
         return

      self._dataToReturnQueue.put(branchDistanceCalculator)
      self.__parent._stop()

   def get(self):
      if self.isFinished():
         return self._dataToReturnQueue.get()
      return None
