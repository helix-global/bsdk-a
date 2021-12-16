using System;

namespace BinaryStudio.DataProcessing
    {
    public class SuperscriptAttribute : Attribute
        {
        public String Text { get;set; }
        public SuperscriptAttribute(String text) {
            Text = text;
            }
        }
    }