import unittest
from mock import patch, MagicMock

from projectpaths import *
import BranchDistanceCalculator
import ConfigurationFile
import DifferenceData

class TestBranchDistanceCalculatorTestData:
   activityDetector_conflict_data = {
      'ourCode': ["something that was in your archipelago", "conflict1", "conflict2", "conflict3", "conflict4"],
      'theirCode': ["something that was in the release archipelago","anotherconflict1","anotherconflict2","anotherconflict3","anotherconflict4"],
      'file': ["bobs-fantastic-file", "bobs-fantastic-file", "bobs-fantastic-file", "bobs-fantastic-file", "bobs-fantastic-file"],
      'line': ["1", "2", "3", "4", "5"]
   }

   activityDetector_contributors = ["bob", "ben", "bill", "jim", "jane", "janice", "jude"]
   
   TimeDistance_distance = 0

   TimeDistance_lastMergeDate = "2015-10-12 21:11:01"

   activityDetector_touchedDirectoriesWithPercentages = [\
         (27.4, 'NG/BlueberrySpecificationTests/Features/'), \
         (18.6, 'NG/InternetGateway/Build/GCC.VS/'), \
         (5.6, 'tools/BDDScripts/FeatureFileConversion/specfrost/tests/'), \
         (3.1, 'NG/MiTime/'), \
         (0.0, 'COMPONENTS/')]

class MockActivityDetector(MagicMock):
   @property
   def ConflictData(self):
      return TestBranchDistanceCalculatorTestData.activityDetector_conflict_data

   @property
   def Contributors(self):
      return TestBranchDistanceCalculatorTestData.activityDetector_contributors

   @property
   def TouchedDirectoriesWithPercentages(self):
      return TestBranchDistanceCalculatorTestData.activityDetector_touchedDirectoriesWithPercentages

class MockTimeDistance(MagicMock):
   @property
   def Distance(self):
      return TestBranchDistanceCalculatorTestData.TimeDistance_distance

   @property
   def LastMergeDate(self):
      return TestBranchDistanceCalculatorTestData.TimeDistance_lastMergeDate

class MockDifferenceData(object):
   
   saveToFileCalled = False
   savedDifferenceData = None
   numberOfContributors = 0
   contributors = []
   touchedDirectoriesWithPercentages = []
   conflictData = {}
   distance = 0
   archipelagoName = ""
   branchName = ""
   lastMergeDate = ""
   pullRequests = []

   @classmethod
   def mock__init__(theClass, numberOfContributors, contributors, touchedDirectoriesWithPercentages, conflictData, distance, archipelago, branch, lastMergeDate, pullRequests):
      theClass.numberOfContributors = numberOfContributors
      theClass.contributors = contributors
      theClass.touchedDirectoriesWithPercentages = touchedDirectoriesWithPercentages
      theClass.conflictData = conflictData
      theClass.distance = distance
      theClass.archipelago = archipelago
      theClass.branch = branch
      theClass.lastMergeDate = lastMergeDate
      theClass.pullRequests = pullRequests

   @classmethod
   def reset(theClass):
      theClass.saveToFile = False
      theClass.numberOfContributors = 0
      theClass.contributors = []
      theClass.touchedDirectoriesWithPercentages = []
      theClass.conflictData = {}
      theClass.distance = 0
      theClass.archipelagoName = ""
      theClass.branchName = ""
      theClass.lastMergeDate = ""
      theClass.pullRequests = []

   @classmethod
   def saveToFile(theClass):
      theClass.saveToFileCalled = True

class MockGithubWrapper(object):
   
   pullRequests = []

   @classmethod
   def reset(theClass):
      pullRequests = []

   @classmethod
   def getPullRequests(theClass, localRemoteWithArchipelagoName):
      return theClass.pullRequests

