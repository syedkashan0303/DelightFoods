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

        [Required]
        [Display(Name = "Name")]
        [NonEmptyName(ErrorMessage = "Name cannot be empty or whitespace.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        [NonEmptyName(ErrorMessage = "Description cannot be empty or whitespace.")]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [Range(1, 500, ErrorMessage = "The Stock must be a positive number or greator than 0.")]
        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, 99999, ErrorMessage = "The Price must be a positive number or greator than 0.")]
        public decimal Price { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        public List<SelectListItem> categoryList { get; set; }

        public List<SelectListItem> ParentcategoryList { get; set; }

        [BindProperty]
        public IList<IFormFile> UploadedFiles { get; set; }

        public List<MediaFiles> MediaFileList { get; set; }

        public class MediaFiles
        {
            public byte[] FileBytes { get; set; }
            public string FileName { get; set; }
            public int FileId { get; set; }
        }



    }
}
