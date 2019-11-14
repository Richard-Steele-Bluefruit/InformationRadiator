from datetime import datetime

def convertDateTimeFromGitISO8601Format(gitDateTimeString):
   listOfDateAndTime = gitDateTimeString.split(' ')

   date = listOfDateAndTime[0]
   time = listOfDateAndTime[1]
   
   listOfDateComponents = date.split('-')
   listOfTimeComponents = time.split(':')
   
   dt = datetime(int(listOfDateComponents[0]), # year
                 int(listOfDateComponents[1]), # month
                 int(listOfDateComponents[2]), # day
                 int(listOfTimeComponents[0]), # hour
                 int(listOfTimeComponents[1]), # minute
                 int(listOfTimeComponents[2])) # second
   return dt