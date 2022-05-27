using Mapster.Common.MemoryMappedTypes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mapster.Rendering;

public static class TileRenderer
{
    public static BaseShape Tessellate(this MapFeatureData feature, ref BoundingBox boundingBox, ref PriorityQueue<BaseShape, int> shapes)
    {
        BaseShape? baseShape = null;

        var featureType = feature.Type;
        MapFeature.HighwayTypes hyghway_results;
        // used data from the enumerator instead of string
        if (feature.Properties.Any(p => p.Key == drawable_terrain_types.highway && Enum.TryParse(p.Value, true, out hyghway_results)))
        {
            var coordinates = feature.Coordinates;
            var road = new Road(coordinates);
            baseShape = road;
            shapes.Enqueue(road, road.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Properties.Any(p => p.Key == drawable_terrain_types.water) && feature.Type != GeometryType.Point)
        {
            var coordinates = feature.Coordinates;

            var waterway = new Waterway(coordinates, feature.Type == GeometryType.Polygon);
            baseShape = waterway;
            shapes.Enqueue(waterway, waterway.ZIndex);
        }
        else if (Border.ShouldBeBorder(feature))
        {
            var coordinates = feature.Coordinates;
            var border = new Border(coordinates);
            baseShape = border;
            shapes.Enqueue(border, border.ZIndex);
        }
        else if (PopulatedPlace.ShouldBePopulatedPlace(feature))
        {
            var coordinates = feature.Coordinates;
            var popPlace = new PopulatedPlace(coordinates, feature);
            baseShape = popPlace;
            shapes.Enqueue(popPlace, popPlace.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Properties.Any(p => p.Key == drawable_terrain_types.railway))
        {
            var coordinates = feature.Coordinates;
            var railway = new Railway(coordinates);
            baseShape = railway;
            shapes.Enqueue(railway, railway.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Properties.Any(p => p.Key == drawable_terrain_types.natural && featureType == GeometryType.Polygon))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, feature);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Properties.Any(p => p.Key == drawable_terrain_types.boundary 
            && Enum.TryParse<drawable_terrain_types_land>(p.Value, true, out var land_type_results) && land_type_results is drawable_terrain_types_land.forest))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Forest);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Properties.Any(p => p.Key == drawable_terrain_types.landuse 
            && Enum.TryParse<drawable_terrain_types_land>(p.Value, true, out var land_type_results)
            && land_type_results is drawable_terrain_types_land.forest or drawable_terrain_types_land.orchard))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Forest);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        else if (feature.Type == GeometryType.Polygon && feature.Properties.Any(
            // used data from the enumerator instead of string
            p => p.Key == drawable_terrain_types.landuse
                && Enum.TryParse<drawable_terrain_types_land>(p.Value, true, out var land_type_results)
                && land_type_results is drawable_terrain_types_land.cemetery
                or drawable_terrain_types_land.industrial or drawable_terrain_types_land.commercial
                or drawable_terrain_types_land.square or drawable_terrain_types_land.construction
                or drawable_terrain_types_land.military or drawable_terrain_types_land.quarry or drawable_terrain_types_land.brownfield))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Residential);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        else if (feature.Type == GeometryType.Polygon && feature.Properties.Any(
            // used data from the enumerator instead of string
            p => p.Key == drawable_terrain_types.landuse 
                && Enum.TryParse<drawable_terrain_types_land>(p.Value, true, out var land_type_results)
                && land_type_results is drawable_terrain_types_land.farm or drawable_terrain_types_land.meadow
                or drawable_terrain_types_land.grass or drawable_terrain_types_land.greenfield
                or drawable_terrain_types_land.recreation_ground or drawable_terrain_types_land.winter_sports
                or drawable_terrain_types_land.allotments))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Plain);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Type == GeometryType.Polygon &&
                 feature.Properties.Any(p => p.Key == drawable_terrain_types.landuse
                 && Enum.TryParse<drawable_terrain_types_land>(p.Value, true, out var land_type_results)
                 && land_type_results is drawable_terrain_types_land.reservoir
                 or drawable_terrain_types_land.basin))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Water);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Type == GeometryType.Polygon && feature.Properties.Any(p => p.Key == drawable_terrain_types.building))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Residential);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Type == GeometryType.Polygon && feature.Properties.Any(p => p.Key == drawable_terrain_types.leisure))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Residential);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }
        // used data from the enumerator instead of string
        else if (feature.Type == GeometryType.Polygon && feature.Properties.Any(p => p.Key == drawable_terrain_types.amenity))
        {
            var coordinates = feature.Coordinates;
            var geoFeature = new GeoFeature(coordinates, GeoFeature.GeoFeatureType.Residential);
            baseShape = geoFeature;
            shapes.Enqueue(geoFeature, geoFeature.ZIndex);
        }

        if (baseShape != null)
        {
            for (var j = 0; j < baseShape.ScreenCoordinates.Length; ++j)
            {
                boundingBox.MinX = Math.Min(boundingBox.MinX, baseShape.ScreenCoordinates[j].X);
                boundingBox.MaxX = Math.Max(boundingBox.MaxX, baseShape.ScreenCoordinates[j].X);
                boundingBox.MinY = Math.Min(boundingBox.MinY, baseShape.ScreenCoordinates[j].Y);
                boundingBox.MaxY = Math.Max(boundingBox.MaxY, baseShape.ScreenCoordinates[j].Y);
            }
        }

        return baseShape;
    }

    public static Image<Rgba32> Render(this PriorityQueue<BaseShape, int> shapes, BoundingBox boundingBox, int width, int height)
    {
        var canvas = new Image<Rgba32>(width, height);

        // Calculate the scale for each pixel, essentially applying a normalization
        var scaleX = canvas.Width / (boundingBox.MaxX - boundingBox.MinX);
        var scaleY = canvas.Height / (boundingBox.MaxY - boundingBox.MinY);
        var scale = Math.Min(scaleX, scaleY);

        // Background Fill
        canvas.Mutate(x => x.Fill(Color.White));
        while (shapes.Count > 0)
        {
            var entry = shapes.Dequeue();
            // FIXME: Hack
            if (entry.ScreenCoordinates.Length < 2)
            {
                continue;
            }
            entry.TranslateAndScale(boundingBox.MinX, boundingBox.MinY, scale, canvas.Height);
            canvas.Mutate(x => entry.Render(x));
        }

        return canvas;
    }

    public struct BoundingBox
    {
        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;
    }
}
