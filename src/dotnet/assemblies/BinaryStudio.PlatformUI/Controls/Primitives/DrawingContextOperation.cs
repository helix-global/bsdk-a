using System;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class DrawingContextOperation : IDisposable
        {
        private Object Operation { get;set; }
        private DrawingContext DrawingContext { get;set; }
        public DrawingContextOperation(DrawingContext context, Transform operation) {
            DrawingContext = context;
            if (operation != null) {
                Operation = operation;
                context.PushTransform(operation);
                }
            }

        void IDisposable.Dispose() {
            if ((DrawingContext != null) && (Operation != null)) {
                DrawingContext.Pop();
                DrawingContext = null;
                Operation = null;
                }
            }
        }
    }