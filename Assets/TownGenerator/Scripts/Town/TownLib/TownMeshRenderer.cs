using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeshUtils;
using Town.Geom;
using UnityEngine;

namespace Town
{
    public class TownMeshRenderer
    {
        private Town town;
        public TownOptions options;
        private TownMeshRendererOptions rendererOptions;

        private Transform child;
        public Transform root;
        public Transform mapRoot;
        public GameObject Waters;
        public GameObject BuildingsOverlay;
        public GameObject BuildingsMesh;
        public GameObject WallsOverlay;
        public GameObject WallsMesh;
        public GameObject Roads;
        public string name;

        private const string CamMapLayer = "Map";
        private const string CamPlayerLayer = "NotMap";
        private const string CamWorldLayer = "Default";

        public TownMeshRenderer(
            Town town,
            TownOptions options,
            TownMeshRendererOptions rendererOptions
        )
        {
            this.town = town;
            this.options = options;
            this.name = town.name;

            root = rendererOptions.Root;
            mapRoot = rendererOptions.MapRoot;
            this.rendererOptions = rendererOptions;
        }

        public GameObject GenerateOverlay()
        {
            if (TownGlobalObject.townsData[town.coord].TownGameObject == null)
                TownGlobalObject.townsData[town.coord].TownGameObject = new GameObject(town.name);

            var go = TownGlobalObject.townsData[town.coord].TownGameObject;

            go.transform.parent = root;
            child = go.transform;
            child.transform.localPosition = Vector3.zero;
            //  var bounds = town.GetCityWallsBounds().Expand(100);

            TownOptions skeletonOptions = options;

            //skeletonOptions.IOC = false;
            //skeletonOptions.Farm = true;
            //skeletonOptions.Roads = true;
            //skeletonOptions.Walls = true;
            //skeletonOptions.CityDetail = true;

            var geometry = town.GetTownGeometry(skeletonOptions);

            UnityEngine.Random.InitState(options.Seed.GetHashCode());

            DrawOverlay(geometry, null);

            return go;
        }

