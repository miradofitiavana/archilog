using System;
using System.Collections.Generic;
using System.Text;

namespace APILibrary.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NotJsonAttribute : Attribute
    {
    }
}
