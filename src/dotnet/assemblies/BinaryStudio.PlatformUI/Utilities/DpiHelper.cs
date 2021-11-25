using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BinaryStudio.PlatformUI;
using BinaryStudio.PlatformUI.Shell;
using Rectangle = System.Drawing.Rectangle;
using Image = System.Drawing.Image;

namespace BinaryStudio.Utilities.DPI
    {
public class DpiHelper
	{
		private delegate void PixelProcessor(ref Byte alpha, ref Byte red, ref Byte green, ref Byte blue);

		private static DpiHelper _default;

		protected const Double DefaultLogicalDpi = 96.0;

		private ImageScalingMode _imageScalingMode;

		private BitmapScalingMode _bitmapScalingMode;

		private Boolean? _usePreScaledImages;

		private MatrixTransform transformFromDevice;

		private MatrixTransform transformToDevice;

		private Double _preScaledImageLayoutTransformScaleX;

		private Double _preScaledImageLayoutTransformScaleY;

		public static DpiHelper Default
		{
			get
			{
				if (_default == null)
				{
					_default = GetHelper(100);
				}
				return _default;
			}
		}

		public ImageScalingMode ImageScalingMode
		{
			get
			{
				if (_imageScalingMode == ImageScalingMode.Default)
				{
					var dpiScalePercentX = DpiScalePercentX;
					var defaultImageScalingMode = GetDefaultImageScalingMode(dpiScalePercentX);
					_imageScalingMode = GetImageScalingModeOverride(dpiScalePercentX, defaultImageScalingMode);
					if (!Enum.IsDefined(typeof(ImageScalingMode), _imageScalingMode) || _imageScalingMode == ImageScalingMode.Default)
					{
						_imageScalingMode = defaultImageScalingMode;
					}
				}
				return _imageScalingMode;
			}
		}

		public BitmapScalingMode BitmapScalingMode
		{
			get
			{
				if (_bitmapScalingMode == BitmapScalingMode.Unspecified)
				{
					var dpiScalePercentX = DpiScalePercentX;
					var defaultBitmapScalingMode = GetDefaultBitmapScalingMode(dpiScalePercentX);
					_bitmapScalingMode = GetBitmapScalingModeOverride(dpiScalePercentX, defaultBitmapScalingMode);
					if (!Enum.IsDefined(typeof(BitmapScalingMode), _bitmapScalingMode) || _bitmapScalingMode == BitmapScalingMode.Unspecified)
					{
						_bitmapScalingMode = defaultBitmapScalingMode;
					}
				}
				return _bitmapScalingMode;
			}
		}

		public Boolean UsePreScaledImages
		{
			get
			{
				if (!_usePreScaledImages.HasValue)
				{
					_usePreScaledImages = new Boolean?(GetUsePreScaledImagesOverride(DpiScalePercentX, true));
				}
				return _usePreScaledImages.Value;
			}
		}

		public MatrixTransform TransformFromDevice
		{
			get
			{
				return transformFromDevice;
			}
		}

		public MatrixTransform TransformToDevice
		{
			get
			{
				return transformToDevice;
			}
		}

		public Double DeviceDpiX
		{
			get;
			private set;
		}

		public Double DeviceDpiY
		{
			get;
			private set;
		}

		public Double LogicalDpiX
		{
			get;
			private set;
		}

		public Double LogicalDpiY
		{
			get;
			private set;
		}

		public Boolean IsScalingRequired
		{
			get
			{
				return DeviceDpiX != LogicalDpiX || DeviceDpiY != LogicalDpiY;
			}
		}

		public Double DeviceToLogicalUnitsScalingFactorX
		{
			get
			{
				return TransformFromDevice.Matrix.M11;
			}
		}

		public Double DeviceToLogicalUnitsScalingFactorY
		{
			get
			{
				return TransformFromDevice.Matrix.M22;
			}
		}

		public Double LogicalToDeviceUnitsScalingFactorX
		{
			get
			{
				return TransformToDevice.Matrix.M11;
			}
		}

		public Double LogicalToDeviceUnitsScalingFactorY
		{
			get
			{
				return TransformToDevice.Matrix.M22;
			}
		}

		public Int32 DpiScalePercentX
		{
			get
			{
				return (Int32)Math.Round(LogicalToDeviceUnitsScalingFactorX * 100.0);
			}
		}

		public Int32 DpiScalePercentY
		{
			get
			{
				return (Int32)Math.Round(LogicalToDeviceUnitsScalingFactorY * 100.0);
			}
		}

		public Double PreScaledImageLayoutTransformScaleX
		{
			get
			{
				if (_preScaledImageLayoutTransformScaleX == 0.0)
				{
					if (!UsePreScaledImages)
					{
						_preScaledImageLayoutTransformScaleX = 1.0;
					}
					else
					{
						var dpiScalePercentX = DpiScalePercentX;
						if (dpiScalePercentX < 200)
						{
							_preScaledImageLayoutTransformScaleX = 1.0;
						}
						else
						{
							_preScaledImageLayoutTransformScaleX = 1.0 / (dpiScalePercentX / 100);
						}
					}
				}
				return _preScaledImageLayoutTransformScaleX;
			}
		}

		public Double PreScaledImageLayoutTransformScaleY
		{
			get
			{
				if (_preScaledImageLayoutTransformScaleY == 0.0)
				{
					if (!UsePreScaledImages)
					{
						_preScaledImageLayoutTransformScaleY = 1.0;
					}
					else
					{
						var dpiScalePercentY = DpiScalePercentY;
						if (dpiScalePercentY < 200)
						{
							_preScaledImageLayoutTransformScaleY = 1.0;
						}
						else
						{
							_preScaledImageLayoutTransformScaleY = 1.0 / (dpiScalePercentY / 100);
						}
					}
				}
				return _preScaledImageLayoutTransformScaleY;
			}
		}

		public static DpiHelper GetHelper(Int32 zoomPercent)
		{
			var logicalDpi = 96.0 * zoomPercent / 100.0;
			return new DpiHelper(logicalDpi);
		}

		protected DpiHelper(Double logicalDpi)
		{
			LogicalDpiX = logicalDpi;
			LogicalDpiY = logicalDpi;
			var dC = NativeMethods.GetDC(IntPtr.Zero);
			if (dC != IntPtr.Zero)
			{
				DeviceDpiX = NativeMethods.GetDeviceCaps(dC, 88);
				DeviceDpiY = NativeMethods.GetDeviceCaps(dC, 90);
				NativeMethods.ReleaseDC(IntPtr.Zero, dC);
			}
			else
			{
				DeviceDpiX = LogicalDpiX;
				DeviceDpiY = LogicalDpiY;
			}
			var identity = System.Windows.Media.Matrix.Identity;
			var identity2 = System.Windows.Media.Matrix.Identity;
			identity.Scale(DeviceDpiX / LogicalDpiX, DeviceDpiY / LogicalDpiY);
			identity2.Scale(LogicalDpiX / DeviceDpiX, LogicalDpiY / DeviceDpiY);
			transformFromDevice = new MatrixTransform(identity2);
			transformFromDevice.Freeze();
			transformToDevice = new MatrixTransform(identity);
			transformToDevice.Freeze();
		}

		private ImageScalingMode GetDefaultImageScalingMode(Int32 dpiScalePercent)
		{
			if (dpiScalePercent % 100 == 0)
			{
				return ImageScalingMode.NearestNeighbor;
			}
			if (dpiScalePercent < 100)
			{
				return ImageScalingMode.HighQualityBilinear;
			}
			return ImageScalingMode.MixedNearestNeighborHighQualityBicubic;
		}

		private BitmapScalingMode GetDefaultBitmapScalingMode(Int32 dpiScalePercent)
		{
			if (dpiScalePercent % 100 == 0)
			{
				return BitmapScalingMode.NearestNeighbor;
			}
			if (dpiScalePercent < 100)
			{
				return BitmapScalingMode.LowQuality;
			}
			return BitmapScalingMode.HighQuality;
		}

		protected virtual ImageScalingMode GetImageScalingModeOverride(Int32 dpiScalePercent, ImageScalingMode defaultImageScalingMode)
		{
			return defaultImageScalingMode;
		}

		protected virtual BitmapScalingMode GetBitmapScalingModeOverride(Int32 dpiScalePercent, BitmapScalingMode defaultBitmapScalingMode)
		{
			return defaultBitmapScalingMode;
		}

		protected virtual Boolean GetUsePreScaledImagesOverride(Int32 dpiScalePercent, Boolean defaultUsePreScaledImages)
		{
			return defaultUsePreScaledImages;
		}

		private InterpolationMode GetInterpolationMode(ImageScalingMode scalingMode)
		{
			switch (scalingMode)
			{
			case ImageScalingMode.BorderOnly:
			case ImageScalingMode.NearestNeighbor:
				return InterpolationMode.NearestNeighbor;
			case ImageScalingMode.Bilinear:
				return InterpolationMode.Bilinear;
			case ImageScalingMode.Bicubic:
				return InterpolationMode.Bicubic;
			case ImageScalingMode.HighQualityBilinear:
				return InterpolationMode.HighQualityBilinear;
			case ImageScalingMode.HighQualityBicubic:
				return InterpolationMode.HighQualityBicubic;
			}
			return GetInterpolationMode(ImageScalingMode);
		}

		private ImageScalingMode GetActualScalingMode(ImageScalingMode scalingMode)
		{
			if (scalingMode != ImageScalingMode.Default)
			{
				return scalingMode;
			}
			return ImageScalingMode;
		}

		public Double LogicalToDeviceUnitsX(Double value)
		{
			return value * LogicalToDeviceUnitsScalingFactorX;
		}

		public Double LogicalToDeviceUnitsY(Double value)
		{
			return value * LogicalToDeviceUnitsScalingFactorY;
		}

		public Double DeviceToLogicalUnitsX(Double value)
		{
			return value * DeviceToLogicalUnitsScalingFactorX;
		}

		public Double DeviceToLogicalUnitsY(Double value)
		{
			return value * DeviceToLogicalUnitsScalingFactorY;
		}

		public Single LogicalToDeviceUnitsX(Single value)
		{
			return (Single)LogicalToDeviceUnitsX((Double)value);
		}

		public Single LogicalToDeviceUnitsY(Single value)
		{
			return (Single)LogicalToDeviceUnitsY((Double)value);
		}

		public Int32 LogicalToDeviceUnitsX(Int32 value)
		{
			return (Int32)Math.Round(LogicalToDeviceUnitsX((Double)value));
		}

		public Int32 LogicalToDeviceUnitsY(Int32 value)
		{
			return (Int32)Math.Round(LogicalToDeviceUnitsY((Double)value));
		}

		public Single DeviceToLogicalUnitsX(Single value)
		{
			return (Single)(value * DeviceToLogicalUnitsScalingFactorX);
		}

		public Single DeviceToLogicalUnitsY(Single value)
		{
			return (Single)(value * DeviceToLogicalUnitsScalingFactorY);
		}

		public Int32 DeviceToLogicalUnitsX(Int32 value)
		{
			return (Int32)Math.Round(value * DeviceToLogicalUnitsScalingFactorX);
		}

		public Int32 DeviceToLogicalUnitsY(Int32 value)
		{
			return (Int32)Math.Round(value * DeviceToLogicalUnitsScalingFactorY);
		}

		public Double RoundToDeviceUnitsX(Double value)
		{
			return DeviceToLogicalUnitsX(Math.Round(LogicalToDeviceUnitsX(value)));
		}

		public Double RoundToDeviceUnitsY(Double value)
		{
			return DeviceToLogicalUnitsY(Math.Round(LogicalToDeviceUnitsY(value)));
		}

		public System.Windows.Point LogicalToDeviceUnits(System.Windows.Point logicalPoint)
		{
			return TransformToDevice.Transform(logicalPoint);
		}

		public Rect LogicalToDeviceUnits(Rect logicalRect)
		{
			var result = logicalRect;
			result.Transform(TransformToDevice.Matrix);
			return result;
		}

		public System.Windows.Size LogicalToDeviceUnits(System.Windows.Size logicalSize)
		{
			return new System.Windows.Size(logicalSize.Width * LogicalToDeviceUnitsScalingFactorX, logicalSize.Height * LogicalToDeviceUnitsScalingFactorY);
		}

		public Thickness LogicalToDeviceUnits(Thickness logicalThickness)
		{
			return new Thickness(logicalThickness.Left * LogicalToDeviceUnitsScalingFactorX, logicalThickness.Top * LogicalToDeviceUnitsScalingFactorY, logicalThickness.Right * LogicalToDeviceUnitsScalingFactorX, logicalThickness.Bottom * LogicalToDeviceUnitsScalingFactorY);
		}

		public System.Windows.Point DeviceToLogicalUnits(System.Windows.Point devicePoint)
		{
			return TransformFromDevice.Transform(devicePoint);
		}

		public Rect DeviceToLogicalUnits(Rect deviceRect)
		{
			var result = deviceRect;
			result.Transform(TransformFromDevice.Matrix);
			return result;
		}

		public System.Windows.Size DeviceToLogicalUnits(System.Windows.Size deviceSize)
		{
			return new System.Windows.Size(deviceSize.Width * DeviceToLogicalUnitsScalingFactorX, deviceSize.Height * DeviceToLogicalUnitsScalingFactorY);
		}

		public Thickness DeviceToLogicalUnits(Thickness deviceThickness)
		{
			return new Thickness(deviceThickness.Left * DeviceToLogicalUnitsScalingFactorX, deviceThickness.Top * DeviceToLogicalUnitsScalingFactorY, deviceThickness.Right * DeviceToLogicalUnitsScalingFactorX, deviceThickness.Bottom * DeviceToLogicalUnitsScalingFactorY);
		}

		public void SetDeviceLeft(ref Window window, Double deviceLeft)
		{
			window.Left = deviceLeft * DeviceToLogicalUnitsScalingFactorX;
		}

		public Double GetDeviceLeft(Window window)
		{
			return window.Left * LogicalToDeviceUnitsScalingFactorX;
		}

		public void SetDeviceTop(ref Window window, Double deviceTop)
		{
			window.Top = deviceTop * DeviceToLogicalUnitsScalingFactorY;
		}

		public Double GetDeviceTop(Window window)
		{
			return window.Top * LogicalToDeviceUnitsScalingFactorY;
		}

		public void SetDeviceWidth(ref Window window, Double deviceWidth)
		{
			window.Width = deviceWidth * DeviceToLogicalUnitsScalingFactorX;
		}

		public Double GetDeviceWidth(Window window)
		{
			return window.Width * LogicalToDeviceUnitsScalingFactorX;
		}

		public void SetDeviceHeight(ref Window window, Double deviceHeight)
		{
			window.Height = deviceHeight * DeviceToLogicalUnitsScalingFactorY;
		}

		public Double GetDeviceHeight(Window window)
		{
			return window.Height * LogicalToDeviceUnitsScalingFactorY;
		}

		public Rect GetDeviceRect(Window window)
		{
			RECT rECT;
			NativeMethods.GetWindowRect(new WindowInteropHelper(window).Handle, out rECT);
			return new Rect(new System.Windows.Point(rECT.Left, rECT.Top), new System.Windows.Size(rECT.Width, rECT.Height));
		}

		public System.Windows.Size GetDeviceActualSize(FrameworkElement element)
		{
			var logicalSize = new System.Windows.Size(element.ActualWidth, element.ActualHeight);
			return LogicalToDeviceUnits(logicalSize);
		}

		public System.Drawing.Point LogicalToDeviceUnits(System.Drawing.Point logicalPoint)
		{
			return new System.Drawing.Point(LogicalToDeviceUnitsX(logicalPoint.X), LogicalToDeviceUnitsY(logicalPoint.Y));
		}

		public System.Drawing.Size LogicalToDeviceUnits(System.Drawing.Size logicalSize)
		{
			return new System.Drawing.Size(LogicalToDeviceUnitsX(logicalSize.Width), LogicalToDeviceUnitsY(logicalSize.Height));
		}

		public Rectangle LogicalToDeviceUnits(Rectangle logicalRect)
		{
			return new Rectangle(LogicalToDeviceUnitsX(logicalRect.X), LogicalToDeviceUnitsY(logicalRect.Y), LogicalToDeviceUnitsX(logicalRect.Width), LogicalToDeviceUnitsY(logicalRect.Height));
		}

		public PointF LogicalToDeviceUnits(PointF logicalPoint)
		{
			return new PointF(LogicalToDeviceUnitsX(logicalPoint.X), LogicalToDeviceUnitsY(logicalPoint.Y));
		}

		public SizeF LogicalToDeviceUnits(SizeF logicalSize)
		{
			return new SizeF(LogicalToDeviceUnitsX(logicalSize.Width), LogicalToDeviceUnitsY(logicalSize.Height));
		}

		public RectangleF LogicalToDeviceUnits(RectangleF logicalRect)
		{
			return new RectangleF(LogicalToDeviceUnitsX(logicalRect.X), LogicalToDeviceUnitsY(logicalRect.Y), LogicalToDeviceUnitsX(logicalRect.Width), LogicalToDeviceUnitsY(logicalRect.Height));
		}

		public void LogicalToDeviceUnits(ref Bitmap bitmapImage, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			LogicalToDeviceUnits(ref bitmapImage, System.Drawing.Color.Transparent, scalingMode);
		}

		public void LogicalToDeviceUnits(ref Bitmap bitmapImage, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
            Image image = bitmapImage;
			LogicalToDeviceUnits(ref image, backgroundColor, scalingMode);
			bitmapImage = (Bitmap)image;
		}

		public void LogicalToDeviceUnits(ref Image image, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			LogicalToDeviceUnits(ref image, System.Drawing.Color.Transparent, scalingMode);
		}

		public void LogicalToDeviceUnits(ref Image image, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(image, "image");
			if (!IsScalingRequired)
			{
				return;
			}
			var image2 = CreateDeviceFromLogicalImage(image, backgroundColor, scalingMode);
			image.Dispose();
			image = image2;
		}

		private System.Drawing.Size GetPrescaledImageSize(System.Drawing.Size size)
		{
			return new System.Drawing.Size(size.Width * (DpiScalePercentX / 100), size.Height * (DpiScalePercentY / 100));
		}

		public Image CreateDeviceFromLogicalImage(Image logicalImage, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			return CreateDeviceFromLogicalImage(logicalImage, System.Drawing.Color.Transparent, scalingMode);
		}

		private void ProcessBitmapPixels(Bitmap image, PixelProcessor pixelProcessor)
		{
			var rect = new Rectangle(0, 0, image.Width, image.Height);
			var bitmapData = image.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			try
			{
				var scan = bitmapData.Scan0;
				var num = Math.Abs(bitmapData.Stride) * bitmapData.Height;
				var array = new Byte[num];
				Marshal.Copy(scan, array, 0, num);
				for (var i = 0; i < array.Length; i += 4)
				{
					pixelProcessor(ref array[i + 3], ref array[i + 2], ref array[i + 1], ref array[i]);
				}
				Marshal.Copy(array, 0, scan, num);
			}
			finally
			{
				image.UnlockBits(bitmapData);
			}
		}

		public ImageSource ScaleLogicalImageForDeviceSize(ImageSource image, System.Windows.Size deviceImageSize, BitmapScalingMode scalingMode)
		{
			Validate.IsNotNull(image, "image");
			var drawingGroup = new DrawingGroup();
			drawingGroup.Children.Add(new ImageDrawing(image, new Rect(deviceImageSize)));
			var drawingVisual = new DrawingVisual();
			using (var drawingContext = drawingVisual.RenderOpen())
			{
				RenderOptions.SetBitmapScalingMode(drawingGroup, scalingMode);
				drawingContext.DrawDrawing(drawingGroup);
			}
			var renderTargetBitmap = new RenderTargetBitmap((Int32)deviceImageSize.Width, (Int32)deviceImageSize.Height, LogicalDpiX, LogicalDpiY, PixelFormats.Default);
			renderTargetBitmap.Render(drawingVisual);
			var bitmapFrame = BitmapFrame.Create(renderTargetBitmap);
			bitmapFrame.Freeze();
			return bitmapFrame;
		}

		public Image CreateDeviceFromLogicalImage(Image logicalImage, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(logicalImage, "logicalImage");
			var imageScalingMode = GetActualScalingMode(scalingMode);
			var size = logicalImage.Size;
			var size2 = LogicalToDeviceUnits(size);
			if (imageScalingMode == ImageScalingMode.MixedNearestNeighborHighQualityBicubic)
			{
				var prescaledImageSize = GetPrescaledImageSize(size);
				if (prescaledImageSize == size)
				{
					imageScalingMode = ImageScalingMode.HighQualityBicubic;
				}
				else if (prescaledImageSize == size2)
				{
					imageScalingMode = ImageScalingMode.NearestNeighbor;
				}
				else if (prescaledImageSize == System.Drawing.Size.Empty)
				{
					imageScalingMode = ImageScalingMode.HighQualityBilinear;
				}
				else
				{
					var image = ScaleLogicalImageForDeviceSize(logicalImage, prescaledImageSize, backgroundColor, ImageScalingMode.NearestNeighbor);
					imageScalingMode = ImageScalingMode.HighQualityBicubic;
					logicalImage = image;
				}
			}
			return ScaleLogicalImageForDeviceSize(logicalImage, size2, backgroundColor, imageScalingMode);
		}

		private Image ScaleLogicalImageForDeviceSize(Image logicalImage, System.Drawing.Size deviceImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode)
		{
			Validate.IsNotNull(logicalImage, "logicalImage");
			var interpolationMode = GetInterpolationMode(scalingMode);
			var pixelFormat = logicalImage.PixelFormat;
			var clrMagenta = System.Drawing.Color.FromArgb(255, 0, 255);
			var clrNearGreen = System.Drawing.Color.FromArgb(0, 254, 0);
			var clrTransparentHalo = System.Drawing.Color.FromArgb(0, 246, 246, 246);
			var clrActualBackground = backgroundColor;
			var bitmap = logicalImage as Bitmap;
			if (scalingMode != ImageScalingMode.NearestNeighbor && bitmap != null)
			{
				if (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
				{
					var rect = new Rectangle(0, 0, logicalImage.Width, logicalImage.Height);
					logicalImage = bitmap.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
					bitmap = (logicalImage as Bitmap);
					if (backgroundColor != System.Drawing.Color.Transparent && backgroundColor.A != 255)
					{
						backgroundColor = System.Drawing.Color.FromArgb(255, backgroundColor);
					}
				}
				ProcessBitmapPixels(bitmap, delegate(ref Byte alpha, ref Byte red, ref Byte green, ref Byte blue)
				{
					if (backgroundColor != System.Drawing.Color.Transparent)
					{
						if (alpha == backgroundColor.A && red == backgroundColor.R && green == backgroundColor.G && blue == backgroundColor.B)
						{
							alpha = clrTransparentHalo.A;
							red = clrTransparentHalo.R;
							green = clrTransparentHalo.G;
							blue = clrTransparentHalo.B;
							clrActualBackground = backgroundColor;
							return;
						}
					}
					else
					{
						if (alpha == clrMagenta.A && red == clrMagenta.R && green == clrMagenta.G && blue == clrMagenta.B)
						{
							alpha = clrTransparentHalo.A;
							red = clrTransparentHalo.R;
							green = clrTransparentHalo.G;
							blue = clrTransparentHalo.B;
							clrActualBackground = clrMagenta;
							return;
						}
						if (alpha == clrNearGreen.A && red == clrNearGreen.R && green == clrNearGreen.G && blue == clrNearGreen.B)
						{
							alpha = clrTransparentHalo.A;
							red = clrTransparentHalo.R;
							green = clrTransparentHalo.G;
							blue = clrTransparentHalo.B;
							clrActualBackground = clrNearGreen;
						}
					}
				});
				if (clrActualBackground == System.Drawing.Color.Transparent && pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
				{
					if (backgroundColor != System.Drawing.Color.Transparent)
					{
						clrActualBackground = backgroundColor;
					}
					else
					{
						clrActualBackground = clrMagenta;
					}
				}
			}
			Image image;
			if (!(logicalImage is Bitmap))
			{
				if (logicalImage is Metafile)
				{
					var dC = NativeMethods.GetDC(IntPtr.Zero);
					try
					{
						image = new Metafile(dC, EmfType.EmfPlusDual);
						goto IL_1B5;
					}
					finally
					{
						NativeMethods.ReleaseDC(IntPtr.Zero, dC);
					}
				}
				throw new ArgumentException("Unsupported image type for High DPI conversion", nameof(logicalImage));
			}
			image = new Bitmap(deviceImageSize.Width, deviceImageSize.Height, logicalImage.PixelFormat);
			IL_1B5:
			using (var graphics = Graphics.FromImage(image))
			{
				graphics.InterpolationMode = interpolationMode;
				graphics.Clear(backgroundColor);
				var srcRect = new RectangleF(0f, 0f, logicalImage.Size.Width, logicalImage.Size.Height);
				srcRect.Offset(-0.5f, -0.5f);
				var destRect = new RectangleF(0f, 0f, deviceImageSize.Width, deviceImageSize.Height);
				if (scalingMode == ImageScalingMode.BorderOnly)
				{
					destRect = new RectangleF(0f, 0f, srcRect.Width, srcRect.Height);
					destRect.Offset((deviceImageSize.Width - srcRect.Width) / 2f, (deviceImageSize.Height - srcRect.Height) / 2f);
				}
				graphics.DrawImage(logicalImage, destRect, srcRect, GraphicsUnit.Pixel);
			}
			bitmap = (image as Bitmap);
			if (scalingMode != ImageScalingMode.NearestNeighbor && bitmap != null)
			{
				ProcessBitmapPixels(bitmap, delegate(ref Byte alpha, ref Byte red, ref Byte green, ref Byte blue)
				{
					if (alpha != 255)
					{
						alpha = clrActualBackground.A;
						red = clrActualBackground.R;
						green = clrActualBackground.G;
						blue = clrActualBackground.B;
					}
				});
				if (pixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
				{
					var rect2 = new Rectangle(0, 0, image.Width, image.Height);
					image = bitmap.Clone(rect2, pixelFormat);
				}
			}
			return image;
		}

		public void LogicalToDeviceUnits(ref Bitmap imageStrip, System.Drawing.Size logicalImageSize, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			LogicalToDeviceUnits(ref imageStrip, logicalImageSize, System.Drawing.Color.Transparent, scalingMode);
		}

		public void LogicalToDeviceUnits(ref Bitmap imageStrip, System.Drawing.Size logicalImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(imageStrip, "imageStrip");
			if (!IsScalingRequired)
			{
				return;
			}
			var bitmap = CreateDeviceFromLogicalImage(imageStrip, logicalImageSize, backgroundColor, scalingMode);
			imageStrip.Dispose();
			imageStrip = bitmap;
		}

		public Bitmap CreateDeviceFromLogicalImage(Bitmap logicalBitmapStrip, System.Drawing.Size logicalImageSize, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			return CreateDeviceFromLogicalImage(logicalBitmapStrip, logicalImageSize, System.Drawing.Color.Transparent, scalingMode);
		}

		public Bitmap CreateDeviceFromLogicalImage(Bitmap logicalBitmapStrip, System.Drawing.Size logicalImageSize, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(logicalBitmapStrip, "logicalBitmapStrip");
			Validate.IsNotNull(logicalImageSize, "logicalImageSize");
			if (logicalImageSize.Width == 0 || logicalBitmapStrip.Height % logicalImageSize.Width != 0 || logicalImageSize.Height != logicalBitmapStrip.Height)
			{
				throw new ArgumentException("logicalImageSize not matching the logicalBitmap size");
			}
			var num = logicalBitmapStrip.Width / logicalImageSize.Width;
			var num2 = LogicalToDeviceUnitsX(logicalImageSize.Width);
			var num3 = LogicalToDeviceUnitsY(logicalImageSize.Height);
			var bitmap = new Bitmap(num * num2, num3, logicalBitmapStrip.PixelFormat);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				for (var i = 0; i < num; i++)
				{
					var srcRect = new RectangleF(i * logicalImageSize.Width, 0f, logicalImageSize.Width, logicalImageSize.Height);
					srcRect.Offset(-0.5f, -0.5f);
					var destRect = new RectangleF(0f, 0f, logicalImageSize.Width, logicalImageSize.Height);
					var image = new Bitmap(logicalImageSize.Width, logicalImageSize.Height, logicalBitmapStrip.PixelFormat);
					using (var graphics2 = Graphics.FromImage(image))
					{
						graphics2.InterpolationMode = InterpolationMode.NearestNeighbor;
						graphics2.DrawImage(logicalBitmapStrip, destRect, srcRect, GraphicsUnit.Pixel);
					}
					LogicalToDeviceUnits(ref image, backgroundColor, scalingMode);
					srcRect = new RectangleF(0f, 0f, num2, num3);
					srcRect.Offset(-0.5f, -0.5f);
					destRect = new RectangleF(i * num2, 0f, num2, num3);
					graphics.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
				}
			}
			return bitmap;
		}

		public void LogicalToDeviceUnits(ref Icon icon, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(icon, "icon");
			if (!IsScalingRequired)
			{
				return;
			}
			var icon2 = CreateDeviceFromLogicalImage(icon, scalingMode);
			icon.Dispose();
			icon = icon2;
		}

		public Icon CreateDeviceFromLogicalImage(Icon logicalIcon, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(logicalIcon, "logicalIcon");
			var size = LogicalToDeviceUnits(logicalIcon.Size);
			var icon = new Icon(logicalIcon, size);
			if (icon.Size.Width != size.Width && icon.Size.Width != 0)
			{
				var logicalImage = icon.ToBitmap();
				var bitmap = (Bitmap)CreateDeviceFromLogicalImage(logicalImage, System.Drawing.Color.Transparent, scalingMode);
				var hicon = bitmap.GetHicon();
				icon = Icon.FromHandle(hicon);
				icon = (icon.Clone() as Icon);
				NativeMethods.DestroyIcon(hicon);
			}
			return icon;
		}

		public void LogicalToDeviceUnits(ref ImageList imageList, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			LogicalToDeviceUnits(ref imageList, System.Drawing.Color.Transparent, scalingMode);
		}

		public void LogicalToDeviceUnits(ref ImageList imageList, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(imageList, "imageList");
			if (!IsScalingRequired)
			{
				return;
			}
			var imageList2 = CreateDeviceFromLogicalImage(imageList, backgroundColor, scalingMode);
			imageList.Dispose();
			imageList = imageList2;
		}

		public ImageList CreateDeviceFromLogicalImage(ImageList logicalImageList, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			return CreateDeviceFromLogicalImage(logicalImageList, System.Drawing.Color.Transparent, scalingMode);
		}

		public ImageList CreateDeviceFromLogicalImage(ImageList logicalImageList, System.Drawing.Color backgroundColor, ImageScalingMode scalingMode = ImageScalingMode.Default)
		{
			Validate.IsNotNull(logicalImageList, "logicalImageList");
			var imageList = new ImageList();
			imageList.Site = logicalImageList.Site;
			imageList.Tag = logicalImageList.Tag;
			imageList.ColorDepth = logicalImageList.ColorDepth;
			imageList.TransparentColor = logicalImageList.TransparentColor;
			imageList.ImageSize = LogicalToDeviceUnits(logicalImageList.ImageSize);
			for (var i = 0; i < logicalImageList.Images.Count; i++)
			{
				var logicalImage = logicalImageList.Images[i];
				var value = CreateDeviceFromLogicalImage(logicalImage, backgroundColor, scalingMode);
				imageList.Images.Add(value);
			}
			foreach (var current in logicalImageList.Images.Keys)
			{
				var num = logicalImageList.Images.IndexOfKey(current);
				if (num != -1)
				{
					imageList.Images.SetKeyName(num, current);
				}
			}
			return imageList;
		}
	}
    }