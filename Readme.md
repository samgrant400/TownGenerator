## Medieval Town(s) Generator ## 
### (With MapMagic2, and SplineMesh implementation) ###

Requires Unity 2021.1 (2021 for [benoit-dumas/SplineMesh](https://github.com/benoit-dumas/SplineMesh) support)
This version requires the free Assets Mapmagic2, SplineMesh to operate and the 'Objects' and the 'Splines' extensions for MapMagic2 which are not free.

The Town Init service now creates multiple cities, spreads them based on a minimum distance and the CityLink tool provides automatic splines that link the locations.

[zulfajuniadi/TownGenerator](https://github.com/zulfajuniadi/TownGenerator/) Ported the code from [runegri/Town](https://github.com/runegri/Town) to generate meshes instead of SVG.
Then Twobob modified it to generate [MapMagic2](https://assetstore.unity.com/packages/tools/terrain/mapmagic-2-165180) objects, splines and other things like a floating mini-map area overlay from the Area Patches.   This also contains a version of [twobob/MapMagic2Scripts](https://github.com/twobob/MapMagic2Scripts/commit/e93d9ed3122215709945521cb473cc59809e7067)

Generated Town: (This is using not-included prefabs in a feature complete project)

![Generated](https://user-images.githubusercontent.com/915232/135077644-ad10915b-ae30-492b-8c15-72760cceb319.png "Generated")

Sample Scene Demo: Included

![WhiteBox](https://user-images.githubusercontent.com/915232/136688810-9419bb6e-1c60-4856-b458-2386b7f88cd2.png "WhiteBox")

![CloseUp](https://user-images.githubusercontent.com/915232/135194891-b0e9310b-6e7a-4f63-944c-b247a5faf57e.png "closeUp")

Generator Options:

Maximum and minimum spread of the town cell count

Overall world size mutliplier

The other values are required for operation and the SampleScene demonstrates complete population and provides prefabs.

- Materials: Various materials for different parts of the map
- Various other supporting objects

Usage:

View the `TownTileBuilder.cs` and `TownMeshBuilder.cs` file to see how the generation is initiated.

Todos:

1. Migrate the scripts to use UnityEngine.Vector2.
2. Reduce GC allocation for generation (Now its around 100MB)
3. Make the generation multithreaded via jobs
4. Support water
7. Finish a complete a whitebox prefab demo example
8. Rewrite SplineMesh as an Output

### License ###

Copyright 2018 Zulfa Juniadi  & 2021 Twobob

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Includes a version of https://github.com/benoit-dumas/SplineMesh  (same license)
https://github.com/benoit-dumas/SplineMesh/blob/master/LICENSE


---

## SUPPORT THIS DEVELOPER: https://paypal.me/Geekwife     
###  A kind soul has agreed to collect any money from other nice people that want to support more scripts like these and time spent making these solid / better
