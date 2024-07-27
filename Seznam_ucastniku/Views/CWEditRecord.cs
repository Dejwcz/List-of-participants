using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using Seznam_ucastniku.DataAcces;
using System.Windows;
using System.Windows.Controls;


namespace Seznam_ucastniku
{
    public partial class MainWindow
    {
        public class CWEditRecord : CWNewRecord
        {
            private int _recordId;
            Button BDelete;

            public CWEditRecord(int recordId) : base()
            {
                _recordId = recordId;
                this.Title = "Editace záznamu";
                this.Height = 535;
                StackPanel SPMain = this.Content as StackPanel;
                Button BDelete = new Button { Content = "Smazat záznam", Margin = new Thickness(25), Width = 150, Height = 30, 
                    HorizontalAlignment = HorizontalAlignment.Center, };
                BDelete.Click += (sender, e) => DeleteRecord();
                SPMain.Children.Add(BDelete);
                this.Content = SPMain;
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
            private async void DeleteRecord()
            {
                using (var context = new SUDBContext())
                {
                    var record = await context.Records.FirstAsync(r => r.Id == _recordId);
                    if (record != null)
                    {
                        context.Remove(record);
                        await context.SaveChangesAsync();
                        this.Close();
                    }
                }
            }

        }
    }

}