        public GameObject Generate()
        {
            // we use the global GameObject for this town

            if (TownGlobalObject.townsData[town.coord].TownGameObject == null)
                TownGlobalObject.townsData[town.coord].TownGameObject = new GameObject(town.name);

            var go = TownGlobalObject.townsData[town.coord].TownGameObject;

            go.transform.parent = root;
            child = go.transform;
            child.transform.localPosition = Vector3.zero;
            //  var bounds = town.GetCityWallsBounds().Expand(100);

            var geometry = town.GetTownGeometry(options);
            MeshUtils.Polygon poly;

            var vertices = new List<Vector3>();
            UnityEngine.Random.InitState(options.Seed.GetHashCode());

            if (options.Water)
            {
                Waters = new GameObject("Waters");
                Waters.transform.parent = child;
                Waters.transform.localPosition = Vector3.zero;
                foreach (var water in geometry.Water)
                {
                    foreach (var vertex in water.Vertices)
                    {
                        vertices.Add(new Vector3(vertex.x, -3f, vertex.y));
                    }
                    poly = new MeshUtils.Polygon(
                        "Water",
                        vertices,
                        3f,
                        rendererOptions.WaterMaterial,
                        Waters.transform
                    );
                    poly.Transform.localPosition = Vector3.zero;
                    vertices.Clear();
                }
            }
            else
            {
                Waters = null;
            }

            BuildingsMesh = new GameObject("Buildings Mesh");
            BuildingsMesh.transform.parent = child;
            BuildingsMesh.transform.localPosition = new Vector3(0, -child.position.y, 0);
            BuildingsMesh.layer = LayerMask.NameToLayer("Ignore Raycast");

            BuildingsOverlay = new GameObject("Buildings Overlay");
            BuildingsOverlay.transform.parent = child;
            BuildingsOverlay.transform.localPosition = new Vector3(0, 950, 0);

            List<Building> onsite = town.GetTownGeometry(town.Options).Buildings;

            foreach (var building in onsite)
            {
                foreach (var vertex in building.Shape.Vertices)
                {
                    vertices.Add(
                        new Vector3(
                            ScaleToWorldWithOffset(vertex.x, town.townOffset.x),
                            0,
                            ScaleToWorldWithOffset(vertex.y, town.townOffset.y)
                        )
                    );
                }

                var buildingHeight = UnityEngine.Random.Range(20f, 45f);
                if (building.Description == "Castle")
                {
                    buildingHeight = UnityEngine.Random.Range(50f, 65f);
                }

                poly = new MeshUtils.Polygon(
                    town.name + "_" + building.Description,
                    vertices,
                    //vertices.Select(x => new Vector3(x.x, 0, x.z)).ToList(),
                    //0.1f,
                    buildingHeight,
                    //rendererOptions.TownModelsOverlay,
                    rendererOptions.BuildingMaterial,
                    BuildingsMesh.transform,
                    true
                );

                poly.Transform.localPosition = Vector3.zero;

                poly.GameObject.AddComponent<LerpToGround>().time = 0f;
                // poly.GameObject.AddComponent<SpawnAndWriteSigns>().Sign =
                //     TownGlobalObjectService.rendererOptions.signPrefab;

                //poly.GameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                // Vector3 c = new Vector3(0, 0, 0);
                // foreach (var vertex in vertices)
                // {
                //     c += vertex;
                // }

                // c.x = c.x / vertices.Count;
                // c.y = c.y / vertices.Count;
                // c.z = c.z / vertices.Count;

                poly = new MeshUtils.Polygon(
                    building.Description,
                    //vertices,
                    vertices.Select(x => new Vector3(x.x, 0, x.z)).ToList(),
                    0.1f,
                    //UnityEngine.Random.Range(5f, 10f),
                    rendererOptions.BuildingMaterial,
                    BuildingsOverlay.transform,
                    false
                );

                //    var scrpt =   poly.GameObject.AddComponent<FindOverlay>();

                //     Vector3 thecenter = new Vector3(vertices.Sum(x => x.x), vertices.Sum(x => x.y), vertices.Sum(x => x.z)) / vertices.Count;

                //    scrpt.startingoffset = thecenter;









                // GameObject thingGo = GameObject.Instantiate<GameObject>(thing, poly.GameObject.transform, true);

                //// town.Center.x

                // thingGo.transform.localPosition = new Vector3(0,100,0);


                //if (options.IOC)
                //{



                //    poly.GameObject.layer = LayerMask.NameToLayer("IOC");


                //    IOClod scrip = poly.GameObject.AddComponent<IOClod>();
                //    scrip.Occludee = true;
                //    scrip.Static = true;
                //    scrip.Lod1 = 0;
                //    scrip.Lod2 = 0;
                //    scrip.LodMargin = 0;
                //    scrip.LodOnly = false;
                //}

                //SignCommands.WriteAndMove(
                //                      building.Description,
                //                      poly.Transform.position + c,
                //                      Quaternion.identity,
                //                      poly.GameObject);



                poly.Transform.localPosition = Vector3.zero;

                poly.GameObject.layer = LayerMask.NameToLayer(CamMapLayer);

                vertices.Clear();
            }

            if (options.Roads)
            {
                DrawRoads(geometry, null);
            }

            if (options.Walls)
            {
                DrawWalls(geometry, null);
            }

            //var curpos = GameObject.FindGameObjectWithTag("Player").transform;

            // Den.Tools.Coord newPositionAsCoord = new Den.Tools.Coord((int)(curpos.position.x * 0.001), (int)(curpos.position.z * 0.001));

            // if  (      Den.Tools.Coord.Distance((town.coord + town.mapOffset.ToCoord()   ), newPositionAsCoord) < 4)
            // //if (options.Walls)
            // {
            //     DrawWallSplines(geometry, null);
            //     DrawWalls(geometry, null);
            // }
            // else
            // {
            //Walls = null;
            //}

            return go;
        }

