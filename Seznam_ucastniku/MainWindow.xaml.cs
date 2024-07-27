using Seznam_ucastniku.DataAcces;
using Seznam_ucastniku.Entities;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Drawing.Charts;
using SystemThickness = System.Windows.Thickness;


namespace Seznam_ucastniku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        public class CWCounts : Window
        {
            protected Button BBack;
            protected DataGrid DGCounts;

            public CWCounts() 
            {
                InicializeComponents();
            }
            protected void InicializeComponents()
            {
                Width = 1000; Height = 300; WindowStartupLocation = WindowStartupLocation.CenterScreen; ResizeMode = ResizeMode.NoResize;
                Topmost = true; Title = "Součty";

                StackPanel SPMain = new StackPanel();
                DGCounts = new DataGrid { Width = 900, Height = 150, Margin = new SystemThickness(25), HorizontalAlignment = HorizontalAlignment.Center,
                    IsReadOnly = true };
                SPMain.Children.Add(DGCounts);
                BBack = new Button { Content = "Zpět", HorizontalAlignment = HorizontalAlignment.Center, Width = 200, Height = 25};
                BBack.Click += (sender, e) => this.Close();
                SPMain.Children.Add(BBack);
                this.Content = SPMain;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            LoadData();

        }
        private void LoadData()
        {
            AppSettings appSettings = new AppSettings();
            appSettings = SettingsManager.LoadSettings();
            if (appSettings != null)
            {
                LPeriod.Content = $@"Zvolené období od: {DateOnly.FromDateTime(appSettings.StartDate)} do: {DateOnly.FromDateTime(appSettings.EndDate)}";
                
            }
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
        private void BPeriodChange_Click(object sender, RoutedEventArgs e)
        {
            
            var WPeriodChange = new CWPeoriodChange();
            WPeriodChange.ShowDialog();
        }

        private void BStats_Click(object sender, RoutedEventArgs e)
        {
            var WCounts = new CWCounts();
            WCounts.ShowDialog();
        }

        private void BTest_Click(object sender, RoutedEventArgs e)
        {
            AppSettings appSettings = new AppSettings();
            appSettings = SettingsManager.LoadSettings();
            TimeSpan dif = appSettings.EndDate - appSettings.StartDate;
            int numberOfDays =  Convert.ToInt32((dif.TotalDays.ToString())) + 1;
            MessageBox.Show(numberOfDays.ToString());
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
    public class AppSettings
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class SettingsManager
    {
        private static readonly string settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyAppSettings.xml");
        public static void SaveSettings(AppSettings settings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            using (FileStream fs = new FileStream(settingsFilePath, FileMode.Create))
            {
                serializer.Serialize(fs, settings);
            }
        }
        public static AppSettings LoadSettings()
        {
            if (!File.Exists(settingsFilePath))
                return new AppSettings(); // Vrátí defaultní hodnoty

            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            using (FileStream fs = new FileStream(settingsFilePath, FileMode.Open))
            {
                return (AppSettings)serializer.Deserialize(fs);
            }
        }
    }

}