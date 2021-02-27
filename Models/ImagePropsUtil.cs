using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ricoai.Models
{
    public static class ImagePropsUtil
    {
        public class ImageMeta
        {
            public short ImageWidth { get; set; }
            public short ImageHeight { get; set; }
            public string EquipMake { get; set; }
            public string EquipModel { get; set; }
            public short Orientation { get; set; }
            public float XResolution { get; set; }
            public float YResolution { get; set; }

            public string ResolutionUnit { get; set; }

            public string SoftwareUsed { get; set; }

            public string DateTime { get; set; }

            public short YCbCrPosition { get; set; }

            public string ExifExposureTime { get; set; }

            public float ExifFNumber { get; set; }

            public short ExifExposureProg { get; set; }

            public short ExifISOSpeed { get; set; }

            public string ExifDTOrig { get; set; }

            public string ExifDTDigitized { get; set; }

            public float ExifShutterSpeed { get; set; }

            public float ExifAperture { get; set; }

            public float ExifBrigthness { get; set; }

            public float ExifExposureBias { get; set; }

            public float ExifMaxAperture { get; set; }

            public short ExifMeteringMode { get; set; }

            public short ExifFlash { get; set; }

            public float ExifFocalLength { get; set; }

            public string ExifDTSubsec { get; set; }

            public string ExifDTOrigSS { get; set; }

            public string ExifDTDigSS { get; set; }

            public short ExifColorSpace { get; set; }

            public long ExifPixXDim { get; set; }

            public long ExifPixYDim { get; set; }

            public short ExifSensingMethod { get; set; }

            public short ExifSceneType { get; set; }

            public float GpsLongitude { get; set; }

            public string GpsLongitudeRef { get; set; }

            public float GpsLatitude { get; set; }

            public string GpsLatitudeRef { get; set; }

            public float GpsAltitude { get; set; }

            public string JsonMeta { get; set; }
        }

        protected class ImageProps
        {
            public string ItemNum { get; set; }
            public string Id { get; set; }
            public string Type { get; set; }
            public string Length { get; set; }
            public string TypeString { get; set; }
            public string Desc { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/xddt0dz7%28v=vs.110%29.aspx
        /// </summary>
        public static void PrintProperties(this Image image)
        {
            // Get the PropertyItems property from image.
            var propItems = image.PropertyItems;

            // For each PropertyItem in the array, display the ID, type, and length.
            int count = 0;
            foreach (var propItem in propItems)
            {
                count++;
                // raw data
                Console.WriteLine("Property Item " + count.ToString());
                Console.WriteLine("   Id: 0x" + propItem.Id.ToString("x"));
                Console.WriteLine("   Type: " + propItem.Type.ToString());
                Console.WriteLine("   Length: " + propItem.Len.ToString() + " bytes");

                // human readable string
                Console.WriteLine("   Type String: " + PropItemToTypeString[propItem.Type]);
                Console.WriteLine("   Description: " + (PropItemIdToDescriotionString.ContainsKey(propItem.Id) ? PropItemIdToDescriotionString[propItem.Id] : "Unknown"));
                Console.WriteLine("   Value      : " + propItem.ReadPropertValueAsString().Trim(100));
            }
        }

        public static string GetProperties(this Image image)
        {
            List<ImageProps> list = new List<ImageProps>();

            // Get the PropertyItems property from image.
            var propItems = image.PropertyItems;

            ImageMeta meta = new ImageMeta();

            // For each PropertyItem in the array, display the ID, type, and length.
            int count = 0;
            foreach (var propItem in propItems)
            {
                count++;

                ImageProps imgProp = new ImageProps();
                imgProp.ItemNum = count.ToString();
                imgProp.Id = propItem.Id.ToString("x");
                imgProp.Type = propItem.Type.ToString();
                imgProp.Length = propItem.Len.ToString() + " bytes";
                imgProp.TypeString = PropItemToTypeString[propItem.Type];
                imgProp.Desc = (PropItemIdToDescriotionString.ContainsKey(propItem.Id) ? PropItemIdToDescriotionString[propItem.Id] : "Unknown");
                imgProp.Value = propItem.ReadPropertValueAsString().Trim(100);

                list.Add(imgProp);

                // Set the meta data to the object
                DecodePropertyItem(propItem, ref meta);
            }

            meta.JsonMeta = JsonConvert.SerializeObject(list);

            return JsonConvert.SerializeObject(meta);
        }

        /// <summary>
        /// Get the meta data from the image.
        /// </summary>
        /// <param name="file">File from the API.</param>
        /// <returns>JSON String.</returns>
        public static async Task<string> GetProperties(IFormFile file)
        {
            try
            {
                using (var imageMemoryStream = new MemoryStream())
                {

                    // Add the file to the memory stream
                    await file.CopyToAsync(imageMemoryStream);

                    using (Bitmap bitmap = new Bitmap(imageMemoryStream))
                    {
                        return GetProperties(bitmap);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }

            return "";
        }

        /// <summary>
        /// Decode the property value to an actual value.
        /// </summary>
        /// <param name="propItem">Property item of the image.</param>
        /// <param name="meta">Reference to meta object.</param>
        public static void DecodePropertyItem(PropertyItem propItem, ref ImageMeta meta)
        {
            try
            {
                string value = propItem.ReadPropertValueAsString().Trim(100);

                switch (propItem.Id)
                {
                    case 0x0100:
                        meta.ImageWidth = short.Parse(value);
                        break;
                    case 0x0101:
                        meta.ImageHeight = short.Parse(value);
                        break;
                    case 0x010f:
                        meta.EquipMake = value;
                        break;
                    case 0x0110:
                        meta.EquipModel = value;
                        break;
                    case 0x0112:
                        meta.Orientation = short.Parse(value);
                        break;
                    case 0x011a:
                        string[] splitStr_XRes = value.Split(',');
                        int val1_XRes = int.Parse(splitStr_XRes[0]);
                        int val2_XRes = int.Parse(splitStr_XRes[1]);
                        meta.XResolution = val1_XRes / val2_XRes;
                        break;
                    case 0x011b:
                        string[] splitStr_YRes = value.Split(',');
                        int val1_YRes = int.Parse(splitStr_YRes[0]);
                        int val2_YRes = int.Parse(splitStr_YRes[1]);
                        meta.YResolution = val1_YRes / val2_YRes;
                        break;
                    case 0x0128:
                        meta.ResolutionUnit = value;
                        break;
                    case 0x0131:
                        meta.SoftwareUsed = value;
                        break;
                    case 0x0132:
                        meta.DateTime = value;
                        break;
                    case 0x0213:
                        meta.YCbCrPosition = short.Parse(value);
                        break;
                    case 0x829a:
                        string[] splitStr_ExpTime = value.Split(',');
                        meta.ExifExposureTime = splitStr_ExpTime[0] + "/" + splitStr_ExpTime[1];
                        break;
                    case 0x829d:
                        string[] splitStr_FNum = value.Split(',');
                        int val1_FNum = int.Parse(splitStr_FNum[0]);
                        int val2_FNum = int.Parse(splitStr_FNum[1]);
                        meta.ExifFNumber = val1_FNum / val2_FNum;
                        break;
                    case 0x8822:
                        meta.ExifExposureProg = short.Parse(value);
                        break;
                    case 0x8827:
                        meta.ExifISOSpeed = short.Parse(value);
                        break;
                    case 0x9003:
                        meta.ExifDTOrig = value;
                        break;
                    case 0x9004:
                        meta.ExifDTDigitized = value;
                        break;
                    case 0x9201:
                        string[] splitStr_ShutSpd = value.Split(',');
                        int val1_ShutSpd = int.Parse(splitStr_ShutSpd[0]);
                        int val2_ShutSpd = int.Parse(splitStr_ShutSpd[1]);
                        meta.ExifShutterSpeed = val1_ShutSpd / val2_ShutSpd;
                        break;
                    case 0x9202:
                        string[] splitStr_Apt = value.Split(',');
                        int val1_Apt = int.Parse(splitStr_Apt[0]);
                        int val2_Apt = int.Parse(splitStr_Apt[1]);
                        meta.ExifAperture = val1_Apt / val2_Apt;
                        break;
                    case 0x9203:
                        string[] splitStr_Bright = value.Split(',');
                        int val1_Bright = int.Parse(splitStr_Bright[0]);
                        int val2_Bright = int.Parse(splitStr_Bright[1]);
                        meta.ExifBrigthness = val1_Bright / val2_Bright;
                        break;
                    case 0x9204:
                        string[] splitStr_ExpBias = value.Split(',');
                        int val1_ExpBias = int.Parse(splitStr_ExpBias[0]);
                        int val2_ExpBias = int.Parse(splitStr_ExpBias[1]);
                        meta.ExifExposureBias = val1_ExpBias / val2_ExpBias;
                        break;
                    case 0x9205:
                        string[] splitStr_MaxApt = value.Split(',');
                        int val1_MaxApt = int.Parse(splitStr_MaxApt[0]);
                        int val2_MaxApt = int.Parse(splitStr_MaxApt[1]);
                        meta.ExifMaxAperture = val1_MaxApt / val2_MaxApt;
                        break;
                    case 0x9207:
                        meta.ExifMeteringMode = short.Parse(value);
                        break;
                    case 0x9209:
                        meta.ExifFlash = short.Parse(value);
                        break;
                    case 0x920a:
                        string[] splitStr_Focal = value.Split(',');
                        int val1_Focal = int.Parse(splitStr_Focal[0]);
                        int val2_Focal = int.Parse(splitStr_Focal[1]);
                        meta.ExifFocalLength = val1_Focal / val2_Focal;
                        break;
                    case 0x9290:
                        meta.ExifDTSubsec = value;
                        break;
                    case 0x9291:
                        meta.ExifDTOrigSS = value;
                        break;
                    case 0x9292:
                        meta.ExifDTDigSS = value;
                        break;
                    case 0xa001:
                        meta.ExifColorSpace = short.Parse(value);
                        break;
                    case 0xa002:
                        meta.ExifPixXDim = long.Parse(value);
                        break;
                    case 0xa003:
                        meta.ExifPixYDim = long.Parse(value);
                        break;
                    case 0x0001:
                        meta.GpsLatitudeRef = value;
                        break;
                    case 0x0002:
                        string[] splitStr_Lat = value.Split(',');
                        int val1_Lat = int.Parse(splitStr_Lat[0]);
                        int val2_Lat = int.Parse(splitStr_Lat[1]);
                        float latDeg = val1_Lat / val2_Lat;

                        int val3_lat = int.Parse(splitStr_Lat[2]);
                        int val4_lat = int.Parse(splitStr_Lat[3]);
                        float latMin = val3_lat / val4_lat;

                        int val5_lat = int.Parse(splitStr_Lat[4]);
                        int val6_lat = int.Parse(splitStr_Lat[5]);
                        float latSec = val5_lat / val6_lat;

                        meta.GpsLatitude = latDeg + (latMin / 60.0f) + (latSec / 3600.0f);
                        break;
                    case 0x0003:
                        meta.GpsLongitudeRef = value;
                        break;
                    case 0x0004:
                        string[] splitStr_Lon = value.Split(',');
                        int val1_Lon = int.Parse(splitStr_Lon[0]);
                        int val2_Lon = int.Parse(splitStr_Lon[1]);
                        float lonDeg = val1_Lon / val2_Lon;

                        int val3_lon = int.Parse(splitStr_Lon[2]);
                        int val4_lon = int.Parse(splitStr_Lon[3]);
                        float lonMin = val3_lon / val4_lon;

                        int val5_lon = int.Parse(splitStr_Lon[4]);
                        int val6_lon = int.Parse(splitStr_Lon[5]);
                        float lonSec = val5_lon / val6_lon;

                        meta.GpsLatitude = lonDeg + (lonMin / 60.0f) + (lonSec / 3600.0f);
                        break;
                    case 0x0006:
                        string[] splitStr_Alt = value.Split(',');
                        int val1_Alt = int.Parse(splitStr_Alt[0]);
                        int val2_Alt = int.Parse(splitStr_Alt[1]);
                        meta.GpsAltitude = val1_Alt / val2_Alt;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }
        }


        const int BYTES = 1;
        const int ASCII = 2;
        const int SHORT = 3;
        const int LONG = 4;
        const int RATIONAL = 5;
        const int SLONG = 9;
        const int SRATIONAL = 10;

        /// <summary>
        /// According to http://msdn.microsoft.com/en-us/library/system.drawing.imaging.propertyitem.type(v=vs.110).aspx
        /// </summary>
        private readonly static Dictionary<int, string> PropItemToTypeString = new Dictionary<int, string>()
        {
            {BYTES,     "Array of bytes"},
            {ASCII,     "ASCII"},
            {SHORT,     "Short (16 bit int)"},
            {LONG,      "Long (32 bit int)"},
            {RATIONAL,  "Rational"},
            {6,         "Not Used"},
            {7,         "Undefined"},
            {8,         "Not Used"},
            {SLONG,     "SLong"},
            {SRATIONAL, "SRational"}
        };

        /// <summary>
        /// According to http://msdn.microsoft.com/de-de/library/ms534416.aspx
        /// </summary>
        private readonly static Dictionary<int, string> PropItemIdToDescriotionString = new Dictionary<int, string>()
        {
            {0x013B, "PropertyTagArtist"},
            {0x0102, "PropertyTagBitsPerSample"},
            {0x0109, "PropertyTagCellHeight"},
            {0x0108, "PropertyTagCellWidth"},
            {0x5091, "PropertyTagChrominanceTable"},
            {0x0140, "PropertyTagColorMap"},
            {0x501A, "PropertyTagColorTransferFunction"},
            {0x0103, "PropertyTagCompression"},
            {0x8298, "PropertyTagCopyright"},
            {0x0132, "PropertyTagDateTime"},
            {0x010D, "PropertyTagDocumentName"},
            {0x0150, "PropertyTagDotRange"},
            {0x010F, "PropertyTagEquipMake"},
            {0x0110, "PropertyTagEquipModel"},
            {0x9202, "PropertyTagExifAperture"},
            {0x9203, "PropertyTagExifBrightness"},
            {0xA302, "PropertyTagExifCfaPattern"},
            {0xA001, "PropertyTagExifColorSpace"},
            {0x9102, "PropertyTagExifCompBPP"},
            {0x9101, "PropertyTagExifCompConfig"},
            {0x9004, "PropertyTagExifDTDigitized"},
            {0x9292, "PropertyTagExifDTDigSS"},
            {0x9003, "PropertyTagExifDTOrig"},
            {0x9291, "PropertyTagExifDTOrigSS"},
            {0x9290, "PropertyTagExifDTSubsec"},
            {0x9204, "PropertyTagExifExposureBias"},
            {0xA215, "PropertyTagExifExposureIndex"},
            {0x8822, "PropertyTagExifExposureProg"},
            {0x829A, "PropertyTagExifExposureTime"},
            {0xA300, "PropertyTagExifFileSource"},
            {0x9209, "PropertyTagExifFlash"},
            {0xA20B, "PropertyTagExifFlashEnergy"},
            {0x829D, "PropertyTagExifFNumber"},
            {0x920A, "PropertyTagExifFocalLength"},
            {0xA210, "PropertyTagExifFocalResUnit"},
            {0xA20E, "PropertyTagExifFocalXRes"},
            {0xA20F, "PropertyTagExifFocalYRes"},
            {0xA000, "PropertyTagExifFPXVer"},
            {0x8769, "PropertyTagExifIFD"},
            {0xA005, "PropertyTagExifInterop"},
            {0x8827, "PropertyTagExifISOSpeed"},
            {0x9208, "PropertyTagExifLightSource"},
            {0x927C, "PropertyTagExifMakerNote"},
            {0x9205, "PropertyTagExifMaxAperture"},
            {0x9207, "PropertyTagExifMeteringMode"},
            {0x8828, "PropertyTagExifOECF"},
            {0xA002, "PropertyTagExifPixXDim"},
            {0xA003, "PropertyTagExifPixYDim"},
            {0xA004, "PropertyTagExifRelatedWav"},
            {0xA301, "PropertyTagExifSceneType"},
            {0xA217, "PropertyTagExifSensingMethod"},
            {0x9201, "PropertyTagExifShutterSpeed"},
            {0xA20C, "PropertyTagExifSpatialFR"},
            {0x8824, "PropertyTagExifSpectralSense"},
            {0x9206, "PropertyTagExifSubjectDist"},
            {0xA214, "PropertyTagExifSubjectLoc"},
            {0x9286, "PropertyTagExifUserComment"},
            {0x9000, "PropertyTagExifVer"},
            {0x0152, "PropertyTagExtraSamples"},
            {0x010A, "PropertyTagFillOrder"},
            {0x5100, "PropertyTagFrameDelay"},
            {0x0121, "PropertyTagFreeByteCounts"},
            {0x0120, "PropertyTagFreeOffset"},
            {0x0301, "PropertyTagGamma"},
            {0x5102, "PropertyTagGlobalPalette"},
            {0x0006, "PropertyTagGpsAltitude"},
            {0x0005, "PropertyTagGpsAltitudeRef"},
            {0x0018, "PropertyTagGpsDestBear"},
            {0x0017, "PropertyTagGpsDestBearRef"},
            {0x001A, "PropertyTagGpsDestDist"},
            {0x0019, "PropertyTagGpsDestDistRef"},
            {0x0014, "PropertyTagGpsDestLat"},
            {0x0013, "PropertyTagGpsDestLatRef"},
            {0x0016, "PropertyTagGpsDestLong"},
            {0x0015, "PropertyTagGpsDestLongRef"},
            {0x000B, "PropertyTagGpsGpsDop"},
            {0x000A, "PropertyTagGpsGpsMeasureMode"},
            {0x0008, "PropertyTagGpsGpsSatellites"},
            {0x0009, "PropertyTagGpsGpsStatus"},
            {0x0007, "PropertyTagGpsGpsTime"},
            {0x8825, "PropertyTagGpsIFD"},
            {0x0011, "PropertyTagGpsImgDir"},
            {0x0010, "PropertyTagGpsImgDirRef"},
            {0x0002, "PropertyTagGpsLatitude"},
            {0x0001, "PropertyTagGpsLatitudeRef"},
            {0x0004, "PropertyTagGpsLongitude"},
            {0x0003, "PropertyTagGpsLongitudeRef"},
            {0x0012, "PropertyTagGpsMapDatum"},
            {0x000D, "PropertyTagGpsSpeed"},
            {0x000C, "PropertyTagGpsSpeedRef"},
            {0x000F, "PropertyTagGpsTrack"},
            {0x000E, "PropertyTagGpsTrackRef"},
            {0x0000, "PropertyTagGpsVer"},
            {0x0123, "PropertyTagGrayResponseCurve"},
            {0x0122, "PropertyTagGrayResponseUnit"},
            {0x5011, "PropertyTagGridSize"},
            {0x500C, "PropertyTagHalftoneDegree"},
            {0x0141, "PropertyTagHalftoneHints"},
            {0x500A, "PropertyTagHalftoneLPI"},
            {0x500B, "PropertyTagHalftoneLPIUnit"},
            {0x500E, "PropertyTagHalftoneMisc"},
            {0x500F, "PropertyTagHalftoneScreen"},
            {0x500D, "PropertyTagHalftoneShape"},
            {0x013C, "PropertyTagHostComputer"},
            {0x8773, "PropertyTagICCProfile"},
            {0x0302, "PropertyTagICCProfileDescriptor"},
            {0x010E, "PropertyTagImageDescription"},
            {0x0101, "PropertyTagImageHeight"},
            {0x0320, "PropertyTagImageTitle"},
            {0x0100, "PropertyTagImageWidth"},
            {0x5103, "PropertyTagIndexBackground"},
            {0x5104, "PropertyTagIndexTransparent"},
            {0x014D, "PropertyTagInkNames"},
            {0x014C, "PropertyTagInkSet"},
            {0x0209, "PropertyTagJPEGACTables"},
            {0x0208, "PropertyTagJPEGDCTables"},
            {0x0201, "PropertyTagJPEGInterFormat"},
            {0x0202, "PropertyTagJPEGInterLength"},
            {0x0205, "PropertyTagJPEGLosslessPredictors"},
            {0x0206, "PropertyTagJPEGPointTransforms"},
            {0x0200, "PropertyTagJPEGProc"},
            {0x0207, "PropertyTagJPEGQTables"},
            {0x5010, "PropertyTagJPEGQuality"},
            {0x0203, "PropertyTagJPEGRestartInterval"},
            {0x5101, "PropertyTagLoopCount"},
            {0x5090, "PropertyTagLuminanceTable"},
            {0x0119, "PropertyTagMaxSampleValue"},
            {0x0118, "PropertyTagMinSampleValue"},
            {0x00FE, "PropertyTagNewSubfileType"},
            {0x014E, "PropertyTagNumberOfInks"},
            {0x0112, "PropertyTagOrientation"},
            {0x011D, "PropertyTagPageName"},
            {0x0129, "PropertyTagPageNumber"},
            {0x5113, "PropertyTagPaletteHistogram"},
            {0x0106, "PropertyTagPhotometricInterp"},
            {0x5111, "PropertyTagPixelPerUnitX"},
            {0x5112, "PropertyTagPixelPerUnitY"},
            {0x5110, "PropertyTagPixelUnit"},
            {0x011C, "PropertyTagPlanarConfig"},
            {0x013D, "PropertyTagPredictor"},
            {0x013F, "PropertyTagPrimaryChromaticities"},
            {0x5005, "PropertyTagPrintFlags"},
            {0x5008, "PropertyTagPrintFlagsBleedWidth"},
            {0x5009, "PropertyTagPrintFlagsBleedWidthScale"},
            {0x5007, "PropertyTagPrintFlagsCrop"},
            {0x5006, "PropertyTagPrintFlagsVersion"},
            {0x0214, "PropertyTagREFBlackWhite"},
            {0x0128, "PropertyTagResolutionUnit"},
            {0x5003, "PropertyTagResolutionXLengthUnit"},
            {0x5001, "PropertyTagResolutionXUnit"},
            {0x5004, "PropertyTagResolutionYLengthUnit"},
            {0x5002, "PropertyTagResolutionYUnit"},
            {0x0116, "PropertyTagRowsPerStrip"},
            {0x0153, "PropertyTagSampleFormat"},
            {0x0115, "PropertyTagSamplesPerPixel"},
            {0x0155, "PropertyTagSMaxSampleValue"},
            {0x0154, "PropertyTagSMinSampleValue"},
            {0x0131, "PropertyTagSoftwareUsed"},
            {0x0303, "PropertyTagSRGBRenderingIntent"},
            {0x0117, "PropertyTagStripBytesCount"},
            {0x0111, "PropertyTagStripOffsets"},
            {0x00FF, "PropertyTagSubfileType"},
            {0x0124, "PropertyTagT4Option"},
            {0x0125, "PropertyTagT6Option"},
            {0x0151, "PropertyTagTargetPrinter"},
            {0x0107, "PropertyTagThreshHolding"},
            {0x5034, "PropertyTagThumbnailArtist"},
            {0x5022, "PropertyTagThumbnailBitsPerSample"},
            {0x5015, "PropertyTagThumbnailColorDepth"},
            {0x5019, "PropertyTagThumbnailCompressedSize"},
            {0x5023, "PropertyTagThumbnailCompression"},
            {0x503B, "PropertyTagThumbnailCopyRight"},
            {0x501B, "PropertyTagThumbnailData"},
            {0x5033, "PropertyTagThumbnailDateTime"},
            {0x5026, "PropertyTagThumbnailEquipMake"},
            {0x5027, "PropertyTagThumbnailEquipModel"},
            {0x5012, "PropertyTagThumbnailFormat"},
            {0x5014, "PropertyTagThumbnailHeight"},
            {0x5025, "PropertyTagThumbnailImageDescription"},
            {0x5021, "PropertyTagThumbnailImageHeight"},
            {0x5020, "PropertyTagThumbnailImageWidth"},
            {0x5029, "PropertyTagThumbnailOrientation"},
            {0x5024, "PropertyTagThumbnailPhotometricInterp"},
            {0x502F, "PropertyTagThumbnailPlanarConfig"},
            {0x5016, "PropertyTagThumbnailPlanes"},
            {0x5036, "PropertyTagThumbnailPrimaryChromaticities"},
            {0x5017, "PropertyTagThumbnailRawBytes"},
            {0x503A, "PropertyTagThumbnailRefBlackWhite"},
            {0x5030, "PropertyTagThumbnailResolutionUnit"},
            {0x502D, "PropertyTagThumbnailResolutionX"},
            {0x502E, "PropertyTagThumbnailResolutionY"},
            {0x502B, "PropertyTagThumbnailRowsPerStrip"},
            {0x502A, "PropertyTagThumbnailSamplesPerPixel"},
            {0x5018, "PropertyTagThumbnailSize"},
            {0x5032, "PropertyTagThumbnailSoftwareUsed"},
            {0x502C, "PropertyTagThumbnailStripBytesCount"},
            {0x5028, "PropertyTagThumbnailStripOffsets"},
            {0x5031, "PropertyTagThumbnailTransferFunction"},
            {0x5035, "PropertyTagThumbnailWhitePoint"},
            {0x5013, "PropertyTagThumbnailWidth"},
            {0x5037, "PropertyTagThumbnailYCbCrCoefficients"},
            {0x5039, "PropertyTagThumbnailYCbCrPositioning"},
            {0x5038, "PropertyTagThumbnailYCbCrSubsampling"},
            {0x0145, "PropertyTagTileByteCounts"},
            {0x0143, "PropertyTagTileLength"},
            {0x0144, "PropertyTagTileOffset"},
            {0x0142, "PropertyTagTileWidth"},
            {0x012D, "PropertyTagTransferFunction"},
            {0x0156, "PropertyTagTransferRange"},
            {0x013E, "PropertyTagWhitePoint"},
            {0x011E, "PropertyTagXPosition"},
            {0x011A, "PropertyTagXResolution"},
            {0x0211, "PropertyTagYCbCrCoefficients"},
            {0x0213, "PropertyTagYCbCrPositioning"},
            {0x0212, "PropertyTagYCbCrSubsampling"},
            {0x011F, "PropertyTagYPosition"},
            {0x011B, "PropertyTagYResolution"},
        };

        public static string ReadPropertValueAsString(this PropertyItem propItem)
        {
            switch (propItem.Type)
            {
                case BYTES:
                    {
                        return propItem.Value.ToCommaSeparatedString();
                    }
                case ASCII:
                    {
                        return System.Text.Encoding.ASCII.GetString(propItem.Value).TrimEnd('\0');
                    }
                case SHORT:
                    {
                        var value = propItem.Value;
                        var ret = new ushort[propItem.Len / 2];
                        for (int i = 0; i < propItem.Len; i += 2)
                        {
                            ret[i / 2] = (ushort)(value[i + 1] << 8 | value[i]);
                        }
                        return ret.ToCommaSeparatedString();
                    }
                case LONG:
                case RATIONAL:
                    {
                        var value = propItem.Value;
                        var ret = new uint[propItem.Len / 4];
                        for (int i = 0; i < propItem.Len; i += 4)
                        {
                            ret[i / 4] = (uint)(value[i + 3] << 24 | value[i + 2] << 16 | value[i + 1] << 8 | value[i]);
                        }
                        return ret.ToCommaSeparatedString();
                    }
                case SLONG:
                case SRATIONAL:
                    {
                        var value = propItem.Value;
                        var ret = new int[propItem.Len / 4];
                        for (int i = 0; i < propItem.Len; i += 4)
                        {
                            ret[i / 4] = value[i + 3] << 24 | value[i + 2] << 16 | value[i + 1] << 8 | value[i];
                        }
                        return ret.ToCommaSeparatedString();
                    }
                default:
                    return "";
            }
        }

        private static string ToCommaSeparatedString<T>(this ICollection<T> collection)
        {
            return collection != null ? string.Join(",", collection.ToArray()) : null;
        }

        private static string Trim(this string text, int limit, string suffix = "...")
        {
            if (text.Length > limit)
            {
                return text.Substring(0, limit) + suffix;
            }
            return text;
        }
    }
}
