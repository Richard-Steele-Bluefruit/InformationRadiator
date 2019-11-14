import unittest
from mock import patch, MagicMock

from projectpaths import *
import TimeFormatter

class TestTimeFormatter(unittest.TestCase):

   def test_parses_ISO8601_string_into_datetime(self):
      # Given
      iso8601String = "2020-05-12 21:52:17 +0100"

      # When
      actual = TimeFormatter.convertDateTimeFromGitISO8601Format(iso8601String)

      # Then
      expectedYear = 2020
      self.assertEqual(expectedYear, actual.year)
      expectedMonth = 5
      self.assertEqual(expectedMonth, actual.month)
      expectedDay = 12
      self.assertEqual(expectedDay, actual.day)
      expectedHour = 21
      self.assertEqual(expectedHour, actual.hour)
      expectedMinute = 52
      self.assertEqual(expectedMinute, actual.minute)
      expectedSecond = 17
      self.assertEqual(expectedSecond, actual.second)

if __name__ == '__main__':
    unittest.main()