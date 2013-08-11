// Guids.cs
// MUST match guids.h
using System;

namespace UtkarshShigihalli.VisualStudioStatusBarDemo
{
    static class GuidList
    {
        public const string guidVisualStudioStatusBarDemoPkgString = "f6556444-a16c-44ab-a989-d28c23448f33";
        public const string guidVisualStudioStatusBarDemoCmdSetString = "984a1279-d240-4597-a6d8-73c01779eac6";

        public static readonly Guid guidVisualStudioStatusBarDemoCmdSet = new Guid(guidVisualStudioStatusBarDemoCmdSetString);
    };
}