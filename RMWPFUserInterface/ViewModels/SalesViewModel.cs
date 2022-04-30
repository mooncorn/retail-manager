﻿using Caliburn.Micro;
using RMWPFUserInterface.Helpers;
using RMWPFUserInterface.Library.Api;
using RMWPFUserInterface.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMWPFUserInterface.ViewModels
{
    public class SalesViewModel : Screen
    {
        private BindingList<ProductModel> _products;
        private ProductModel _selectedProduct;
        private BindingList<CartItemModel> _cart;
        private int _itemQuantity;
        private IProductAPIConsumer _productAPIConsumer;
        private ISaleAPIConsumer _saleAPIConsumer;
        private IConfigHelper _configHelper;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value; 
                NotifyOfPropertyChange(() => Products);
            }
        }

        public ProductModel SelectedProduct 
        { 
            get { return _selectedProduct; } 
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
                NotifyOfPropertyChange(() => Products);
            } 
        }

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        public int ItemQuantity
        {
            get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        public string SubTotal { get { return CalculateSubTotal().ToString("C"); } }

        public string Tax { get { return CalculateTaxAmount().ToString("C"); } }

        public string Total { get { return (CalculateSubTotal() + CalculateTaxAmount()).ToString("C"); } }

        public bool CanAddToCart { get { return ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity; } }

        public bool CanRemoveFromCart
        {
            get
            {
                return false;
            }
        }

        public bool CanCheckOut { get { return _cart.Count > 0; } }

        public SalesViewModel(IProductAPIConsumer productAPIConsumer, ISaleAPIConsumer saleAPIConsumer, IConfigHelper configHelper)
        {
            _productAPIConsumer = productAPIConsumer;
            _saleAPIConsumer = saleAPIConsumer;
            _configHelper = configHelper;
            Cart = new BindingList<CartItemModel>();
            _itemQuantity = 1;
        }

        protected override async void OnViewLoaded(object view)
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            Products = new BindingList<ProductModel>(await _productAPIConsumer.GetAll());
        }

        public void AddToCart()
        {
            var existingItem = Cart.FirstOrDefault((item) => item.Product.Id == SelectedProduct.Id);
            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity;
                Cart.ResetBindings();
            }
            else
            {
                CartItemModel cartItem = new CartItemModel
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity,
                };

                Cart.Add(cartItem);
            }

            SelectedProduct.QuantityInStock -= ItemQuantity;
            Products.ResetBindings();
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public void RemoveFromCart()
        {

        }

        public async Task CheckOut()
        {
            SaleModel saleModel = new SaleModel();

            foreach (CartItemModel item in Cart)
            {
                saleModel.SaleDetails.Add(new SaleDetailsModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleAPIConsumer.PostSale(saleModel);
        }

        public decimal CalculateSubTotal()
        {
            return Cart.Sum((cartItem) => cartItem.Product.RetailPrice * cartItem.QuantityInCart);
        }

        public decimal CalculateTaxAmount()
        {
            decimal taxRate = Convert.ToDecimal(_configHelper.TaxRate)/100;

            return Cart
                .Where((cartItem) => cartItem.Product.IsTaxable)
                .Sum((cartItem) => cartItem.Product.RetailPrice * cartItem.QuantityInCart * taxRate);
        }
    }
}
