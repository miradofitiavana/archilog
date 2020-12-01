using APILibrary.core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace APILibrary.core.Models
{
    public abstract class ModelBase
    {
        [CannotFilter]
        public int ID { get; set; }
    }
}
