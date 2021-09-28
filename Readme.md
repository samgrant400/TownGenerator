## Medieval Town Generator ##

Developed unity 2021.1 (2021 for https://github.com/benoit-dumas/SplineMesh support)
https://github.com/zulfajuniadi/TownGenerator/ Ported the code from [runegri/Town](https://github.com/runegri/Town) to generate meshes instead of SVG.
Then Twobob modified it to generate MapMagic2 objects, splines and other things like a floating mini-map area overlay from the Area Patches.

Generated Town:

![Generated](https://user-images.githubusercontent.com/915232/135077644-ad10915b-ae30-492b-8c15-72760cceb319.png "Generated")

Generator Options:

![Generator Options](https://user-images.githubusercontent.com/915232/135077936-1e61dcf8-f6f9-4748-bd0b-53fa739e2d2b.png  "Generator Options")

Town Options:

- Walls: Does the generated town have walls
- Towers: Does the generated town have towers
- Water: CURRENTLY NOT SUPPORTED

Renderer Options:

- Root: The parent of the generated town.
- Materials: Various materials for different parts of the map

Usage:

View the `TownTileBuilder.cs` and `TownMeshBuilder.cs` file to see how the generation is initiated.

Todos:

1. Migrate the scripts to use UnityEngine.Vector2.
2. Reduce GC allocation for generation (Now its around 100MB)
3. Make the generation multithreaded via jobs
4. Support water
5. remove all the options that can be removed but maintian functionality
6. rework interface to be explcit and friendly

### License ###

Copyright 2018 Zulfa Juniadi  & 2021 Twobob

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Includes a version of https://github.com/benoit-dumas/SplineMesh  (same license)
https://github.com/benoit-dumas/SplineMesh/blob/master/LICENSE


