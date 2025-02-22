﻿using Microsoft.EntityFrameworkCore;
using Seznam_ucastniku.DataAcces;
using Seznam_ucastniku.Entities;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Linq;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace Seznam_ucastniku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {       
        public class CWNewRecord : Window
        {
            protected TextBox TFirstName, TLastName, TNickName;
            protected DatePicker DPInDate, DPOutDate;
            protected CheckBox CInDateLunch, COutDateLunch;

            public CWNewRecord()
            {
                InicializeComponents();
            }
            protected void InicializeComponents()
            {
                Width = 425; Height = 500; WindowStartupLocation = WindowStartupLocation.CenterScreen; ResizeMode = ResizeMode.NoResize; 
                Topmost = true; Title = "Nový záznam";

                StackPanel SPMain = new StackPanel();
                StackPanel SPLine1 = new StackPanel { Orientation = Orientation.Horizontal };
                StackPanel SPLine2 = new StackPanel { Orientation = Orientation.Horizontal };
                StackPanel SPLine3 = new StackPanel { Orientation = Orientation.Horizontal };
                StackPanel SPLine4 = new StackPanel { Orientation = Orientation.Horizontal };
                StackPanel SPLine5 = new StackPanel { Orientation = Orientation.Horizontal };
                StackPanel SPLine6 = new StackPanel { Orientation = Orientation.Horizontal };

                Label LFirstName = new Label { Content = "Jméno", Margin = new Thickness(20), Width = 100, Height = 30 };
                TFirstName = new TextBox { Margin = new Thickness(20), Width=200, Height = 30 };
                SPLine1.Children.Add(LFirstName);SPLine1.Children.Add(TFirstName);
                SPMain.Children.Add(SPLine1);
                Label LLastName = new Label { Content = "Příjmení", Margin = new Thickness(20), Width = 100, Height = 30 };
                TLastName = new TextBox { Margin = new Thickness(20), Width = 200, Height = 30 };
                SPLine2.Children.Add(LLastName); SPLine2.Children.Add(TLastName);
                SPMain.Children.Add(SPLine2);
                Label LNickName = new Label { Content = "Přezdívka", Margin = new Thickness(20), Width = 100, Height = 30 };
                TNickName = new TextBox { Margin = new Thickness(20), Width = 200, Height = 30 };
                SPLine3.Children.Add(LNickName); SPLine3.Children.Add(TNickName);
                SPMain.Children.Add(SPLine3);
                Label LInDate = new Label { Content = "Příjezd", Margin = new Thickness(20), Width = 80, Height = 30 };
                DPInDate = new DatePicker { Width = 150, Height = 30 };
                Label LInDateLunch = new Label { Content = "Oběd", Margin = new Thickness(20), Width = 40, Height = 30 };
                CInDateLunch = new CheckBox { IsChecked = false, VerticalAlignment = VerticalAlignment.Center };
                SPLine4.Children.Add(LInDate); SPLine4.Children.Add(DPInDate);SPLine4.Children.Add(LInDateLunch);SPLine4.Children.Add(CInDateLunch);
                SPMain.Children.Add(SPLine4);
                Label LOutDate = new Label { Content = "Odjezd", Margin = new Thickness(20), Width = 80, Height = 30 };
                DPOutDate = new DatePicker { Width = 150, Height = 30 };
                Label LOutDateLunch = new Label { Content = "Oběd", Margin = new Thickness(20), Width = 40, Height = 30 };
                COutDateLunch = new CheckBox { IsChecked = false, VerticalAlignment = VerticalAlignment.Center };
                SPLine5.Children.Add(LOutDate); SPLine5.Children.Add(DPOutDate); SPLine5.Children.Add(LOutDateLunch); SPLine5.Children.Add(COutDateLunch);
                SPMain.Children.Add(SPLine5);
                Button BSave = new Button { Content = "Ulož", Margin = new Thickness(25), Width = 150, Height = 30 };
                Button BBack = new Button { Content = "Zpět", Margin = new Thickness(25), Width = 150, Height = 30 };
                BSave.Click += (sender, e) => SaveRecord();
                BBack.Click += (sender, e) => this.Close();
                SPLine6.Children.Add(BSave);SPLine6.Children.Add(BBack);
                SPMain.Children.Add(SPLine6);
                this.Content = SPMain;
            }
            public virtual async void SaveRecord()
            {
                Record NewRecord = new Record
                {
                    FirstName = this.TFirstName.Text,
                    LastName = TLastName.Text,
                    NickName = TNickName.Text,
                    InDay = DateOnly.FromDateTime(DPInDate.SelectedDate.Value),
                    InDayLunch = CInDateLunch.IsChecked,
                    OutDay = DateOnly.FromDateTime(DPOutDate.SelectedDate.Value),
                    OutDayLunch = COutDateLunch.IsChecked,
                };
                await using (var context = new SUDBContext())
                {
                    await context.Records.AddAsync(NewRecord);
                    await context.SaveChangesAsync();
                }
                MessageBox.Show(this,"Záznam byl uložen");
                this.Close();
            }
        }
        public class CWEditRecord : CWNewRecord
        {
            private int _recordId;

            public CWEditRecord(int recordId) : base()
            {
                _recordId = recordId;
                this.Title = "Editace záznamu";
                LoadRecordData();
            }
            private async void LoadRecordData() 
            {
                using (var context = new SUDBContext())
                {
                    var record = await context.Records.FirstAsync(r => r.Id == _recordId);
                    if (record != null)
                    {
                        TFirstName.Text = record.FirstName;
                        TLastName.Text = record.LastName;
                        TNickName.Text = record.NickName;
                        DPInDate.SelectedDate = DateTime.Parse(record.InDay.ToString());
                        CInDateLunch.IsChecked = record.InDayLunch;
                        DPOutDate.SelectedDate = DateTime.Parse(record.OutDay.ToString());
                        COutDateLunch.IsChecked = record.OutDayLunch;
                    }
                }
            }
            public override async void SaveRecord()
            {
                using (var context = new SUDBContext())
                {
                    var record = context.Records.FirstOrDefault(r => r.Id == _recordId);
                    if (record != null)
                    {
                        record.FirstName = TFirstName.Text;
                        record.LastName = TLastName.Text;
                        record.NickName = TNickName.Text;
                        record.InDay = DateOnly.FromDateTime(DPInDate.SelectedDate.Value);
                        record.InDayLunch = CInDateLunch.IsChecked;
                        record.OutDay = DateOnly.FromDateTime(DPOutDate.SelectedDate.Value);
                        record.OutDayLunch = COutDateLunch.IsChecked;

                        await context.SaveChangesAsync();
                    }
                }
                MessageBox.Show(this, "Záznam byl aktualizován");
                this.Close();
            }
            
        }
        public class CWNickNamesList : Window
        {
            protected Label LSelectedDate, LSumOfRecords;
            protected DatePicker DPSelectedDate;
            protected DataGrid DGList;
            protected Button BExport, BBack;
            public CWNickNamesList()
            {
                InicializeComponents();
            }
            protected void InicializeComponents()
            {
                Width = 425; Height = 500; WindowStartupLocation = WindowStartupLocation.CenterScreen; ResizeMode = ResizeMode.NoResize;
                Topmost = true; Title = "Seznam s přezdívkami";

                StackPanel SPMain = new StackPanel();
                StackPanel SPLine1 = new StackPanel { Orientation = Orientation.Horizontal };
                StackPanel SPLine2 = new StackPanel { Orientation = Orientation.Horizontal };
                LSelectedDate = new Label { Content = "Vyber datum", Margin = new Thickness(20), Width = 100, Height = 30 };
                DPSelectedDate = new DatePicker { Width = 150, Height = 30 };
                DPSelectedDate.SelectedDateChanged += async (sender, e) => await ShowList();
                SPLine1.Children.Add(LSelectedDate); SPLine1.Children.Add(DPSelectedDate);
                SPMain.Children.Add(SPLine1);
                LSumOfRecords = new Label { Content = "Celkem: ", Width = 100, Height = 30 };
                SPMain.Children.Add(LSumOfRecords);
                BExport = new Button { Content = "Export do xls", Margin = new Thickness(25), Width = 150, Height = 30 };
                BBack = new Button { Content = "Zpět", Margin = new Thickness(25), Width = 150, Height = 30 };
                BExport.Click += async (sender, e) => await ExportToXls();
                BBack.Click += (sender, e) => this.Close();
                SPLine2.Children.Add(BExport);SPLine2.Children.Add(BBack);
                SPMain.Children.Add(SPLine2);
                DGList = new DataGrid { Width = 375, Height = 300, AutoGenerateColumns = false };
                DataGridTextColumn firstNameCollum = new DataGridTextColumn { Header = "Jméno", Binding = new Binding("FirstName") };
                DataGridTextColumn lastNameCollum = new DataGridTextColumn { Header = "Příjmení", Binding = new Binding("LastName") };
                DataGridTextColumn nickNameCollum = new DataGridTextColumn { Header = "Přezdívka", Binding = new Binding("NickName") };
                DGList.Columns.Add(firstNameCollum); DGList.Columns.Add(lastNameCollum); DGList.Columns.Add(nickNameCollum);   
                SPMain.Children.Add(DGList);
                this.Content = SPMain;
            }
            public async Task ExportToXls() 
            {
                DateOnly SelectedDate = DateOnly.FromDateTime(DPSelectedDate.SelectedDate.Value);
                using (var context = new SUDBContext())
                {
                    var selectedRecords = await context.Records.Where(q => (q.InDay <= SelectedDate && q.OutDay >= SelectedDate)).ToListAsync();
                    if (selectedRecords.Count == 0) { MessageBox.Show("Není co ukládat"); }
                    else
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog
                        {
                            Title = "Uložit soubor",
                            Filter = "XLSX files (*.xlsx)|*.xlsx",
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                        };

                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string filePath = saveFileDialog.FileName;
                            var workbook = new XLWorkbook();
                            var worksheet = workbook.Worksheets.Add("Seznam");
                            worksheet.Cell(1, 1).Value = "Jméno";
                            worksheet.Cell(1, 2).Value = "Příjmení";
                            worksheet.Cell(1, 3).Value = "Přezdívka";
                            for (int i = 0; i<selectedRecords.Count; i++)
                            {
                                worksheet.Cell(i + 2, 1).Value = selectedRecords[i].FirstName;
                                worksheet.Cell(i + 2, 2).Value = selectedRecords[i].LastName;
                                worksheet.Cell(i + 2, 3).Value = selectedRecords[i].NickName;                         
                            }
                            try
                            {
                                workbook.SaveAs(filePath);
                            }
                            catch (Exception ex) { MessageBox.Show("Chyba při ukládání souboru: " + ex.Message); }
                            MessageBox.Show("Soubor byl uložen do: " + filePath);
                        }
                    }
                }
            }
            public async Task ShowList() 
            {
                DateOnly SelectedDate = DateOnly.FromDateTime(DPSelectedDate.SelectedDate.Value);
                using (var context = new SUDBContext())
                {
                    var selectedRecords = await context.Records.Where(q => (q.InDay <= SelectedDate && q.OutDay >= SelectedDate)).ToListAsync();
                    LSumOfRecords.Content = $"Celkem: { selectedRecords.Count}";
                    DGList.ItemsSource = selectedRecords;
                }
                
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            using (var context = new SUDBContext())
            {
                var records = context.Records.ToList();
                DGRecords.ItemsSource = records;
            }
                
        }
        private void BNewRecord_Click(object sender, RoutedEventArgs e)
        {
            var WNewRecord = new CWNewRecord();
            WNewRecord.ShowDialog();
        }
        private void DGRecords_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid datagrid && datagrid.SelectedItem is Record selectedRecord)
            {
                int selectedId = selectedRecord.Id;
                var WEditRecord = new CWEditRecord(selectedId);
                WEditRecord.ShowDialog();
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            LoadData();
        }
        private void BNickNameList_Click(object sender, RoutedEventArgs e)
        {
            var WNickNameList = new CWNickNamesList();
            WNickNameList.ShowDialog();
        }
    }
    public class BoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "ano" : "ne";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.ToLower() == "ano";
            }
            return null;
        }
    }

}