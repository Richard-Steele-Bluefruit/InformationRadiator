import getpass

import ConfigurationFile
import github as PyGithub
import Git

class GithubWrapper(object):

   _github = None
   _repository = None

   @classmethod
   def init(theClass, plainTextPassword):

      #############
      #   this "ConfigurationFile.repositoryName" should really be gotten 
      #   from git, as we we use local git origin when calling "Git.getURLForRemote(...)"
      #############

      if len(plainTextPassword) == 0:
         print "Warning: in ArchipelagoGithub::init(), No github password given, can't get pull requests"
         print 
         return

      username = ConfigurationFile.githubUsername

      try:
         theClass._github = PyGithub.Github(username, plainTextPassword)

         for organisation in theClass._github.get_user().get_orgs():
            for repo in organisation.get_repos():
               if repo.name == ConfigurationFile.repositoryName:
                  theClass._repository = repo
                  return

      except PyGithub.BadCredentialsException:
         print "Error: in ArchipelagoGithub::init(), Couldn't connect to github"
         print 
         return

      if theClass._repository == None:
         print "Error: in ArchipelagoGithub::init(), could not find \"", ConfigurationFile.repositoryName, "\" in list of repositories"
         print 
         return

   @classmethod
   def getPullRequests(theClass, localRemoteWithArchipelagoName):
      if theClass._repository == None or theClass._github == None:
         print "Error: in ArchipelagoGithub::getPullRequests(...), cannot get pull requests when init() has failed (did you call init() first?)"
         print 
         return False

      githubUsername = theClass._getUsername(localRemoteWithArchipelagoName)

      if githubUsername == False:
         localRemoteName = localRemoteWithArchipelagoName.split("/")[0]
         print "Error in: GithubWrapper::getUsernameAndRepository(...), there is no remote called ", localRemoteName
         print 
         return False

      specificUserRepository = theClass._getRepository(theClass._repository, githubUsername)
      if specificUserRepository == None:
         print "Error in: GithubWrapper::getPullRequests(...), could not get a fork for", githubUsername
         print 
         return False

      pullRequestTitles = []
      archipelagoName = localRemoteWithArchipelagoName.split('/')[1]
      for pullRequest in specificUserRepository.get_pulls():
         whereItIsBeingPullRequestedTo = pullRequest.base.ref
         if whereItIsBeingPullRequestedTo == archipelagoName:
            usernameAndBranch = pullRequest.head.label
            usernameAndBranch = usernameAndBranch.replace(':', '/')
            pullRequestTitles.append(usernameAndBranch)

      return pullRequestTitles

   @classmethod
   def _getUsername(theClass, localRemoteWithArchipelagoName):
      localRemoteName = localRemoteWithArchipelagoName.split("/")[0]
      
      urlForRemote = Git.getURLForRemote(localRemoteName)
      if urlForRemote == False:
         return False

      # e.g. git@github.com:Dominic-Roberts/respoistoryName.git
      githubURLAndgithubUsername = urlForRemote.split(':')[-1]
      githubUsername = githubURLAndgithubUsername.split('/')[0]

      # The user has typed this in, so the capitalisation might be rubbish
      githubUsername = githubUsername.lower()

      return githubUsername

   @classmethod
   def _getRepository(theClass, repository, githubUsername):
      for fork in repository.get_forks():
         # It is possible for the url and the name to be different in capitalisation (when we get 
         #  the "githubUsername", we use the url from local git's push url which the user has typed in)
         forkUsername = fork.owner.login.lower()
         correctUsername = (forkUsername == githubUsername)

         if correctUsername:
            return fork

         innerFork = theClass._getRepository(fork, githubUsername)
         if innerFork != None:
            return innerFork
