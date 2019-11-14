import unittest
from mock import patch, MagicMock
import Queue
import tempfile
import datetime

from projectpaths import *
import Git

class TestGitTestData(object):
   git_merge_base_mdodkins_Development_with_MrBobArchipelago = \
"""
95f8b004f55982dc8a552133b70c7eaaa0accf8b
"""

   git_log_ancestry_path_SHA_with_MrBobArchipelago = \
"""
commit 95f8b004f55982dc8a552133b70c7eaaa0accf8b
Merge: 23e2899 d0600b3
Author: mdodkins <matthew@bluefruit.co.uk>
Date:   Tue Sep 1 06:50:09 2015 +0100

    Merge pull request #691 from Laurent-AbSw/Mains_signal_loss_fix
    
    Dispatches timer callbacks to MiCan event queue

commit bf2a39b6d9aa7b99774b5e1f9ee1c4acdcf491a7
Merge: d40a832 95f8b00
Author: Dominic Roberts <dominic.roberts@bluefruit.co.uk>
Date:   Tue Sep 1 10:18:45 2015 +0100

    Merge branch 'TheLampArchipelago' of git@github.com:mdodkins/SchneiderMiSeries.git into MrBobArchipelago-merge-mdodkins

commit c7a0b03a53c40a276d8c84cb517a4ae63ce3d9cb
Merge: 86de044 bf2a39b
Author: Dominic-Roberts <dominic.roberts@bluefruit.co.uk>
Date:   Tue Sep 1 10:21:34 2015 +0100

    Merge pull request #70 from Dominic-Roberts/MrBobArchipelago-merge-mdodkins
    
    Mr bob archipelago merge mdodkins

commit f2611a3aec6a44afc3b436b9bf80e36d7b66e992
Author: Dominic Roberts <dominic.roberts@bluefruit.co.uk>
Date:   Thu Oct 1 13:38:19 2015 +0100

    This should not get into a pull request, if it does it is unintentional,
    so please revert.

"""

   git_for_each_ref_authorname_refname = open("TestGitTestData-git_for_each_ref_authorname_refname.txt", 'r').read()

   git_show_with_ISO8601 = """2015-09-30 13:00:00 +0100"""

   git_merge_base_andi_dev_with_rock_archi = open("TestGitTestData-git_merge_base_andi_dev_with_rock_archi.txt", 'r').read()

   git_shortlog_since_2015_09_13_13_00_00_00_plus_0100 = \
"""   420   Jan Bunes
   350\tyspeake
   346\tTim Yorke
   270\tMatthewDennis
   230\tLucyPovey
   197\tJames Parker
   191\tmdodkins
   184\tLaurent Rabatel
   184\tTodd Dowty
   179\tDominic Roberts
   149\tAshlei
   145\tEduard Gruber
   139\tPabloMansanet
   133\tAndreas Karg
   127\tJamesHollow
   123\tNeil Dingley
   122\tKeith Moore
   108\tMatthew
    80\tLucy Povey
    69\tdenzilpearce
    47\tAdi
    33\tIgor Beljutins
    18\tSimon Clements
    16\trichard.keast
"""

   git_dirstat = \
