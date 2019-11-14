from __future__ import division
import pygame
import math
import random
import os

from DrawableIsland import DrawableIsland
import ConfigurationFile
from TextBox import TextBox
import ArchipelagoData

class GUI:

   WindowClosedEvent = pygame.QUIT
   F5KeyEvent = pygame.K_F5
   F6KeyEvent = pygame.K_F6
   EscapeKeyEvent = pygame.K_ESCAPE
   DeleteKeyEvent = pygame.K_DELETE
   MouseClickReleaseEvent = pygame.MOUSEBUTTONUP

   def __init__(self):

      self.centreScreenPosition = (0, 0)
      self.__screen = None
      self.__background = None
      self.__clock = pygame.time.Clock()
      self.__events = []
      self.__lastClickedPosition = None
      self._lastDrawnList = []

      screenWidth = ConfigurationFile.screenWidth
      screenHeight = ConfigurationFile.screenHeight
      self.centreScreenPosition = (int(screenWidth/2), int(screenHeight/2))

      DrawableIsland.maximumTextDrawY = screenHeight
      DrawableIsland.maximumTextDrawX = screenWidth

      # set window starting position 
      windowPositionX = 50
      windowPositionY = 50
      os.environ['SDL_VIDEO_WINDOW_POS'] = str(windowPositionX) + "," + str(windowPositionY)

      pygame.init()
      self.__screen = pygame.display.set_mode((screenWidth, screenHeight))

      self.__background = pygame.Surface(self.__screen.get_size())
      self.__drawCleanBackground()
      self.__background = self.__background.convert()

      pygame.display.set_caption(ConfigurationFile.windowName)

      self.clearScreen()

   def go(self):
      self.__events = []

      FPS = 30
      milliseconds = self.__clock.tick(FPS) # do not go faster than this frame rate

      for event in pygame.event.get():
         if event.type == self.WindowClosedEvent:
            self.__events.append(self.WindowClosedEvent)

         if event.type == pygame.KEYDOWN:
            if (event.key == self.F5KeyEvent):
               self.__events.append(self.F5KeyEvent)
            if (event.key == self.F6KeyEvent):
               self.__events.append(self.F6KeyEvent)
            if (event.key == self.EscapeKeyEvent):
               self.__events.append(self.EscapeKeyEvent)
            if (event.key == self.DeleteKeyEvent):
               self.__events.append(self.DeleteKeyEvent)
         if event.type == self.MouseClickReleaseEvent:
            self.__events.append(self.MouseClickReleaseEvent)
            self.__lastClickedPosition = pygame.mouse.get_pos()

   def drawMessage(self, messageText):
      self.__drawCleanBackground()

      veryLargeTextSize = 72
      font = pygame.font.SysFont(None, veryLargeTextSize)
      renderWithColour = True
      black = (0, 0, 0)
      textObject = font.render(messageText, renderWithColour, black)
      drawPosition = (0, self.centreScreenPosition[1])
      self.__background.blit(textObject, drawPosition)
      self.__drawWholeScreen()

   def drawIslandsWithNames(self, archipelagosData):
      self._setLocationsAndSizesOfArchipelagos(archipelagosData)

      if len(self._lastDrawnList) == 0:
         self._lastDrawnList = self.__createDrawableIslands(archipelagosData)
      else:
         # user might have been deleting them
         for drawable in self._lastDrawnList:
            drawable.visible = True

      self.__drawCleanBackground()
      for drawable in self._lastDrawnList:
         drawable.draw(self.__background)

      self.__drawWholeScreen()

   def __createDrawableIslands(self, archipelagosData):
      drawList = []
      for archipelago in archipelagosData:
         RGBColourTuple = archipelago.colour
         XYLocationTuple = archipelago.location
         size = archipelago.size
         islandName = archipelago.name

         if archipelago.isReleaseArchipelago:
            archipelago.distanceFromCentre = 0
         else:
            archipelago.distanceFromCentre = archipelago.differenceData.distance

         island = DrawableIsland(archipelago)

         drawList.append(island)

      return drawList

   def getEvents(self):
      # we want this list to be cleared after accessing it, so that we don't get key repeats
      copyOfEventsArray = list(self.__events)
      self.__events = []
      return copyOfEventsArray

   def displayNameOfIslandAtMouseLocation(self):
      # we might be displaying a message or something
      if len(self._lastDrawnList) == 0:
         return

      self.__drawCleanBackground()
      self.__redrawExisitingIslands()

      for drawable in self._lastDrawnList:
         hasClickedOnCircle = drawable.collides(self.__lastClickedPosition)
         if hasClickedOnCircle:
            drawable.drawNameAndDistance(self.__background)
            break

      # otherwise, they might have clicked off an island and want the text to disappear
      self.__drawWholeScreen()

   def doNotRenderSelectedIsland(self):
      self.__drawCleanBackground()

      for drawable in self._lastDrawnList:
         hasClickedOnCircle = drawable.collides(self.__lastClickedPosition)
         if hasClickedOnCircle:
            drawable.visible = False
            break

      self.__redrawExisitingIslands()
      self.__drawWholeScreen()


   def getColours(self, numberOfColoursNeeded):
      colours = []

      for unused in xrange(numberOfColoursNeeded):
         
         makeSureColourIsNeverTooPale = 20

         redAmount = random.randint(0, 255 - makeSureColourIsNeverTooPale)
         greenAmount = random.randint(0, 255 - makeSureColourIsNeverTooPale)
         blueAmount = random.randint(0, 255 - makeSureColourIsNeverTooPale)

         rGBColour = (redAmount, greenAmount, blueAmount)
         colours.append(rGBColour)

      return colours

   def _setLocationsAndSizesOfArchipelagos(self, archipelagosData):
      colourLocationSizeList = []

      for index, archipelago in enumerate(archipelagosData):

         if archipelago.isReleaseArchipelago:
            centreDotSize = 5
            location = self.centreScreenPosition
            archipelago.size = centreDotSize
            archipelago.location = location
            continue

         numberOfBranches = len(archipelagosData) -1 # -1 for release archipelago
         distanceFromCentre = archipelago.differenceData.distance
         if distanceFromCentre > ConfigurationFile.maxDistanceFromCentreForScreenSize:
            distanceFromCentre = ConfigurationFile.maxDistanceFromCentreForScreenSize
         if distanceFromCentre < ConfigurationFile.minDistanceFromCentreForScreenSize:
            distanceFromCentre = ConfigurationFile.minDistanceFromCentreForScreenSize
         location = self._getXYLocationOfIsland(distanceFromCentre, numberOfBranches, index)

         size = int(archipelago.differenceData.distance)
         if size > ConfigurationFile.maxSizeOfDrawableToStopCollidingWithCentre:
            size = ConfigurationFile.maxSizeOfDrawableToStopCollidingWithCentre
         if size < ConfigurationFile.minSizeOfDrawableToBeVisible:
            size = ConfigurationFile.minSizeOfDrawableToBeVisible

         archipelago.size = size
         archipelago.location = location

   def _getXYLocationOfIsland(self, distance, numberOfBranches, branchIndex):         
      degreesForDistanceBetweenIslands = (360 / numberOfBranches) * branchIndex
      degreesForDistanceBetweenIslands = math.radians(degreesForDistanceBetweenIslands)
      cos = math.cos(degreesForDistanceBetweenIslands)
      sin = math.sin(degreesForDistanceBetweenIslands)

      circleStartingX = distance
      circleStartingY = 0
      XRelativeToOrigin = (circleStartingX * cos) - (circleStartingY * sin)
      YRelativeToOrigin = (circleStartingX * sin) + (circleStartingY * cos)
      XRelativeToCentre = XRelativeToOrigin + self.centreScreenPosition[0]
      YRelativeToCentre = YRelativeToOrigin + self.centreScreenPosition[1]

      location = (int(XRelativeToCentre), int(YRelativeToCentre))
      return location

   def __redrawExisitingIslands(self):
      DrawableIsland.setAllToNotBeHighlighted()
      for drawable in self._lastDrawnList:
         drawable.draw(self.__background)

   def __drawCleanBackground(self):
      blue = (30,144,255)
      self.__background.fill(blue)

   def __drawWholeScreen(self):
      self.__screen.blit(self.__background, (0,0))
      pygame.display.flip()

   def clearScreen(self):
      self.__drawCleanBackground()
      self.__drawWholeScreen()

   def getUserInput(self, question, hideInput = False):
      seaGreen = (30,144,100)
      backgroundColour = seaGreen
      black = (0,0,0)
      fontColour = black
      width = 800
      height = 60
      fontSize = 48
      textBox = TextBox(self.__screen, 
         question, 
         fontColour, 
         backgroundColour, 
         width, 
         height, 
         fontSize, 
         hideInput)

      return textBox.input
