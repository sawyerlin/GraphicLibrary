using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using GraphicLibrary.Graphics;


namespace GraphicLibrary.DefaultGraphicNodes
{
    public class DefaultGraphicNode : AbstractGraphicNode
    {
        public DefaultGraphicNode() { }

        public override string ToString()
        {
            StringBuilder spacebuilder = new StringBuilder();
            for (int i = 0; i < Depth; i++)
                spacebuilder.Append(" ");

            string space = spacebuilder.ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append(space + "ID = " + Id)
                .Append(" TITLE = " + Title)
                .Append(" DEPTH = " + Depth);

            return sb.ToString();
        }

        public override AbstractJsonGraphicNode ToJsonGraphicNode()
        {
            return new DefaultJsonGraphicNode
            {
                Id = this.Id,
                Title = this.Title
            };
        }
    }
}