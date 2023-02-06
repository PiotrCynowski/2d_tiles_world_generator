## 2d_tiles_world_generator
Unity Project with an 2D tiles world generator

for world generator go to Scenes/WorldGenerator.unity

*example of generated tiles world*
![alt text](https://github.com/PiotrCynowski/2d_tiles_world_generator/blob/master/map_example.png?raw=true)

In main scene there are 2 prefabs:
#### - Grid
*with Level Generator 2D script*

script have reference to biomes - scriptable objects with tile references for spawning ground and trees, also setting for chance to spawn and biom noise map

To generate world you can use provided buttons
![alt text](https://github.com/PiotrCynowski/2d_tiles_world_generator/blob/master/buttons.png?raw=true)

add border variety - is adding tiles mix on their borders
clear - is used for clearing both tilemaps

Grid have two tilemaps parented to it, for ground and trees

#### - CameraContainer
*gameobject with isometric positoin of an ortographic camera, grid is adjusted with it*
