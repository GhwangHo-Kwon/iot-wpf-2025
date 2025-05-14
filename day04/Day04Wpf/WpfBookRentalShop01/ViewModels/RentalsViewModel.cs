using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class RentalsViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;

        public ObservableCollection<KeyValuePair<int, string>> MemberName { get; set; }
        public ObservableCollection<KeyValuePair<int, string>> BookName { get; set; }

        private ObservableCollection<Rental> _rentals;
        public ObservableCollection<Rental> Rentals {
            get => _rentals;
            set => SetProperty(ref _rentals, value);
        }

        private Rental _selectedRental;
        public Rental SelectedRental {
            get => _selectedRental;
            set {
                SetProperty(ref _selectedRental, value);
                _isUpdate = true;
            }
        }

        private bool _isUpdate;

        public RentalsViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator;
            InitVariable();

            LoadControlFromBookDb();
            LoadControlFromMemberDb();
            LoadGridFromDb();
        }

        private void InitVariable()
        {
            SelectedRental = new Rental
            {
                Idx = 0,
                MemberIdx = 0,
                MemberNames = string.Empty,
                BookIdx = 0,
                BookNames = string.Empty,
                RentalDate = null,
                ReturnDate = null
            };

            _isUpdate = false;
        }

        [RelayCommand]
        public void SetInit()
        {
            InitVariable();
        }

        [RelayCommand]
        public async void SaveData()
        {
            try
            {
                string query = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    if (_isUpdate)
                    {
                        query = @"UPDATE rentaltbl
                                     SET memberidx = @memberidx,
                                         bookidx = @bookidx,
                                         rentaldate = @rentaldate,
                                         returndate = @returndate
                                   WHERE idx = @idx";
                    }
                    else
                    {
                        query = @"INSERT INTO rentaltbl (memberidx, bookidx, rentaldate, returndate)
                                                 VALUES (@memberidx, @bookidx, @rentaldate, @returndate)";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@memberidx", SelectedRental.MemberIdx);
                    cmd.Parameters.AddWithValue("@bookidx", SelectedRental.BookIdx);
                    cmd.Parameters.AddWithValue("@rentaldate", SelectedRental.RentalDate);
                    cmd.Parameters.AddWithValue("@returndate", SelectedRental.ReturnDate);
                    if (_isUpdate) cmd.Parameters.AddWithValue("@idx", SelectedRental.Idx);

                    var resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info("책렌탈 데이터 저장완료");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장", "저장성공!");
                    }
                    else
                    {
                        Common.LOGGER.Warn("책렌탈 데이터 저장실패!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장", "저장실패!!");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            LoadGridFromDb();
        }

        [RelayCommand]
        public async void DelData()
        {
            if (!_isUpdate)
            {
                await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "데이터를 선택하세요");
                return;
            }

            var result = await this.dialogCoordinator.ShowMessageAsync(this, "삭제여부", "삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative) return;  // Cancel했으면 메서드 빠져나감

            try
            {
                string query = "DELETE FROM rentaltbl WHERE idx = @idx";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@idx", SelectedRental.Idx);

                    var resultCnt = cmd.ExecuteNonQuery();

                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info($"책렌탈 데이터 {SelectedRental.MemberNames} / {SelectedRental.BookNames} 삭제완료");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제성공");
                    }
                    else
                    {
                        Common.LOGGER.Warn("책렌탈 데이터 삭제 실패!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제실패!!");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            LoadGridFromDb();
        }

        private async void LoadControlFromMemberDb()
        {
            try
            {
                string query = "SELECT idx, names FROM membertbl";

                ObservableCollection<KeyValuePair<int, string>> membernames = new ObservableCollection<KeyValuePair<int, string>>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader(); // 데이터 가져올때

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("idx");
                        var names = reader.GetString("names");

                        membernames.Add(new KeyValuePair<int, string>(idx, names));
                    }
                }

                MemberName = membernames;
            }
            catch (MySqlException ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
            Common.LOGGER.Info("회원이름 데이터 로드");
        }

        private async void LoadControlFromBookDb()
        {
            try
            {
                string query = "SELECT idx, names FROM bookstbl";

                ObservableCollection<KeyValuePair<int, string>> booknames = new ObservableCollection<KeyValuePair<int, string>>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader(); // 데이터 가져올때

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("idx");
                        var names = reader.GetString("names");

                        booknames.Add(new KeyValuePair<int, string>(idx, names));
                    }
                }

                BookName = booknames;
            }
            catch (MySqlException ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
            Common.LOGGER.Info("책종류 데이터 로드");
        }

        private async void LoadGridFromDb()
        {
            try
            {
                string query = @"SELECT r.idx, r.memberidx, m.names as MNames, r.bookidx, b.names as BNames, r.rentaldate, r.returndate
                                   FROM rentaltbl as r, bookstbl as b, membertbl as m
                                  WHERE r.memberidx = m.idx and r.bookidx = b.idx
                                  ORDER BY r.idx";

                ObservableCollection<Rental> rentals = new ObservableCollection<Rental>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("idx");
                        var memberidx = reader.GetInt32("memberidx");
                        var mnames = reader.GetString("MNames");
                        var bookidx = reader.GetInt32("bookidx");
                        var bnames = reader.GetString("BNames");
                        DateTime? rentaldate = reader.IsDBNull("rentaldate") ? (DateTime?)null : reader.GetDateTime("rentaldate");
                        DateTime? returndate = reader.IsDBNull("returndate") ? (DateTime?)null : reader.GetDateTime("returndate");

                        rentals.Add(new Rental
                        {
                            Idx = idx,
                            MemberIdx = memberidx,
                            MemberNames = mnames,
                            BookIdx = bookidx,
                            BookNames = bnames,
                            RentalDate = rentaldate,
                            ReturnDate = returndate
                        });
                    }
                }

                Rentals = rentals;
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            Common.LOGGER.Info("책렌탈 데이터 로드");
        }
    }
}
