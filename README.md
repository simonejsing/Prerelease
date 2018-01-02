# Prerelease

## TODO list
Alpha missing features:
* Split-screen camera/render
* Bug: Camera accounts for invisible player(s) when centering
* Ores: Diamond, Gold, Iron
* 500 inventory capacity (game state upgrade and upgrade rule for capacity)
* Limit block placement to adjacant grid locations 
* Save when running on xbox - how?

Bugs:
* Player does not collect drops immediately, first when center is inside drop

Beta features:
* Render player's target
* Longer targeting range
* Make player able to squeeze through 1x1 block gaps
* Save drops
* Camera boundaries
* Split screen
* Camera zooms out when players are far between

Rendering:
* Render drops
* Prerender drops to texture per sector
* Make the prerendered texture bob up and down

Terrain generator:

Terrain widget:
* Fallback rendering of on-screen part of sector
* Incremental prerendering of sectors to prevent lag when new sector loads

Sectors:
