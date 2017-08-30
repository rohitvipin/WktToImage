namespace WktToImage
{
    public class PositionEntity
    {
        public PositionEntity()
        {
            
        }

        public PositionEntity(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
