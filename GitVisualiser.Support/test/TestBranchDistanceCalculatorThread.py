import unittest
from mock import patch, MagicMock

from projectpaths import *
import MockQuickThread # does some magic to override QuickThread with a mock
import BranchDistanceCalculatorThread

@patch('BranchDistanceCalculator.BranchDistanceCalculator', MagicMock())
class TestBranchDistanceCalculatorThread(unittest.TestCase):

   def setUp(self):
      MockQuickThread.MockQuickThread.reset()

   def test_branch_is_not_finished_when_it_has_not_been_run(self):
      # Given
      branchDistanceCalculatorThread = BranchDistanceCalculatorThread.BranchDistanceCalculatorThread("releaseArchipelago", "branchToCompare")
   
      # Then
      self.assertFalse(branchDistanceCalculatorThread.isFinished())
   
   def test_branch_is_finished_when_it_has_been_run(self):
      # Given
      branchDistanceCalculatorThread = BranchDistanceCalculatorThread.BranchDistanceCalculatorThread("releaseArchipelago", "branchToCompare")
   
      # When
      branchDistanceCalculatorThread.go()
   
      # Then
      self.assertTrue(branchDistanceCalculatorThread.isFinished())
   
   def test_unable_to_get_result_when_thread_has_not_finished(self):
      # Given
      MockQuickThread.MockQuickThread.forceToNeverbeAbleToStop = True
      branchDistanceCalculatorThread = BranchDistanceCalculatorThread.BranchDistanceCalculatorThread("releaseArchipelago", "branchToCompare")
   
      # When
      branchDistanceCalculatorThread.go()
   
      # Then
      expected = None
      actual = branchDistanceCalculatorThread.get()
   
      self.assertEqual(expected, actual)
   
   def test_able_to_get_result_when_thread_has_finished(self):
      # Given
      branchDistanceCalculatorThread = BranchDistanceCalculatorThread.BranchDistanceCalculatorThread("releaseArchipelago", "branchToCompare")
   
      # When
      branchDistanceCalculatorThread.go()
   
      # Then
      actual = branchDistanceCalculatorThread.get()
   
      self.assertTrue(isinstance(actual, MagicMock))

   def test_attempts_to_restart_when_go_is_called_twice(self):
      # Given
      branchDistanceCalculatorThread = BranchDistanceCalculatorThread.BranchDistanceCalculatorThread("releaseArchipelago", "branchToCompare")

      # When
      branchDistanceCalculatorThread.go()
      branchDistanceCalculatorThread.go()
   
      # Then
      self.assertTrue(MockQuickThread.MockQuickThread.restartCalled)

if __name__ == '__main__':
    unittest.main()