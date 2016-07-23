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

        //private string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1;HDR=NO'";
        //private string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1;HDR=NO'";

        NordubbProductions allProductions = new NordubbProductions();
        DataTable chosenExcelFileDataTable = new DataTable();

        // Dato for å lage en uløpsdato. Finner dagens dato fra nettet
        DateTime expDate = new DateTime(2016, 9, 1);
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

            txtActorName.Focus();

            //string userDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString();

            //GlobalVariables.dubToolDir = "";

            //GlobalVariables.dubToolDir = userDocumentsPath + @"\dubtool\";

            allProductions = ExcelScanning.scanDubtoolFolder(lboxShowFiles, txtActorName.Text.ToString());


        }

        private void btnRescanFolder_Click(object sender, RoutedEventArgs e)
        {
            //NordubbProductions allProductions = new NordubbProductions();
            lboxShowFiles.Items.Clear();
            spShowResult.Items.Clear();
            allProductions = ExcelScanning.scanDubtoolFolder(lboxShowFiles, txtActorName.Text.ToString());

        }



        private void btnCheckAllEps_Click(object sender, RoutedEventArgs e)
        {
            string searchString = afterSearchButtonPressed();
            Calculations.calculateAllSeriesAndEpisodes(allProductions, searchString, spShowResult, chckIntro, lblTotalNumLines);
        }

        // Globale variabler
        public class GlobalVariables
        {
            public static DataTable theTable { get; set; }
            public static string searchType { get; set; }
            public static string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1;HDR=NO'";
            //public static string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1;HDR=NO'";
            public static string dubToolDir { get; set; }

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
                Calculations.calculateByOneEpisode(chosenExcelFileDataTable, searchString, spShowResult, chckIntro, comboLinesPrHour.SelectedValue.ToString(), lblTotalNumLines);

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
            ExcelScanning.scanDubtoolFolder(lboxShowFiles, txtActorName.Text.ToString());
        }


        // Laster inn valgte fil/episode i DataTable
        private void findFileFromSelectionAndPrintOut(string selectedFile)
        {
            foreach (var episode in allProductions.productions)
            {
                if (selectedFile.Contains(episode.trimFilename(episode.excelFileName, GlobalVariables.dubToolDir)))
                {
                    dataGridView1.DataContext = episode.frontPageDataTable.DefaultView;
                    chosenExcelFileDataTable = episode.frontPageDataTable;
                    Calculations.calculateByOneEpisode(chosenExcelFileDataTable, txtActorName.Text.ToString(), spShowResult, chckIntro, comboLinesPrHour.SelectedValue.ToString(), lblTotalNumLines);

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


        // Listbox changes
        private void lboxShowFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lboxShowFiles.Items.Count > 0)
            {
                string selectedFile = lboxShowFiles.SelectedItem.ToString();
                chooseEpisode(selectedFile);
            }

        }

        private void comboLinesPrHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainWindow.IsLoaded)
            {
                if (lboxShowFiles.Items.Count > 0)
                {
                    string selectedFile = lboxShowFiles.SelectedItem.ToString();
                    chooseEpisode(selectedFile);
                    spShowResult.Items.Clear();
                    Calculations.calculateByOneEpisode(chosenExcelFileDataTable, txtActorName.Text.ToString(), spShowResult, chckIntro, comboLinesPrHour.SelectedValue.ToString(), lblTotalNumLines);
                }
            }


        }

        private void radioSearchActor_Checked(object sender, RoutedEventArgs e)
        {

            checkSearchType();
        }

        private void radioSearchRole_Checked(object sender, RoutedEventArgs e)
        {
            checkSearchType();
        }

        private void checkSearchType()
        {


            if (radioSearchActor.IsChecked.Value)
            {
                GlobalVariables.searchType = "actor";
            }
            else
            {
                GlobalVariables.searchType = "role";
            }



        }

        private void SetFocusToSearchBox(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.S && !txtActorName.IsFocused)
            //{


            //}

            //if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
            //{

            //    MessageBox.Show("trykket");
            //}

            if (e.Key == Key.F4)
            {

                txtActorName.Focus();
                txtActorName.Clear();
            }
        }


        private void dubtoolFolder_Checked(object sender, RoutedEventArgs e)
        {
            if (radioLocalFolder.IsChecked.Value)
            {
                GlobalVariables.dubToolDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + @"\dubtool\";
                //MessageBox.Show(GlobalVariables.dubToolDir);
            }
            else if (serverFolder.IsChecked.Value)
            {
                GlobalVariables.dubToolDir = @"N:\MANUS\ -Dubtool- \";
                //MessageBox.Show(GlobalVariables.dubToolDir);
            }
        }
    }
}
