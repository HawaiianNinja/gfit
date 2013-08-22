using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gFit.Models.ViewModels
{
    public class DataVM
    {

        public string Data { get; set; }
        public string Caption { get; set; }

        public DataVM(string data, string caption)
        {
            this.Data = data;
            this.Caption = caption;
        }

        public DataVM(object data, string caption)
        {
            this.Data = data.ToString();
            this.Caption = caption;
        }




    }
}