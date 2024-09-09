using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCRUD.DTO;
using WebApiCRUD.Models;
using WebApiCRUD.Repositories;

namespace WebApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            return Ok((await _unitOfWork.Category.GetAll()));
        }
        //----------------------------------------------------------------------------------------------------
        [HttpGet("C")]
        [Authorize]
        public async Task<ActionResult<List<CategoryWithProductCountDTO>>> GetCategoriesWithProductCount()
        {
            IEnumerable<Category> Categories = (await _unitOfWork.Category.GetAll(IncludeWord: "Products"));
            List<CategoryWithProductCountDTO> CategoriesDto = new();
            CategoryWithProductCountDTO categoryDto;
            foreach (var item in Categories)
            {
                categoryDto = new();
                categoryDto.CategoryId=item.Id;
                categoryDto.CategoryName=item.Name;
                categoryDto.ProdutCount = item.Products.Count();

                CategoriesDto.Add(categoryDto);
            }
            return CategoriesDto;
        }
        //----------------------------------------------------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            return Ok((await _unitOfWork.Category.GetFirstOrDefault(e=>e.Id==id)));
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            await _unitOfWork.Category.AddAsync(category);
            await _unitOfWork.Save();
            return CreatedAtAction("GetCategory", new {id=category.Id }, category);
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            Category categoryFromDb = await _unitOfWork.Category.GetFirstOrDefault(e => e.Id == id);
            if (categoryFromDb != null)
            {
                // Map
                categoryFromDb.Name = category.Name;
                categoryFromDb.Description = category.Description;

                _unitOfWork.Category.Update(categoryFromDb);
                await _unitOfWork.Save();
                return NoContent();
            }
            return NotFound("Category Not Found");
        }
        //----------------------------------------------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = await _unitOfWork.Category.GetFirstOrDefault(e => e.Id == id);
            if (category != null)
            {
                _unitOfWork.Category.Delete(category);
                await _unitOfWork.Save();
                return NoContent();
            }
            return NotFound("category not found");
        }
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------

    }
}