        private void DrawOverlay(TownGeometry geometry, StringBuilder sb)
        {
            MeshUtils.Polygon poly;
            var vertices = new List<Vector3>();
            var overlays = new GameObject("Overlays");

            overlays.gameObject.layer = LayerMask.NameToLayer(CamMapLayer);

            overlays.transform.parent = child;
            overlays.transform.localPosition = new Vector3(0, 0, 0); // Vector3.zero;
            foreach (var patch in geometry.Overlay)
            {
                if (patch.Water)
                    continue;
                if (patch.Area.GetType() == typeof(EmptyArea))
                    continue;
                if (patch.Area.GetType() == typeof(FarmArea))
                    continue; // here exclude outlying areas. you could put this back.
                var type = patch.Area.ToString();
                //float previousOffset = 900f;
                float overLayOffset = 950f;
                foreach (var vertex in patch.Shape.Vertices)
                {
                    float MovedX = ScaleToWorldWithOffset(vertex.x, town.townOffset.x);
                    float MovedY = ScaleToWorldWithOffset(vertex.y, town.townOffset.y);

                    //    MapMagic.Terrains.TerrainTile tileFound = TownGlobalObjectService.MapMagicObjectRef.tiles.FindByWorldPosition(MovedX, MovedY);

                    try
                    {
                        // var thing =
                        //     TownGlobalObjectService.MapMagicObjectRef.tiles.FindByWorldPosition(
                        //         vertex.x,
                        //         vertex.y
                        //     );
                        //Terrain ter = thing.ActiveTerrain;
                        //TerrainData terData = ter.terrainData;
                        //previousOffset = terData.GetHeight((int)MovedX, (int)MovedY) + 900;
                        vertices.Add(new Vector3(MovedX, overLayOffset, MovedY));
                    }
                    catch // (Exception ex)
                    {
                        if (patch.IsCityCenter)
                            overLayOffset = 0f;

                        //  Debug.Log(ex.Message);
                        vertices.Add(new Vector3(MovedX, overLayOffset, MovedY));
                    }
                }

                float offset = 3f;
                Material mat;
                if (patch.HasCastle)
                {
                    // offset = 3.4f;
                    mat = rendererOptions.CastleGroundMaterial;
                }
                else if (patch.IsCityCenter)
                {
                    //  offset = 3.2f;
                    mat = rendererOptions.CityCenterGround;
                }
                else if (patch.WithinWalls)
                {
                    if (type == "Town.RichArea")
                    {
                        mat = rendererOptions.RichArea;
                    }
                    else if (type == "Town.PoorArea")
                    {
                        mat = rendererOptions.PoorArea;
                    }
                    else
                    {
                        mat = rendererOptions.WithinWallsGroundMaterial;
                    }
                }
                else
                {
                    if (!options.Farm && type == "Town.EmptyArea")
                    {
                        mat = rendererOptions.HiddenCity;
                    }
                    else if (options.Farm && type == "Town.FarmArea")
                    {
                        mat = rendererOptions.FarmArea;
                    }
                    else
                    {
                        mat = rendererOptions.OuterCityGroundMaterial;
                    }
                }
                poly = new MeshUtils.Polygon(
                    patch.Area.GetType().ToString(),
                    vertices,
                    offset,
                    mat,
                    overlays.transform,
                    false
                );
                poly.GameObject.layer = LayerMask.NameToLayer(CamMapLayer);
                poly.Transform.localPosition = Vector3.zero;

                if (patch.IsCityCenter)
                {
                    //    poly.GameObject.isStatic = false;
                    // poly = new MeshUtils.Polygon(
                    //     patch.Area.GetType().ToString(),
                    //     vertices,
                    //     0.2f,
                    //     mat,
                    //     child.transform,
                    //     true
                    // );
                    // poly.GameObject.AddComponent<LerpToGround>();
                }

                vertices.Clear();
            }
        }

        private void DrawRoads(TownGeometry geometry, StringBuilder sb)
        {
            Roads = new GameObject("Roads");
            Roads.layer = LayerMask.NameToLayer("Map");
            Roads.transform.parent = child;
            Roads.transform.localPosition = Vector3.zero;
            Cube cube;

            foreach (var road in geometry.Roads)
            {
                Geom.Vector2 last = new Geom.Vector2(0, 0);
                foreach (var current in road)
                {
                    if (last.x != 0 && last.y != 0)
                    {
                        float MovedlastX = ScaleToWorldWithOffset(last.x, town.townOffset.x);
                        float MovedlastY = ScaleToWorldWithOffset(last.y, town.townOffset.y);

                        float MovedcurrentX = ScaleToWorldWithOffset(current.x, town.townOffset.x);
                        float MovedcurrentY = ScaleToWorldWithOffset(current.y, town.townOffset.y);

                        cube = new Cube(
                            "Road",
                            GetLineVertices(
                                MovedlastX,
                                MovedcurrentX,
                                MovedlastY,
                                MovedcurrentY,
                                3
                            ),
                            .2f,
                            rendererOptions.RoadMaterial,
                            Roads.transform,
                            false,
                            true
                        );
                        cube.Transform.localPosition = new Vector3(0, 1000f, 0);
                        cube.GameObject.layer = LayerMask.NameToLayer("Map");
                    }
                    last = current;
                }
            }
        }

