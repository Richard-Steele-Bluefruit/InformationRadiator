import QuickThread

class MockQuickThread(QuickThread.QuickThread):

   forceToNeverbeAbleToStop = False
   restartCalled = False

   @classmethod
   def reset(theClass):
      theClass.forceToNeverbeAbleToStop = False
      theClass.restartCalled = False

   def __init__(self, target, args):
      self._target = target
      self._args = args

      self._hasBeenStarted = False

   def _start(self):
      if self._hasBeenStarted == True:
         raise RuntimeError("threads can only be started once")
      self._hasBeenStarted = True
      self._target(self, self._args)

   def _restart(self):
      MockQuickThread.restartCalled = True

   def _stop(self):
      pass

   def isFinished(self):
      if self._hasBeenStarted:
         return not MockQuickThread.forceToNeverbeAbleToStop
      return False

QuickThread.QuickThread = MockQuickThread # use our mock class instead of the real thing