from threading import Thread
import threading
import time

class QuickThread(object):

   def __init__(self, functionToCall, arguments):
      self.__stopThreadEvent = None
      self.__thread = None
      self.__arguments = None
      self.__functionToCall = None

      self.__stopThreadEvent = threading.Event()
      self.__thread = Thread(target=functionToCall, args=arguments)
      self.__thread.daemon = True   # thread dies if running when program closed
      self.__arguments = arguments
      self.__functionToCall = functionToCall

   def _start(self):
      self.__stopThreadEvent.clear()
      self.__thread.start()

   def _restart(self):
      if not self.isFinished():
         self._stop()
         if not self.isFinished(): 
            # then it isn't going to finish any time soon
            raise RuntimeError("unable to restart thread, thread is still running")

      self.__thread = Thread(target=self.__functionToCall, args=self.__arguments)
      self.__thread.daemon = True

      self._start()

   def _stop(self):
      self.__stopThreadEvent.set()

   def isFinished(self):
      isFinished = self.__stopThreadEvent.is_set()
      return isFinished