        static Vector3 GetTerrainPos(float x, float y)
        {
            //Create object to store raycast data

            //Create origin for raycast that is above the terrain. I chose 100.
            Vector3 origin = new Vector3(x, 500f, y);

            //Send the raycast.
            // Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 501f);


            // TODO DO THE MASK SELECTION THING
            LayerMask mask = LayerMask.GetMask("Default");

            Ray ray = new Ray(origin, Vector3.down);

            Physics.Raycast(ray, out RaycastHit hit, 501f, mask);

            //  Debug.Log("Terrain location found at " + hit.point);
            return hit.point;
        }

        //private void MakeFenceFromEdges(TownGeometry geometry, GameObject FenceSpline, List<Geom.Vector2> replacedGates, IEnumerable<Edge> EdgeIEnumerable, string prettyName)
        //{
        //    int i = 0;

        //    foreach (var (wall, splineFormer) in from wall in EdgeIEnumerable
        //                                         let splineFormer = GameObject.Instantiate(TownHolder.Instance.FenceSplineFormer, FenceSpline.transform)
        //                                         select (wall, splineFormer))
        //    {
        //        splineFormer.name = String.Format("{0}_Wall{1}{2}", town.name, prettyName, i);
        //        splineFormer.transform.localPosition = Vector3.zero;
        //        var Former = splineFormer.GetComponent<SplineFormer>();
        //        var start = wall.A;
        //        var end = wall.B;
        //        if (wall.A == wall.B)
        //            continue;
        //        Former.SegmentsNumber = Math.Max(1, (int)Mathf.Sqrt(Geom.Vector2.DistanceSquare(wall.A, wall.B)));
        //        // optionally offset the ends to fit the Gates
        //        if (geometry.Gates.Contains(start))
        //        {
        //            replacedGates.Add(start);
        //            start = start + Geom.Vector2.Scale(end - start, 0.3f);
        //            wall.A = start;
        //            geometry.Gates.Add(start);
        //        }

        //        if (geometry.Gates.Contains(end))
        //        {
        //            replacedGates.Add(end);
        //            end = end - Geom.Vector2.Scale(end - start, 0.3f);
        //            wall.B = end;
        //            geometry.Gates.Add(end);
        //        }

        //        float MovedstartX = ScaleToWorldWithOffset(start.x, town.townOffset.x);
        //        float MovedstartY = ScaleToWorldWithOffset(start.y, town.townOffset.y);
        //        float MovedendX = ScaleToWorldWithOffset(end.x, town.townOffset.x);
        //        float MovedendY = ScaleToWorldWithOffset(end.y, town.townOffset.y);
        //        float startPlace, endPlace;
        //        startPlace = GetTerrainPos(MovedstartX, MovedstartY).y;
        //        endPlace = GetTerrainPos(MovedendX, MovedendY).y;
        //        Former.StartNode.transform.position = new Vector3(MovedstartX, startPlace, MovedstartY);
        //        Former.EndNode.transform.position = new Vector3(MovedendX, endPlace, MovedendY);

        //        i = i + 1;
        //    }
        //}

