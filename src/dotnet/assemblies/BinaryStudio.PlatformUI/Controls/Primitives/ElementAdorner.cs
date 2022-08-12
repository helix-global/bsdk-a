using System.Windows;
using System.Windows.Documents;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class ElementAdorner : Adorner
        {
        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Documents.Adorner" /> class.</summary>
        /// <param name="adornedElement">The element to bind the adorner to.</param>
        /// <exception cref="T:System.ArgumentNullException">adornedElement is null.</exception>
        public ElementAdorner(UIElement adornedElement)
            : base(adornedElement)
            {
            }
        }
    }