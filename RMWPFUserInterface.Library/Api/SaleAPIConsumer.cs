﻿using RMWPFUserInterface.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RMWPFUserInterface.Library.Api
{
    public class SaleAPIConsumer : ISaleAPIConsumer
    {
        private IAPIHelper _apiHelper;

        public SaleAPIConsumer(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task PostSale(SaleModel sale)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale))
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
