using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookRentalShop01.Models
{
    public class Rental : ObservableObject
    {
        private int _idx;
        private int _memberidx;
        private string _membernames;
        private int _bookidx;
        private string _booknames;
        private DateTime? _rentaldate;
        private DateTime? _returndate;

        public int Idx {
            get => _idx;
            set => _idx = value;
        }
        public int MemberIdx {
            get => _memberidx;
            set => SetProperty(ref _memberidx, value);
        }
        public string MemberNames {
            get => _membernames;
            set => SetProperty(ref _membernames, value);
        }
        public int BookIdx {
            get => _bookidx;
            set => SetProperty(ref _bookidx, value);
        }
        public string BookNames {
            get => _booknames;
            set => SetProperty(ref _booknames, value);
        }
        public DateTime? RentalDate {
            get => _rentaldate;
            set => SetProperty(ref _rentaldate, value);
        }
        public DateTime? ReturnDate {
            get => _returndate;
            set => SetProperty(ref _returndate, value);
        }
    }
}
