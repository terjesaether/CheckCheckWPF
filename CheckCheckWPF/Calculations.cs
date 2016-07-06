using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CheckCheckWPF
{
    public class Calculations
    {
        // En Episode-class består av Episodenummer og Role med antall linjer
        public static List<Episode> calculateSearchResultsByEpisode(DataTable dt, string searchString)
        {
            string currEpsLines = "";
            string linesTotal;
            string linesDone;
            string[] linesArray;

            // Dette er en liste med Episoder, Episoder med en liste over rollene i den episoden
            var episodeList = new List<Episode>();


            // Går gjennom alle kolonnene: [rad][kolonne]
            for (int epColumn = 4; epColumn <= 16; epColumn++)
            {
                int rowCompensation = 0;

                Episode episode = new Episode(); // Oppretter et ny episodeobjekt

                episode.seriesName = dt.Rows[0][0].ToString();

                // Sjekker om manuset er i orden: sjekker hvor tittelen står:
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(episode.seriesName))
                    {
                        rowCompensation++;
                        episode.seriesName = dt.Rows[rowCompensation][0].ToString();
                        //break;                    
                    }
                    else
                    {
                        break;
                    }
                }
                episode.episodeNumber = dt.Rows[2 + rowCompensation][epColumn].ToString(); // Legger til epnr

                episode.deliveryDate = dt.Rows[4 + rowCompensation][epColumn].ToString();
                //episode.roleNames = new List<RoleNameAndNumOfLines>(); // Liste med rollenavn
                episodeList.Add(episode);
                string currentRoleName;
                string currentRoleLineNumbersString;

                // Nedover
                for (int row = (5 + rowCompensation); row < dt.Rows.Count; row++)
                {
                    // Hvis kolonnen ikke er tom..
                    if (dt.Rows[row][epColumn] != null)
                    {
                        // Hvis skuespillercellen inneholder søkestrengen..
                        if (dt.Rows[row][3].ToString().ToLower().Contains(searchString))
                        {
                            // Deler opp replikk-cellen
                            currEpsLines = dt.Rows[row][epColumn].ToString().ToLower();
                            linesArray = currEpsLines.Split(new char[] { '/' }, StringSplitOptions.None);

                            if (linesArray.Length == 2)
                            {
                                linesDone = linesArray[0];
                                linesTotal = linesArray[1];

                                // Sjekker om det er mer enn 0 replikker igjen:
                                if (Convert.ToInt32(linesTotal) - Convert.ToInt32(linesDone) > 0)
                                {
                                    // Finner episodenummer i kolonne:
                                    currentRoleName = dt.Rows[row][1].ToString().ToUpper();
                                    currentRoleLineNumbersString = (Convert.ToInt32(linesTotal) - Convert.ToInt32(linesDone)).ToString();

                                    var r = new RoleNameAndNumOfLines
                                    {
                                        roleName = currentRoleName,
                                        numOfLines = currentRoleLineNumbersString,
                                        totalNumOfLines = linesTotal
                                    };

                                    // Legger til rollenavnet i episoden:
                                    episode.roleNames.Add(r);

                                }
                            }
                        }
                    }
                }
            }

            return episodeList;
        }

        // Regner ut valgt serie
        public static void calculateByOneEpisode(DataTable dt, string searchString, ListBox flowLayoutPanel1, CheckBox chckIntro, TextBlock lblTotalNumLines)
        {
            List<Episode> episodeList = Calculations.calculateSearchResultsByEpisode(dt, searchString);

            int totNumLines = PrintResult.printResultByEpisode(episodeList, flowLayoutPanel1, chckIntro);
            decimal t = decimal.Round((Convert.ToDecimal(totNumLines) / 90), 2);
            lblTotalNumLines.Text = "TOTALT antall replikker: " + Convert.ToString(totNumLines) + ". Det vil ta " + t + " timer å dubbe ferdig.";

        }

        // Regner ut alle seriene 
        public static void calculateAllSeriesAndEpisodes(NordubbProductions productions, string searchString, ListBox flowLayoutPanel1, CheckBox chckIntro, TextBlock lblTotalNumLines)
        {
            int totNumLines = 0;
            foreach (var item in productions.productions)
            {
                //string seriesName = "";

                List<Episode> episodeList = Calculations.calculateSearchResultsByEpisode(item.frontPageDataTable, searchString);

                if (episodeList.Count > 0)
                {
                    //seriesName = item.seriesName.ToString();
                    totNumLines = totNumLines + PrintResult.printResultByEpisode(episodeList, flowLayoutPanel1, chckIntro);
                }

                // Må gi Utskriften en mulighet til å skrive ut navn på eps

            };

            decimal t = decimal.Round((Convert.ToDecimal(totNumLines) / 90), 2);
            lblTotalNumLines.Text = "TOTALT antall replikker: " + totNumLines.ToString() + ".  Det vil ta " + t + " timer å dubbe ferdig.";
        }


    }
}
