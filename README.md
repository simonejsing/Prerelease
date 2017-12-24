# Prerelease

## TODO list
Rendering:
* Render drops
* Prerender drops to texture per sector
* Make the prerendered texture bob up and down

Terrain widget:
* Fallback rendering of on-screen part of sector
* Find a good data structure to store pre-rendered sectors in, that has fast access to recently used sectors
	* Reuse the left/right/up/down sector pointer implementation?
* Store a managing object per pre-rendered sector
* Register terrain modifications in managing object and update texture with changes during pre-render loop
* Incremental prerendering

Sectors:
* Implement sector links to left/right/up/down for constant time lookup
* Implement a sector pointer class that can iterate between sectors
* Optimize sector pointer to use dictionary key when asked to iterate move than X sectors away

