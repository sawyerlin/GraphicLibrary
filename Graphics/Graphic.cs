using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GraphicLibrary.Interfaces;
using GraphicLibrary.DefaultGraphicNodes;
using System.Collections;

namespace GraphicLibrary.Graphics
{
    public class Graphic
    {
        #region Fields

        private List<AbstractGraphicNode> _nodelist;
        private Stack<AbstractGraphicNode> _nodestack;
        private IGraphicGenerator _generator;

        #endregion

        #region Properties

        public List<AbstractGraphicNode> NodeList
        {
            get
            {
                return this._nodelist;
            }
        }

        #endregion

        #region Constructor

        public Graphic(IGraphicGenerator generator)
        {
            this._generator = generator;
            //this._nodelist = generator.Generate();
            this._nodelist = generator.GenerateRootAssets();
            this._nodestack = new Stack<AbstractGraphicNode>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DescendTo(int id)
        {
            while (this._nodestack.Count > 0)
            {
                var parentNode = this._nodestack.Pop();
                if (parentNode.Children == null)
                    this._generator.GenerateLevelOneChildren(parentNode.Id);

                var findNode =
                    parentNode.Children.
                        Find(node => node.Id == id);
                if (findNode != null)
                {
                    this._nodestack.Push(parentNode);
                    this._nodestack.Push(findNode);
                    return true;
                }
            }

            if (_nodestack.Count <= 0)
            {
                var findNode = (AbstractGraphicNode)this.GetGraphicHashTable()[id];
                this._nodestack.Push(findNode);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool AscendTo(int id)
        {
            if (this._nodestack.ToList().Exists(node => node.Id == id))
            {
                while (this._nodestack.Peek().Id != id)
                    this._nodestack.Pop();

                return true;
            }

            return false;
        }

        public void AscendToEnd()
        {
            this._nodestack.Clear();
        }

        public void AddChildren(List<AbstractGraphicNode> children)
        {
            if (this._nodestack.Count > 0) this._nodestack.Peek().Children.AddRange(children);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void DeleteChild(int id)
        {
            // todo: à tester
            List<AbstractGraphicNode> children = this._nodestack.Peek().Children;
            children.RemoveAll(n => n.Id == id);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AbstractGraphicNode FindNodeById(int id)
        {
            return this._findNodeById(id, this._nodelist);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool UpdateNode(AbstractGraphicNode node)
        {
            AbstractGraphicNode oldNode = this._findNodeById(node.Id, this._nodelist);


            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteNodeById(int id)
        {
            return this._deleteNodeById(id, this._nodelist);
        }

        public Hashtable GetGraphicHashTable()
        {
            return this._generator.GetHashTable();
        }

        #endregion

        #region Private Methods

        private AbstractGraphicNode _findNodeById(double id, List<AbstractGraphicNode> nodelist)
        {
            AbstractGraphicNode findNode = nodelist.Where(node => node.Id == id).FirstOrDefault();

            if (findNode == null)
                foreach (var node in nodelist)
                {
                    findNode = node.Children == null ? null : this._findNodeById(id, node.Children);
                    if (findNode != null) break;
                }

            return findNode;
        }

        private bool _deleteNodeById(double id, List<AbstractGraphicNode> nodelist)
        {
            AbstractGraphicNode findNode = null;

            if (nodelist.Exists(node =>
            {
                if (node.Id == id)
                {
                    findNode = node;
                    return true;
                }
                return false;
            }))
            {
                nodelist.Remove(findNode);
                return true;
            }
            else
            {
                bool flag = false;

                foreach (var node in nodelist)
                {
                    if (flag = this._deleteNodeById(id, node.Children ?? new List<AbstractGraphicNode>())) break;
                }

                return flag;
            }
        }

        private string _graphicToString(List<AbstractGraphicNode> nodelist)
        {
            string result = "";

            if (nodelist != null)
            {
                result = nodelist.Aggregate(result,
                        (current, next) => current += (next.ToString() + "\n" + _graphicToString(next.Children)));
            }

            return result;
        }

        #endregion

        #region Override Methods

        public override string ToString()
        {
            return this._graphicToString(this._nodelist);
        }

        #endregion

        #region Public Methods Web

        public List<AbstractJsonGraphicNode> GetJsonGraphicOnList()
        {
            //return this._generateJsonGraphicOnList(this._nodelist);
            List<AbstractJsonGraphicNode> results = new List<AbstractJsonGraphicNode>();

            foreach (AbstractGraphicNode value in this._generator.GetHashTable().Values)
                results.Add(value.ToJsonGraphicNode());

            return results.OrderBy(node => node.Id).ToList();
        }

        public AbstractJsonGraphicNode GetJsonGraphicNodeById(int id)
        {
            AbstractGraphicNode node = this._findNodeById(id, this._nodelist);
            return new DefaultJsonGraphicNode { Id = node.Id, Title = node.Title };
        }

        public AbstractJsonGraphicNode GetJsonGraphicNode()
        {
            return this._nodestack.Count > 0 ? this._nodestack.Peek().ToJsonGraphicNode() : null;
        }

        public List<AbstractJsonGraphicNode> GetJsonNodeChildren()
        {
            AbstractGraphicNode parent = null;

            if (this._nodestack.Count > 0)
            {
                parent = this._nodestack.Peek();
                if (parent.Children == null)
                    this._generator.GenerateLevelOneChildren(parent.Id);
                return parent.Children
                    .Select(child => child.ToJsonGraphicNode())
                    .OrderBy(child => child.Id)
                    .ToList();
            }
            else
                return new List<AbstractJsonGraphicNode>();
        }

        public List<AbstractJsonGraphicNode> GetJsonNodeChildrenById(int id)
        {
            var oGraphicNodes = this._findNodeById(id, this._nodelist).Children;
            return oGraphicNodes != null ? oGraphicNodes.Select(
                node => node.ToJsonGraphicNode())
                .ToList() : null;
        }

        #endregion

        #region Private Methods Web

        private List<AbstractJsonGraphicNode> _generateJsonGraphicOnList(List<AbstractGraphicNode> nodelist)
        {
            if (nodelist == null)
                return new List<AbstractJsonGraphicNode>();
            List<AbstractJsonGraphicNode> jsonNodeList = new List<AbstractJsonGraphicNode>();
            return nodelist.Aggregate(jsonNodeList, (current, next) =>
            {
                current.Add(next.ToJsonGraphicNode());
                current.AddRange(_generateJsonGraphicOnList(next.Children));
                return current;
            });
        }

        #endregion
    }
}