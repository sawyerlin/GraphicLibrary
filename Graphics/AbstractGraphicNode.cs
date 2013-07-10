using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicLibrary.Graphics
{
    public abstract class AbstractGraphicNode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Depth { get; set; }
        public List<AbstractGraphicNode> Children { get; set; }

        public abstract AbstractJsonGraphicNode ToJsonGraphicNode();
    }
}