"""   0.3% Blueberry/Blueberry/
   0.0% COMPONENTS/CLI/PROJECT_ARM/
   0.8% COMPONENTS/CLI/Source/Commands/General/
   2.0% COMPONENTS/CLI/Tests/Tests/
  20.0% NG/BlueberrySpecificationTests/Features/
   0.7% NG/CommonMocks/
   0.0% NG/InternetGateway/Build/GCC.Eclipse/
   5.4% NG/InternetGateway/Build/GCC.VS/
   0.0% NG/InternetGateway/InternetGatewayBaseCode/
   2.2% tools/BDDScripts/FeatureFileConversion/specfrost/conversion/
   1.5% tools/BDDScripts/FeatureFileConversion/specfrost/tests/testdata/
   1.9% tools/BDDScripts/FeatureFileConversion/specfrost/tests/
   0.0% tools/BDDScripts/FeatureFileConversion/specfrost/util/
   0.0% tools/BDDScripts/FeatureFileConversion/specfrost/
   3.4% tools/BDDScripts/FeatureFileConversion/tests/
   2.2% tools/BDDScripts/FeatureFileConversion/
   1.3% tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtility/
   0.0% tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtilityTests/Resources/2JsonFiles/
   0.0% tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtilityTests/Resources/
   1.4% tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtilityTests/
   0.0% tools/GoogleStressTest/
"""

   archipelagos_recently_active = [
      'DenzilPearce/TuvaluArchipelago', 
      'PabloMansanet/MatthewArchipelago', 
      'PabloMansanet/NaboombuArchipelago', 
      'PabloMansanet/NowhereArchipelago', 
      'andreaskartg/DwayneTheRockJohnsonArchipelago', 
      'andreaskartg/ScillyArchipelago', 
      'andreaskartg/TheWorldArchipelago', 
      'jameshollow/AlcatrazArchipelago', 
      'laurent/NaboombuArchipelago', 
      'mdodkins/MatthewArchipelago', 
      'mdodkins/NaboombuArchipelago', 
      'mdodkins/TheLampArchipelago', 
      'origin/MrBobArchipelago', 
      'rich/CollidingArchipelagoes', 
      'rich/ScillyArchipelago', 
      'rich/TheWorldArchipelago', 
      'rich/ZanzibarArchipelago', 
      'yspeake/CanaryArchipelago', 
      'yspeake/ScillyArchipelago', 
      'yspeake/TheLampArchipelago', 
      'yspeake/YansArchipelago'
   ]

   # note that each line has two spaces on the end, this is important!
   git_branch_with_only_valid_archipelago_names = \
"""
remotes/DenzilPearce/TuvaluArchipelago  
remotes/PabloMansanet/MatthewArchipelago  
remotes/PabloMansanet/NaboombuArchipelago  
remotes/PabloMansanet/NowhereArchipelago  
remotes/PabloMansanet/WintermuteArchipelago  
remotes/andreaskartg/Drop2KickstarterArchipelago  
remotes/andreaskartg/DwayneTheRockJohnsonArchipelago  
remotes/andreaskartg/ScillyArchipelago  
remotes/andreaskartg/TheWorldArchipelago  
remotes/jameshollow/AlcatrazArchipelago  
remotes/laurent/JapanArchipelago  
remotes/laurent/NaboombuArchipelago  
remotes/mdodkins/MatthewArchipelago  
remotes/mdodkins/NaboombuArchipelago  
remotes/mdodkins/TheLampArchipelago  
remotes/origin/AlcatrazArchipelago-fix-IG-build  
remotes/origin/MrBobArchipelago  
remotes/rich/AtlantisArchipelago  
remotes/rich/CollidingArchipelagoes  
remotes/rich/ScillyArchipelago  
remotes/rich/TheWorldArchipelago  
remotes/rich/ZanzibarArchipelago  
remotes/yspeake/CanaryArchipelago  
remotes/yspeake/CanaryArchipelago_WithoutJobRunnerTests  
remotes/yspeake/DoggerArchipelago  
remotes/yspeake/ScillyArchipelago  
remotes/yspeake/TheLampArchipelago  
remotes/yspeake/YansArchipelago  
"""

