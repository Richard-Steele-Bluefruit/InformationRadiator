import unittest
from mock import patch, MagicMock
import tempfile

from projectpaths import *
import MergeConflictDetector

class TestMergeConflictDetector_ConflitFileTestData:

   single_conflict = \
"""
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 .gitignore
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 .gitignore
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 .gitignore
@@ -4,7 +4,11 @@

 TestResults
 *.xcuserstate
+<<<<<<< .our
 *.deppedy-doo-dah
+=======
+*.what a wonderful day
+>>>>>>> .their
 *.tmp
 *Results.xml
"""

   multiple_conflicts = \
"""
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 .gitignore
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 .gitignore
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 .gitignore
@@ -4,7 +4,11 @@

 TestResults
 *.xcuserstate
+<<<<<<< .our
 *.deppedy-doo-dah
+=======
+*.what a wonderful day
+>>>>>>> .their
 *.tmp
 *Results.xml
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 /somewhere/some/amazing/file
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 /somewhere/some/amazing/file
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 /somewhere/some/amazing/file
@@ -21,7 +21,11 @@
some unchanged data
goodness gracious, some data
+<<<<<<< .our
-some line that was taken away
+some line that replaced the line that was taken away
+=======
-some line that was taken away
+a line which also wished to replace that other line
+>>>>>>> .their
a couple of lines that just
happened to come after the conflict
"""

   multiple_conflicts_with_a_automatic_merge_in_the_middle = \
"""
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 .gitignore
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 .gitignore
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 .gitignore
@@ -4,7 +4,11 @@

 TestResults
 *.xcuserstate
+<<<<<<< .our
 *.deppedy-doo-dah
+=======
+*.what a wonderful day
+>>>>>>> .their
 *.tmp
 *Results.xml
merged
  result 100644 27501efbe295e4908c69c2ee09e42b2fe6c55884 BuildServer/Quickbuild/EmbeddedProjects.msbuildproj
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 BuildServer/Quickbuild/EmbeddedProjects.msbuildproj
@@ -13,7 +13,7 @@
 	</ItemGroup>
 
 	<Target Name="Build">
-		<MSBuild Projects="@(ProjectDependency)" Targets="Build All" BuildInParallel="false" Properties="PathToMainFolder=$(PathToMainFolder);PathToLibraries=$(PathToLibraries)"/>
+		<MSBuild Projects="@(ProjectDependency)" Targets="Build All" BuildInParallel="true" Properties="PathToMainFolder=$(PathToMainFolder);PathToLibraries=$(PathToLibraries)"/>
 	</Target>
   
 </Project>
\ No newline at end of file
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 /somewhere/some/amazing/file
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 /somewhere/some/amazing/file
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 /somewhere/some/amazing/file
@@ -21,7 +21,11 @@
some unchanged data
goodness gracious, some data
+<<<<<<< .our
-some line that was taken away
+some line that replaced the line that was taken away
+=======
-some line that was taken away
+a line which also wished to replace that other line
+>>>>>>> .their
a couple of lines that just
happened to come after the conflict
"""

   multiple_conflicts_in_the_same_file = \
"""
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 /somewhere/some/amazing/file
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 /somewhere/some/amazing/file
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 /somewhere/some/amazing/file
@@ -4,7 +4,11 @@

 TestResults
 *.xcuserstate
+<<<<<<< .our
 *.deppedy-doo-dah
+=======
+*.what a wonderful day
+>>>>>>> .their
 *.tmp
 *Results.xml
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 /somewhere/some/amazing/file
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 /somewhere/some/amazing/file
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 /somewhere/some/amazing/file
@@ -21,7 +21,11 @@
some unchanged data
goodness gracious, some data
+<<<<<<< .our
-some line that was taken away
+some line that replaced the line that was taken away
+=======
-some line that was taken away
+a line which also wished to replace that other line
+>>>>>>> .their
a couple of lines that just
happened to come after the conflict
"""

   no_conflict_one_merge = \
