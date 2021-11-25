using System;
using System.Windows;

namespace BinaryStudio.PlatformUI
    {
    internal sealed class NameResolver
        {
        private String name;
        private FrameworkElement nameScopeReferenceElement;

        /// <summary>
        /// Occurs when the resolved element has changed.
        /// </summary>
        public event EventHandler<NameResolvedEventArgs> ResolvedElementChanged;

        /// <summary>
        /// Gets or sets the name of the element to attempt to resolve.
        /// </summary>
        /// <value>The name to attempt to resolve.</value>
        public String Name {
            get { return name; }
            set
                {
                var @object = Object;
                name = value;
                UpdateObjectFromName(@object);
                }
            }

        /// <summary>
        /// The resolved object. Will return the reference element if TargetName is null or empty, or if a resolve has not been attempted.
        /// </summary>
        public DependencyObject Object { get {
            return (String.IsNullOrEmpty(Name) && HasAttempedResolve)
                    ? NameScopeReferenceElement
                    : ResolvedObject;
            }}

        /// <summary>
        /// Gets or sets the reference element from which to perform the name resolution.
        /// </summary>
        /// <value>The reference element.</value>
        public FrameworkElement NameScopeReferenceElement
            {
            get { return nameScopeReferenceElement; }
            set
                {
                var oldNameScopeReference = NameScopeReferenceElement;
                nameScopeReferenceElement = value;
                OnNameScopeReferenceElementChanged(oldNameScopeReference);
                }
            }

        #region P:ActualNameScopeReferenceElement:FrameworkElement
        private FrameworkElement ActualNameScopeReferenceElement { get {
            return (NameScopeReferenceElement == null || !NameScopeReferenceElement.IsLoaded)
                    ? null
                    : GetActualNameScopeReference(NameScopeReferenceElement);
            }}
        #endregion

        private DependencyObject ResolvedObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the reference element load is pending.
        /// </summary>
        /// <value>
        /// <c>True</c> if [pending reference element load]; otherwise, <c>False</c>.
        /// </value>
        /// <remarks>
        /// If the Host has not been loaded, the name will not be resolved.
        /// In that case, delay the resolution and track that fact with this property.
        /// </remarks>
        private Boolean PendingReferenceElementLoad { get; set; }
        private Boolean HasAttempedResolve { get; set; }

        private void OnNameScopeReferenceElementChanged(FrameworkElement oldNameScopeReference) {
            if (PendingReferenceElementLoad) {
                oldNameScopeReference.Loaded -= OnNameScopeReferenceLoaded;
                PendingReferenceElementLoad = false;
                }
            HasAttempedResolve = false;
            UpdateObjectFromName(Object);
            }

        /// <summary>
        /// Attempts to update the resolved object from the name within the context of the namescope reference element.
        /// </summary>
        /// <param name="oldObject">The old resolved object.</param>
        /// <remarks>
        /// Resets the existing target and attempts to resolve the current TargetName from the
        /// context of the current Host. If it cannot resolve from the context of the Host, it will
        /// continue up the visual tree until it resolves. If it has not resolved it when it reaches
        /// the root, it will set the Target to null and write a warning message to Debug output.
        /// </remarks>
        private void UpdateObjectFromName(DependencyObject oldObject) {
            DependencyObject resolvedObject = null;
            ResolvedObject = null;
            if (NameScopeReferenceElement != null) {
                if (!NameScopeReferenceElement.IsLoaded) {
                    NameScopeReferenceElement.Loaded += OnNameScopeReferenceLoaded;
                    PendingReferenceElementLoad = true;
                    return;
                    }
                if (!String.IsNullOrEmpty(Name)) {
                    var actualNameScopeReferenceElement = ActualNameScopeReferenceElement;
                    if (actualNameScopeReferenceElement != null) {
                        resolvedObject = (actualNameScopeReferenceElement.FindName(Name) as DependencyObject);
                        }
                    }
                }
            HasAttempedResolve = true;
            ResolvedObject = resolvedObject;
            if (!Equals(oldObject,Object)) {
                OnObjectChanged(oldObject, Object);
                }
            }

        private void OnObjectChanged(DependencyObject oldTarget, DependencyObject newTarget) {
            if (ResolvedElementChanged != null) {
                ResolvedElementChanged(this, new NameResolvedEventArgs(oldTarget, newTarget));
                }
            }

        private FrameworkElement GetActualNameScopeReference(FrameworkElement initialReferenceElement) {
            var frameworkElement = initialReferenceElement;
            if (IsNameScope(initialReferenceElement)) {
                frameworkElement = ((initialReferenceElement.Parent as FrameworkElement) ?? frameworkElement);
                }
            return frameworkElement;
            }

        private Boolean IsNameScope(FrameworkElement frameworkElement) {
            var e = frameworkElement.Parent as FrameworkElement;
            if (e != null) {
                var obj = e.FindName(Name);
                return obj != null;
                }
            return false;
            }

        private void OnNameScopeReferenceLoaded(Object sender, RoutedEventArgs e) {
            PendingReferenceElementLoad = false;
            NameScopeReferenceElement.Loaded -= OnNameScopeReferenceLoaded;
            UpdateObjectFromName(Object);
            }
        }
    }