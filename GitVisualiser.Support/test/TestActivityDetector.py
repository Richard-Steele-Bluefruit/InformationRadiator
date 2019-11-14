import unittest
from mock import patch, MagicMock

from projectpaths import *
import ActivityDetector

class TestActivityDetectorTestData:
   git_contributor_names_and_number_of_commits = [
      '145', 'Tim Yorke', 
      '145', 'mdodkins', 
      '127', 'LucyPovey', 
      '122', 'Jan Bunes', 
      '84', 'Todd Dowty', 
      '80', 'Neil Dingley', 
      '75', 'MatthewDennis', 
      '67', 'Eduard Gruber', 
      '60', 'Ashlei', 
      '59', 'JamesHollow', 
      '59', 'PabloMansanet', 
      '49', 'Andreas Karg', 
      '47', 'denzilpearce', 
      '45', 'Matthew', 
      '42', 'Dominic Roberts', 
      '36', 'Laurent Rabatel', 
      '31', 'yspeake', 
      '29', 'James Parker', 
      '19', 'Igor Beljutins', 
      '8', 'ToddDowty', 
      '2', 'AndreasKarg', 
      '2', 'DenzilPearce', 
      '2', 'Samuel Riley', 
      '1', 'Keith Moore', 
      '1', 'Simon Clements']

   git_touched_directories_with_percentages = [
      (5.6, 'tools/BDDScripts/FeatureFileConversion/specfrost/tests/'), 
      (5.5, 'tools/BDDScripts/FeatureFileConversion/tests/'), 
      (5.5, 'NG/'), 
      (4.7, 'tools/ExternalFlashBuilderUtility/'), 
      (3.7, 'tools/BDDScripts/FeatureFileConversion/'), 
      (3.7, 'COMPONENTS/Common/Tests/'), 
      (0.0, 'tools/BDDScripts/FeatureFileConversion/specfrost/conversion/'), 
      (3.6, 'COMPONENTS/RichUI/Screens/'), 
      (3.1, 'NG/MiTime/'), 
      (27.4, 'NG/BlueberrySpecificationTests/Features/'), 
      (18.6, 'NG/InternetGateway/Build/GCC.VS/'), 
      (0.0, 'COMPONENTS/')
   ]

   mergeconflictdetector_conflict_data = {
      'ourCode': ["something that was in your archipelago"],
      'theirCode': ["something that was in the release archipelago"],
      'file': ["bobs-fantastic-file"],
      'line': ["1"]
   }

# necessary as we need to mock out the init and the property
class MockMergeConflictDetector(MagicMock):
   @property
   def ConflictData(self):
      return TestActivityDetectorTestData.mergeconflictdetector_conflict_data

@patch('Git.getCommitsAndAuthorsSinceLastMergeWithArchipelago', MagicMock(
      return_value=TestActivityDetectorTestData.git_contributor_names_and_number_of_commits))
@patch('MergeConflictDetector.MergeConflictDetector', MockMergeConflictDetector())
class TestActivityDetector(unittest.TestCase):

   @patch('Git.getTouchedDirectoriesWithPercentageTouched', MagicMock(
      return_value=TestActivityDetectorTestData.git_touched_directories_with_percentages))
   def test_loads_contributor_names(self):
      # When
      activityDetector = ActivityDetector.ActivityDetector("archipelago", "conflictingbranch")
   
      # Then
      expectedCommitters = [\
         'Tim Yorke', \
         'mdodkins', \
         'LucyPovey', \
         'Jan Bunes', \
         'Todd Dowty', \
         'Neil Dingley', \
         'MatthewDennis', \
         'Eduard Gruber', \
         'Ashlei', \
         'JamesHollow', \
         'PabloMansanet', \
         'Andreas Karg', \
         'denzilpearce', \
         'Matthew', \
         'Dominic Roberts', \
         'Laurent Rabatel', \
         'yspeake', \
         'James Parker', \
         'Igor Beljutins', \
         'ToddDowty', \
         'AndreasKarg', \
         'DenzilPearce', \
         'Samuel Riley', \
         'Keith Moore', \
         'Simon Clements']
      actualCommitters = activityDetector.Contributors
      self.assertEqual(expectedCommitters, actualCommitters)
   
   @patch('Git.getTouchedDirectoriesWithPercentageTouched', MagicMock(
      return_value=TestActivityDetectorTestData.git_touched_directories_with_percentages))
   def test_loads_the_number_of_commits_by_contributors(self):
      # When
      activityDetector = ActivityDetector.ActivityDetector("archipelago", "conflictingbranch")
   
      # Then
      expectedTotalCommits = 145+145+127+122+84+80+75+67+60+59+59+49+47+45+42+36+31+29+19+8+2+2+2+1+1
      actualTotalCommits = activityDetector.numberOfCommitsByContributors
      self.assertEqual(expectedTotalCommits, actualTotalCommits)
   
   @patch('Git.getTouchedDirectoriesWithPercentageTouched', MagicMock(
      return_value=TestActivityDetectorTestData.git_touched_directories_with_percentages))
   def test_loads_touched_directories(self):
      # When
      activityDetector = ActivityDetector.ActivityDetector("archipelago", "conflictingbranch")
   
      # Then
      expectedTouchedDirectoriesWithPercentages = [\
         (27.4, 'NG/BlueberrySpecificationTests/Features/'), \
         (18.6, 'NG/InternetGateway/Build/GCC.VS/'), \
         (5.6, 'tools/BDDScripts/FeatureFileConversion/specfrost/tests/'), \
         (5.5, 'tools/BDDScripts/FeatureFileConversion/tests/'), \
         (5.5, 'NG/'), \
         (4.7, 'tools/ExternalFlashBuilderUtility/'), \
         (3.7, 'tools/BDDScripts/FeatureFileConversion/'), \
         (3.7, 'COMPONENTS/Common/Tests/'), \
         (3.6, 'COMPONENTS/RichUI/Screens/'), \
         (3.1, 'NG/MiTime/'), \
         (0.0, 'tools/BDDScripts/FeatureFileConversion/specfrost/conversion/'), \
         (0.0, 'COMPONENTS/')]
      actualTouchedDirectoriesWithPercentages = activityDetector.TouchedDirectoriesWithPercentages
      self.assertEqual(expectedTouchedDirectoriesWithPercentages, actualTouchedDirectoriesWithPercentages)

   @patch('Git.getTouchedDirectoriesWithPercentageTouched', MagicMock(
      return_value=[]))
   def test_loads_empty_list_of_touched_directories(self):
      # When
      activityDetector = ActivityDetector.ActivityDetector("archipelago", "conflictingbranch")
   
      # Then
      expectedTouchedDirectoriesWithPercentages = []
      actualTouchedDirectoriesWithPercentages = activityDetector.TouchedDirectoriesWithPercentages
      self.assertEqual(expectedTouchedDirectoriesWithPercentages, actualTouchedDirectoriesWithPercentages)
   
   @patch('Git.getTouchedDirectoriesWithPercentageTouched', MagicMock(
      return_value=TestActivityDetectorTestData.git_touched_directories_with_percentages))
   def test_loads_conflict_data_from_MergeConflictDetector(self):
      # When
      activityDetector = ActivityDetector.ActivityDetector("archipelago", "conflictingbranch")
   
      # Then
      expectedConflictData = TestActivityDetectorTestData.mergeconflictdetector_conflict_data
      actualConflictData = activityDetector.ConflictData
      self.assertEqual(expectedConflictData, actualConflictData)

if __name__ == '__main__':
    unittest.main()