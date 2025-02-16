# Changelog

<!-- Release Template

### <major>.<minor>.<bugfix>-<yyyy>-<mm>-<dd>

#### Features
-

#### Changes
<!-- Note: If it is not a new feature but also not a bug fix add this section. ->
-

#### Bug Fixes
<!-- Note: Describe the problem not the solution. People will search the problem not solution. ->
-

#### Other/Technical
<!-- Note: Describe changes that do not effect the end user but may effect developers. ->
-

-->

### [0.0.0-2024-12-12](https://github.com/maxdevos49/gate-adventures/pull/6)

#### Changes
- Added camera functionality to move around the map

### [0.0.0-2024-03-15](https://github.com/maxdevos49/gate-adventures/pull/5)

#### Other/Technical
- Added support for managing 1 or more scenes at a time. 
  - Create a new scene using `IScene` or `Scene`. 
  - Start or Stop scenes using `SceneManager.Start` or `SceneManager.Stop` respectively by passing a instance of a `IScene`. 
  - Run scenes in parallel using `SceneManager.StartOverlay`.
  - Added 3 example scenes demonstrating how a scene can be used or implemented. Example scenes are named TileScene, HelloScene, and WorldScene.
- Added unit tests for the SceneManager class

### [0.0.0-2024-03-14](https://github.com/maxdevos49/gate-adventures/pull/4)

#### Other/Technical
- Added support for loading and rendering tile maps
- Added example map using the tile editor named [Tiled](https://www.mapeditor.org/)
- Documented how to use Tiled [here](./Tiled/README.md)

### [0.0.0-2024-03-10](https://github.com/maxdevos49/gate-adventures/pull/3)

#### Other/Technical
- Setup a [xUnit](https://xunit.net/) test project. Run with the `dotnet test` command

### [0.0.0-2024-03-09](https://github.com/maxdevos49/gate-adventures/pull/2)

#### Other/Technical
- Setup changelog template
- Setup .editorconfig based on Microsoft's [recommendations](https://github.com/dotnet/docs/blob/main/.editorconfig). Be sure to install the [EditorConfig](https://editorconfig.org/) plugin for your relevant code editor
- Setup Github actions to check the formatting and build for pull requests  
