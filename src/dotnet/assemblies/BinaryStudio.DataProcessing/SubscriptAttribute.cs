using System;

namespace BinaryStudio.DataProcessing
    {
    public class SubscriptAttribute : Attribute
        {
        public String Text { get;set; }
        public SubscriptAttribute(String text) {
            Text = text;
            }
        }
    }