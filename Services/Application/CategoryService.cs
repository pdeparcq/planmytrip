using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanMyTrip.Web.Services.External;
using PlanMyTrip.Web.Models;

namespace PlanMyTrip.Web.Services.Application
{
    public class CategoryService
    {
        private FoursquareService _service;
        private List<Contract.Data.Category> _mainCategories;

        public CategoryService(FoursquareService service)
        {
            _service = service;
            FetchMainCategories();
        }

        public IEnumerable<Category> GetMainCategories()
        {
            var categories = new List<Category>();
            foreach (var category in _mainCategories)
            {
                categories.Add(CreateCategory(category));
            }
            return categories;
        }

        public Category FindCategory(string id)
        {
            return FindCategory(id, GetMainCategories());
        }

        private Category FindCategory(string id, IEnumerable<Category> _categories)
        {
            foreach (var c in _categories)
            {    
                if (c.Id == id)
                    return c;
                var found = FindCategory(id, c.Children);
                if (found != null)
                    return found;
            }
            return null;
        }

        private void FetchMainCategories()
        {
            _mainCategories = new List<Contract.Data.Category>(_service.GetCategories());
        }

        private static Category CreateCategory(Contract.Data.Category category, Category parent=null)
        {
            var c = new Category();
            c.Id = category.id;
            c.Name = category.name;
            c.ShortName = category.shortName;
            c.Icon = string.Format("{0}bg_32{1}",category.icon.prefix,category.icon.suffix);
            c.Parent = parent;
            c.IsSelected = true;
            if (category.categories != null && category.categories.Any())
            {
                foreach (var subCategory in category.categories)
                {
                    c.AddChildCategory(CreateCategory(subCategory,c));
                }
            }
            return c;
        }
    }
}