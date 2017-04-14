﻿using System;

namespace Chloe.Entity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public TableAttribute() { }
        public TableAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
