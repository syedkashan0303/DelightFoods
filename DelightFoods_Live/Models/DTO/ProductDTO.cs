using DelightFoods_Live.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DelightFoods_Live.Models.DTO
{
    public class ProductDTO
    {
        public ProductDTO()
        {
            categoryList = new List<SelectListItem>();
            ParentcategoryList = new List<SelectListItem>();
            MediaFileList = new List<MediaFiles>();
            UploadedFiles = new List<IFormFile>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        public decimal Price { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        public List<SelectListItem> categoryList { get; set; }

        public List<SelectListItem> ParentcategoryList { get; set; }

        [BindProperty]
        public IList<IFormFile> UploadedFiles { get; set; }

        public List<MediaFiles> MediaFileList { get; set; }
        public string MediaFilePath { get; set; }


        public class MediaFiles
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public int FileId { get; set; }
        }



    }
}
