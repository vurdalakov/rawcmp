namespace Vurdalakov.RawCompare
{
    using System;

    public class RawCompareException : Exception
    {
        public Int32 ExitCode { get; private set; }

        public RawCompareException(Int32 exitCode, String format, params Object[] args) : base(String.Format(format, args))
        {
            ExitCode = exitCode;
        }
    }
}
