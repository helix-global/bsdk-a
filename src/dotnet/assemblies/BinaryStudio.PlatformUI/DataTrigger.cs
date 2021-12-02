using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using BinaryStudio.PlatformUI.Extensions;
using TriggerActionCollection = System.Windows.TriggerActionCollection;

namespace BinaryStudio.PlatformUI
    {
    [ContentProperty("Setters")]
    public class DataTrigger : TriggerBase<DependencyObject> {
        public DataTrigger() {
            EnterActions = new TriggerActionCollection();
            ExitActions  = new TriggerActionCollection();
            EnterSetters = Setters;
            ExitSetters = new SetterBaseCollection();
            }

        #region P:Binding:BindingBase
        private BindingBase binding;
        public BindingBase Binding {
            get
                {
                VerifyAccess();
                return binding;
                }
            set
                {
                VerifyAccess();
                if (IsSealed) { throw new InvalidOperationException(); }
                binding = value;
                if (binding != null) {
                    BindingOperations.SetBinding(this, TriggerValueProperty, binding);
                    }
                }
            }
        #endregion
        #region P:Value:Object
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Object), typeof(DataTrigger), new PropertyMetadata(default(Object)));
        public Object Value {
            get { return (Object) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
            }
        #endregion
        #region P:Setters:SetterBaseCollection
        private SetterBaseCollection setters;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SetterBaseCollection Setters { get {
            VerifyAccess();
            return setters ?? (setters = new SetterBaseCollection());
            }}
        #endregion
        #region P:TriggerValue:Object
        internal static readonly DependencyProperty TriggerValueProperty = DependencyProperty.Register("TriggerValue", typeof(Object), typeof(DataTrigger), new PropertyMetadata(default(Object), OnTriggerValueChanged));
        private static void OnTriggerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var source = d as DataTrigger;
            if (source != null) {
                source.OnTriggerValueChanged();
                }
            }

        #region M:Equals(Object,Object):Boolean
        private new static Boolean Equals(Object x, Object y) {
            if (ReferenceEquals(x,y)) { return true; }
            if ((x == null) || (y == null)) { return false; }
            if (x.GetType() == y.GetType()) { return Object.Equals(x,y); }
            if ((x is String) && (y.GetType().IsValueType)) {
                var converter = TypeDescriptor.GetConverter(y);
                if (converter != null) {
                    if (converter.CanConvertFrom(typeof(String))) {
                        return Object.Equals(y, converter.ConvertFromString((String)x));
                        }
                    }
                }
            if ((y is String) && (x.GetType().IsValueType)) {
                var converter = TypeDescriptor.GetConverter(x);
                if (converter != null) {
                    if (converter.CanConvertFrom(typeof(String))) {
                        return Object.Equals(x, converter.ConvertFromString((String)y));
                        }
                    }
                }
            return Object.Equals(x,y);
            }
        #endregion

        private void InvokeSetters(FrameworkElement source, SetterBaseCollection setters) {
            foreach (var setter in setters.OfType<Setter>()) {
                if (setter != null) {
                    var resolver = new NameResolver {
                        NameScopeReferenceElement = source,
                        Name = setter.TargetName
                        };
                    var r = resolver.Object;
                    if (r != null) {
                        if (setter.Property != null) {
                            r.SetValue(setter.Property, setter.Value);
                            }
                        }
                    }
                }
            }

        private void PlayActions(FrameworkElement source, TriggerActionCollection actions) {
            if (source != null) {
                foreach (var action in actions) {
                    if (action is BeginStoryboard) {
                        var storyboard = ((BeginStoryboard)action).Storyboard;
                        if (storyboard != null) {
                            if (Scope != null) {
                                NameScope.SetNameScope(storyboard, Scope);
                                NameScope.SetNameScope(action, Scope);
                                }
                            storyboard.Begin(source);
                            }
                        }
                    }
                }
            }

        private void OnTriggerValueChanged() {
            var source = AssociatedObject as FrameworkElement;
            if (Equals(TriggerValue, Value)) {
                if (source != null) {
                    if (source.IsLoaded) {
                        InvokeSetters(source, EnterSetters);
                        PlayActions(source, EnterActions);
                        }
                    else
                        {
                        source.DoAfterLoaded(()=> {
                            InvokeSetters(source, EnterSetters);
                            PlayActions(source, EnterActions);
                            });
                        }
                    }
                if (TriggerActivated != null) {
                    TriggerActivated(this, new RoutedEventArgs(TriggerActivatedEvent));
                    }
                }
            else
                {
                if (source != null) {
                    if (source.IsLoaded) {
                        PlayActions(source, ExitActions);
                        InvokeSetters(source, ExitSetters);
                        }
                    else
                        {
                        source.DoAfterLoaded(()=> {
                            PlayActions(source, ExitActions);
                            InvokeSetters(source, ExitSetters);
                            });
                        }
                    }
                }
            }

        internal Object TriggerValue {
            get { return (Object) GetValue(TriggerValueProperty); }
            set { SetValue(TriggerValueProperty, value); }
            }
        #endregion

        public TriggerActionCollection EnterActions {get; }
        public TriggerActionCollection ExitActions {get; }
        public SetterBaseCollection EnterSetters {get; }
        public SetterBaseCollection ExitSetters {get; }
        private INameScope Scope {get;set; }

        #region M:OnAttached
        protected override void OnAttached() {
            base.OnAttached();
            NameScopeReferenceElement = AssociatedObject as FrameworkElement;
            if (NameScopeReferenceElement != null) {
                NameScopeReferenceElement.DoAfterLoaded(()=> {
                    Scope = FindScope(AssociatedObject);
                    });
                }
            OnTriggerValueChanged();
            }
        #endregion

        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
        //    base.OnPropertyChanged(e);
        //    Debug.Print("DataTrigger:OnPropertyChanged:{0}:{1}->{2}", e.Property.Name,
        //        (e.OldValue != null)
        //            ? String.Format(@"""{0}""", e.OldValue)
        //            : "(null)",
        //        (e.NewValue != null)
        //            ? String.Format(@"""{0}""", e.NewValue)
        //            : "(null)"
        //        );
        //    }

        public static readonly RoutedEvent TriggerActivatedEvent = EventManager.RegisterRoutedEvent("TriggerActivated",RoutingStrategy.Bubble,typeof(RoutedEventHandler),typeof(DataTrigger));
        internal FrameworkElement NameScopeReferenceElement;

        public event RoutedEventHandler TriggerActivated;

        internal static INameScope FindScope(DependencyObject d) {
            return (INameScope)(typeof(FrameworkElement).
                GetMethod("FindScope", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(DependencyObject) },null).
                Invoke(null, new Object[] { d }));
            }

        //#region M:CreateCollection:TriggerActionCollection
        //private static TriggerActionCollection CreateCollection() {
        //    var ctor = typeof(TriggerActionCollection).GetConstructor(BindingFlags.NonPublic |BindingFlags.Instance, null, Type.EmptyTypes, null);
        //    return (TriggerActionCollection)ctor.Invoke(new Object[0]);
        //    }
        //#endregion
        }    }