using Autodesk.UI.Windows;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cust_IFC_Exporter
{
    public class TreeNode<T>:INotifyPropertyChanged
    {
        public T Data { get; set; }

        public bool? IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;

                OnPropertyChanged(nameof(IsSelected));

                SyncChildValue();

                SyncParentValue();
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;

                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private void SyncParentValue()
        {
            if (ParentNode == null)
            {
                return;
            }

            if (ParentNode.CheckIfChildrenAllTrue())
            {
                ParentNode.IsSelected = true;
            }
            else if (ParentNode.CheckIfChildrenAllFalse())
            {
                ParentNode.IsSelected = false;
            }
            else
            {
                ParentNode.IsSelected = null;
            }
        }

        private bool? _isSelected = false;

        private bool _isVisible = true;

        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();

        public TreeNode<T> ParentNode { get; set; }

        public TreeNode(T data)
        {
            Data = data;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SyncChildValue()
        {
            if (Children.Count == 0)
            {
                return;
            }

            if(IsSelected==true)
            {
                foreach (var child in Children)
                {
                    if(child.IsSelected!=true)
                    {
                        child.IsSelected = true;
                    }                                                                    
                }
                /*
                if(ParentNode!=null && !ParentNode.IsSelected)
                {
                    foreach(var child in ParentNode.Children)
                    {
                        if(child.IsSelected==false)
                        {
                            return;
                        }
                    }

                    ParentNode.IsSelected = true;
                }
                */
            }
            else if(IsSelected==false)
            {
                foreach (var child in Children)
                {
                    if(child.IsSelected != false)
                    {
                        child.IsSelected = false;
                    }                 
                }
            }
        }

        public bool CheckIfChildrenAllTrue()
        {
            foreach(var child in Children)
            {
                if(child.IsSelected!=true)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckIfChildrenAllFalse()
        {
            foreach (var child in Children)
            {
                if (child.IsSelected!=false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
