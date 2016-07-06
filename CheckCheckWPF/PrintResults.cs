using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CheckCheckWPF
{
    public class PrintResult
    {


        public static int printResultByEpisode(List<Episode> episodeList, ListBox resultPanel, CheckBox checkBoxNoIntro)
        {
            int totNumLines = 0;
            int counter = 0;

            foreach (var item in episodeList)
            {
                if (item.roleNames.Count > 0)
                {
                    int totNumLinesPrEpisode = 0;

                    var container = new StackPanel { Margin = new Thickness(5, 5, 0, 5) };
                    var mainText = new TextBlock { FontSize = 15 };

                    var textBlockHeading = new TextBlock { FontSize = 18, FontWeight = FontWeights.Bold, Foreground = Brushes.WhiteSmoke, Background = Brushes.DarkBlue, Height = 24 };

                    var textBlockEpNumber = new TextBlock { FontSize = 16, FontWeight = FontWeights.Normal, Foreground = Brushes.WhiteSmoke, Background = Brushes.Green, Height = 22 };

                    var removeButton = new Button { Content = "Ferdig", Width = 80, Height = 20, HorizontalAlignment = HorizontalAlignment.Left };

                    removeButton.Click += btn_Click;

                    TimeSpan span = new TimeSpan();
                    string warningText = "";
                    string daysText = " dager.";
                    string deliveryText = "";

                    if (item.deliveryDate.ToString() != "")
                    {
                        try
                        {
                            DateTime deliveryDate = Convert.ToDateTime(item.deliveryDate.ToString());
                            span = deliveryDate.Subtract(DateTime.Now);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Åh heisann! Virker som det er et Excelark som ikke er i orden. Kan det være " + item.seriesName.ToString() + ", ep " + item.episodeNumber.ToString() + " ?");
                            break;
                        }

                        if (span.Days < 5)
                        {
                            warningText = " På tide å KLIPPE!!";
                        }
                        deliveryText = ". Skal leveres om " + span.Days.ToString() + daysText + warningText;
                    }




                    if (span.Days == 1)
                    {
                        daysText = " dag.";
                    }

                    // TextBlockHeading
                    textBlockHeading.Text = string.Format("{0} - Episode{1} {2}", item.seriesName.ToUpper(), item.episodeNumber, deliveryText);

                    // TextBlockEpNumber
                    textBlockEpNumber.Text = "Ep: " + item.episodeNumber.ToString() + "\n";


                    string resultTemp = "";

                    resultTemp += Environment.NewLine;

                    for (int role = 0; role < item.roleNames.Count; role += 1)
                    {
                        if (!(checkBoxNoIntro.IsChecked.Value && (item.roleNames[role].roleName.ToString().ToLower() == "intro" || item.roleNames[role].roleName.ToString().ToLower() == "outro" || item.roleNames[role].roleName.ToString().ToLower() == "plakat")))
                        {
                            resultTemp += item.roleNames[role].roleName.ToString() + ": " + item.roleNames[role].numOfLines.ToString() + ", ";

                            // Regner ut antall replikker
                            totNumLinesPrEpisode += Convert.ToInt32(item.roleNames[role].numOfLines);
                            totNumLines += totNumLinesPrEpisode;
                        }
                    }
                    // Tar bort komma:
                    string result = resultTemp.Substring(0, resultTemp.Length - 2);
                    result += " || Totalt " + totNumLinesPrEpisode.ToString() + " rep.";

                    if (totNumLinesPrEpisode > 0)
                    {
                        // Sjekker om få replikker
                        if (totNumLinesPrEpisode < 3)
                        {
                            result += " ..kanskje vi har en pickup her?";
                        }

                        //result += Environment.NewLine;
                        result += "\r\n\r\n";


                        // Slår sammen hele WrapPanelet med TextBlock'ene:
                        mainText.Text = result;
                        container.Children.Add(textBlockHeading);
                        container.Children.Add(textBlockEpNumber);
                        container.Children.Add(mainText);
                        container.Children.Add(removeButton);

                        resultPanel.Items.Add(container);

                    }

                }
                counter++;
            }
            return totNumLines;
        }

        public static void btn_Click(object sender, EventArgs e)
        {
            var objButton = (Button)sender;
            StackPanel objWrap = (StackPanel)objButton.Parent;
            //StackPanel objStackPanel = (StackPanel)objWrap.Parent;
            var objContainer = (ListBox)objWrap.Parent;

            //MessageBox.Show(objWrap.Name.ToString() + " ble fjernet.");

            //objStackPanel.Children.Remove(objWrap);
            objContainer.Items.Remove(objWrap);


        }



        //public static void CalculateAllEpisodes(NordubbProductions productions, FlowLayoutPanel resultPanel)
        //{
        //    foreach (var item in productions.productions)
        //    {
        //        //printResultByEpisode(item.frontPageDataTable, panel);
        //    }
        //}

    }
}