"""
merged
  result 100644 27501efbe295e4908c69c2ee09e42b2fe6c55884 BuildServer/Quickbuild/EmbeddedProjects.msbuildproj
  our    100644 7170bff724d9236cf60d418500181b2aebefca4a BuildServer/Quickbuild/EmbeddedProjects.msbuildproj
@@ -13,7 +13,7 @@
 	</ItemGroup>
 
 	<Target Name="Build">
-		<MSBuild Projects="@(ProjectDependency)" Targets="Build All" BuildInParallel="false" Properties="PathToMainFolder=$(PathToMainFolder);PathToLibraries=$(PathToLibraries)"/>
+		<MSBuild Projects="@(ProjectDependency)" Targets="Build All" BuildInParallel="true" Properties="PathToMainFolder=$(PathToMainFolder);PathToLibraries=$(PathToLibraries)"/>
 	</Target>
   
 </Project>
\ No newline at end of file

"""

   no_conflict_multiple_merges = \
"""
merged
  result 100644 27501efbe295e4908c69c2ee09e42b2fe6c55884 BuildServer/Quickbuild/EmbeddedProjects.msbuildproj
  our    100644 7170bff724d9236cf60d418500181b2aebefca4a BuildServer/Quickbuild/EmbeddedProjects.msbuildproj
@@ -13,7 +13,7 @@
 	</ItemGroup>
 
 	<Target Name="Build">
-		<MSBuild Projects="@(ProjectDependency)" Targets="Build All" BuildInParallel="false" Properties="PathToMainFolder=$(PathToMainFolder);PathToLibraries=$(PathToLibraries)"/>
+		<MSBuild Projects="@(ProjectDependency)" Targets="Build All" BuildInParallel="true" Properties="PathToMainFolder=$(PathToMainFolder);PathToLibraries=$(PathToLibraries)"/>
 	</Target>
   
 </Project>
\ No newline at end of file
merged
  result 100644 4ef29077f41d3db47f0869ae233e52650d0ef2b3 COMPONENTS/CLI/PROJECT_ARM/CLI.ewp
  our    100644 ac10141092f54853daa1e6994f32aac4829945f2 COMPONENTS/CLI/PROJECT_ARM/CLI.ewp
@@ -2027,7 +2027,7 @@
         <name>$PROJ_DIR$\..\Source\Commands\General\CLICommandName.h</name>
       </file>
       <file>
-        <name>$PROJ_DIR$\..\Source\Commands\General\CliFileReceiver.cpp</name>
+        <name>$PROJ_DIR$\..\Source\Commands\General\CLIFileReceiver.cpp</name>
       </file>
       <file>
         <name>$PROJ_DIR$\..\Source\Commands\General\DebugPrintToggleCommand.cpp</name>
@@ -2069,6 +2069,9 @@
         <name>$PROJ_DIR$\..\Source\Commands\General\RaiseEventCommand.cpp</name>
       </file>
       <file>
+        <name>$PROJ_DIR$\..\Source\Commands\General\RecvCommand.cpp</name>
+      </file>
+      <file>
         <name>$PROJ_DIR$\..\Source\Commands\General\RfStackCommand.cpp</name>
       </file>
       <file>
@@ -2092,6 +2095,9 @@
       <file>
         <name>$PROJ_DIR$\..\Source\Commands\General\XModemFileReceiver.cpp</name>
       </file>
+      <file>
+        <name>$PROJ_DIR$\..\Source\Commands\General\XModemFileSender.cpp</name>
+      </file>
     </group>
     <group>
       <name>Properties</name>
merged
  result 100644 8e4f0f02325e6930745d56a4826025644f2e3ea9 COMPONENTS/CLI/Source/Commands/General/CLICommandName.h
  our    100644 53bf44f868cc964652176e7fc9dd90d71521567a COMPONENTS/CLI/Source/Commands/General/CLICommandName.h
@@ -6,6 +6,7 @@
    enum Value
    {      
       Send = 0,
+      Recv,
       Format,
       Type,
       PrintDirectory,
merged
  result 100644 6299472a39e04b77e0f20f1d1fdfd24d8420f700 COMPONENTS/CLI/Source/Commands/General/CliFileReceiver.cpp
  our    100644 8a5a2d837ca9c85f8816e22ef45eff4950ec77db COMPONENTS/CLI/Source/Commands/General/CliFileReceiver.cpp
@@ -1,7 +1,7 @@
 #include <stdio.h>
 
 #include "CLIFileReceiver.h"
-#include "XModemResult.h"
+#include "XModemSendResult.h"

"""

   automatic_merge_with_changed_in_both = \
