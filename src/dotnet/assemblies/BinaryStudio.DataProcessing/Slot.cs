using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BinaryStudio.DataProcessing.Annotations;

namespace BinaryStudio.DataProcessing
    {
    public static class Slot
        {
        public static Slot<T1>             Create<T1>(T1 item1) { return new Slot<T1>(item1); }
        public static Slot<T1,T2>          Create<T1,T2>(T1 item1, T2 item2) { return new Slot<T1,T2>(item1, item2); }
        public static Slot<T1,T2,T3>       Create<T1,T2,T3>(T1 item1, T2 item2, T3 item3) { return new Slot<T1,T2,T3>(item1, item2, item3); }
        public static Slot<T1,T2,T3,T4>    Create<T1,T2,T3,T4>(T1 item1, T2 item2, T3 item3, T4 item4) { return new Slot<T1,T2,T3,T4>(item1, item2, item3, item4); }
        public static Slot<T1,T2,T3,T4,T5> Create<T1,T2,T3,T4,T5>(T1 item1,T2 item2,T3 item3,T4 item4,T5 item5) { return new Slot<T1,T2,T3,T4,T5>(item1,item2,item3,item4,item5); }
        }

    #region T:Slot<T1>
    [DebuggerDisplay(@"\{{Item1}\}")]
    public class Slot<T1> : INotifyPropertyChanged
        {
        #region P:Item1:T1
        private T1 r;
        public T1 Item1 {
            get { return r; }
            set
                {
                if (!Equals(r,value)) {
                    r = value;
                    OnPropertyChanged(nameof(Item1));
                    }
                }
            }
        #endregion
        public Slot()
            {
            }

        public Slot(T1 item1) {
            Item1 = item1;
            }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    #endregion
    #region T:Slot<T1,T2>
    [DebuggerDisplay(@"\{{Item1},{Item2}\}")]
    public class Slot<T1,T2> : Slot<T1>
        {
        #region P:Item2:T2
        private T2 r;
        public T2 Item2 {
            get { return r; }
            set
                {
                if (!Equals(r,value)) {
                    r = value;
                    OnPropertyChanged(nameof(Item2));
                    }
                }
            }
        #endregion

        public Slot()
            {
            }
        public Slot(T1 item1, T2 item2)
            :base(item1)
            {
            Item2 = item2;
            }
        }
    #endregion
    #region T:Slot<T1,T2,T3>
    [DebuggerDisplay(@"\{{Item1},{Item2},{Item3}\}")]
    public class Slot<T1,T2,T3> : Slot<T1,T2>
        {
        #region P:Item3:T3
        private T3 r;
        public T3 Item3 {
            get { return r; }
            set
                {
                if (!Equals(r,value)) {
                    r = value;
                    OnPropertyChanged(nameof(Item3));
                    }
                }
            }
        #endregion

        public Slot()
            {
            }
        public Slot(T1 item1, T2 item2, T3 item3) 
            :base(item1, item2)
            {
            Item3 = item3;
            }
        }
    #endregion
    #region T:Slot<T1,T2,T3,T4>
    [DebuggerDisplay(@"\{{Item1},{Item2},{Item3},{Item4}\}")]
    public class Slot<T1,T2,T3,T4> : Slot<T1,T2,T3>
        {
        #region P:Item4:T4
        private T4 r;
        public T4 Item4 {
            get { return r; }
            set
                {
                if (!Equals(r,value)) {
                    r = value;
                    OnPropertyChanged(nameof(Item4));
                    }
                }
            }
        #endregion

        public Slot()
            {
            }
        public Slot(T1 item1, T2 item2, T3 item3, T4 item4)
            :base(item1, item2, item3)
            {
            Item4 = item4;
            }
        }
    #endregion
    #region T:Slot<T1,T2,T3,T4,T5>
    [DebuggerDisplay(@"\{{Item1},{Item2},{Item3},{Item4},{Item5}\}")]
    public class Slot<T1,T2,T3,T4,T5> : Slot<T1,T2,T3,T4>
        {
        #region P:Item5:T5
        private T5 r;
        public T5 Item5 {
            get { return r; }
            set
                {
                if (!Equals(r,value)) {
                    r = value;
                    OnPropertyChanged(nameof(Item5));
                    }
                }
            }
        #endregion
        public Slot()
            {
            }
        public Slot(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
            :base(item1, item2, item3, item4)
            {
            Item5 = item5;
            }
        }
    #endregion
    #region T:Slot<T1,T2,T3,T4,T5,T6>
    [DebuggerDisplay(@"\{{Item1},{Item2},{Item3},{Item4},{Item5},{Item6}\}")]
    public class Slot<T1,T2,T3,T4,T5,T6> : Slot<T1,T2,T3,T4,T5>
        {
        #region P:Item6:T6
        private T6 r;
        public T6 Item6 {
            get { return r; }
            set
                {
                if (!Equals(r,value)) {
                    r = value;
                    OnPropertyChanged(nameof(Item6));
                    }
                }
            }
        #endregion
        public Slot()
            {
            }
        public Slot(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
            :base(item1, item2, item3, item4, item5)
            {
            Item6 = item6;
            }
        }
    #endregion
    }