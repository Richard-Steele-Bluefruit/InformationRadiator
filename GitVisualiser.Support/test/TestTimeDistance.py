import unittest
from mock import patch, MagicMock

from projectpaths import *
import TimeDistance

class TestTimeDistance(unittest.TestCase):

   @patch('Git.getLastCommitDateAndTimeString', MagicMock(return_value="2015-09-29 13:34:59 +0100"))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2015-09-30 13:34:59 +0100"))
   def test_determines_single_day_difference(self):
      # When
      longTimeSinceMergeWithArchipelagoModifier = 0
      branchIsNotUpToDateWithArchipelagoModifier = 0
      timeDistance = TimeDistance.TimeDistance(
         "release-archipelago", 
         "some-branch", 
         longTimeSinceMergeWithArchipelagoModifier, 
         branchIsNotUpToDateWithArchipelagoModifier)

      # Then
      expected = 1.0
      actual = timeDistance.Distance
      self.assertEqual(expected, actual)

      self.assertEqual("2015-09-30 13:34:59", timeDistance.LastMergeDate)

   @patch('Git.getLastCommitDateAndTimeString', MagicMock(return_value="2015-09-29 13:34:59 +0100"))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2015-10-9 13:34:59 +0100"))
   def test_determines_multiple_day_difference(self):
      # When
      longTimeSinceMergeWithArchipelagoModifier = 0
      branchIsNotUpToDateWithArchipelagoModifier = 0
      timeDistance = TimeDistance.TimeDistance(
         "release-archipelago", 
         "some-branch", 
         longTimeSinceMergeWithArchipelagoModifier, 
         branchIsNotUpToDateWithArchipelagoModifier)

      # Then
      expected = 10.0
      actual = timeDistance.Distance
      self.assertEqual(expected, actual)

   @patch('Git.getLastCommitDateAndTimeString', MagicMock(return_value="2015-09-29 13:34:59 +0100"))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2016-09-28 13:34:59 +0100"))
   def test_determines_more_than_two_week_difference(self):
      # When
      longTimeSinceMergeWithArchipelagoModifier = 10
      branchIsNotUpToDateWithArchipelagoModifier = 0
      timeDistance = TimeDistance.TimeDistance(
         "release-archipelago", 
         "some-branch", 
         longTimeSinceMergeWithArchipelagoModifier, 
         branchIsNotUpToDateWithArchipelagoModifier)

      # Then
      expected = 365.0 + 10
      actual = timeDistance.Distance
      self.assertEqual(expected, actual)

   @patch('Git.getLastCommitDateAndTimeString', MagicMock(return_value="2015-09-30 13:00:00 +0100"))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2015-09-30 18:00:00 +0100"))
   def test_determines_if_merged_up_to_archipelago_and_a_few_hours_beyond(self):
      # When
      longTimeSinceMergeWithArchipelagoModifier = 0
      branchIsNotUpToDateWithArchipelagoModifier = 1  # per hour
      timeDistance = TimeDistance.TimeDistance(
         "release-archipelago", 
         "some-branch", 
         longTimeSinceMergeWithArchipelagoModifier, 
         branchIsNotUpToDateWithArchipelagoModifier)

      # Then
      expected = 5.0 # 5 hours
      # cast to int as we don't care about the fractional difference between the last commit 
      #  and the last merge with the archipelago
      actual = int(timeDistance.Distance) 
      self.assertAlmostEqual(expected, actual)

      actualRemainder = timeDistance.Distance - int(timeDistance.Distance)
      self.assertGreater(actualRemainder, 0)

   @patch('Git.getLastCommitDateAndTimeString', MagicMock(return_value="2015-09-30 13:00:00 +0100"))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2015-09-29 13:00:00 +0100"))
   def test_determines_if_merged_up_to_archipelago_and_more_than_a_few_hours_beyond(self):
      # When
      longTimeSinceMergeWithArchipelagoModifier = 0
      branchIsNotUpToDateWithArchipelagoModifier = 1
      timeDistance = TimeDistance.TimeDistance(
         "release-archipelago", 
         "some-branch", 
         longTimeSinceMergeWithArchipelagoModifier, 
         branchIsNotUpToDateWithArchipelagoModifier)

      # Then
      expected = 1.0 + 24.0# 1 day commit difference + 24 hours since
      actual = timeDistance.Distance
      self.assertEqual(expected, actual)

   @patch('Git.getLastCommitDateAndTimeString', MagicMock(return_value="2014-09-30 13:00:00 +0100"))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2015-09-30 13:00:00 +0100"))
   def test_determines_more_than_two_week_difference_with_all_possible_modifiers_active(self):
      # When
      longTimeSinceMergeWithArchipelagoModifier = 100
      branchIsNotUpToDateWithArchipelagoModifier = 1
      timeDistance = TimeDistance.TimeDistance(
         "release-archipelago", 
         "some-branch", 
         longTimeSinceMergeWithArchipelagoModifier, 
         branchIsNotUpToDateWithArchipelagoModifier)

      # Then
      expected = 365.0 + 8760.0 + 100# 365 days + hours in a year + long time modifier
      actual = timeDistance.Distance
      self.assertEqual(expected, actual)

   # if this happens in the normal running of the whole thing, then something has 
   #  seriously gone wrong
   def test_does_not_exit_gracefully_when_git_commands_fail(self):
      hasErrored = False
      try:
         longTimeSinceMergeWithArchipelagoModifier = 0
         branchIsNotUpToDateWithArchipelagoModifier = 0
         timeDistance = TimeDistance.TimeDistance(
            "release-archipelago", 
            "some-branch", 
            longTimeSinceMergeWithArchipelagoModifier, 
            branchIsNotUpToDateWithArchipelagoModifier)
      except AttributeError:
         hasErrored = True

      self.assertTrue(hasErrored)

if __name__ == '__main__':
    unittest.main()