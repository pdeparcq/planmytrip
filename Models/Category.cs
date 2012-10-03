using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace PlanMyTrip.Web.Models
{
    public class Category : IEquatable<Category>
    {
        private List<Category> _children;

        public string Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Icon { get; set; }
        public bool IsSelected { get; set; }

        [ScriptIgnore]
        public Category Parent { get; set; }
        
        public Category()
        {
            _children = new List<Category>();
        }

        public void AddChildCategory(Category c)
        {
            _children.Add(c);
        }

        [ScriptIgnore]
        public IEnumerable<Category> Children
        {
            get
            {
                return _children.AsEnumerable();
            }
        }

        public IEnumerable<string> BreadCrumb
        {
            get
            {
                if (Parent != null)
                    return new List<string>(Parent.BreadCrumb) { ShortName };
                return new List<string> { ShortName };
            }
        }

        [ScriptIgnore]
        public Category RootCategory
        {
            get
            {
                if (Parent == null)
                    return this;
                return Parent.RootCategory;
            }
        }

        public bool Equals(Category other)
        {
            return other != null && other.Id == Id;
        }
    }
}