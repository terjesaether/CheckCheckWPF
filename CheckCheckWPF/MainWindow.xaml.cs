using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CheckCheckWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1;HDR=NO'";
        //private string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1;HDR=NO'";

        NordubbProductions allProductions = new NordubbProductions();
        DataTable chosenExcelFileDataTable = new DataTable();

        // Dato for å lage en uløpsdato. Finner dagens dato fra nettet
        DateTime expDate = new DateTime(2016, 8, 15);
        //DateTime systemTime = DateTime.Now;
        DateTime systemTime = GetSystemTime.GetNetworkTime();

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DateTime.Compare(systemTime, expDate) > 0)
            {
                string svar = Utils.Prompt.ShowDialog("Enter password");
                if (svar != "drikk")
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }


            SplashScreen splash = new SplashScreen("NDIcon.ico");
            splash.Show(true);

            allProductions = ExcelScanning.scanDubtoolFolder(lboxShowFiles, Excel03ConString, txtActorName.Text.ToString(), allProductions);
        }

        private void btnRescanFolder_Click(object sender, RoutedEventArgs e)
        {

            WrapPanel newWrapPanel = CreatePanel.CreateNewWrapPanel();
            spShowResult.Items.Add(newWrapPanel);

        }



        private void btnCheckAllEps_Click(object sender, RoutedEventArgs e)
        {
            string searchString = afterSearchButtonPressed();
            Calculations.calculateAllSeriesAndEpisodes(allProductions, searchString, spShowResult, chckIntro, lblTotalNumLines);
        }

        // Globale variabler
        public class Variables
        {
            public static DataTable theTable { get; set; }
        }

        private void btnCheckActor_Click(object sender, RoutedEventArgs e)
        {
            if (chosenExcelFileDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Velg en serie din løk.");
            }
            else
            {
                string searchString = afterSearchButtonPressed();
                Calculations.calculateByOneEpisode(chosenExcelFileDataTable, searchString, spShowResult, chckIntro, lblTotalNumLines);

            }
        }


        public string afterSearchButtonPressed()
        {
            string searchString = txtActorName.Text.ToString().ToLower();
            spShowResult.Items.Clear();
            lblChosenDubber.Text = searchString.ToUpper();
            return searchString;
        }



        private void btnListFolder_Click(object sender, EventArgs e)
        {
            ExcelScanning.scanDubtoolFolder(lboxShowFiles, Excel03ConString, txtActorName.Text.ToString(), allProductions);
        }


        // Laster inn valgte fil/episode i DataTable
        private void findFileFromSelectionAndPrintOut(string selectedFile)
        {
            foreach (var episode in allProductions.productions)
            {
                if (selectedFile.Contains(episode.trimFilename(episode.excelFileName, Utils.dubToolDir)))
                {
                    dataGridView1.DataContext = episode.frontPageDataTable.DefaultView;
                    chosenExcelFileDataTable = episode.frontPageDataTable;
                    Calculations.calculateByOneEpisode(chosenExcelFileDataTable, txtActorName.Text.ToString(), spShowResult, chckIntro, lblTotalNumLines);

                }
            }
        }


        // Finner valgte fil i liste eller combobox
        private void chooseEpisode(string selectedFile)
        {
            //lblChosenEpisodeFrontpage.Text = "Du har valgt: " + selectedFile;
            spShowResult.Items.Clear();
            // Laster inn valgte fil inn i minnet og skriver ut
            findFileFromSelectionAndPrintOut(selectedFile);
        }



        private void btnRescanFolder_Click(object sender, EventArgs e)
        {
            allProductions = ExcelScanning.scanDubtoolFolder(lboxShowFiles, Excel03ConString, txtActorName.Text.ToString(), allProductions);
        }

        // Listbox changes
        private void lboxShowFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFile = lboxShowFiles.SelectedItem.ToString();
            chooseEpisode(selectedFile);
        }
    }
}
