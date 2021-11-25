using System.Windows;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public interface IProgressRingTemplateSettings
    {
                double EllipseDiameter { get; }
        Thickness EllipseOffset { get; }
        double MaxSideLength { get; }

    }
}