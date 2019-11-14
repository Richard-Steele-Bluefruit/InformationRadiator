import unittest
from mock import patch, MagicMock

from projectpaths import *
import ConfigurationFile
import Github
import github as PyGithub

class MockPullRequestBranch(object):

   @property
   def head(self):
      return self # test slime/laziness

   def __init__(self, name):
      self.label = name
      self.ref = name

class MockPullRequest(object):

   def __init__(self, nameOfWhereThisIsBeingPullRequestedTo, nameOfpullRequestedBranch):
      self.base = MockPullRequestBranch(nameOfWhereThisIsBeingPullRequestedTo)
      self.head = MockPullRequestBranch(nameOfpullRequestedBranch)

class MockRepositoryOwner(object):

   def __init__(self, name):
      self.login = name

class MockRepository(object):

   pullRequestTupleList = []

   @classmethod
   def reset(theClass):
      pullRequestTupleList = []

   def __init__(self, name, forks = []):
      self.name = name  # used in GithubWrapper::init()
      self.full_name = name   # used in GithubWrapper::getPullRequests(...)
      self.forks = forks
      self.owner = MockRepositoryOwner(name.split('/')[0])

   def get_forks(self):
      return self.forks

   def get_pulls(self):
      return [MockPullRequest(tuple[0], tuple[1]) for tuple in MockRepository.pullRequestTupleList]

class MockOrganisation(object):

   def __init__(self, name, repositoryList):
      self.name = name
      self.repositoryList = repositoryList

   def get_repos(self):
      return self.repositoryList

class MockUser(object):

   organisationMockObjectsList = []

   @classmethod
   def reset(theClass):
      theClass.organisationMockObjectsList = []

   def get_orgs(theClass):
      return theClass.organisationMockObjectsList

class MockPyGithub(object):

   username = None
   password = None
   expectedPassword = None
   expectedUsername = None

   @classmethod
   def reset(theClass):
      theClass.username = None
      theClass.password = None
      theClass.expectedPassword = None
      theClass.expectedUsername = None

   def __init__(self, username, password):

      incorrectPassword = (password != MockPyGithub.expectedPassword)
      incorrectUsername = (username != MockPyGithub.expectedUsername)
      if incorrectPassword or incorrectUsername:
         status = "status"
         data = "data"
         raise PyGithub.BadCredentialsException(status, data)

      MockPyGithub.username = username
      MockPyGithub.password = password

   def get_user(self):
      return MockUser()

