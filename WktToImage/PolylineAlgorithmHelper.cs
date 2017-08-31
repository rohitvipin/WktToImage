using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WktToImage
{
    /// <summary>
    /// Url for reference : https://developers.google.com/maps/documentation/static-maps/intro
    /// Based on : https://developers.google.com/maps/documentation/utilities/polylinealgorithm
    /// </summary>
    public static class PolylineAlgorithmHelper
    {
        private const string GoogleMapsDomainUrl = "https://maps.googleapis.com";
        private const string GoogleClientId = "gme-softwaresolutions";

        private static string Encode(IEnumerable<PositionEntity> points)
        {
            var plat = 0;
            var plng = 0;
            var encodedCoordinates = new StringBuilder();
            foreach (var coordinate in points)
            {
                // Round to 5 decimal places and drop the decimal
                var late5 = (int)(coordinate.Latitude * 1e5);
                var lnge5 = (int)(coordinate.Longitude * 1e5);
                // Encode the differences between the coordinates
                encodedCoordinates.Append(EncodeSignedNumber(late5 - plat));
                encodedCoordinates.Append(EncodeSignedNumber(lnge5 - plng));
                // Store the current coordinates
                plat = late5;
                plng = lnge5;
            }
            return encodedCoordinates.ToString();
        }

        /// <summary>
        /// Encode a signed number in the encode format.
        /// </summary>
        /// <param name="num">The signed number</param>
        /// <returns>The encoded string</returns>
        private static string EncodeSignedNumber(int num)
        {
            var sgnNum = num << 1; //shift the binary value
            if (num < 0) //if negative invert
            {
                sgnNum = ~sgnNum;
            }
            return EncodeNumber(sgnNum);
        }

        /// <summary>
        /// Encode an unsigned number in the encode format.
        /// </summary>
        /// <param name="num">The unsigned number</param>
        /// <returns>The encoded string</returns>
        private static string EncodeNumber(int num)
        {
            var encodeString = new StringBuilder();
            while (num >= 0x20)
            {
                encodeString.Append((char)((0x20 | (num & 0x1f)) + 63));
                num >>= 5;
            }

            return encodeString.Append((char)(num + 63)).ToString();
        }

        public static string EncodedStaticMapUrl(PositionEntity center, string wkt)
        {
            if (string.IsNullOrWhiteSpace(wkt))
            {
                return PointMap(center);
            }

            var encodedMultiPolyline = new StringBuilder();
            var enumerable = PolygonHelper.GetPolygonsPositionsFromWkt(wkt)
                ?.Select(x => x.Coordinates.Select(y => new PositionEntity
                {
                    Latitude = y.Latitude,
                    Longitude = y.Longitude
                }));
            if (enumerable == null)
            {
                return PointMap(center);
            }

            foreach (var polygon in enumerable)
            {
                var encodePolyLine = Encode(polygon);
                encodedMultiPolyline.Append($"&path=fillcolor:0xAA000033%7Ccolor:0xFFFFFF00%7Cenc:{encodePolyLine}");
            }

            var encodedStaticMapUrl = $"{GoogleMapsDomainUrl}/maps/api/staticmap?size=800x120&scale=2{encodedMultiPolyline}";
            return encodedStaticMapUrl;
        }

        public static string EncodedStaticMapUrl(PositionEntity center, List<string> wktList)
        {
            if (wktList?.Count >= 0 != true)
            {
                return PointMap(center);
            }

            var encodedMultiPolyline = new StringBuilder();
            foreach (var wkt in wktList)
            {
                var enumerable = PolygonHelper.GetPolygonsPositionsFromWkt(wkt)
                        ?.Select(x => x.Coordinates.Select(y => new PositionEntity
                        {
                            Latitude = y.Latitude,
                            Longitude = y.Longitude
                        }));
                if (enumerable == null)
                {
                    return PointMap(center);
                }

                foreach (var polygon in enumerable)
                {
                    var encodePolyLine = Encode(polygon);
                    encodedMultiPolyline.Append($"&path=fillcolor:0xAA000033%7Ccolor:0xFFFFFF00%7Cenc:{encodePolyLine}");
                }
            }

            var encodedStaticMapUrl = $"{GoogleMapsDomainUrl}/maps/api/staticmap?size=800x120&scale=2{encodedMultiPolyline}";
            return encodedStaticMapUrl;
        }

        private static string PointMap(PositionEntity center) => center == null ? "" : $"{GoogleMapsDomainUrl}/maps/api/staticmap?size=800x120&scale=2&center={center?.Latitude},{center?.Longitude}&zoom=9&markers=label:S%7C{center?.Latitude},{center?.Longitude}";

        public static string StaticMapUrl(PositionEntity center, string wkt)
        {
            if (string.IsNullOrWhiteSpace(wkt))
            {
                return PointMap(center);
            }

            var pathCordinates = string.Join("%7C", PolygonHelper.GetPolygonsPositionsFromWkt(wkt)
                ?.Select(x => x.Coordinates?.Select(y => $"{y.Latitude},{y.Longitude}"))
                .SelectMany(z => z));

            return $"{GoogleMapsDomainUrl}/maps/api/staticmap?size=800x120&scale=2&path=fillcolor:0xAA000033%7Ccolor:0xFFFFFF00%7C{pathCordinates}";
        }
    }
}
