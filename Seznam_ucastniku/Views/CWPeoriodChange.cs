using System.Windows;
using System.Windows.Controls;
using SystemOrientation = System.Windows.Controls.Orientation;
using SystemThickness = System.Windows.Thickness;


namespace Seznam_ucastniku
{
    public partial class MainWindow
    {
        public class CWPeoriodChange : Window
        {
            protected DatePicker DPPeriodStart, DPPeriodEnd;
            protected Label LPeriod;
            protected Button BSave, BBack;

            public CWPeoriodChange()
            {
                InicializeComponents();
            }

            protected void InicializeComponents()
            {
                Width = 425; Height = 270; WindowStartupLocation = WindowStartupLocation.CenterScreen; ResizeMode = ResizeMode.NoResize;
                Topmost = true; Title = "Volba období pro zobrazení";

                StackPanel SPMain = new StackPanel();
                StackPanel SPLine2 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                StackPanel SPLine3 = new StackPanel { Orientation = SystemOrientation.Horizontal };
                LPeriod = new Label { Content = "Zvol počáteční a koncové datum pro zobrazení", Margin = new SystemThickness(20), Height = 25, 
                    HorizontalAlignment = HorizontalAlignment.Center };
                SPMain.Children.Add(LPeriod);
                DPPeriodStart = new DatePicker { DisplayDate = DateTime.Now, Height = 25, Width = 150, Margin = new SystemThickness(25) };
                DPPeriodEnd = new DatePicker { DisplayDate = DateTime.Now, Height = 25, Width = 150, Margin = new SystemThickness(25) };
                SPLine2.Children.Add(DPPeriodStart);SPLine2.Children.Add(DPPeriodEnd);
                SPMain.Children.Add(SPLine2);
                BSave = new Button { Content = "Ulož", Margin = new SystemThickness(25), Width = 150, Height = 30 };
                BBack = new Button { Content = "Zpět", Margin = new SystemThickness(25), Width = 150, Height = 30 };
                BSave.Click += (sender, e) => SaveDates();
                BBack.Click += (sender, e) => this.Close();
                SPLine3.Children.Add(BSave); SPLine3.Children.Add(BBack);
                SPMain.Children.Add(SPLine3);
                this.Content = SPMain;
            }
            private void SaveDates() 
            {
                AppSettings appSettings = new AppSettings();
                appSettings = SettingsManager.LoadSettings();
                if (DPPeriodStart.SelectedDate != null)
                {
                    if (DPPeriodEnd.SelectedDate != null)
                    {
                        if (DPPeriodEnd.SelectedDate >= DPPeriodStart.SelectedDate)
                        {
                            appSettings.StartDate = (DateTime)DPPeriodStart.SelectedDate;
                            appSettings.EndDate = (DateTime)DPPeriodEnd.SelectedDate;
                            SettingsManager.SaveSettings(appSettings);
                            this.Close();
                        }
                        else MessageBox.Show("Zadané období není platné", "Chyba");
                    }
                    else MessageBox.Show("Prosím vyberte koncové datum.", "Chyba");
                }
                else MessageBox.Show("Prosím vyberte počáteční datum.", "Chyba");
            }
        }
    }

}