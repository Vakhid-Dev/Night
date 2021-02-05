using System;
using System.Collections.Generic;
using System.Linq;
public class Tree_Recursive
{
    public partial class UL
    {
        public int FID_NP { get; set; }
        public string OGRN { get; set; }
        public Nullable<long> C_data { get; set; }
        public Nullable<long> UL_ID { get; set; }
    }
    public class Tree : UL
    {
        public UL Data { get; private set; }
        public UL Left { get; set; }
        public UL Right { get; set; }
        public UL Parent { get; set; }


        public void Insert(Tree ul)
        {
            if (Data == null || Data == ul)
            {
                Data = ul;
                return;
            }
            if (Data.C_data > ul.C_data)
            {
                if (Left == null)
                {
                    using (var db = new TestEntities())
                    {
                        Left = ul;
                    }
                }

                Insert(ul, (Tree)Left, this);
            }
            else
            {
                if (Right == null)
                {
                    using (var db = new TestEntities())
                    {
                        Right = ul;
                    }


                }
                Insert(ul, (Tree)Right, this);
            }
        }

        private void Insert(Tree ul, Tree node, Tree parent)
        {

            if (node.Data == null || node.Data == ul)
            {
                node.Data = ul;
                node.Parent = parent;
                return;
            }
            if (node.Data.C_data > ul.C_data)
            {

                if (node.Left == null)
                {
                    using (var db = new TestEntities())
                    {
                        node.Left = ul;
                    }
                }
                Insert(ul, (Tree)node.Left, node);
            }
            else
            {
                if (node.Right == null)
                {
                    using (var db = new TestEntities())
                    {
                        node.Right = ul;
                    }

                }
                Insert(ul, (Tree)node.Right, node);
            }
        }
        public Tree Find(Tree data)
        {
            if (Data == data) return this;
            if (Data.C_data > data.C_data)
            {
                return Find(data, (Tree)Left);
            }

            return Find(data, (Tree)Right);
        }

        public Tree Find(Tree data, Tree node)
        {
            if (node == null) return null;
            if (node.Data == data) return node;
            if (node.Data.C_data > data.C_data)
            {
                return Find(data, (Tree)node.Left);
            }

            return Find(data, (Tree)node.Right);
        }
    }
    public class CommonConverter
    {
        public static CheckIP.Tree FromDalToBl(UL tree)
        {
            var obj = new Tree
            {
                FID_NP = tree.FID_NP,
                UL_ID = tree.UL_ID,
                OGRN = tree.OGRN,
                C_data = tree.C_data,


            };

            return obj;
        }
    }


}
