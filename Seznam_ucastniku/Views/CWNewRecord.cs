using Seznam_ucastniku.DataAcces;
using Seznam_ucastniku.Entities;
using System.Windows;
using System.Windows.Controls;
using SystemOrientation = System.Windows.Controls.Orientation;
using SystemThickness = System.Windows.Thickness;


namespace Seznam_ucastniku
{
    public partial class MainWindow
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
                AppSettings appSettings = new AppSettings();
                appSettings = SettingsManager.LoadSettings();

                Width = 425; Height = 500; WindowStartupLocation = WindowStartupLocation.CenterScreen; ResizeMode = ResizeMode.NoResize; 
                Topmost = true; Title = "Nový záznam";

                StackPanel SPMain = new StackPanel();
                StackPanel SPLine1 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                StackPanel SPLine2 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                StackPanel SPLine3 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                StackPanel SPLine4 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                StackPanel SPLine5 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                StackPanel SPLine6 = new StackPanel { Orientation = SystemOrientation.Horizontal };

                Label LFirstName = new Label { Content = "Jméno", Margin = new SystemThickness(20), Width = 100, Height = 30 };
                TFirstName = new TextBox { Margin = new SystemThickness(20), Width=200, Height = 30 };
                SPLine1.Children.Add(LFirstName);SPLine1.Children.Add(TFirstName);
                SPMain.Children.Add(SPLine1);
                Label LLastName = new Label { Content = "Příjmení", Margin = new SystemThickness(20), Width = 100, Height = 30 };
                TLastName = new TextBox { Margin = new SystemThickness(20), Width = 200, Height = 30 };
                SPLine2.Children.Add(LLastName); SPLine2.Children.Add(TLastName);
                SPMain.Children.Add(SPLine2);
                Label LNickName = new Label { Content = "Přezdívka", Margin = new SystemThickness(20), Width = 100, Height = 30 };
                TNickName = new TextBox { Margin = new SystemThickness(20), Width = 200, Height = 30 };
                SPLine3.Children.Add(LNickName); SPLine3.Children.Add(TNickName);
                SPMain.Children.Add(SPLine3);
                Label LInDate = new Label { Content = "Příjezd", Margin = new SystemThickness(20), Width = 80, Height = 30 };
                DPInDate = new DatePicker { Width = 150, Height = 30, DisplayDateStart = appSettings.StartDate, DisplayDateEnd = appSettings.EndDate };
                Label LInDateLunch = new Label { Content = "Oběd", Margin = new SystemThickness(20), Width = 40, Height = 30 };
                CInDateLunch = new CheckBox { IsChecked = false, VerticalAlignment = VerticalAlignment.Center };
                SPLine4.Children.Add(LInDate); SPLine4.Children.Add(DPInDate);SPLine4.Children.Add(LInDateLunch);SPLine4.Children.Add(CInDateLunch);
                SPMain.Children.Add(SPLine4);
                Label LOutDate = new Label { Content = "Odjezd", Margin = new SystemThickness(20), Width = 80, Height = 30 };
                DPOutDate = new DatePicker { Width = 150, Height = 30, DisplayDateStart = appSettings.StartDate, DisplayDateEnd = appSettings.EndDate };
                Label LOutDateLunch = new Label { Content = "Oběd", Margin = new SystemThickness(20), Width = 40, Height = 30 };
                COutDateLunch = new CheckBox { IsChecked = false, VerticalAlignment = VerticalAlignment.Center };
                SPLine5.Children.Add(LOutDate); SPLine5.Children.Add(DPOutDate); SPLine5.Children.Add(LOutDateLunch); SPLine5.Children.Add(COutDateLunch);
                SPMain.Children.Add(SPLine5);
                Button BSave = new Button { Content = "Ulož", Margin = new SystemThickness(25), Width = 150, Height = 30 };
                Button BBack = new Button { Content = "Zpět", Margin = new SystemThickness(25), Width = 150, Height = 30 };
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
    }

}