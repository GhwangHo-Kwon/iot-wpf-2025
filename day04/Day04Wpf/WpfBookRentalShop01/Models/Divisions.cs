using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookRentalShop01.Models
{
    public class Divisions : ObservableObject
    {
        private string _division;
        private string _dnames;

        public string Division
        {
            get => _division;
            set => SetProperty(ref _division, value);
        }
        public string DNames
        {
            get => _dnames;
            set => SetProperty(ref _dnames, value);
        }
    }
}
