using System;

namespace equity_cracker
{
    static class EquityThings
    {
        public static void Recap()
        {

        }

        public class MyException : Exception
        {
            public MyException() : base() { }
            public MyException(string message) : base(message) { }
            public MyException(string message, Exception e) : base(message, e) { }

            private string strExtraInfo;
            public string ExtraErrorInfo
            {
                get
                {
                    return strExtraInfo;
                }

                set
                {
                    strExtraInfo = value;
                }
            }
        }
    }
}
