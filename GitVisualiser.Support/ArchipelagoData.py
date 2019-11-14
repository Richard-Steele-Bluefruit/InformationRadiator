
class ArchipelagoData(object):

   _black = (0,0,0)

   def __init__(self, name, colour = _black):
      self.name = name
      self.colour = colour

      self.isReleaseArchipelago = False
      self.differenceData = None
      self.size = None
      self.location = None