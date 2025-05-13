using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class BooksViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator;

        private ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        private Book _selectedBook;
        public Book SelectedBook {
            get => _selectedBook;
            set {
                SetProperty(ref _selectedBook, value);
                _isUpdate = true; // 수정상태
            }
        }

        private Divisions _divisions;
        public Divisions Divisions {
            get => _divisions;
            set{
                SetProperty(ref _divisions, value);
                _isUpdate = true; // 수정상태
            }
        }

        private bool _isUpdate;

        public BooksViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator;
            InitVariable();
            LoadControlFromDb();
            LoadGridFromDb();
        }

        private void InitVariable()
        {
            SelectedBook = new Book
            {
                Idx = 0,
                Author = string.Empty,
                Division = string.Empty,
                Names = string.Empty,
                ReleaseDate = DateTime.Now,
                ISBN = string.Empty,
                Price = 0,
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
                        query = @"UPDATE bookstbl
                                     SET author = @author,
                                         division = @division,
                                         names = @names,
                                         releasedate = @releasedate,
                                         isbn = @isbn,
                                         price = @price
                                   WHERE idx = @idx";
                    }
                    else
                    {
                        query = @"INSERT INTO membertbl (author, division, names, releasedate, isbn, price)
                                                 VALUES (@author, @division, @names, @releasedate, @isbn, @price);";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@author", SelectedBook.Author);
                    cmd.Parameters.AddWithValue("@division", SelectedBook.Division);
                    cmd.Parameters.AddWithValue("@releasedate", SelectedBook.ReleaseDate);
                    cmd.Parameters.AddWithValue("@isbn", SelectedBook.ISBN);
                    cmd.Parameters.AddWithValue("@price", SelectedBook.Price);
                    if (_isUpdate) cmd.Parameters.AddWithValue("@idx", SelectedBook.Idx);

                    var resultCnt = cmd.ExecuteNonQuery();
                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info("회원 데이터 저장완료");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장", "저장성공!");
                    }
                    else
                    {
                        Common.LOGGER.Warn("회원 데이터 저장실패!");
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
        public void DelData()
        {

        }

        private async void LoadControlFromDb()
        {
            string query = "SELECT division, names FROM divtbl";

            ObservableCollection<Divisions> divisions = new ObservableCollection<Divisions>();

            using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader(); // 데이터 가져올때

                    while (reader.Read())
                    {
                        var division = reader.GetString("division");
                        var dnames = reader.GetString("names");

                        divisions.Add(new Divisions
                        {
                            Division = division,
                            DNames = dnames
                        });
                    }

                    Divisions = divisions;
                }
                catch (MySqlException ex)
                {
                    Common.LOGGER.Error(ex.Message);
                    await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
                }
            }

            Common.LOGGER.Info("책종류 데이터 로드");
        }

        private async void LoadGridFromDb()
        {
            try
            {
                string query = @"SELECT b.Idx, b.Author, b.Division, b.Names, b.ReleaseDate, b.ISBN, b.Price, d.Names AS dNames
                                   FROM bookstbl AS b, divtbl AS d
                                  WHERE b.Division = d.Division
                                  ORDER BY b.Idx";
                ObservableCollection<Book> books = new ObservableCollection<Book>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var idx = reader.GetInt32("Idx");
                        var division = reader.GetString("Division");
                        var dnames = reader.GetString("dNames");
                        var names = reader.GetString("Names");
                        var author = reader.GetString("Author");
                        var isbn = reader.GetString("ISBN");
                        var releasedate = reader.GetDateTime("ReleaseDate");
                        var price = reader.GetInt32("Price");

                        books.Add(new Book
                        {
                            Idx = idx,
                            Division = division,
                            DNames = dnames,
                            Names = names,
                            Author = author,
                            ISBN = isbn,
                            ReleaseDate = releasedate,
                            Price = price
                        });
                    }
                }

                Books = books;
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            Common.LOGGER.Info("책관리 데이터 로드");
        }
    }
}