        private void DrawWalls(TownGeometry geometry, StringBuilder sb)
        {
            Cube cube;
            WallsMesh = new GameObject("Walls Mesh");
            WallsMesh.transform.parent = child;
            //WallsMesh.transform.localPosition = Vector3.zero;
            WallsMesh.transform.localPosition = new Vector3(town.townOffset.x, 0, town.townOffset.y);
            WallsOverlay = new GameObject("Walls Overlay");
            WallsOverlay.transform.parent = child;
            WallsOverlay.gameObject.layer = LayerMask.NameToLayer(CamMapLayer);
            //Walls.transform.localPosition = Vector3.zero;
            WallsOverlay.transform.localPosition = new Vector3(town.townOffset.x, 970, town.townOffset.y);
            var replacedGates = new List<Geom.Vector2>();
            foreach (var wall in geometry.Walls)
            {
                var start = wall.A;
                var end = wall.B;

                //start = new Geom.Vector2(startX, startY);
                //end = new Geom.Vector2(endX, endY);

                if (geometry.Gates.Contains(start))
                {
                    replacedGates.Add(start);
                    start += Geom.Vector2.Scale(end - start, 0.3f);
                    wall.A = start;
                    geometry.Gates.Add(start);
                }

                if (geometry.Gates.Contains(end))
                {
                    replacedGates.Add(end);
                    end -= Geom.Vector2.Scale(end - start, 0.3f);
                    wall.B = end;
                    geometry.Gates.Add(end);
                }
                cube = new Cube(
                    "Wall",
                    GetLineVertices(
                        TownGlobalObjectService.WorldMultiplier * start.x,
                        TownGlobalObjectService.WorldMultiplier * end.x,
                        TownGlobalObjectService.WorldMultiplier * start.y,
                        TownGlobalObjectService.WorldMultiplier * end.y,
                        // ScaleToWorldWithOffset(start.x, town.townOffset.x),
                        // ScaleToWorldWithOffset(end.x, town.townOffset.x),
                        // ScaleToWorldWithOffset(start.y, town.townOffset.y),
                        // ScaleToWorldWithOffset(end.y, town.townOffset.y),
                        TownGlobalObjectService.WorldMultiplier * 1
                    ),
                    0.1f,
                    rendererOptions.WallMaterial,
                    WallsOverlay.transform
                );
                //cube.Transform.localPosition = new Vector3(town.townOffset.x, 0, town.townOffset.y);
                cube.Transform.localPosition = Vector3.zero;
                cube.GameObject.layer = WallsOverlay.gameObject.layer;
                //cube.GameObject.AddComponent<LerpToGround>();

                cube = new Cube(
                    "WallMesh",
                    GetLineVertices(
                        TownGlobalObjectService.WorldMultiplier * start.x,
                        TownGlobalObjectService.WorldMultiplier * end.x,
                        TownGlobalObjectService.WorldMultiplier * start.y,
                        TownGlobalObjectService.WorldMultiplier * end.y,
                        //ScaleToWorldWithOffset(start.x, town.townOffset.x),
                        //ScaleToWorldWithOffset(end.x, town.townOffset.x),
                        //ScaleToWorldWithOffset(start.y, town.townOffset.y),
                        //ScaleToWorldWithOffset(end.y, town.townOffset.y),
                        TownGlobalObjectService.WorldMultiplier * 1
                    ),
                    TownGlobalObjectService.WorldMultiplier * 5,
                    rendererOptions.WallMaterial,
                    WallsMesh.transform,
                    false
                );
                //cube.Transform.localPosition = new Vector3(town.townOffset.x, 0, town.townOffset.y);
                cube.Transform.localPosition = Vector3.zero;
                cube.GameObject.AddComponent<LerpToGround>().time = 0f;
                // cube.GameObject.AddComponent<LerpToGround>();
            }

            foreach (var replacedGate in replacedGates.Distinct())
            {
                geometry.Gates.Remove(replacedGate);
            }

            foreach (var tower in geometry.Towers)
            {
                cube = new Cube(
                    "Tower",
                    GetVertices(
                        (int) TownGlobalObjectService.WorldMultiplier * 4, 
                        (int) TownGlobalObjectService.WorldMultiplier * 4, 
                        TownGlobalObjectService.WorldMultiplier * tower.x - TownGlobalObjectService.WorldMultiplier * 2, 
                        TownGlobalObjectService.WorldMultiplier * tower.y - TownGlobalObjectService.WorldMultiplier * 2
                        //(int) ScaleToWorldWithOffset(4, town.townOffset.x),
                        //(int) ScaleToWorldWithOffset(4, town.townOffset.y),
                        //ScaleToWorldWithOffset(tower.x - 2, town.townOffset.x),
                        //ScaleToWorldWithOffset(tower.y - 2, town.townOffset.y)
                    ),
                    0.1f,
                    rendererOptions.TowerMaterial,
                    WallsOverlay.transform
                );
                cube.GameObject.layer = WallsOverlay.gameObject.layer;
                //cube.Transform.localPosition = new Vector3(town.townOffset.x, 0, town.townOffset.y);
                cube.Transform.localPosition = Vector3.zero;
                cube = new Cube(
                    "TowerMesh",
                    GetVertices(
                        (int) TownGlobalObjectService.WorldMultiplier * 3, 
                        (int) TownGlobalObjectService.WorldMultiplier * 3, 
                        TownGlobalObjectService.WorldMultiplier * tower.x - TownGlobalObjectService.WorldMultiplier * 2, 
                        TownGlobalObjectService.WorldMultiplier * tower.y - TownGlobalObjectService.WorldMultiplier * 2
                        //(int) ScaleToWorldWithOffset(4, town.townOffset.x),
                        //(int) ScaleToWorldWithOffset(4, town.townOffset.y),
                        //ScaleToWorldWithOffset(tower.x - 2, town.townOffset.x),
                        //ScaleToWorldWithOffset(tower.y - 2, town.townOffset.y)
                    ),
                    TownGlobalObjectService.WorldMultiplier * 6,
                    rendererOptions.TowerMaterial,
                    WallsMesh.transform,
                    false
                );
                cube.Transform.localPosition = Vector3.zero;
                cube.GameObject.AddComponent<LerpToGround>().time = 0f;
                //cube.Transform.localPosition = new Vector3(town.townOffset.x, 0, town.townOffset.y);
            }

            foreach (var gate in geometry.Gates)
            {
                cube = new Cube(
                    "Gate",
                    GetVertices(
                        (int) TownGlobalObjectService.WorldMultiplier * 3, 
                        (int) TownGlobalObjectService.WorldMultiplier * 3, 
                        TownGlobalObjectService.WorldMultiplier * gate.x - (TownGlobalObjectService.WorldMultiplier * 2), 
                        TownGlobalObjectService.WorldMultiplier * gate.y - (TownGlobalObjectService.WorldMultiplier * 2)
                        //(int) ScaleToWorldWithOffset(4, town.townOffset.x),
                        //(int) ScaleToWorldWithOffset(4, town.townOffset.y),
                        //ScaleToWorldWithOffset(gate.x - 2, town.townOffset.x),
                        //ScaleToWorldWithOffset(gate.y - 2, town.townOffset.y)
                    ),
                    0.1f,
                    rendererOptions.GateMaterial,
                    WallsOverlay.transform
                );
                cube.GameObject.layer = WallsOverlay.gameObject.layer;
                //cube.Transform.localPosition = new Vector3(town.townOffset.x, 0, town.townOffset.y);
                cube.Transform.localPosition = Vector3.zero;
                cube = new Cube(
                    "GateMesh",
                    GetVertices(
                        (int) TownGlobalObjectService.WorldMultiplier * 3, 
                        (int) TownGlobalObjectService.WorldMultiplier * 3, 
                        TownGlobalObjectService.WorldMultiplier * gate.x - (TownGlobalObjectService.WorldMultiplier * 2), 
                        TownGlobalObjectService.WorldMultiplier * gate.y - (TownGlobalObjectService.WorldMultiplier * 2)
                        // (int) ScaleToWorldWithOffset(4, town.townOffset.x),
                        // (int) ScaleToWorldWithOffset(4, town.townOffset.y),
                        // ScaleToWorldWithOffset(gate.x - 2, town.townOffset.x),
                        // ScaleToWorldWithOffset(gate.y - 2, town.townOffset.y)
                    ),
                    TownGlobalObjectService.WorldMultiplier * 6,
                    rendererOptions.GateMaterial,
                    WallsMesh.transform,
                    false
                );
                cube.Transform.localPosition = Vector3.zero;
                //cube.Transform.localPosition = new Vector3(town.townOffset.x, 0, town.townOffset.y);
                cube.GameObject.AddComponent<LerpToGround>().time = 0f;
            }
        }

