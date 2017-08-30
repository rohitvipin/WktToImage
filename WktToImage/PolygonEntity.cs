using System.Collections.Generic;

namespace WktToImage
{
    public class PolygonEntity
    {
        public List<PositionEntity> Coordinates { get; set; } = new List<PositionEntity>();
        public float StrokeWidth { get; set; }
    }
}