@patch("github.Github", MockPyGithub)
class TestGithub(unittest.TestCase):

   def setUp(self):
      MockPyGithub.reset()
      MockUser.reset()

      ConfigurationFile.githubUserName = None
      ConfigurationFile.repositoryName = None

      Github.GithubWrapper._github = None
      Github.GithubWrapper._repository = None

   def test_init_does_not_create_PyGithub_object_if_no_password_is_given(self):
      # Given
      ConfigurationFile.githubUsername = "username"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn")
         ])
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "username"
   
      # When
      password = ""
      Github.GithubWrapper.init(password)
   
      # Then
      self.assertEqual(None, Github.GithubWrapper._github)
      self.assertEqual(None, Github.GithubWrapper._repository)

   def test_init_connects_to_github_and_sets_up_repository(self):
      # Given
      ConfigurationFile.githubUsername = "username"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn")
         ])
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "username"
   
      # When
      password = "password"
      Github.GithubWrapper.init(password)
   
      # Then
      self.assertNotEqual(None, Github.GithubWrapper._github)
      self.assertNotEqual(None, Github.GithubWrapper._repository)
   
      self.assertEqual("username", MockPyGithub.username)
      self.assertEqual("password", MockPyGithub.password)
   
   def test_incorrect_password_does_not_initialise_github(self):
      # Given
      ConfigurationFile.githubUsername = "username"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn")
         ])
      ]
      MockPyGithub.expectedPassword = "the correct password"
      MockPyGithub.expectedUsername = "username"
   
      # When
      password = "the incorrect password"
      Github.GithubWrapper.init(password)
   
      # Then
      self.assertEqual(None, Github.GithubWrapper._github)
      self.assertEqual(None, Github.GithubWrapper._repository)
   
   def test_incorrect_user_does_not_initialise_github(self):
      # Given
      ConfigurationFile.githubUsername = "an incorrect username"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn")
         ])
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "the correct username"
   
      # When
      password = "password"
      Github.GithubWrapper.init(password)
   
      # Then
      self.assertEqual(None, Github.GithubWrapper._github)
      self.assertEqual(None, Github.GithubWrapper._repository)
   
   def test_repository_not_set_up_if_there_are_no_matching_repositories_on_github(self):
      # Given
      ConfigurationFile.githubUsername = "username"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("Project1"), 
            MockRepository("Project2")
         ])
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "username"
   
      # When
      password = "password"
      Github.GithubWrapper.init(password)
   
      # Then
      self.assertNotEqual(None, Github.GithubWrapper._github)
      self.assertEqual(None, Github.GithubWrapper._repository)
   
   @patch("Git.getURLForRemote", MagicMock(return_value="git@github.com:Dominic-Roberts/ProjectWeAreInterestedIn.git"))
   def test_get_pull_requests_for_archipelago(self):
      # Given
      ConfigurationFile.githubUsername = "Dominic-Roberts"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn", [
               MockRepository("Dominic-Roberts/ProjectWeAreInterestedIn"), 
               MockRepository("some-guy/ProjectWeAreInterestedIn"), 
               MockRepository("some-being/ProjectWeAreInterestedIn")
            ]),
            MockRepository("some-other-project", [
               MockRepository("Dominic-Roberts/some-other-project"), 
            ]),
            MockRepository("some-unrelated-project", [
               MockRepository("Dominic-Roberts/some-unrelated-project")
            ])
         ])
      ]
      MockRepository.pullRequestTupleList = [
         ("some-branch", "pull-request-name1"),
         ("Development", "pull-request-name2"),
         ("JimArchipelago", "MIS-123 that pull request that changed everything"),
         ("JimArchipelago", "MIS-527 just a few changes"),
         ("Development", "pull-request-name3"),
         ("JimArchipelago", "MIG-27 fix for world hunger"),
         ("Development", "pull-request-name4")
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "Dominic-Roberts"
   
      password = "password"
      Github.GithubWrapper.init(password)
      
      # When
      localRemoteWithArchipelagoName = "dom/JimArchipelago"
      actual = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      expected = [
         "MIS-123 that pull request that changed everything",
         "MIS-527 just a few changes",
         "MIG-27 fix for world hunger"
      ]
      self.assertEqual(expected, actual)

   @patch("Git.getURLForRemote", MagicMock(return_value="git@github.com:Dominic-Roberts/ProjectWeAreInterestedIn.git"))
   def test_get_pull_requests_returns_empty_list_if_no_pull_requests_to_archipelago(self):
      # Given
      ConfigurationFile.githubUsername = "Dominic-Roberts"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn", [
               MockRepository("Dominic-Roberts/ProjectWeAreInterestedIn"), 
               MockRepository("some-guy/ProjectWeAreInterestedIn"), 
               MockRepository("some-being/ProjectWeAreInterestedIn")
            ]),
            MockRepository("some-other-project", [
               MockRepository("Dominic-Roberts/some-other-project"), 
            ]),
            MockRepository("some-unrelated-project", [
               MockRepository("Dominic-Roberts/some-unrelated-project")
            ])
         ])
      ]
      MockRepository.pullRequestTupleList = [   # none for "JimArchipelago"
         ("some-branch", "pull-request-name1"),
         ("Development", "pull-request-name2"),
         ("Development", "pull-request-name3"),
         ("Development", "pull-request-name4")
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "Dominic-Roberts"
   
      password = "password"
      Github.GithubWrapper.init(password)
      
      # When
      localRemoteWithArchipelagoName = "dom/JimArchipelago"
      actual = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      expected = []
      self.assertEqual(expected, actual)

   @patch("Git.getURLForRemote", MagicMock(return_value=False))
   def test_get_pull_requests_returns_false_if_there_is_no_remote_with_that_name(self):
      # Given
      ConfigurationFile.githubUsername = "Dominic-Roberts"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn")
         ])
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "Dominic-Roberts"
   
      password = "password"
      Github.GithubWrapper.init(password)
      
      # When
      localRemoteWithArchipelagoName = "dom/JimArchipelago"
      actual = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      self.assertFalse(actual)

   @patch("Git.getURLForRemote", MagicMock(return_value="git@github.com:Dominic-Roberts/ProjectWeAreInterestedIn.git"))
   def test_get_pull_requests_returns_false_if_incorrect_configuration_file_is_used(self):
      # Given
      ConfigurationFile.githubUsername = "Dominic-Roberts"
      ConfigurationFile.repositoryName = "IncorrectRepositoryName"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn"),
            MockRepository("some-other-project")
         ])
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "Dominic-Roberts"
   
      password = "password"
      Github.GithubWrapper.init(password)
      
      # When
      localRemoteWithArchipelagoName = "dom/JimArchipelago"
      actual = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      self.assertFalse(actual)
   
   def test_cannot_get_pull_requests_if_failed_to_set_up_internal_github_object(self):
      # Given
      ConfigurationFile.githubUsername = "Dominic-Roberts"
      ConfigurationFile.repositoryName = "IncorrectRepositoryName"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn"),
            MockRepository("some-other-project")
         ])
      ]
      MockPyGithub.expectedPassword = "incorrect password"
      MockPyGithub.expectedUsername = "Dominic-Roberts"
   
      password = "password"
      Github.GithubWrapper.init(password)
      self.assertEqual(None, Github.GithubWrapper._github)
      self.assertEqual(None, Github.GithubWrapper._repository)
      
      # When
      localRemoteWithArchipelagoName = "dom/JimArchipelago"
      actual = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      self.assertFalse(actual)

   def test_cannot_get_pull_requests_if_there_are_no_organisations_with_the_right_repository(self):
      # Given
      ConfigurationFile.githubUsername = "Dominic-Roberts"
      ConfigurationFile.repositoryName = "repository not in any organisation"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("incredi-bad project"), 
            MockRepository("some-other-project")
         ]),
         MockOrganisation("some-open-source_organisation", [
            MockRepository("free stuff yay")
         ]),
         MockOrganisation("wait, you work for two companies?!", [
            MockRepository("pokemon"), 
            MockRepository("project2")
         ])
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "Dominic-Roberts"
   
      password = "password"
      Github.GithubWrapper.init(password)
      self.assertEqual(None, Github.GithubWrapper._repository)
      
      # When
      localRemoteWithArchipelagoName = "dom/JimArchipelago"
      actualPullRequests = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      self.assertFalse(actualPullRequests)

   @patch("Git.getURLForRemote", MagicMock(return_value="git@github.com:JamesHollow/ProjectWeAreInterestedIn.git"))
   def test_able_to_get_forks_that_are_not_forked_from_the_top_repository(self):
      # Given
      ConfigurationFile.githubUsername = "mdodkins"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn",
               [
                  MockRepository("JamesHollow/ProjectWeAreInterestedIn"),
                  MockRepository("PabloMansanet/ProjectWeAreInterestedIn"), 
                  MockRepository("Laurent-AbSw/ProjectWeAreInterestedIn")
               ]), 
            MockRepository("some-other-project")
         ])
      ]
      MockRepository.pullRequestTupleList = [
         ("some-branch", "pull-request-name1"),
         ("AlcatrazArchipelago", "pull-request-name2"),
         ("Development", "pull-request-name3"),
         ("AlcatrazArchipelago", "pull-request-name4")
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "mdodkins"
   
      password = "password"
      Github.GithubWrapper.init(password)
      self.assertNotEqual(None, Github.GithubWrapper._github)
      self.assertNotEqual(None, Github.GithubWrapper._repository)
      
      # When
      localRemoteWithArchipelagoName = "jameshollow/AlcatrazArchipelago"
      actualPullRequests = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      expected = [
         "pull-request-name2", 
         "pull-request-name4"
      ]
      self.assertEqual(expected, actualPullRequests)

   @patch("Git.getURLForRemote", MagicMock(return_value="git@github.com:RichardKeast/ProjectWeAreInterestedIn.git"))
   def test_get_pull_requests_does_not_care_if_the_remotes_push_url_has_different_capitalisation(self):
      # Given
      ConfigurationFile.githubUsername = "Dominic-Roberts"
      ConfigurationFile.repositoryName = "ProjectWeAreInterestedIn"
      MockUser.organisationMockObjectsList = [
         MockOrganisation("Absw", [
            MockRepository("ProjectWeAreInterestedIn", [
               MockRepository("richardkeast/ProjectWeAreInterestedIn"), # note different capitalisation of "richardkeast" to the patched function   
               MockRepository("some-guy/ProjectWeAreInterestedIn"), 
               MockRepository("some-being/ProjectWeAreInterestedIn")
            ]),
            MockRepository("some-other-project", [
               MockRepository("richardkeast/some-other-project"), 
            ]),
            MockRepository("some-unrelated-project", [
               MockRepository("richardkeast/some-unrelated-project")
            ])
         ])
      ]
      MockRepository.pullRequestTupleList = [
         ("some-branch", "pull-request-name1"),
         ("Development", "pull-request-name2"),
         ("TheWorldArchipelago", "MIS-123 that pull request that changed everything"),
         ("TheWorldArchipelago", "MIS-527 just a few changes"),
         ("Development", "pull-request-name3"),
         ("TheWorldArchipelago", "MIG-27 fix for world hunger"),
         ("Development", "pull-request-name4")
      ]
      MockPyGithub.expectedPassword = "password"
      MockPyGithub.expectedUsername = "Dominic-Roberts"
   
      password = "password"
      Github.GithubWrapper.init(password)
      
      # When
      localRemoteWithArchipelagoName = "rich/TheWorldArchipelago"
      actual = Github.GithubWrapper.getPullRequests(localRemoteWithArchipelagoName)
      
      # Then
      expected = [
         "MIS-123 that pull request that changed everything",
         "MIS-527 just a few changes",
         "MIG-27 fix for world hunger"
      ]
      self.assertEqual(expected, actual)

if __name__ == '__main__':
    unittest.main()