        private Vector3[] GetVertices(int width, int length, float offsetX, float offsetZ)
        {
            return new Vector3[]
            {
                new Vector3(offsetX, 0, offsetZ),
                new Vector3(offsetX, 0, offsetZ + length),
                new Vector3(offsetX + width, 0, offsetZ + length),
                new Vector3(offsetX + width, 0, offsetZ)
            };
        }

        // private Vector3[] GetLineVertices (float startX, float endX, float startY, float endY, float thickness = 1f)
        // {
        //     var p1 = new Vector3 (startX, 0, startY);
        //     var p2 = new Vector3 (endX, 0, endY);
        //     var dir = (p1 - p2).normalized;
        //     var norm = Vector3.Cross (dir, Vector3.up);
        //     var halfThickness = (norm * thickness) / 2;
        //     var p3 = p2 + halfThickness;
        //     var p4 = p1 + halfThickness + dir / 2;
        //     p1 = p1 - halfThickness + dir / 2;
        //     p2 = p2 - halfThickness;
        //     return new Vector3[]
        //     {
        //         p1,
        //         p2,
        //         p3,
        //         p4
        //     };
        // }

        // private void DrawWalls(TownGeometry geometry, StringBuilder sb)
        // {
        //     return;

        //     Cube cube;
        //     WallsMesh = new GameObject("WallsMesh");
        //     WallsMesh.transform.parent = child;
        //     WallsMesh.transform.localPosition = Vector3.zero;
        //     Walls = new GameObject("Walls");
        //     Walls.transform.parent = child;
        //     Walls.transform.localPosition = Vector3.zero;
        //     var replacedGates = new List<Geom.Vector2>();
        //     foreach (var wall in geometry.Walls)
        //     {
        //         var start = wall.A;
        //         var end = wall.B;

