import pygame

import ConfigurationFile
from Shape import Shape

class DrawableIsland:

   maximumTextDrawY = 0
   maximumTextDrawX = 0
   _highlightedLineThickness = 5
   _highlightColour = (255, 255, 0)  # yellow
   _anInstanceIsHighlighted = False

   @classmethod
   def setAllToNotBeHighlighted(theClass):
      theClass._anInstanceIsHighlighted = False

   def __init__(self, archipelagoData):

      self._archipelagoData = archipelagoData

      self.visible = True
      self._font = pygame.font.SysFont(None, ConfigurationFile.textSize)
      self._textLocation = self._setupTextLocation()

      shape = Shape(self._archipelagoData.size, self._archipelagoData.location)
      self._pointsToDraw = shape.get()
      sizeOffsetModifier = 6
      self._innerIslandPoints = shape.get(sizeOffsetModifier)

   def _setupTextLocation(self):
      x = self._archipelagoData.location[0]
      y = self._archipelagoData.location[1]
      paddingToMoveTextAwayFromWindowBorder = 10

      # right
      distanceTextWidthInPixels = self._font.size(self._archipelagoData.name)[0]
      if (x + distanceTextWidthInPixels) > DrawableIsland.maximumTextDrawX:
         x = DrawableIsland.maximumTextDrawX - distanceTextWidthInPixels - paddingToMoveTextAwayFromWindowBorder

      distanceTextHeightInPixels = self._font.size(self._archipelagoData.name)[1]
      heightOfTwoLines = distanceTextHeightInPixels * 2
      # bottom
      if (y + heightOfTwoLines) > DrawableIsland.maximumTextDrawY:
         y = DrawableIsland.maximumTextDrawY - heightOfTwoLines - paddingToMoveTextAwayFromWindowBorder
      # top
      elif y < 0:
         y = paddingToMoveTextAwayFromWindowBorder

      textLocation = (x, y)
      return textLocation

   def draw(self, placeToDrawTo):
      if self.visible:
         fillShapeConstant = 0
         # draw cliffs
         brown = (139,69,19)
         pygame.draw.polygon(placeToDrawTo, brown, self._pointsToDraw, fillShapeConstant)
         # draw inner part with green to make it look like an island
         innerIslandColour = (34,139,34)  # green
         sizeOffsetModifier = 6
         pygame.draw.polygon(placeToDrawTo, innerIslandColour, self._innerIslandPoints, fillShapeConstant)

         # draw number of pull requests in unique colour for better identification
         if not self._archipelagoData.isReleaseArchipelago:
            self._drawNumberOfPullRequestsText(placeToDrawTo)

         self._drawIfHighlighted(placeToDrawTo)

   def _drawNumberOfPullRequestsText(self, placeToDrawTo):
      numberOfPullRequests = None
      if not self._archipelagoData.differenceData.pullRequests:
         numberOfPullRequests = 0
      else:
         numberOfPullRequests = len(self._archipelagoData.differenceData.pullRequests)
      numberOfPullRequestsText = str(numberOfPullRequests)

      black = (0,0,0)
      outlineColour = black

      outerOutlineModifier = 10
      self._drawPullRequestText(placeToDrawTo, numberOfPullRequestsText, outlineColour, outerOutlineModifier)

      innerOutlineModifier = -20
      self._drawPullRequestText(placeToDrawTo, numberOfPullRequestsText, outlineColour, innerOutlineModifier)

      # actual text
      self._drawPullRequestText(placeToDrawTo, numberOfPullRequestsText, self._archipelagoData.colour)

   def _drawPullRequestText(self, placeToDrawTo, text, colour, textSizeModifierPercent = 0):
      isPositive = (textSizeModifierPercent > 0)
      if not isPositive:
         textSizeModifierPercent = -textSizeModifierPercent

      textSizeModifierPixels = (self._archipelagoData.size / 100) * textSizeModifierPercent

      if not isPositive:
         textSizeModifierPixels = -textSizeModifierPixels

      islandSize = self._archipelagoData.size + textSizeModifierPixels
      if islandSize < ConfigurationFile.minimumPullRequestNumberFontSizePixels:
         islandSize = ConfigurationFile.minimumPullRequestNumberFontSizePixels

      renderWithColour = True
      font = pygame.font.SysFont(None, islandSize)
      fontSize = font.size(text)
      centeredTextLocationX = self._archipelagoData.location[0] - (fontSize[0] / 2)
      centeredTextLocationY = self._archipelagoData.location[1] - (fontSize[1] / 2)
      centeredTextLocation = (centeredTextLocationX, centeredTextLocationY)
      textObject = font.render(text, renderWithColour, colour)
      placeToDrawTo.blit(textObject, centeredTextLocation)

   def _drawIfHighlighted(self, placeToDrawTo):
         mouseLocation = pygame.mouse.get_pos()
         thisCircleIsHighlighted = False
         if self.collides(mouseLocation):
            thisCircleIsHighlighted = True

         isntTooSmallToBeHighlighted = (self._archipelagoData.size > DrawableIsland._highlightedLineThickness)
         if thisCircleIsHighlighted and isntTooSmallToBeHighlighted and not DrawableIsland._anInstanceIsHighlighted:
            pygame.draw.polygon(placeToDrawTo, DrawableIsland._highlightColour, self._pointsToDraw, DrawableIsland._highlightedLineThickness)
            DrawableIsland._anInstanceIsHighlighted = thisCircleIsHighlighted

   def drawNameAndDistance(self, placeToDrawTo, colour = (0,0,0)):
      if self.visible:
         self._drawNameText(placeToDrawTo, colour)
         self._drawDistanceText(placeToDrawTo, colour)

   def _drawNameText(self, placeToDrawTo, colour):
      renderWithColour = True
      textToDraw = self._archipelagoData.name 

      white = (255, 255, 255)
      backgroundColour = white
      textObject = self._font.render(self._archipelagoData.name, renderWithColour, colour, backgroundColour)
      placeToDrawTo.blit(textObject, self._textLocation)

   def _drawDistanceText(self, placeToDrawTo, colour):
      if self._archipelagoData.distanceFromCentre != 0:
         distanceText = str(int(self._archipelagoData.distanceFromCentre))
         renderWithColour = True

         x = self._textLocation[0]
         y = self._textLocation[1]
         y = y + ConfigurationFile.textSize
         nextLineBelowLocation = (x, y)

         white = (255, 255, 255)
         backgroundColour = white
         textObject = self._font.render(distanceText, renderWithColour, colour, backgroundColour)
         placeToDrawTo.blit(textObject, nextLineBelowLocation)

   def collides(self, thingWeMayBeCollidingWithLocation):
      if self.visible:
         radius = self._archipelagoData.size
         x = self._archipelagoData.location[0] - radius
         y = self._archipelagoData.location[1] - radius
         boundingBox = pygame.Rect(x, y, radius*2, radius*2)
         return boundingBox.collidepoint(thingWeMayBeCollidingWithLocation)