"""
changed in both
  base   100644 09de3626906ca9371bf63bf1f00d21011edd1ee9 .gitignore
  our    100644 2b982d826d8df49c2a909f73888df934d2bc11b1 .gitignore
  their  100644 dad56c0fbb6a9fa999ec298dd1d86ed787eacf04 .gitignore
@@ -4,7 +4,11 @@

 TestResults
 *.xcuserstate
 *.deppedy-doo-dah
+*.what a wonderful day
 *.tmp
 *Results.xml
"""

   automatic_merge_with_two_changed_in_both_next_to_each_other = \
"""
changed in both
  base   100644 ce72c55000e56c1710c881de57f6a9ca7e8bbc46 BuildServer/Quickbuild/UnitTests.sln
  our    100644 7d217772c494c8ec8ad132ca32c1fbdd3194276b BuildServer/Quickbuild/UnitTests.sln
  their  100644 a294e6b66dd2575797dcfb860634d9c7c9281251 BuildServer/Quickbuild/UnitTests.sln
changed in both
  base   100644 b1c63f7d34bf1f57c518cf1b4c6dda383c817010 COMPONENTS/CLI/Source/Commands/General/XModemFileReceiver.cpp
  our    100644 6bbe802475b9796b7012dc7f65d3c62bf46ab175 COMPONENTS/CLI/Source/Commands/General/XModemFileReceiver.cpp
  their  100644 562bed3b90f462ccf19046886a7dfe1ce4f85a0a COMPONENTS/CLI/Source/Commands/General/XModemFileReceiver.cpp
@@ -2,21 +2,12 @@
 #include "RtosInterface.h"
 #include "Logger.h"
 #include "CLICommand.h"
-
-XModemFileReceiver* XModemFileReceiver::m_xModemFileReceiver = 0;
-
-static void TransferTimeoutCallback(void*)
-{
-   if (XModemFileReceiver::m_xModemFileReceiver)
-      XModemFileReceiver::m_xModemFileReceiver->TransferTimeout();
-}
+#include "boost/bind.hpp"
 
 XModemFileReceiver::XModemFileReceiver(SerialPortDriverInterface &serialPortDriver,
-                                       RtosTimerInterface &rtosTimer,
                                        SerialInputRedirectorInterface &serialInputRedirector) :
    // injections
    m_serialPortDriver(serialPortDriver),
-   m_rtosTimer(rtosTimer),
    m_serialInputRedirector(serialInputRedirector),
    // members
    m_packetHandler(0),
@@ -32,13 +23,17 @@
    m_logPacketsStarted(0),
    m_logPacketsCorrect(0),
    m_logUnexpectedBytes(0),
-   m_paddingBytes(0)
+   m_paddingBytes(0),
+   m_transferTimer(boost::bind(&XModemFileReceiver::TransferTimeout, this),
+                   XModemFileReceiver::TransferTimeoutPeriodInMs,
+                   false,
+                   "XModemTransferTimer")
 {
-   m_xModemFileReceiver = this;
 }
 
 bool XModemFileReceiver::Start()
 {
+<<<<<<< .our
    if (!CreateTransferTimer())
       return false;
 
@@ -48,6 +43,8 @@
    }
    DebugLoggerFlags::SetAllDebugFlags(false);    //Reset all debug flags to prevent any output
    
+=======
+>>>>>>> .their
    m_serialInputRedirector.RedirectTo(this);
    m_logPacketsStarted = 0;
    m_logPacketsCorrect = 0;
@@ -58,21 +55,7 @@
 
    // send NAK to start transfer
    m_serialPortDriver.PutCharAndWaitForever(NAK);
-   m_rtosTimer.Start();
-   return true;
-}
"""

class helper_ToWriteIntoMergeConflictTempFile(object):
   
   data = None

   @classmethod
   def reset(theClass):
      theClass.data = None