        //         if (wall.A == wall.B)
        //             continue;

        //         if (geometry.Gates.Contains(start))
        //         {
        //             replacedGates.Add(start);
        //             start = start + Geom.Vector2.Scale(end - start, 0.3f);
        //             wall.A = start;
        //             geometry.Gates.Add(start);
        //         }

        //         if (geometry.Gates.Contains(end))
        //         {
        //             replacedGates.Add(end);
        //             end = end - Geom.Vector2.Scale(end - start, 0.3f);
        //             wall.B = end;
        //             geometry.Gates.Add(end);
        //         }

        //         float MovedstartX = ScaleToWorldWithOffset(start.x, town.townOffset.x);
        //         float MovedstartY = ScaleToWorldWithOffset(start.y, town.townOffset.y);

        //         float MovedendX = ScaleToWorldWithOffset(end.x, town.townOffset.x);
        //         float MovedendY = ScaleToWorldWithOffset(end.y, town.townOffset.y);
        //         float startPlace,
        //             endPlace;

        //         var thing = TownGlobalObjectService.MapMagicObjectRef.tiles.FindByWorldPosition(
        //             MovedstartX,
        //             MovedstartY
        //         );
        //         // var thing = TownGlobalObjectService.MapMagicObjectRef.tiles.FindByWorldPosition(start.x, start.y);
        //         // if (thing !=null) // sigh
        //         if (false)
        //         {
        //             Terrain ter = thing.ActiveTerrain;
        //             TerrainData terData = ter.terrainData;
        //             startPlace = terData.GetHeight((int)MovedstartX, (int)MovedstartY);
        //         }
        //         else
        //         {
        //             startPlace = GetTerrainPos(MovedstartX, MovedstartY).y;
        //         }

        //         // if (thing != null) // sigh
        //         if (false)
        //         {
        //             thing = TownGlobalObjectService.MapMagicObjectRef.tiles.FindByWorldPosition(
        //                 MovedendX,
        //                 MovedendY
        //             );
        //             Terrain ter = thing.ActiveTerrain;
        //             TerrainData terData = ter.terrainData;
        //             endPlace = terData.GetHeight((int)MovedendX, (int)MovedendY);
        //         }
        //         else
        //         {
        //             endPlace = GetTerrainPos(MovedendX, MovedendY).y;
        //         }

        //         //cube = new Cube("Wall", GetLineVertices(
        //         //    ScaleToWorldWithOffset(start.x, town.townOffset.x),
        //         //     ScaleToWorldWithOffset(end.x, town.townOffset.x),
        //         //     ScaleToWorldWithOffset(start.y, town.townOffset.y),
        //         //     ScaleToWorldWithOffset(end.y, town.townOffset.y)
        //         //), 0.1f, rendererOptions.WallMaterial, Walls.transform);
        //         //cube.Transform.localPosition = Vector3.zero;


