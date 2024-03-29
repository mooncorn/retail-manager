﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;

namespace RMApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class ProductController : ControllerBase
    {
        private IProductData _productData;

        public ProductController(IProductData productData)
        {
            _productData = productData;
        }

        [HttpGet]
        [Route("All")]
        public List<ProductDBModel> GetAll()
        {
            return _productData.GetAll();
        }
    }
}
