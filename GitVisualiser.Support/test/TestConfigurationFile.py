import unittest
from mock import patch, MagicMock

from projectpaths import *
import ConfigurationFile

class TestConfigurationFileTestData:
   config_txt_full = \
"""
githubUsername username
repositoryName projectName
amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent 1000 
releaseArchipelago andreaskartg/Development
longTimeSinceMergeWithArchipelagoModifier 20
branchIsNotUpToDateInHoursWithArchipelagoModifier 10
forEveryConflictModifier 2
forEveryContributorModifier 10
touchedWithAHighPercentageThreshold 60
forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier 10
textSize 24
maxSizeOfDrawableToStopCollidingWithCentre 100
maxDistanceFromCentreForScreenSize 400
minDistanceFromCentreForScreenSize 20
minSizeOfDrawableToBeVisible 10
windowName Archipelagoerisermatron
screenWidth 1024
screenHeight 768
automaticallyUpdateRepository false
blackList [TheLampArchipelago]    
minimumPullRequestNumberFontSizePixels 40
"""

   config_txt_incomplete = \
"""
githubUsername username
repositoryName projectName
amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent 1000 
releaseArchipelago andreaskartg/Development
longTimeSinceMergeWithArchipelagoModifier 20
"""

   config_txt_multiple_blacklist_items = \
"""
githubUsername username
repositoryName projectName
amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent 1000 
releaseArchipelago andreaskartg/Development
longTimeSinceMergeWithArchipelagoModifier 20
branchIsNotUpToDateInHoursWithArchipelagoModifier 10
forEveryConflictModifier 2
forEveryContributorModifier 10
touchedWithAHighPercentageThreshold 60
forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier 10
textSize 24
maxSizeOfDrawableToStopCollidingWithCentre 100
maxDistanceFromCentreForScreenSize 400
minDistanceFromCentreForScreenSize 20
minSizeOfDrawableToBeVisible 10
windowName Archipelagoerisermatron
screenWidth 1024
screenHeight 768
automaticallyUpdateRepository false
blackList [TheLampArchipelago,MrBobArchipelago,MrJimArchipelago,MissHillaryArchipelago]  
minimumPullRequestNumberFontSizePixels 40  
"""

   config_txt_with_random_comments = \
"""
githubUsername username
repositoryName projectName
amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent 1000 
releaseArchipelago andreaskartg/Development
longTimeSinceMergeWithArchipelagoModifier 20
branchIsNotUpToDateInHoursWithArchipelagoModifier 10 # oh look a comment
forEveryConflictModifier 2
forEveryContributorModifier 10
touchedWithAHighPercentageThreshold 60
forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier 10
textSize 24 # good golly, another comment
maxSizeOfDrawableToStopCollidingWithCentre 100
maxDistanceFromCentreForScreenSize 400
minDistanceFromCentreForScreenSize 20  # say it not?! A comment sir?
minSizeOfDrawableToBeVisible 10
windowName Archipelagoerisermatron
screenWidth 1024
screenHeight 768
automaticallyUpdateRepository false
blackList [TheLampArchipelago]            # heathen, thine comments affect me not!
minimumPullRequestNumberFontSizePixels 40
"""

   config_txt_with_bad_comments = \
"""
githubUsername username
repositoryName projectName
amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent 1000# a comment with no space before the hash
releaseArchipelago andreaskartg/Development
longTimeSinceMergeWithArchipelagoModifier 20
branchIsNotUpToDateInHoursWithArchipelagoModifier 10
forEveryConflictModifier 2
forEveryContributorModifier 10
touchedWithAHighPercentageThreshold 60
forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier 10
textSize 24#le-problem
maxSizeOfDrawableToStopCollidingWithCentre 100
maxDistanceFromCentreForScreenSize 400
minDistanceFromCentreForScreenSize 20
minSizeOfDrawableToBeVisible 10
windowName Archipelagoerisermatron# comments like this are gonna cause problems
screenWidth 1024
screenHeight 768
automaticallyUpdateRepository false
blackList [TheLampArchipelago]
minimumPullRequestNumberFontSizePixels 40
"""

class MockFile(MagicMock):

   config_txt = None

   @classmethod
   def reset(theClass):
      theClass.config_txt = None

   def read(self):
      return MockFile.config_txt

   def close(self):
      pass

