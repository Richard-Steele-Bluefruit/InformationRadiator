"""
Compare last commit time with current archipelago last commit time
Compare last commit time with time of last merge with archipelago 

TimeDistance.distance does NOT return a time in hours or minutes, etc, it's in its own arbitrary metric

Metric is:
lastCommitDayDifference + lastMergeDayDifference
For every hour since merging, add
If we've spent a long time since merging, add some extra
"""

import datetime
import Git
import TimeFormatter

class TimeDistance:

   @property
   def Distance(self):
      return self._distance

   @property
   def LastMergeDate(self):
      return self._lastMergeDate

   def __init__(self, archipelagoName, branchName, longTimeSinceMergeWithArchipelagoModifier, branchIsNotUpToDateWithArchipelagoModifier):
      self._distance = 0.0
      self.timeSinceLastCommit = None
      self.overTwoWeeksSinceLastMerge = False
      self.__minutesSinceLastMergeWithArchipelago = 0
      self.__longTimeSinceMergeWithArchipelagoModifier = 0
      self.__branchIsNotUpToDateWithArchipelagoModifier = 0
      self._lastMergeDate = None

      self.__longTimeSinceMergeWithArchipelagoModifier = longTimeSinceMergeWithArchipelagoModifier
      self.__branchIsNotUpToDateWithArchipelagoModifier = branchIsNotUpToDateWithArchipelagoModifier

      lastCommitDayDifference = self.__compareLastCommitTimeWithArchipelagoLastCommitTime(archipelagoName, branchName)
      lastMergeDayDifference = self.__compareLastCommitTimeWithTimeOfLastMergeWithArchipelago(archipelagoName, branchName)
      
      self._distance = self.__calculateDistance(lastCommitDayDifference, lastMergeDayDifference)

   def __compareLastCommitTimeWithArchipelagoLastCommitTime(self, archipelagoName, branchName):
      differenceTimedelta = self.__getLastCommitTimeDifferenceAsTimedelta(archipelagoName, branchName)
      self.timeSinceLastCommit = differenceTimedelta
      
      difference = differenceTimedelta.total_seconds()
      
      oneHour = 60 * 60
      oneDay = oneHour * 24
      
      # every day is worth one point
      dayDifference = float(difference / oneDay)
      return dayDifference

   def __getLastCommitTimeDifferenceAsTimedelta(self, archipelagoCommit, branchCommit):
      archipelagoLastCommitDatetime = self.__getLastCommitDateTimeInArchipelago(archipelagoCommit)
      differenceTimedelta = self.__getCommitTimeDifferenceAsTimedelta(archipelagoLastCommitDatetime, branchCommit)
      return differenceTimedelta

   def __getLastCommitDateTimeInArchipelago(self, commit):
      lastCommitDateTimeString = Git.getLastCommitDateAndTimeString(commit)
      return TimeFormatter.convertDateTimeFromGitISO8601Format(lastCommitDateTimeString)
      
   def __compareLastCommitTimeWithTimeOfLastMergeWithArchipelago(self, archipelagoName, branchName):
      differenceTimedelta = self.__getLastMergeCommitTimeDifferenceAsTimedelta(archipelagoName, branchName)
      difference = differenceTimedelta.total_seconds()
      
      self.__minutesSinceLastMergeWithArchipelago = int(difference / 60)
      
      oneHour = 60 * 60
      oneDay = oneHour * 24

      # every day is worth one point
      differenceInDays = float(difference / oneDay)

      twoWeeksInDays = 14
      if differenceInDays >= twoWeeksInDays:
        self.overTwoWeeksSinceLastMerge = True
      
      return differenceInDays

   def __getLastMergeCommitTimeDifferenceAsTimedelta(self, archipelagoCommit, branchName):
      archipelagoLastCommitDatetime = self.__getLastMergeDateTimeWithArchipelago(archipelagoCommit, branchName)
      self._lastMergeDate = str(archipelagoLastCommitDatetime)
      differenceTimedelta = self.__getCommitTimeDifferenceAsTimedelta(archipelagoLastCommitDatetime, branchName)
      return differenceTimedelta

   def __getLastMergeDateTimeWithArchipelago(self, archipelagoCommit, branchName):
      lastCommitDateTimeString = Git.getLastMergeDateAndTimeString(archipelagoCommit, branchName)
      return TimeFormatter.convertDateTimeFromGitISO8601Format(lastCommitDateTimeString)

   def __getCommitTimeDifferenceAsTimedelta(self, archipelagoLastCommitDatetime, branchCommit):
      branchLastCommitDatetime = self.__getLastCommitDateTimeInBranch(branchCommit)
      
      differenceAsTimedelta = None
      if archipelagoLastCommitDatetime > branchLastCommitDatetime:
         differenceAsTimedelta = archipelagoLastCommitDatetime - branchLastCommitDatetime
      else:
         # need to consider archipelago that has had other things merged first
         differenceAsTimedelta = branchLastCommitDatetime - archipelagoLastCommitDatetime
      
      return differenceAsTimedelta

   def __getLastCommitDateTimeInBranch(self, branch):
      lastCommitDateTimeString = Git.getLastCommitDateAndTimeString(branch)
      return TimeFormatter.convertDateTimeFromGitISO8601Format(lastCommitDateTimeString)

   def __calculateDistance(self, lastCommitDayDifference, lastMergeDayDifference):
      distance = 0
      # potentially tiny (might be less than 1 if the difference is less than a day) but 
      #  important to keep the branch from appearing to be at the same place at the 
      #  archipelago (if everything else is equal)
      commitDistance = lastCommitDayDifference + lastMergeDayDifference
      distance += commitDistance
      
      # more than two weeks is a long time, add modifier
      if self.overTwoWeeksSinceLastMerge:
         distance += self.__longTimeSinceMergeWithArchipelagoModifier

      hoursSinceLastMergeWithArchipelago = self.__minutesSinceLastMergeWithArchipelago / 60
      upToDateDistance = hoursSinceLastMergeWithArchipelago * self.__branchIsNotUpToDateWithArchipelagoModifier
      distance += upToDateDistance
      
      return distance