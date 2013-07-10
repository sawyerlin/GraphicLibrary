using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphicLibrary.Interfaces;
using GraphicLibrary.DefaultGraphicNodes;
using GraphicLibrary.Graphics;

namespace GraphicLibrary.Generators
{
    // This class will generate 
    public class DefaultGenerator : IGraphicGenerator
    {
        #region Fields

        private int _depth;
        private int _width;

        #endregion

        #region Constructor

        public DefaultGenerator(
            int depth,
            int width
        )
        {
            this._depth = depth;
            this._width = width;
        }

        #endregion

        #region Public Methods

        public List<AbstractGraphicNode> Generate()
        {
            int id = 0;
            return this._generateNodes(ref id);
        }

        #endregion

        #region Private Methods

        private List<AbstractGraphicNode> _generateNodes(
            ref int id,
            int currentDepth = 1
        )
        {
            List<AbstractGraphicNode> currentNodes = new List<AbstractGraphicNode>();

            for (int i = 1; i <= this._width; i++)
            {
                // Initialiser a node.
                var node = new DefaultGraphicNode { Id = id, Title = "Asset" + id, Depth = currentDepth };
                id++;

                // Initialiser children.
                if (currentDepth < this._depth)
                    node.Children = _generateNodes(ref id, currentDepth + 1);

                currentNodes.Add(node);
            }

            return currentNodes;
        }

        #endregion

        public System.Collections.Hashtable GetHashTable()
        {
            return null;
        }


        public void GenerateLevelOneChildren(int parentId)
        {
            throw new NotImplementedException();
        }


        public List<AbstractGraphicNode> GenerateRootAssets()
        {
            throw new NotImplementedException();
        }
    }
}