@patch('__builtin__.open', MockFile)
class TestConfigurationFile(unittest.TestCase):

   def setUp(self):
      MockFile.reset()

   def test_Read_loads_values_into_all_configuration_settings(self):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_full

      # When
      ConfigurationFile.read("config.txt")

      # Then
      self.assertNotEqual(None, ConfigurationFile.githubUsername)
      self.assertNotEqual(None, ConfigurationFile.repositoryName)
      self.assertNotEqual(None, ConfigurationFile.amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)
      self.assertNotEqual(None, ConfigurationFile.releaseArchipelago)
      self.assertNotEqual(None, ConfigurationFile.longTimeSinceMergeWithArchipelagoModifier)
      self.assertNotEqual(None, ConfigurationFile.branchIsNotUpToDateInHoursWithArchipelagoModifier)
      self.assertNotEqual(None, ConfigurationFile.forEveryConflictModifier)
      self.assertNotEqual(None, ConfigurationFile.forEveryContributorModifier)
      self.assertNotEqual(None, ConfigurationFile.touchedWithAHighPercentageThreshold)
      self.assertNotEqual(None, ConfigurationFile.forEveryDirectoryThatHasBeenTouchedWithAHighPercentageModifier)
      self.assertNotEqual(None, ConfigurationFile.textSize)
      self.assertNotEqual(None, ConfigurationFile.maxSizeOfDrawableToStopCollidingWithCentre)
      self.assertNotEqual(None, ConfigurationFile.maxDistanceFromCentreForScreenSize)
      self.assertNotEqual(None, ConfigurationFile.minDistanceFromCentreForScreenSize)
      self.assertNotEqual(None, ConfigurationFile.minSizeOfDrawableToBeVisible)
      self.assertNotEqual(None, ConfigurationFile.windowName)
      self.assertNotEqual(None, ConfigurationFile.screenWidth)
      self.assertNotEqual(None, ConfigurationFile.screenHeight)
      self.assertNotEqual(None, ConfigurationFile.automaticallyUpdateRepository)
      self.assertNotEqual(None, ConfigurationFile.blackList)
      self.assertNotEqual(None, ConfigurationFile.minimumPullRequestNumberFontSizePixels)

   @patch('ConfigurationFile._exitProgram')
   def test_Read_exits_program_if_it_could_not_read_all_of_the_configurations(self, exitProgramMock):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_incomplete

      # When
      ConfigurationFile.read("config.txt")

      # Then
      self.assertTrue(exitProgramMock.called)

   def test_Read_number_values_as_int(self):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_full

      # When
      ConfigurationFile.read("config.txt")

      # Then
      self.assertEqual(1024, ConfigurationFile.screenWidth)
      self.assertNotEqual("1024", ConfigurationFile.screenWidth)

   def test_Read_boolean_values_as_bool(self):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_full

      # When
      ConfigurationFile.read("config.txt")

      # Then
      self.assertEqual(False, ConfigurationFile.automaticallyUpdateRepository)
      self.assertNotEqual("false", ConfigurationFile.automaticallyUpdateRepository)

   def test_Read_lists_with_single_element_as_array(self):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_full

      # When
      ConfigurationFile.read("config.txt")

      # Then
      expectedArray = ["TheLampArchipelago"]
      self.assertEquals(len(expectedArray), len(ConfigurationFile.blackList))
      self.assertEquals(expectedArray, ConfigurationFile.blackList)
      incorrectlyParsedArray = "[TheLampArchipelago]"
      self.assertNotEqual(incorrectlyParsedArray, ConfigurationFile.automaticallyUpdateRepository)

   def test_Read_lists_with_multiple_elements_as_array(self):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_multiple_blacklist_items

      # When
      ConfigurationFile.read("config.txt")

      # Then
      expectedArray = ["TheLampArchipelago","MrBobArchipelago","MrJimArchipelago","MissHillaryArchipelago"]
      self.assertEqual(len(expectedArray), len(ConfigurationFile.blackList))
      self.assertEqual(expectedArray, ConfigurationFile.blackList)
      incorrectlyParsedArray = "[TheLampArchipelago,MrBobArchipelago,MrJimArchipelago,MissHillaryArchipelago]"
      self.assertNotEqual(incorrectlyParsedArray, ConfigurationFile.automaticallyUpdateRepository)

   @patch('ConfigurationFile._exitProgram')
   def test_Read_is_able_to_handle_comments_with_at_least_one_space_before_the_comment_identifier(self, exitProgramMock):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_with_random_comments

      # When
      ConfigurationFile.read("config.txt")

      # Then
      self.assertFalse(exitProgramMock.called)

   def test_Read_is_not_able_to_handle_comments_with_no_spaces_before_the_comment_identifier(self):
      # Given
      MockFile.config_txt = TestConfigurationFileTestData.config_txt_with_bad_comments

      # When
      ConfigurationFile.read("config.txt")

      # Then
      self.assertEqual("1000#", ConfigurationFile.amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)
      self.assertEqual("24#le-problem", ConfigurationFile.textSize)
      self.assertEqual("Archipelagoerisermatron#", ConfigurationFile.windowName)

if __name__ == '__main__':
    unittest.main()