@patch('ActivityDetector.ActivityDetector', MockActivityDetector())
@patch('DifferenceData.DifferenceData.__init__', MockDifferenceData.mock__init__)
@patch('DifferenceData.DifferenceData.saveToFile', MockDifferenceData.saveToFile)
@patch('TimeDistance.TimeDistance', MockTimeDistance())
@patch('Github.GithubWrapper.getPullRequests', MockGithubWrapper.getPullRequests)
class TestBranchDistanceCalculator(unittest.TestCase):

   def setUp(self):
      ConfigurationFile.longTimeSinceMergeWithArchipelagoModifier = 0
      ConfigurationFile.branchIsNotUpToDateWithArchipelagoModifier = 0
      ConfigurationFile.forEveryConflictModifier = 0
      ConfigurationFile.forEveryContributorModifier = 0
      ConfigurationFile.touchedWithAHighPercentageThreshold = 0
      ConfigurationFile.forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier = 0
      TestBranchDistanceCalculatorTestData.TimeDistance_distance = 0
      MockDifferenceData.reset()
      MockGithubWrapper.reset()

   def test_modifies_distance_with_conflicts(self):
      # Given
      ConfigurationFile.forEveryConflictModifier = 10

      # When
      branchDistanceCalculator = BranchDistanceCalculator.BranchDistanceCalculator("release-archipelago", "your-archipelago")

      # Then
      expectedDistance = 50
      actualDistance = MockDifferenceData.distance # BranchDistanceCalculator should set this in its "__init(...)"
      self.assertEqual(expectedDistance, actualDistance)

   def test_modifies_distance_with_number_of_contributors(self):
      # Given
      ConfigurationFile.forEveryContributorModifier = 1

      # When
      branchDistanceCalculator = BranchDistanceCalculator.BranchDistanceCalculator("release-archipelago", "your-archipelago")

      # Then
      expectedDistance = 7
      actualDistance = MockDifferenceData.distance
      self.assertEqual(expectedDistance, actualDistance)

   def test_modifies_distance_with_the_time_distance(self):
      # Given
      TestBranchDistanceCalculatorTestData.TimeDistance_distance = 14.4

      # When
      branchDistanceCalculator = BranchDistanceCalculator.BranchDistanceCalculator("release-archipelago", "your-archipelago")

      # Then
      expectedDistance = 14.4
      actualDistance = MockDifferenceData.distance
      self.assertEqual(expectedDistance, actualDistance)

   def test_modifies_distance_with_number_of_directories_touched_above_threshold(self):
      # Given
      ConfigurationFile.forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier = 5
      ConfigurationFile.touchedWithAHighPercentageThreshold = 20

      # When
      branchDistanceCalculator = BranchDistanceCalculator.BranchDistanceCalculator("release-archipelago", "your-archipelago")

      # Then
      expectedDistance = 5
      actualDistance = MockDifferenceData.distance
      self.assertEqual(expectedDistance, actualDistance)

   def test_distance_modification_is_cumulative(self):
      # Given
      ConfigurationFile.forEveryConflictModifier = 10
      ConfigurationFile.forEveryContributorModifier = 1
      TestBranchDistanceCalculatorTestData.TimeDistance_distance = 14.4
      ConfigurationFile.forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier = 5
      ConfigurationFile.touchedWithAHighPercentageThreshold = 20

      # When
      branchDistanceCalculator = BranchDistanceCalculator.BranchDistanceCalculator("release-archipelago", "your-archipelago")

      # Then
      expectedDistance = 50+7+14.4+5
      actualDistance = MockDifferenceData.distance
      self.assertEqual(expectedDistance, actualDistance)

   def test_saves_data_gathered_to_file(self):
      # Given
      ConfigurationFile.forEveryContributorModifier = 1
      MockGithubWrapper.pullRequests = [
         "Bob/MIS-a pull request",
         "Bob/MIS Another",
         "jane/MIG - a fix"
      ]

      # When
      branchDistanceCalculator = BranchDistanceCalculator.BranchDistanceCalculator("release-archipelago", "your-archipelago")

      # Then
      self.assertTrue(MockDifferenceData.saveToFileCalled)

      expected = 7
      self.assertEqual(expected, MockDifferenceData.numberOfContributors)

      expected = ['bob', 'ben', 'bill', 'jim', 'jane', 'janice', 'jude']
      self.assertEqual(expected, MockDifferenceData.contributors)

      expected = [
         (27.4, 'NG/BlueberrySpecificationTests/Features/'), 
         (18.6, 'NG/InternetGateway/Build/GCC.VS/'), 
         (5.6, 'tools/BDDScripts/FeatureFileConversion/specfrost/tests/'), 
         (3.1, 'NG/MiTime/'), 
         (0.0, 'COMPONENTS/')
      ]
      self.assertEqual(expected, MockDifferenceData.touchedDirectoriesWithPercentages)

      expected = {
         'theirCode': ['something that was in the release archipelago', 'anotherconflict1', 'anotherconflict2', 'anotherconflict3', 'anotherconflict4'], 
         'line': ['1', '2', '3', '4', '5'], 
         'file': ['bobs-fantastic-file', 'bobs-fantastic-file', 'bobs-fantastic-file', 'bobs-fantastic-file', 'bobs-fantastic-file'], 
         'ourCode': ['something that was in your archipelago', 'conflict1', 'conflict2', 'conflict3', 'conflict4']
      }
      self.assertEqual(expected, MockDifferenceData.conflictData)

      expected = 7
      self.assertEqual(expected, MockDifferenceData.distance)

      expected = "release-archipelago"
      self.assertEqual(expected, MockDifferenceData.archipelago)

      expected = "your-archipelago"
      self.assertEqual(expected, MockDifferenceData.branch)

      expected = "2015-10-12 21:11:01"
      self.assertEqual(expected, MockDifferenceData.lastMergeDate)

      expected = [
         "Bob/MIS-a pull request",  # bob == a github username, not necessarily the same as "contributors"
         "Bob/MIS Another",
         "jane/MIG - a fix"
      ]
      self.assertEqual(expected, MockDifferenceData.pullRequests)

if __name__ == '__main__':
    unittest.main()