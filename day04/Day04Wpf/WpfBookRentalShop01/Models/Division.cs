using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookRentalShop01.Models
{
    public class Division : ObservableObject
    {
        private string _divisions;
        private string _dnames;

        public string Divisions
        {
            get => _divisions;
            set => SetProperty(ref _divisions, value);
        }
        public string DNames
        {
            get => _dnames;
            set => SetProperty(ref _dnames, value);
        }
    }
}
