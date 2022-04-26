using Caliburn.Micro;
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
        private BindingList<string> _cart;
        private int _itemQuantity;
        private IProductAPIConsumer _productAPIConsumer;

        public BindingList<ProductModel> Products
        {
            get { return _products; }
            set 
            {
                _products = value; 
                NotifyOfPropertyChange(() => Products);
            }
        }

        public BindingList<string> Cart
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
            }
        }

        public string SubTotal
        {
            get { return "$0.00"; }
        }

        public string Tax
        {
            get { return "$0.00"; }
        }

        public string Total
        {
            get { return "$0.00"; }
        }

        public bool CanAddToCart
        {
            get
            {
                return true;
            }
        }

        public bool CanRemoveFromCart
        {
            get
            {
                return true;
            }
        }

        public bool CanCheckOut
        {
            get
            {
                return true;
            }
        }

        public SalesViewModel(IProductAPIConsumer productAPIConsumer)
        {
            _productAPIConsumer = productAPIConsumer;
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

        }

        public void RemoveFromCart()
        {

        }

        public void CheckOut()
        {

        }
    }
}
