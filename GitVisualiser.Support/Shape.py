import math
import random

class Shape:

   _shapeList = []
   _shapeCentreList = []
   __generated = False
   
   @classmethod
   def generate(theClass):
      shapeList = []

      # these are unit measurements (i.e. will be multiplying by these later when resizing)
      shapeOnePoints = [(0,0), (0.5,0), (0.5,0.5), (0,0.5)]
      shapeTwoPoints = [(0,0), (0.5,0.5), (1,0.5), (0.5,1), (1,1)]
      shapeThreePoints = [(-0.25,0.25), (0,0.5), (0.25,-0.25), (1,-0.5)]
      shapeFourPoints = [(0,0), (1,0), (1,0.5)]
      shapeFivePoints = [(0,0), (1,1)]
      shapeSixPoints = [(0,-0.5), (0.5,0.5), (1,0.5)]
      shapeSevenPoints = [(1,0), (0,1)]
      shapeList.append(shapeOnePoints)
      shapeList.append(shapeTwoPoints)
      shapeList.append(shapeThreePoints)
      shapeList.append(shapeFourPoints)
      shapeList.append(shapeFivePoints)
      shapeList.append(shapeSixPoints)
      shapeList.append(shapeSevenPoints)

      theClass._shapeList = shapeList

      shapeCentreList = []
      shapeOneCentre = (-0.25,-0.25)
      shapeTwoCentre = (0.10,0.10)
      shapeThreeCentre = (0,-0.65)
      shapeFourCentre = (0.1,-0.25)
      shapeFiveCentre = (0,0)
      shapeSixCentre = (-0.15,-0.3)
      shapeSevenCentre = (0,0)
      shapeCentreList.append(shapeOneCentre)
      shapeCentreList.append(shapeTwoCentre)
      shapeCentreList.append(shapeThreeCentre)
      shapeCentreList.append(shapeFourCentre)
      shapeCentreList.append(shapeFiveCentre)
      shapeCentreList.append(shapeSixCentre)
      shapeCentreList.append(shapeSevenCentre)

      theClass._shapeCentreList = shapeCentreList

   def __init__(self, size, XYLocation):
      if not Shape.__generated:
         Shape.generate()
         Shape.__generated = True

      typeIndex = random.randint(0, len(Shape._shapeList)-1)
      self._points = Shape._shapeList[typeIndex]
      self._size = size
      self._XYLocation = XYLocation
      self._shapeCentre = Shape._shapeCentreList[typeIndex]

   def get(self, sizeOffsetModifier = None):
      size = self._size
      
      if sizeOffsetModifier != None:
         sizeOffset = self._size/sizeOffsetModifier
         size = self._size - sizeOffset

      # particular to random shape chosen
      shapeCentre = (self._shapeCentre[0] * size, self._shapeCentre[1] * size)
      XYLocation = self._XYLocation
      XYLocation = (XYLocation[0] - shapeCentre[0], XYLocation[1] - shapeCentre[1])

      shapePoints = []
      shapePoints = self._resize(size)
      shapePoints = self._relocate(shapePoints, size, XYLocation)

      kochSizeModifier = 3
      level = 4
      kochPoints = self._kochN(shapePoints, kochSizeModifier, level)
      
      return kochPoints

   def _resize(self, size):
      resizedPoints = []
      for point in self._points:
         newPoint = (point[0] * size, point[1] * size)
         resizedPoints.append(newPoint)

      return resizedPoints

   def _relocate(self, points, size, XYLocation):
      relocatedPoints = []
      drawingOffset = size/2  # otherwise the "centre" of the shape will be in the top left
      for point in points:
         newPoint = (point[0] + XYLocation[0] - drawingOffset, point[1] + XYLocation[1] - drawingOffset)
         relocatedPoints.append(newPoint)

      return relocatedPoints

   def _kochN(self, points, sizeModifier, level):
      for i in range(level):
         koch1Points = self._koch1(points, sizeModifier)
         points = koch1Points

      return points

   def _koch1(self, inputPoints, sizeModifier):
      points = []

      for i,unused in enumerate(inputPoints):
         start = inputPoints[i]
         end = None
         if i+1 == len(inputPoints):
            end = inputPoints[0]
         else:
            end = inputPoints[i+1]
         differenceX = end[0] - start[0]
         differenceY = end[1] - start[1]
         modifierXSpaceToKoch = int(differenceX/sizeModifier)
         modifierYSpaceToKoch = int(differenceY/sizeModifier)

         point1 = (start[0] + modifierXSpaceToKoch, start[1] + modifierYSpaceToKoch)
         point2 = (end[0] - modifierXSpaceToKoch, end[1] - modifierYSpaceToKoch)
         point1And2DifferenceX = point2[0] - point1[0]
         point1And2DifferenceY = point2[1] - point1[1]
         middlePoint = (point1[0] + point1And2DifferenceX, point1[1] + point1And2DifferenceY)
         degreesOfKoch = 60
         middlePoint = self._rotateAroundPointAntiClockwise(point1, middlePoint, degreesOfKoch)

         points.append(start)
         #points.append(point1)
         points.append(middlePoint)
         points.append(point2)
         #points.append(end)

      return points

   def _rotateAroundPointAntiClockwise(self, origin, pointToRotate, degrees):
      radiansInDegrees = math.radians(degrees)
      cos = math.cos(radiansInDegrees)
      sin = math.sin(radiansInDegrees)

      startingXRelativeToXZero = pointToRotate[0] - origin[0]
      startingYRelativeToYZero = pointToRotate[1] - origin[1]
      XRelativeToXZero = (startingXRelativeToXZero * cos) - (-startingYRelativeToYZero * sin)
      YRelativeToYZero = (-startingXRelativeToXZero * sin) + (startingYRelativeToYZero * cos)

      XRelativeToOrigin = XRelativeToXZero + origin[0]
      YRelativeToOrigin = YRelativeToYZero + origin[1]

      rotatedPoint = (int(XRelativeToOrigin), int(YRelativeToOrigin))

      return rotatedPoint