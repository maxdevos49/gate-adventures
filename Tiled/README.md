# Tiled Guide

[Download Tiled](https://www.mapeditor.org/)
[Getting Started](https://doc.mapeditor.org/en/stable/manual/introduction/)

# Loading our project
- Select `File-->Open File or Project-->Select gateAdventures.tiled-project`

This should allow you to see all the maps currently in the project, as well as any added tilemaps. For adding on in addition to that, please see the [Tiled documentation](https://doc.mapeditor.org/en/stable/manual/introduction/).

# Exporting a map to use in Monogame

There are two things that need to be imported. This needs to be done for each map that is added:
1. The various TileSets
2. The map JSON file

1) **Importing the TileSets**
- Move the image files to the Content directory as you would a typical Monogame texture
- Open the MGCB Editor, then for each TileSet, import the files `(Right click in the Project section-->Add Existing Item-->Select the image file)`
- Click on each image file, and in the `properties` section, make sure that it is being imported as a texture, and the `Build Action` is **'Build'**

2) **Importing the JSON**
- After creating a map, choose `File-->Export As-->JSON type`
- Move this file to the Content directory.
- Edit the JSON under `tilesets` to include the correct source path **relative from the Content directory** (e.g. if my file is `projectRoot/Content/assets/maps/tilesets/myTileSet.png`, my source would be `assets\/maps\/tilesets\/myTileSet`). This is a necessary step to make sure that Monogame can load the correct compiled `xnb` files.
- Open up the MGCB Editor, and import the JSON file `(Right click in the Project section-->Add Existing Item-->Select the JSON file)`
- Click on the file, and in the `Properties` section, make sure the `Build Action` is **'Copy'**

After completing both steps, you can select in the MGCB Editor `Build-->Build`