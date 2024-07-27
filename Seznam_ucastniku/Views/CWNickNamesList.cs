using Microsoft.EntityFrameworkCore;
using Seznam_ucastniku.DataAcces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using ClosedXML.Excel;
using SystemOrientation = System.Windows.Controls.Orientation;
using SystemThickness = System.Windows.Thickness;


namespace Seznam_ucastniku
{
    public partial class MainWindow
    {
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
                Width = 425; Height = 550; WindowStartupLocation = WindowStartupLocation.CenterScreen; ResizeMode = ResizeMode.NoResize;
                Topmost = true; Title = "Seznam s přezdívkami";

                StackPanel SPMain = new StackPanel();
                StackPanel SPLine1 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                StackPanel SPLine2 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                LSelectedDate = new Label { Content = "Vyber datum", Margin = new SystemThickness(20), Width = 100, Height = 30 };
                DPSelectedDate = new DatePicker { Width = 150, Height = 30, };
                DPSelectedDate.SelectedDateChanged += async (sender, e) => await ShowList();
                SPLine1.Children.Add(LSelectedDate); SPLine1.Children.Add(DPSelectedDate);
                SPMain.Children.Add(SPLine1);
                LSumOfRecords = new Label { Content = "Celkem: ", Width = 100, Height = 30 };
                SPMain.Children.Add(LSumOfRecords);
                BExport = new Button { Content = "Export do xls", Margin = new SystemThickness(25), Width = 150, Height = 30 };
                BBack = new Button { Content = "Zpět", Margin = new SystemThickness(25), Width = 150, Height = 30 };
                BExport.Click += async (sender, e) => await ExportToXls();
                BBack.Click += (sender, e) => this.Close();
                SPLine2.Children.Add(BExport);SPLine2.Children.Add(BBack);
                SPMain.Children.Add(SPLine2);
                DGList = new DataGrid { Width = 375, Height = 300, AutoGenerateColumns = false, IsReadOnly = true };
                DataGridTextColumn firstNameCollum = new DataGridTextColumn { Header = "Jméno", Binding = new Binding("FirstName") };
                DataGridTextColumn lastNameCollum = new DataGridTextColumn { Header = "Příjmení", Binding = new Binding("LastName") };
                DataGridTextColumn nickNameCollum = new DataGridTextColumn { Header = "Přezdívka", Binding = new Binding("NickName") };
                DGList.Columns.Add(firstNameCollum); DGList.Columns.Add(lastNameCollum); DGList.Columns.Add(nickNameCollum);   
                SPMain.Children.Add(DGList);
                this.Content = SPMain;
            }
            public async Task ExportToXls() 
            {
                if (DPSelectedDate.SelectedDate != null)
                {
                    DateOnly SelectedDate = DateOnly.FromDateTime(DPSelectedDate.SelectedDate.Value);
                    using (var context = new SUDBContext())
                    {
                        var selectedRecords = await context.Records.Where(q => (q.InDay <= SelectedDate && q.OutDay >= SelectedDate)).ToListAsync();
                        if (selectedRecords.Count == 0) { MessageBox.Show("Není co ukládat", "Chyba"); }
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
                                for (int i = 0; i < selectedRecords.Count; i++)
                                {
                                    worksheet.Cell(i + 2, 1).Value = selectedRecords[i].FirstName;
                                    worksheet.Cell(i + 2, 2).Value = selectedRecords[i].LastName;
                                    worksheet.Cell(i + 2, 3).Value = selectedRecords[i].NickName;
                                }
                                try
                                {
                                    workbook.SaveAs(filePath);
                                    MessageBox.Show("Soubor byl uložen do: " + filePath);
                                }
                                catch (Exception ex) { MessageBox.Show("Chyba při ukládání souboru: " + ex.Message, "Chyba"); }
                            }
                        }
                    }
                }
                else MessageBox.Show("Není vybráno žádné datum!", "Chyba");
            }
            public async Task ShowList() 
            {
                if (DPSelectedDate.SelectedDate != null)
                {
                    DateOnly SelectedDate = DateOnly.FromDateTime(DPSelectedDate.SelectedDate.Value);
                    using (var context = new SUDBContext())
                    {
                        var selectedRecords = await context.Records.Where(q => (q.InDay <= SelectedDate && q.OutDay >= SelectedDate)).ToListAsync();
                        LSumOfRecords.Content = $"Celkem: {selectedRecords.Count}";
                        DGList.ItemsSource = selectedRecords;
                    }
                }              
            }
        }
    }

}