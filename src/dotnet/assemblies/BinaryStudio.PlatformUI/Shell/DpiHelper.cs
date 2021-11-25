using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Rectangle = System.Drawing.Rectangle;

namespace BinaryStudio.PlatformUI
    {
    public static class DpiHelper
        {
        public class DpiHelperImplementation : Utilities.DPI.DpiHelper
            {
            //private SettingsStore _settingsStore;

            //protected SettingsStore SettingsStore
            //{
            //	get
            //	{
            //		if (this._settingsStore == null)
            //		{
            //			ShellSettingsManager shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            //			this._settingsStore = shellSettingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            //		}
            //		return this._settingsStore;
            //	}
            //}

            public DpiHelperImplementation() : base(96.0)
                {
                }

            //protected override ImageScalingMode GetImageScalingModeOverride(int dpiScalePercent, ImageScalingMode defaultImageScalingMode)
            //{
            //	string propertyName = string.Format("ImageScaling{0}", dpiScalePercent);
            //	return (ImageScalingMode)this.SettingsStore.GetInt32("General", propertyName, (int)defaultImageScalingMode);
            //}

            //protected override BitmapScalingMode GetBitmapScalingModeOverride(int dpiScalePercent, BitmapScalingMode defaultBitmapScalingMode)
            //{
            //	string propertyName = string.Format("BitmapScaling{0}", dpiScalePercent);
            //	return (BitmapScalingMode)this.SettingsStore.GetInt32("General", propertyName, (int)defaultBitmapScalingMode);
            //}

            //protected override bool GetUsePreScaledImagesOverride(int dpiScalePercent, bool defaultUsePreScaledImages)
            //{
            //	string propertyName = string.Format("UseBitmapPreScaling{0}", dpiScalePercent);
            //	return this.SettingsStore.GetBoolean("General", propertyName, defaultUsePreScaledImages);
            //}
            }

        /// <summary>
        /// Gets an instance of DpiHelper.
        /// </summary>
        public static DpiHelperImplementation Instance
            {
            get;
            private set;
            }

        /// <summary>
        /// Gets the ImageScalingMode algorithm to be used for resizing images in WinForms/Win32. This allows the shell to control the algorithm depending on the dpi zoom scale, and allows the user to override it via registry settings like General\ImageScalingXXX = (ImageScalingMode)value, with XXX the zoom factor in percents, e.g. ImageScaling150, etc.
        /// </summary>
        public static ImageScalingMode ImageScalingMode
            {
            get
                {
                return Instance.ImageScalingMode;
                }
            }

        /// <summary>
        /// Gets the BitmapScalingMode algorithm to be used for resizing images in WPF. This allows the shell to control the algorithm depending on the dpi zoom scale, and allows the user to override it via registry settings like General\BitmapScalingXXX = (BitmapScalingMode)value, with XXX the zoom factor in percents, e.g. BitmapScaling150, etc.
        /// </summary>
        public static BitmapScalingMode BitmapScalingMode
            {
            get
                {
                return Instance.BitmapScalingMode;
                }
            }

        /// <summary>
        /// Determines whether images should be pre-scaled at zoom levels higher than 200%, using NearestNeighbor up to the largest multiple of 100%.
        /// </summary>
        public static Boolean UsePreScaledImages
            {
            get
                {
                return Instance.UsePreScaledImages;
                }
            }

        public static MatrixTransform TransformFromDevice
            {
            get
                {
                return Instance.TransformFromDevice;
                }
            }

        public static MatrixTransform TransformToDevice
            {
            get
                {
                return Instance.TransformToDevice;
                }
            }

        public static Double DeviceDpiX
            {
            get
                {
                return Instance.DeviceDpiX;
                }
            }

        public static Double DeviceDpiY
            {
            get
                {
                return Instance.DeviceDpiY;
                }
            }

        /// <summary>
        /// Gets the logical DPI for the horizontal coordinates.
        /// </summary>
        public static Double LogicalDpiX
            {
            get
                {
                return Instance.LogicalDpiX;
                }
            }

        /// <summary>
        /// Gets the logical DPI for the vertical coordinates.
        /// </summary>
        public static Double LogicalDpiY
            {
            get
                {
                return Instance.LogicalDpiY;
                }
            }

        /// <summary>
        /// Determines whether scaling is required when converting between logical-device units.
        /// </summary>
        public static Boolean IsScalingRequired
            {
            get
                {
                return Instance.IsScalingRequired;
                }
            }

        public static Double DeviceToLogicalUnitsScalingFactorX
            {
            get
                {
                return Instance.DeviceToLogicalUnitsScalingFactorX;
                }
            }

        public static Double DeviceToLogicalUnitsScalingFactorY
            {
            get
                {
                return Instance.DeviceToLogicalUnitsScalingFactorY;
                }
            }

        public static Double LogicalToDeviceUnitsScalingFactorX
            {
            get
                {
                return Instance.LogicalToDeviceUnitsScalingFactorX;
                }
            }

        public static Double LogicalToDeviceUnitsScalingFactorY
            {
            get
                {
                return Instance.LogicalToDeviceUnitsScalingFactorY;
                }
            }

        /// <summary>
        /// Gets the DPI scale percent for the horizontal scale.
        /// </summary>
        public static Int32 DpiScalePercentX
            {
            get
                {
                return Instance.DpiScalePercentX;
                }
            }

        /// <summary>
        /// Gets the DPI scale percent for the vertical scale.
        /// </summary>
        public static Int32 DpiScalePercentY
            {
            get
                {
                return Instance.DpiScalePercentY;
                }
            }

        /// <summary>
        /// Gets the horizontal scale value that should be used with a LayoutTransform/ScaleTransform to scale back an image pre-scaled in HighDPI with DpiPrescaleImageSourceConverter in order to obtain crisp results.
        /// </summary>
        public static Double PreScaledImageLayoutTransformScaleX
            {
            get
                {
                return Instance.PreScaledImageLayoutTransformScaleX;
                }
            }

        /// <summary>
        /// Gets the vertical scale value that should be used with a LayoutTransform/ScaleTransform to scale back an image pre-scaled in HighDPI with DpiPrescaleImageSourceConverter in order to obtain crisp results.
        /// </summary>
        public static Double PreScaledImageLayoutTransformScaleY
            {
            get
                {
                return Instance.PreScaledImageLayoutTransformScaleY;
                }
            }

        static DpiHelper()
            {
            Instance = new DpiHelperImplementation();
            }

        public static Double LogicalToDeviceUnitsX(Double value)
            {
            return Instance.LogicalToDeviceUnitsX(value);
            }

        public static Double LogicalToDeviceUnitsY(Double value)
            {
            return Instance.LogicalToDeviceUnitsY(value);
            }

        /// <summary>
        /// Transforms a horizontal coordinate from device to logical units.
        /// </summary>
        /// <param name="value">The horizontal value in device units.</param>
        /// <returns>The horizontal value in logical units.</returns>
        public static Double DeviceToLogicalUnitsX(Double value)
            {
            return Instance.DeviceToLogicalUnitsX(value);
            }

        /// <summary>
        /// Transforms a vertical coordinate from device to logical units.
        /// </summary>
        /// <param name="value">The vertical value in device units.</param>
        /// <returns>The vertical value in logical units.</returns>
        public static Double DeviceToLogicalUnitsY(Double value)
            {
            return Instance.DeviceToLogicalUnitsY(value);
            }

        public static Single LogicalToDeviceUnitsX(Single value)
            {
            return Instance.LogicalToDeviceUnitsX(value);
            }

        public static Single LogicalToDeviceUnitsY(Single value)
            {
            return Instance.LogicalToDeviceUnitsY(value);
            }

        public static Int32 LogicalToDeviceUnitsX(Int32 value)
            {
            return Instance.LogicalToDeviceUnitsX(value);
            }

        public static Int32 LogicalToDeviceUnitsY(Int32 value)
            {
            return Instance.LogicalToDeviceUnitsY(value);
            }

        /// <summary>
        /// Transforms a horizontal coordinate from device to logical units.
        /// </summary>
        /// <param name="value">The horizontal value in device units.</param>
        /// <returns>The horizontal value in logical units.</returns>
        public static Single DeviceToLogicalUnitsX(Single value)
            {
            return Instance.DeviceToLogicalUnitsX(value);
            }

        /// <summary>
        /// Transforms a vertical coordinate from device to logical units.
        /// </summary>
        /// <param name="value">The vertical value in device units.</param>
        /// <returns>The vertical value in logical units.</returns>
        public static Single DeviceToLogicalUnitsY(Single value)
            {
            return Instance.DeviceToLogicalUnitsY(value);
            }

        /// <summary>
        /// Transforms a horizontal coordinate from device to logical units.
        /// </summary>
        /// <param name="value">The horizontal value in device units.</param>
        /// <returns>The horizontal value in logical units.</returns>
        public static Int32 DeviceToLogicalUnitsX(Int32 value)
            {
            return Instance.DeviceToLogicalUnitsX(value);
            }

        /// <summary>
        /// Transforms a vertical coordinate from device to logical units.
        /// </summary>
        /// <param name="value">The vertical value in device units.</param>
        /// <returns>The vertical value in logical units.</returns>
        public static Int32 DeviceToLogicalUnitsY(Int32 value)
            {
            return Instance.DeviceToLogicalUnitsY(value);
            }

        public static Double RoundToDeviceUnitsX(Double value)
            {
            return Instance.RoundToDeviceUnitsX(value);
            }

        public static Double RoundToDeviceUnitsY(Double value)
            {
            return Instance.RoundToDeviceUnitsY(value);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="logicalPoint">The point in logical units.</param>
        /// <returns>The device units.</returns>
        public static System.Windows.Point LogicalToDeviceUnits(this System.Windows.Point logicalPoint)
            {
            return Instance.LogicalToDeviceUnits(logicalPoint);
            }

        public static Rect LogicalToDeviceUnits(this Rect logicalRect)
            {
            return Instance.LogicalToDeviceUnits(logicalRect);
            }

        public static System.Windows.Size LogicalToDeviceUnits(this System.Windows.Size logicalSize)
            {
            return Instance.LogicalToDeviceUnits(logicalSize);
            }

        public static Thickness LogicalToDeviceUnits(this Thickness logicalThickness)
            {
            return Instance.LogicalToDeviceUnits(logicalThickness);
            }

        public static System.Windows.Point DeviceToLogicalUnits(this System.Windows.Point devicePoint)
            {
            return Instance.DeviceToLogicalUnits(devicePoint);
            }

        public static Rect DeviceToLogicalUnits(this Rect deviceRect)
            {
            return Instance.DeviceToLogicalUnits(deviceRect);
            }

        public static System.Windows.Size DeviceToLogicalUnits(this System.Windows.Size deviceSize)
            {
            return Instance.DeviceToLogicalUnits(deviceSize);
            }

        /// <summary>
        /// Converts the specified measurement to logical units.
        /// </summary>
        /// <param name="deviceThickness">The device thickness.</param>
        /// <returns>The logical units.</returns>
        public static Thickness DeviceToLogicalUnits(this Thickness deviceThickness)
            {
            return Instance.DeviceToLogicalUnits(deviceThickness);
            }

        public static void SetDeviceLeft(this Window window, Double deviceLeft)
            {
            Instance.SetDeviceLeft(ref window, deviceLeft);
            }

        public static Double GetDeviceLeft(this Window window)
            {
            return Instance.GetDeviceLeft(window);
            }

        public static void SetDeviceTop(this Window window, Double deviceTop)
            {
            Instance.SetDeviceTop(ref window, deviceTop);
            }

        public static Double GetDeviceTop(this Window window)
            {
            return Instance.GetDeviceTop(window);
            }

        public static void SetDeviceWidth(this Window window, Double deviceWidth)
            {
            Instance.SetDeviceWidth(ref window, deviceWidth);
            }

        public static Double GetDeviceWidth(this Window window)
            {
            return Instance.GetDeviceWidth(window);
            }

        public static void SetDeviceHeight(this Window window, Double deviceHeight)
            {
            Instance.SetDeviceHeight(ref window, deviceHeight);
            }

        public static Double GetDeviceHeight(this Window window)
            {
            return Instance.GetDeviceHeight(window);
            }

        public static Rect GetDeviceRect(this Window window)
            {
            return Instance.GetDeviceRect(window);
            }

        public static System.Windows.Size GetDeviceActualSize(this FrameworkElement element)
            {
            return Instance.GetDeviceActualSize(element);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="logicalPoint">A logical point.</param>
        /// <returns>The device units.</returns>
        public static System.Drawing.Point LogicalToDeviceUnits(this System.Drawing.Point logicalPoint)
            {
            return Instance.LogicalToDeviceUnits(logicalPoint);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="logicalSize">The logical size</param>
        /// <returns>The device units.</returns>
        public static System.Drawing.Size LogicalToDeviceUnits(this System.Drawing.Size logicalSize)
            {
            return Instance.LogicalToDeviceUnits(logicalSize);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="logicalRect">The rectangle.</param>
        /// <returns>The device units.</returns>
        public static Rectangle LogicalToDeviceUnits(this Rectangle logicalRect)
            {
            return Instance.LogicalToDeviceUnits(logicalRect);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="logicalPoint">A logical point.</param>
        /// <returns>The device units.</returns>
        public static PointF LogicalToDeviceUnits(this PointF logicalPoint)
            {
            return Instance.LogicalToDeviceUnits(logicalPoint);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="logicalSize">The logical size.</param>
        /// <returns>The device units.</returns>
        public static SizeF LogicalToDeviceUnits(this SizeF logicalSize)
            {
            return Instance.LogicalToDeviceUnits(logicalSize);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="logicalRect">The rectangle.</param>
        /// <returns>The device units.</returns>
        public static RectangleF LogicalToDeviceUnits(this RectangleF logicalRect)
            {
            return Instance.LogicalToDeviceUnits(logicalRect);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="bitmapImage">The bitmap.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref Bitmap bitmapImage, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref bitmapImage, scalingMode);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="bitmapImage">The bitmap.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref Bitmap bitmapImage, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref bitmapImage, backgroundColor, scalingMode);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref Image image, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref image, scalingMode);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref Image image, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref image, backgroundColor, scalingMode);
            }

        /// <summary>
        /// Creates and returns a new bitmap or metafile scaled for the device units. When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalImage">The image to scale from logical units to device units.</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the image.</param>
        /// <returns>The image.</returns>
        public static Image CreateDeviceFromLogicalImage(this Image logicalImage, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            return Instance.CreateDeviceFromLogicalImage(logicalImage, scalingMode);
            }

        public static ImageSource ScaleLogicalImageForDeviceSize(ImageSource image, System.Windows.Size deviceImageSize, BitmapScalingMode scalingMode)
            {
            return Instance.ScaleLogicalImageForDeviceSize(image, deviceImageSize, scalingMode);
            }

        /// <summary>
        /// Creates and returns a new bitmap or metafile scaled for the device units. When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalImage">The image to scale from logical units to device units.</param>
        /// <param name="backgroundColor">A color value to be used for the image background. When the interpolation mode is Bilinear or Bicubic, the image's margins are interpolated with the background.</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the image.</param>
        /// <returns>The image.</returns>
        public static Image CreateDeviceFromLogicalImage(this Image logicalImage, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            return Instance.CreateDeviceFromLogicalImage(logicalImage, backgroundColor, scalingMode);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="imageStrip">The image strip.</param>
        /// <param name="logicalImageSize">The logical image size.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref Bitmap imageStrip, System.Drawing.Size logicalImageSize, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref imageStrip, logicalImageSize, scalingMode);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="imageStrip">The image strip.</param>
        /// <param name="logicalImageSize">The logical image size.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref Bitmap imageStrip, System.Drawing.Size logicalImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref imageStrip, logicalImageSize, backgroundColor, scalingMode);
            }

        /// <summary>
        /// Creates and returns a new bitmap or metafile scaled for the device units. When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalBitmapStrip">The logical bitmap strip.</param>
        /// <param name="logicalImageSize">The size of the logical image.</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the image.</param>
        /// <returns>The image.</returns>
        public static Bitmap CreateDeviceFromLogicalImage(this Bitmap logicalBitmapStrip, System.Drawing.Size logicalImageSize, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            return Instance.CreateDeviceFromLogicalImage(logicalBitmapStrip, logicalImageSize, scalingMode);
            }

        /// <summary>
        /// Creates and returns a new bitmap or metafile scaled for the device units. When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalBitmapStrip">The logical bitmap strip.</param>
        /// <param name="logicalImageSize">The size of the logical image.</param>
        /// <param name="backgroundColor">A color value to be used for the image background. When the interpolation mode is Bilinear or Bicubic, the image's margins are interpolated with the background.</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the image.</param>
        /// <returns>The image.</returns>
        public static Bitmap CreateDeviceFromLogicalImage(this Bitmap logicalBitmapStrip, System.Drawing.Size logicalImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            return Instance.CreateDeviceFromLogicalImage(logicalBitmapStrip, logicalImageSize, backgroundColor, scalingMode);
            }

        /// <summary>
        /// Converts (if necessary) the icon by scaling it to device units. When displayed on the device, the scaled icon will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="icon">The bitmap icon to scale from logical units to device units.</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the icon.</param>
        public static void LogicalToDeviceUnits(ref Icon icon, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref icon, scalingMode);
            }

        /// <summary>
        /// Extension method for System.Drawing.Image that creates and returns a new bitmap or metafile scaled for the device units. When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalIcon">The image to scale from logical units to device units</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the image</param>
        /// <returns>Returns Icon.</returns>
        public static Icon CreateDeviceFromLogicalImage(this Icon logicalIcon, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            return Instance.CreateDeviceFromLogicalImage(logicalIcon, scalingMode);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="imageList">The image list.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref ImageList imageList, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref imageList, scalingMode);
            }

        /// <summary>
        /// Converts the specified coordinates from logical units to device units.
        /// </summary>
        /// <param name="imageList">The image list.</param>
        /// <param name="backgroundColor">The background color.</param>
        /// <param name="scalingMode">The scaling mode.</param>
        public static void LogicalToDeviceUnits(ref ImageList imageList, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            Instance.LogicalToDeviceUnits(ref imageList, backgroundColor, scalingMode);
            }

        /// <summary>
        /// Creates and returns a new bitmap or metafile scaled for the device units. When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalImageList">The image list.</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the image.</param>
        /// <returns>The image.</returns>
        public static ImageList CreateDeviceFromLogicalImage(this ImageList logicalImageList, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            return Instance.CreateDeviceFromLogicalImage(logicalImageList, scalingMode);
            }

        /// <summary>
        /// Creates and returns a new bitmap or metafile scaled for the device units. When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalImageList">The image list.</param>
        /// <param name="backgroundColor">A color value to be used for the image background. When the interpolation mode is Bilinear or Bicubic, the image's margins are interpolated with the background.</param>
        /// <param name="scalingMode">The scaling mode to use when scaling the image.</param>
        /// <returns>The image.</returns>
        public static ImageList CreateDeviceFromLogicalImage(this ImageList logicalImageList, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
            {
            return Instance.CreateDeviceFromLogicalImage(logicalImageList, backgroundColor, scalingMode);
            }
        }
    }