class helper_command(object):

   lastRunMultiplePipedCommands = []
   multiplePipedCommandsOutput = []
   _commandResults = Queue.Queue()

   @classmethod
   def reset(theClass):
      theClass._commandResults = Queue.Queue()
      theClass.lastRunMultiplePipedCommands = []
      theClass.multiplePipedCommandsOutput = []

   @classmethod
   def addCommandResults(theClass, list):
      map(theClass._commandResults.put, list)

   @classmethod
   def numberOfCommandsNotRun(theClass):
      storedQueue = Queue.Queue(theClass._commandResults)

      count  = 0
      while not theClass._commandResults.empty():
         theClass._commandResults.get()
         count+=1

      theClass._commandResults = storedQueue

      return count

   @classmethod
   def run(theClass, commandType, commandParameters):
      commandOutput = theClass._commandResults.get()
      if isinstance(commandOutput, bool):
         return commandOutput
      
      # we want to return copies incase the GIT functions play with them and we 
      #  then re-use them in later tests
      commandOutput = str(commandOutput)
      return commandOutput

   @classmethod
   def runToFile(theClass, commandType, commandParameters, tempFile):
      commandOutput = theClass._commandResults.get()
      if isinstance(commandOutput, bool):
         return commandOutput

      tempFile.seek(0)
      tempFile.write(commandOutput)
      tempFile.seek(0)
      return True

   @classmethod
   def runMultiplePipedCommands(theClass, parameters):
      theClass.lastRunMultiplePipedCommands = parameters.split('|')
      theClass.lastRunMultiplePipedCommands = [x.strip() for x in theClass.lastRunMultiplePipedCommands]
      return theClass.multiplePipedCommandsOutput


class MockDateTime(datetime.datetime):

   nowValue = None

   @classmethod
   def reset(theClass):
      theClass.nowValue = None

   @classmethod
   def now(theClass):
      return theClass.nowValue

class MockGetLastMergeDateAndTimeString(object):

   returnValues = {}

   @classmethod
   def reset(theClass):
      theClass.returnValues = {}
   
   @classmethod
   def run(theClass, releaseArchipelago, branch):
      return theClass.returnValues[branch]