        //         //cube = new Cube("WallMesh", GetLineVertices(
        //         //     ScaleToWorldWithOffset(start.x, town.townOffset.x),
        //         //     ScaleToWorldWithOffset(end.x, town.townOffset.x),
        //         //     ScaleToWorldWithOffset(start.y, town.townOffset.y),
        //         //     ScaleToWorldWithOffset(end.y, town.townOffset.y)
        //         //), 4, rendererOptions.WallMaterial, WallsMesh.transform, true);


        //         cube = new Cube(
        //             "WallMesh",
        //             GetLineVertices(MovedstartX, MovedendX, MovedstartY, MovedendY),
        //             4,
        //             rendererOptions.WallMaterial,
        //             WallsMesh.transform,
        //             true
        //         );

        //         //Mesh mesh = cube.MeshFilter.mesh;
        //         //Vector3[] vertices = mesh.vertices;
        //         //UnityEngine.Vector2[] uvs = new UnityEngine.Vector2[vertices.Length];

        //         //for (int i = 0; i < uvs.Length; i++)
        //         //{
        //         //    uvs[i] = new UnityEngine.Vector2(vertices[i].x, vertices[i].z);
        //         //}
        //         //mesh.RecalculateNormals();
        //         //mesh.RecalculateTangents();

        //         //mesh.uv = uvs;

        //         //cube.MeshFilter.mesh.uv = uvs;
        //         cube.Transform.localPosition = new Vector3(0, (startPlace), 0);
        //     }

        //     foreach (var replacedGate in replacedGates.Distinct())
        //     {
        //         geometry.Gates.Remove(replacedGate);
        //     }

        //     if (options.Towers)
        //     {
        //         foreach (var tower in geometry.Towers)
        //         {
        //             cube = new Cube(
        //                 "Tower",
        //                 TownMeshRendererUtils.GetVertices(4, 4, tower.x - 2, tower.y - 2),
        //                 0.1f,
        //                 rendererOptions.TowerMaterial,
        //                 Walls.transform
        //             );
        //             cube.Transform.localPosition = Vector3.zero;
        //             cube = new Cube(
        //                 "TowerMesh",
        //                 TownMeshRendererUtils.GetVertices(4, 4, tower.x - 2, tower.y - 2),
        //                 8,
        //                 rendererOptions.TowerMaterial,
        //                 WallsMesh.transform,
        //                 false
        //             );
        //             cube.Transform.localPosition = Vector3.zero;
        //         }

        //         foreach (var gate in geometry.Gates)
        //         {
        //             cube = new Cube(
        //                 "Gate",
        //                 TownMeshRendererUtils.GetVertices(4, 4, gate.x - 2, gate.y - 2),
        //                 0.1f,
        //                 rendererOptions.GateMaterial,
        //                 Walls.transform
        //             );
        //             cube.Transform.localPosition = Vector3.zero;
        //             cube = new Cube(
        //                 "GateMesh",
        //                 TownMeshRendererUtils.GetVertices(4, 4, gate.x - 2, gate.y - 2),
        //                 6,
        //                 rendererOptions.GateMaterial,
        //                 WallsMesh.transform,
        //                 false
        //             );
        //             cube.Transform.localPosition = Vector3.zero;
        //         }
        //     }
        // }

        private float ScaleToWorldWithOffset(float val, float offset)
        {
            return (TownGlobalObjectService.WorldMultiplier * val) + offset;
        }

        private Vector3[] GetLineVertices(
            float startX,
            float endX,
            float startY,
            float endY,
            float thickness = 1f
        )
        {
            var p1 = new Vector3(startX, 0, startY);
            var p2 = new Vector3(endX, 0, endY);
            var dir = (p1 - p2).normalized;
            var norm = Vector3.Cross(dir, Vector3.up);
            var halfThickness = (norm * thickness) / 2;
            var p3 = p2 + halfThickness;
            var p4 = p1 + halfThickness + dir / 2;
            p1 = p1 - halfThickness + dir / 2;
            p2 = p2 - halfThickness;
            return new Vector3[] { p1, p2, p3, p4 };
        }
    }

    public static class TownMeshRendererUtils
    {
        public static Vector3[] GetVertices(int width, int length, float offsetX, float offsetZ)
        {
            return new Vector3[]
            {
                new Vector3(offsetX, 0, offsetZ),
                new Vector3(offsetX, 0, offsetZ + length),
                new Vector3(offsetX + width, 0, offsetZ + length),
                new Vector3(offsetX + width, 0, offsetZ)
            };
        }
    }
}
