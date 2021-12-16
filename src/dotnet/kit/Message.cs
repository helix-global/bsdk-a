using System;

namespace Kit
    {
    public class Message
        {
        public String Code { get; }
        public Object[] Args { get; }
        public Message(String code, params Object[] args)
            {
            Code = code;
            Args = args;
            }
        }
    }