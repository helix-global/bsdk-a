using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Threading;
using BinaryStudio.PlatformUI;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Properties;

namespace shell
    {
    internal class EObjectIdentiferInfo : NotifyPropertyChangedDispatcherObject<Object>
        {
        private String _inputText;
        private String _friendlyNameFromSystem;
        private String _friendlyNameFromLibraryEnglish;
        private String _friendlyNameFromLibraryRussian;

        public EObjectIdentiferInfo()
            : base(null)
            {
            }

        public String FriendlyNameFromSystem
            {
            get { return _friendlyNameFromSystem; }
            set { SetValue(ref _friendlyNameFromSystem,value); }
            }

        public String FriendlyNameFromLibraryEnglish
            {
            get { return _friendlyNameFromLibraryEnglish; }
            set { SetValue(ref _friendlyNameFromLibraryEnglish,value); }
            }

        public String FriendlyNameFromLibraryRussian
            {
            get { return _friendlyNameFromLibraryRussian; }
            set { SetValue(ref _friendlyNameFromLibraryRussian,value); }
            }

        public String InputText
            {
            get { return _inputText; }
            set
                {
                if (SetValue(ref _inputText, value))
                    {
                    FriendlyNameFromSystem = (new Oid(value)).FriendlyName;
                    FriendlyNameFromLibraryEnglish = OID.ResourceManager.GetString(value, CultureInfo.GetCultureInfo("en-US"));
                    FriendlyNameFromLibraryRussian = OID.ResourceManager.GetString(value, CultureInfo.GetCultureInfo("ru-RU"));
                    //var thread = new Thread((_)=>{
                    //    var culture = CultureInfo.GetCultureInfo("en-US");
                    //    Thread.CurrentThread.CurrentUICulture = culture;
                    //    Thread.CurrentThread.CurrentCulture = culture;
                    //    SetThreadLocale(culture.LCID);
                    //    var r = (new Oid(value)).FriendlyName;
                    //    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(()=>{
                    //        FriendlyNameFromSystem = r;
                    //        }));
                    //    });
                    //thread.Start();
                    //thread.Join();
                    }
                }
            }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern Boolean SetThreadLocale(Int32 Locale);
        }
    }