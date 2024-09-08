using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCRUD.DTO;
using WebApiCRUD.Models;
using WebApiCRUD.Repositories;

namespace WebApiCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok((await _unitOfWork.Product.GetAll()));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GeneralResponse>> GetProduct(int id)
        {
            Product product = (await _unitOfWork.Product.GetFirstOrDefault(e=>e.Id==id, "Category"));
            GeneralResponse generalResponse = new GeneralResponse();
            if (product != null)
            {
                ProductWithCategoryDTO productWithCategoryDTO = new();
                productWithCategoryDTO.ProductPrice = product.Price;
                productWithCategoryDTO.ProductName = product.Name;
                productWithCategoryDTO.ProductDescription = product.Description;
                productWithCategoryDTO.CategoryName = product.Category.Name;


                generalResponse.IsSuccess = true;
                generalResponse.Data = productWithCategoryDTO;
            }
            else
            {
                generalResponse.IsSuccess = false;
                generalResponse.Data = "Id is valid";
            }

            return generalResponse;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            await _unitOfWork.Product.AddAsync(product);
            await _unitOfWork.Save();
            return CreatedAtAction("GetProduct",new {id=product.Id}, product);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id,Product product)
        {
            Product productFromDb=await _unitOfWork.Product.GetFirstOrDefault(e => e.Id == id);

            if (productFromDb != null)
            {
                // Map
                productFromDb.Name = product.Name;
                productFromDb.Description = product.Description;
                productFromDb.Price = product.Price;
                productFromDb.CategoryId = product.CategoryId;

                _unitOfWork.Product.Update(productFromDb);
                await _unitOfWork.Save();

                return NoContent();
            }
            return NotFound("Product not found");
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product product = await _unitOfWork.Product.GetFirstOrDefault(e => e.Id == id);
            if (product != null)
            {
                _unitOfWork.Product.Delete(product);
                await _unitOfWork.Save();
                return NoContent();
            }
            return NotFound("Product not found");
        }

    }
}
