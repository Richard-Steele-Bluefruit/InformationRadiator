### from:
###   http://www.pygame.org/pcr/inputbox/inputbox.py   

import pygame, pygame.font, pygame.event, pygame.draw, string
from pygame.locals import *

class TextBox(object):

   def __init__(self, screenToDrawTo, label, fontColour, backgroundColour, width, height, fontSize, hideInput = False):
      self._screen = screenToDrawTo
      self.fontColour = fontColour
      self.backgroundColour = backgroundColour
      self.width = width
      self.height = height
      self.fontSize = fontSize

      black = (0,0,0)
      self.borderColour = black
      self.hideInput = hideInput

      self.input = self.ask(label)

   def ask(self, question):
      pygame.font.init()
      self.display_box(question + ": ")

      current_string = []
      toDisplay = []
      while True:
         event = self.get_key_event()

         finishedTyping = (event.key == K_RETURN)
         closedWindow = (event.type == QUIT)
         keyPress = (event.type == KEYDOWN and 
            event.key != K_RSHIFT and 
            event.key != K_LSHIFT and 
            event.key != K_LSUPER and
            event.key != K_RSUPER and
            event.key != K_CAPSLOCK and
            event.key != K_NUMLOCK)
         if finishedTyping:
            break
         elif event.key == K_ESCAPE or event.key == K_F5 or closedWindow:
            current_string = []
            break
         elif event.key == K_BACKSPACE:
            current_string = current_string[0:-1]
            toDisplay = toDisplay[0:-1]
         elif keyPress:
            newCharacter = event.unicode
            current_string.append(newCharacter)
            if self.hideInput:
               toDisplay.append('*')
            else:
               toDisplay.append(newCharacter)

         self.display_box(question + ": " + string.join(toDisplay,""))

      return string.join(current_string,"")

   def display_box(self, message):
      fontobject = pygame.font.Font(None, self.fontSize)
      screenWidth = self._screen.get_width()
      screenHeight = self._screen.get_height()
      
      # draw background box
      width = self.width
      height = self.height
      pygame.draw.rect(self._screen, self.backgroundColour,
         ((screenWidth / 2) - (width / 2),
         (screenHeight / 2) - (height / 2),
         self.width, self.height), 0)

      # draw border
      lineWidth = 1
      width = self.width + 2
      height = self.height + 2
      pygame.draw.rect(self._screen, self.borderColour,
         ((screenWidth / 2) - (width / 2),
         (screenHeight / 2) - (height / 2),
         self.width + 2, self.height + 2), lineWidth)

      # draw the text
      padding = 10
      textXLoc = (self.width / 2) - padding
      textYLoc = (self.height / 2) - padding
      height = self.height
      if len(message) != 0:
         self._screen.blit(fontobject.render(message, 1, self.fontColour),
            ((screenWidth / 2) - textXLoc, (screenHeight / 2) - textYLoc))
      pygame.display.flip()

   def get_key_event(self):
      while True:
         event = pygame.event.poll()
         if event.type == KEYDOWN:
            return event
         else:
            pass