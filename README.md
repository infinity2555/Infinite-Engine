# Infinite-Engine

# What is it?
A simple 3D engine made completely in SFML.NET, with just a little of help from AI at more complicated algorithms algorithms.
It can draw multiple objects (for now only cubes are implemented into code), with multiple colors.

to create and draw a object, simply create `Obj name = new()` in Loop.cs.

it takes 4 arguments: 
- position, 
- object type (cube, etc), 
- type of filling (wireframe, only vertices, full),
- fill color.

after that, make sure to add your object World.OBJECTS_IN_WORLD list, or else it won't be drawn.
