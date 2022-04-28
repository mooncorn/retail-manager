using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDataManager.Library
{
    public class ConfigHelper
    {
        public static decimal TaxRate
        {
            get
            {
                try
                {
                    return Convert.ToDecimal(ConfigurationManager.AppSettings.Get("taxRate"));
                }
                catch (FormatException ex)
                {
                    throw new ConfigurationErrorsException($"The tax rate is not set up properly: {ex.Message}");
                }
            }
        }
    }
}
