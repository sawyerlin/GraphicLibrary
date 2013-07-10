using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphicLibrary.Graphics;
using System.Collections;

namespace GraphicLibrary.Interfaces
{
    public interface IGraphicGenerator
    {
        List<AbstractGraphicNode> Generate();

        List<AbstractGraphicNode> GenerateRootAssets();

        void GenerateLevelOneChildren(int parentId);

        Hashtable GetHashTable();
    }
}