@patch('Git.__runCommand', helper_command.run)
@patch('Git.__runCommandAndPipeOutputToFile', helper_command.runToFile)
@patch('Git.__runMultiplePipedCommands', helper_command.runMultiplePipedCommands)
class TestGit(unittest.TestCase):

   def setUp(self):
      helper_command.reset()
      MockDateTime.reset()
      MockGetLastMergeDateAndTimeString.reset()
      self.maxDiff = None  # when it fails, we are going to have large strings sometimes

   def test_getLastCommitDateAndTimeString_successfully(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_show_with_ISO8601
      ])

      # When
      actual = Git.getLastCommitDateAndTimeString("commitsha")

      # Then
      self.assertTrue(actual)
      expectedISO8601DateString = "2015-09-30 13:00:00 +0100"
      self.assertEqual(expectedISO8601DateString, actual)
      self.assertEqual(0, helper_command.numberOfCommandsNotRun())

   def test_getLastCommitDateAndTimeString_checks_log_output(self):
      # Given
      helper_command.addCommandResults([
         False,   # git log fails
         ""
      ])

      # When
      actual = Git.getLastCommitDateAndTimeString("commitsha")

      # Then
      self.assertFalse(actual)
      expectedISO8601DateString = "2015-09-30 13:00:00 +0100"
      self.assertEqual(1, helper_command.numberOfCommandsNotRun())

   def test_getLastMergeDateAndTimeString_successfully(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_merge_base_mdodkins_Development_with_MrBobArchipelago,
         TestGitTestData.git_log_ancestry_path_SHA_with_MrBobArchipelago,
         TestGitTestData.git_for_each_ref_authorname_refname,
         TestGitTestData.git_show_with_ISO8601
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getLastMergeDateAndTimeString(releaseArchipelago, otherBranch)

      # Then
      self.assertTrue(actual)
      expectedISO8601DateString = "2015-09-30 13:00:00 +0100"
      self.assertEqual(expectedISO8601DateString, actual)
      self.assertEqual(0, helper_command.numberOfCommandsNotRun())

   def test_getLastMergeDateAndTimeString_no_commits_exist_after_sha_from_merge_base(self):
      # Given
      helper_command.addCommandResults([
         "incorrectSHAThatIsNotInGitLog",
         "",   # empty output from git command
         TestGitTestData.git_for_each_ref_authorname_refname,
         TestGitTestData.git_show_with_ISO8601
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getLastMergeDateAndTimeString(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(2, helper_command.numberOfCommandsNotRun())

   def test_getLastMergeDateAndTimeString_checks_merge_base_output(self):
      # Given
      helper_command.addCommandResults([
         False,   # merge-base returns false,
         "",   # these following outputs should never be taken
         "",
         "",
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getLastMergeDateAndTimeString(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(4, helper_command.numberOfCommandsNotRun())

   def test_getLastMergeDateAndTimeString_checks_log_output(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_merge_base_mdodkins_Development_with_MrBobArchipelago,
         False,   # git log returns false
         "",
         "",
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getLastMergeDateAndTimeString(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(3, helper_command.numberOfCommandsNotRun())

   def test_getLastMergeDateAndTimeString_checks_for_each_ref_output(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_merge_base_mdodkins_Development_with_MrBobArchipelago,
         TestGitTestData.git_log_ancestry_path_SHA_with_MrBobArchipelago,
         False,   # for_each_ref returns False
         "",
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getLastMergeDateAndTimeString(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(2, helper_command.numberOfCommandsNotRun())

   def test_getLastMergeDateAndTimeString_checks_for_each_ref_output(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_merge_base_mdodkins_Development_with_MrBobArchipelago,
         TestGitTestData.git_log_ancestry_path_SHA_with_MrBobArchipelago,
         TestGitTestData.git_for_each_ref_authorname_refname,
         False,   # git show returns false
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getLastMergeDateAndTimeString(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(1, helper_command.numberOfCommandsNotRun())
      
   def test_getMergeConflictsAndSaveToFile_successfully(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_merge_base_mdodkins_Development_with_MrBobArchipelago,
         TestGitTestData.git_merge_base_andi_dev_with_rock_archi
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      tempFile = tempfile.TemporaryFile()
      commandOutput = Git.getMergeConflictsAndSaveToFile(releaseArchipelago, otherBranch, tempFile)

      actual = tempFile.read()

      # Then
      self.assertEqual(None, commandOutput)
      expectedTextFromTheMiddleOfTheFile = \
"""
-   virtual void SetTimeOfTheWeek(TimeOfTheWeek timeOfTheWeek)
+   virtual void SetTimeOfTheWeek(TimeOfTheWeek timeOfTheWeek) override
    {
    }
 
-   virtual DateTime GetLocalDateTime() { return DateTime(GetDate(), GetTimeOfDay()); }
-   virtual void ApplyDSTAndTimeZoneIfApplicable(DateTime& dateTime){};
+   virtual DateTime GetLocalDateTime() const override { return DateTime(GetDate(), GetTimeOfDay()); }
+   virtual void ApplyDSTAndTimeZoneIfApplicable(DateTime& dateTime) override {};
 private:
    // Don't allow copy and assignment
    NullMockCalendar(const NullMockCalendar &);
merged
  result 100644 d39fd35770bb61d6ba9258067ef36a64bb62bc05 NG/InternetGateway/Build/GCC.VS/.gitignore
  our    100644 449bc819eecff02af1cdc350e85587fe60afc18f NG/InternetGateway/Build/GCC.VS/.gitignore
@@ -1,3 +1,4 @@
 *.crc
 *.bin
-*.zip
\ No newline at end of file
"""
      self.assertTrue(expectedTextFromTheMiddleOfTheFile in actual)
      self.assertEqual(0, helper_command.numberOfCommandsNotRun())

   def test_getMergeConflictsAndSaveToFile_checks_merge_base_output(self):
      # Given
      helper_command.addCommandResults([
         False,   # merge-base fails
         "",
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      tempFile = tempfile.TemporaryFile()
      commandOutput = Git.getMergeConflictsAndSaveToFile(releaseArchipelago, otherBranch, tempFile)

      actual = tempFile.read()

      # Then
      self.assertFalse(commandOutput)
      self.assertEqual(0, len(actual))
      self.assertEqual(2, helper_command.numberOfCommandsNotRun())

   def test_getMergeConflictsAndSaveToFile_checks_merge_tree_output(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_merge_base_mdodkins_Development_with_MrBobArchipelago,
         False,   # merge-tree fails
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      tempFile = tempfile.TemporaryFile()
      commandOutput = Git.getMergeConflictsAndSaveToFile(releaseArchipelago, otherBranch, tempFile)

      actual = tempFile.read()

      # Then
      self.assertFalse(commandOutput)
      self.assertEqual(0, len(actual))
      self.assertEqual(1, helper_command.numberOfCommandsNotRun())

   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2015-09-30 13:00:00 +0100"))
   def test_getCommitsAndAuthorsSinceLastMergeWithArchipelago_successfully(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_shortlog_since_2015_09_13_13_00_00_00_plus_0100,
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getCommitsAndAuthorsSinceLastMergeWithArchipelago(releaseArchipelago, otherBranch)

      # Then
      self.assertTrue(actual)
      self.assertTrue(isinstance(actual, list))
      expected = [
         420, "Jan Bunes",
         350, "yspeake",
         346, "Tim Yorke",
         270, "MatthewDennis",
         230, "LucyPovey",
         197, "James Parker",
         191, "mdodkins",
         184, "Laurent Rabatel",
         184, "Todd Dowty",
         179, "Dominic Roberts",
         149, "Ashlei",
         145, "Eduard Gruber",
         139, "PabloMansanet",
         133, "Andreas Karg",
         127, "JamesHollow",
         123, "Neil Dingley",
         122, "Keith Moore",
         108, "Matthew",
         80, "Lucy Povey",
         69, "denzilpearce",
         47, "Adi",
         33, "Igor Beljutins",
         18, "Simon Clements",
         16, "richard.keast"
      ]
      self.assertEqual(expected, actual)
      self.assertEqual(0, helper_command.numberOfCommandsNotRun())

   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value=False))
   def test_getCommitsAndAuthorsSinceLastMergeWithArchipelago_checks_getLastMergeDateAndTimeString_output(self):
      # Given
      helper_command.addCommandResults([
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getCommitsAndAuthorsSinceLastMergeWithArchipelago(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(1, helper_command.numberOfCommandsNotRun())

   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="2015-09-30 13:00:00 +0100"))
   def test_getCommitsAndAuthorsSinceLastMergeWithArchipelago_checks_shortlog_output(self):
      # Given
      helper_command.addCommandResults([
         False,   # git shortlog fails
         ""
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getCommitsAndAuthorsSinceLastMergeWithArchipelago(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(1, helper_command.numberOfCommandsNotRun())

   def test_getTouchedDirectoriesWithPercentageTouched_successfully(self):
      # Given
      helper_command.addCommandResults([
         TestGitTestData.git_dirstat,
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getTouchedDirectoriesWithPercentageTouched(releaseArchipelago, otherBranch)

      # Then
      self.assertTrue(actual)
      self.assertTrue(isinstance(actual, list))

      expected = [
         (0.3, "Blueberry/Blueberry/"),
         (0.0, "COMPONENTS/CLI/PROJECT_ARM/"),
         (0.8, "COMPONENTS/CLI/Source/Commands/General/"),
         (2.0, "COMPONENTS/CLI/Tests/Tests/"),
         (20.0, "NG/BlueberrySpecificationTests/Features/"),
         (0.7, "NG/CommonMocks/"),
         (0.0, "NG/InternetGateway/Build/GCC.Eclipse/"),
         (5.4, "NG/InternetGateway/Build/GCC.VS/"),
         (0.0, "NG/InternetGateway/InternetGatewayBaseCode/"),
         (2.2, "tools/BDDScripts/FeatureFileConversion/specfrost/conversion/"),
         (1.5, "tools/BDDScripts/FeatureFileConversion/specfrost/tests/testdata/"),
         (1.9, "tools/BDDScripts/FeatureFileConversion/specfrost/tests/"),
         (0.0, "tools/BDDScripts/FeatureFileConversion/specfrost/util/"),
         (0.0, "tools/BDDScripts/FeatureFileConversion/specfrost/"),
         (3.4, "tools/BDDScripts/FeatureFileConversion/tests/"),
         (2.2, "tools/BDDScripts/FeatureFileConversion/"),
         (1.3, "tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtility/"),
         (0.0, "tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtilityTests/Resources/2JsonFiles/"),
         (0.0, "tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtilityTests/Resources/"),
         (1.4, "tools/ExternalFlashBuilderUtility/ExternalFlashBuilderUtilityTests/"),
         (0.0, "tools/GoogleStressTest/")
      ]
      self.assertEqual(expected, actual)
      self.assertEqual(0, helper_command.numberOfCommandsNotRun())

   def test_getTouchedDirectoriesWithPercentageTouched_fails(self):
      # Given
      helper_command.addCommandResults([
         False   # dirstat fails
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getTouchedDirectoriesWithPercentageTouched(releaseArchipelago, otherBranch)

      # Then
      self.assertFalse(actual)
      self.assertEqual(0, helper_command.numberOfCommandsNotRun())

   def test_getTouchedDirectoriesWithPercentageTouched_returns_an_empty_list(self):
      # Given
      helper_command.addCommandResults([
         "" # failed to find any directories touched
      ])

      # When
      releaseArchipelago = "mdodkins/Development"
      otherBranch = "origin/MrBobArchipelago"
      actual = Git.getTouchedDirectoriesWithPercentageTouched(releaseArchipelago, otherBranch)

      # Then
      emptyList = []
      self.assertEqual(emptyList, actual)
      self.assertEqual(0, helper_command.numberOfCommandsNotRun())


   def test_getArchipelagoNames_successfully(self):
      # Given
      helper_command.multiplePipedCommandsOutput = TestGitTestData.git_branch_with_only_valid_archipelago_names

      # When
      actual = Git.getArchipelagoNames()

      # Then 
      self.assertTrue(actual)
      self.assertTrue(isinstance(actual, list))
      expected = [
         "DenzilPearce/TuvaluArchipelago",
         "PabloMansanet/MatthewArchipelago",
         "PabloMansanet/NaboombuArchipelago",
         "PabloMansanet/NowhereArchipelago",
         "PabloMansanet/WintermuteArchipelago",
         "andreaskartg/Drop2KickstarterArchipelago",
         "andreaskartg/DwayneTheRockJohnsonArchipelago",
         "andreaskartg/ScillyArchipelago",
         "andreaskartg/TheWorldArchipelago",
         "jameshollow/AlcatrazArchipelago",
         "laurent/JapanArchipelago",
         "laurent/NaboombuArchipelago",
         "mdodkins/MatthewArchipelago",
         "mdodkins/NaboombuArchipelago",
         "mdodkins/TheLampArchipelago",
         "origin/AlcatrazArchipelago-fix-IG-build",
         "origin/MrBobArchipelago",
         "rich/AtlantisArchipelago",
         "rich/CollidingArchipelagoes",
         "rich/ScillyArchipelago",
         "rich/TheWorldArchipelago",
         "rich/ZanzibarArchipelago",
         "yspeake/CanaryArchipelago",
         "yspeake/CanaryArchipelago_WithoutJobRunnerTests",
         "yspeake/DoggerArchipelago",
         "yspeake/ScillyArchipelago",
         "yspeake/TheLampArchipelago",
         "yspeake/YansArchipelago"
      ]
      self.assertEqual(expected, actual)

   def test_getArchipelagoNames_does_not_add_branch_names_containing_disallowed_strings(self):
      # Given
      helper_command.multiplePipedCommandsOutput = TestGitTestData.git_branch_with_only_valid_archipelago_names

      # When
      actual = Git.getArchipelagoNames()

      # Then 
      self.assertTrue(actual)
      listOfExpectedCommands = [
         "branch -a",
         'grep "remotes/*"',
         'grep -i "Archipelago"'
      ]
      disallowedStrings = [
         "revert",
         "tmp",
         "merge"
      ]
      commandsToGetRidOfStrings = map(lambda x: "grep -vi \"" + x + "\"", disallowedStrings)
      listOfExpectedCommands = listOfExpectedCommands + commandsToGetRidOfStrings

      for command in helper_command.lastRunMultiplePipedCommands:
         self.assertTrue(command in listOfExpectedCommands)

   def test_getArchipelagoNames_checks_runMultiplePipedCommands_output(self):
      # Given
      helper_command.multiplePipedCommandsOutput = False

      # When
      actual = Git.getArchipelagoNames()

      # Then 
      self.assertFalse(actual)

   @patch('Git.getArchipelagoNames', MagicMock(return_value=list(TestGitTestData.archipelagos_recently_active)))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="""2014-11-23 15:28:14 +0000"""))
   @patch('datetime.datetime', MockDateTime)
   def test_getArchipelagoNamesRecentlyActiveInBranch_excludes_release_archipelago(self):
      # Given
      # the same day as Git.getLastMergeDateAndTimeString
      MockDateTime.nowValue = datetime.datetime(2014, 11, 23, 15, 28, 14)

      # When
      releaseArchipelago = "mdodkins/NaboombuArchipelago"
      amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent = 1000
      actual = Git.getArchipelagoNamesRecentlyActiveInBranch(releaseArchipelago, amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)

      # Then
      expected = list(TestGitTestData.archipelagos_recently_active)
      releaseArchipelagoWasInTestData = False
      for x in expected:
         if x == releaseArchipelago:
            expected.remove(x)
            releaseArchipelagoWasInTestData = True
            break;
      self.assertTrue(releaseArchipelagoWasInTestData)

      self.assertItemsEqual(expected, actual)

   @patch('Git.getArchipelagoNames', MagicMock(return_value=list(TestGitTestData.archipelagos_recently_active)))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="""2014-11-23 15:28:14 +0000"""))
   @patch('datetime.datetime', MockDateTime)
   def test_getArchipelagoNamesRecentlyActiveInBranch_successfully(self):
      # Given
      # the same day as Git.getLastMergeDateAndTimeString
      MockDateTime.nowValue = datetime.datetime(2014, 11, 23, 15, 28, 14)

      # When
      releaseArchipelago = "mdodkins/Development"
      amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent = 1000
      actual = Git.getArchipelagoNamesRecentlyActiveInBranch(releaseArchipelago, amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)

      # Then
      self.assertTrue(actual)
      self.assertTrue(isinstance(actual, list))

   @patch('Git.getArchipelagoNames', MagicMock(return_value=list(TestGitTestData.archipelagos_recently_active)))
   @patch('Git.getLastMergeDateAndTimeString', MockGetLastMergeDateAndTimeString.run)
   @patch('datetime.datetime', MockDateTime)
   def test_getArchipelagoNamesRecentlyActiveInBranch_does_not_include_older_or_exclude_younger_branches(self):
      # Given
      MockDateTime.nowValue = datetime.datetime(2014, 11, 23, 15, 28, 14)
      MockGetLastMergeDateAndTimeString.returnValues = {
         'DenzilPearce/TuvaluArchipelago': "2014-12-23 00:00:00 +0000",     
         'PabloMansanet/MatthewArchipelago':"2014-11-10 00:00:00 +0000",      # too old
         'PabloMansanet/NaboombuArchipelago': "2014-11-30 00:00:00 +0000",    # younger, should be ok   
         'PabloMansanet/NowhereArchipelago': "2014-11-23 00:00:00 +0000",     
         'andreaskartg/DwayneTheRockJohnsonArchipelago': "2014-11-23 00:00:00 +0000",     
         'andreaskartg/ScillyArchipelago': "2014-11-27 00:00:00 +0000",       # younger, should be ok
         'andreaskartg/TheWorldArchipelago': "2014-11-23 00:00:00 +0000",     
         'jameshollow/AlcatrazArchipelago': "2014-11-23 00:00:00 +0000",     
         'laurent/NaboombuArchipelago': "2014-11-23 00:00:00 +0000",     
         'mdodkins/MatthewArchipelago': "2014-11-03 00:00:00 +0000",          # too old, 
         'mdodkins/NaboombuArchipelago': "2014-11-1 00:00:00 +0000",          # too old, day
         'mdodkins/TheLampArchipelago': "2014-11-23 00:00:00 +0000",     
         'origin/MrBobArchipelago': "2014-11-23 00:00:00 +0000",     
         'rich/CollidingArchipelagoes': "2014-11-23 00:00:00 +0000",     
         'rich/ScillyArchipelago': "2014-11-23 00:00:00 +0000",     
         'rich/TheWorldArchipelago': "2014-11-23 00:00:00 +0000",     
         'rich/ZanzibarArchipelago': "2015-11-15 00:00:00 +0000",             # younger, should be ok
         'yspeake/CanaryArchipelago': "2014-11-23 00:00:00 +0000",     
         'yspeake/ScillyArchipelago': "2012-11-23 00:00:00 +0000",            # too old, year
         'yspeake/TheLampArchipelago': "2014-11-23 00:00:00 +0000",     
         'yspeake/YansArchipelago': "2014-11-12 00:00:00 +0000"               # too old, day
      }                    

      # When
      releaseArchipelago = "mdodkins/Development"
      amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent = 1
      actual = Git.getArchipelagoNamesRecentlyActiveInBranch(releaseArchipelago, amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)

      # Then
      self.assertTrue(actual)
      self.assertTrue(isinstance(actual, list))
      expected = [
         'DenzilPearce/TuvaluArchipelago', 
         'PabloMansanet/NaboombuArchipelago',
         'PabloMansanet/NowhereArchipelago', 
         'andreaskartg/DwayneTheRockJohnsonArchipelago', 
         'andreaskartg/ScillyArchipelago', 
         'andreaskartg/TheWorldArchipelago', 
         'jameshollow/AlcatrazArchipelago', 
         'laurent/NaboombuArchipelago', 
         'mdodkins/TheLampArchipelago', 
         'origin/MrBobArchipelago', 
         'rich/CollidingArchipelagoes', 
         'rich/ScillyArchipelago', 
         'rich/TheWorldArchipelago', 
         'rich/ZanzibarArchipelago',
         'yspeake/CanaryArchipelago', 
         'yspeake/TheLampArchipelago', 
      ]
      self.assertEqual(expected, actual)

   @patch('Git.getArchipelagoNames', MagicMock(return_value=False))
   @patch('Git.getLastMergeDateAndTimeString', MagicMock(return_value="""2014-11-23 15:28:14 +0000"""))
   @patch('datetime.datetime', MockDateTime)
   def test_getArchipelagoNamesRecentlyActiveInBranch_checks_output_of_getArchipelagoNames(self):
      # Given
      # the same day as Git.getLastMergeDateAndTimeString
      MockDateTime.nowValue = datetime.datetime(2014, 11, 23, 15, 28, 14)

      # When
      releaseArchipelago = "mdodkins/Development"
      amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent = 1000
      actual = Git.getArchipelagoNamesRecentlyActiveInBranch(releaseArchipelago, amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)

      # Then
      self.assertFalse(actual)

   @patch('Git.getArchipelagoNames', MagicMock(return_value=["jim/TheAmazingArchipelagoOfFunAndWonder", "origin/MrBobArchipelago"]))
   @patch('Git.getLastMergeDateAndTimeString', MockGetLastMergeDateAndTimeString.run)
   @patch('datetime.datetime', MockDateTime)
   def test_getArchipelagoNamesRecentlyActiveInBranch_checks_output_of_getLastMergeDateAndTimeString(self):
      # Given
      MockDateTime.nowValue = datetime.datetime(2014, 11, 23, 15, 28, 14)
      MockGetLastMergeDateAndTimeString.returnValues = {
         "jim/TheAmazingArchipelagoOfFunAndWonder": False,
         "origin/MrBobArchipelago": "2014-11-23 15:28:14 +0000"
      }

      # When
      releaseArchipelago = "mdodkins/Development"
      amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent = 1000
      actual = Git.getArchipelagoNamesRecentlyActiveInBranch(releaseArchipelago, amountOfTimeInDaysBeforeBranchIsTooOldToBeConsideredRecent)

      # Then
      expected = ["origin/MrBobArchipelago"]
      self.assertEqual(expected, actual)

if __name__ == '__main__':
    unittest.main()