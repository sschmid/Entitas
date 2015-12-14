using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException(string path)
            : base("Invalid path part " + path)
        {
        }
    }
}