def helper_PumpDataToTemporaryFile(archipelagoName, branchName, mergeConflictTempFile):
      mergeConflictTempFile.seek(0)
      mergeConflictTempFile.write(helper_ToWriteIntoMergeConflictTempFile.data)
      mergeConflictTempFile.seek(0)

@patch('Git.getMergeConflictsAndSaveToFile', helper_PumpDataToTemporaryFile)
class TestMergeConflictDetector_ConflictFile(unittest.TestCase):

   def setUp(self):
      helper_ToWriteIntoMergeConflictTempFile.reset()
      self.maxDiff = None  # when it fails, we are going to have large strings sometimes

   def test_single_conflict(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.single_conflict
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [""" *.deppedy-doo-dah"""],
         'theirCode': ["""+*.what a wonderful day"""],
         'file': [""".gitignore"""],
         'line': [7] # git gives 4 in-between the @...@ but that is actually 3 lines above the conflict
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)
   
   def test_multiple_conflicts(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.multiple_conflicts
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [""" *.deppedy-doo-dah""", """-some line that was taken away\n+some line that replaced the line that was taken away"""],
         'theirCode': ["""+*.what a wonderful day""", """-some line that was taken away\n+a line which also wished to replace that other line"""],
         'file': [""".gitignore""", """/somewhere/some/amazing/file"""],
         'line': [7, 23]
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)
   
   def test_multiple_conflicts_with_an_automatic_merge_in_the_middle(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.multiple_conflicts_with_a_automatic_merge_in_the_middle
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [""" *.deppedy-doo-dah""", """-some line that was taken away\n+some line that replaced the line that was taken away"""],
         'theirCode': ["""+*.what a wonderful day""", """-some line that was taken away\n+a line which also wished to replace that other line"""],
         'file': [""".gitignore""", """/somewhere/some/amazing/file"""],
         'line': [7, 23]
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)
   
   def test_multiple_conflicts_in_the_same_file(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.multiple_conflicts_in_the_same_file
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [""" *.deppedy-doo-dah""", """-some line that was taken away\n+some line that replaced the line that was taken away"""],
         'theirCode': ["""+*.what a wonderful day""", """-some line that was taken away\n+a line which also wished to replace that other line"""],
         'file': ["""/somewhere/some/amazing/file""", """/somewhere/some/amazing/file"""],
         'line': [7, 23]
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)
   
   def test_no_conflicts_only_one_automatic_merge(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.no_conflict_one_merge
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [],
         'theirCode': [],
         'file': [],
         'line': []
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)
   
   def test_no_conflicts_with_multiple_automatic_merges(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.no_conflict_multiple_merges
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [],
         'theirCode': [],
         'file': [],
         'line': []
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)
   

   def test_automatic_merge_with_identifier_saying_that_there_are_changes_in_both_branches(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.automatic_merge_with_changed_in_both
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [],
         'theirCode': [],
         'file': [],
         'line': []
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)
   
   def test_automatic_merge_with_identifier_saying_that_there_are_changes_in_both_branches(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.automatic_merge_with_changed_in_both
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [],
         'theirCode': [],
         'file': [],
         'line': []
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)


   def test_automatic_merge_with_two_identifiers_saying_that_there_are_changes_in_both_branches_next_to_each_other(self):
      # Given
      helper_ToWriteIntoMergeConflictTempFile.data = TestMergeConflictDetector_ConflitFileTestData.automatic_merge_with_two_changed_in_both_next_to_each_other
   
      # When
      mergeConflictDetector = MergeConflictDetector.MergeConflictDetector("releasearchipelago", "somebranch")
   
      # Then
      expectedConflictData = {
         'ourCode': [
"""    if (!CreateTransferTimer())
       return false;
 
@@ -48,6 +43,8 @@
    }
    DebugLoggerFlags::SetAllDebugFlags(false);    //Reset all debug flags to prevent any output
    """
         ],
         'theirCode': [''],
         'file': ['COMPONENTS/CLI/Source/Commands/General/XModemFileReceiver.cpp'],
         'line': [47]
      }
   
      self.assertEqual(expectedConflictData, mergeConflictDetector.ConflictData)

if __name__ == '__main__':
    unittest.main()