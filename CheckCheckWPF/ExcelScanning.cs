﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CheckCheckWPF
{
    public class ExcelScanning
    {
        // Scanner folder og henter inn alle manusene i minnet og fyller opp listboks og comboboks
        public static NordubbProductions scanDubtoolFolder(ListBox lboxShowFiles, string Excel03ConString, string searchString, NordubbProductions allProductions)
        {
            string conStr;
            conStr = string.Empty;
            string sheetName = "";
            

            List<string> dtc = new List<string>();
            dtc = getDubtoolFolderContent();

            // Fyller opp listboksen og comboboksen:       
            Utils.listFilesFromMemoryList(dtc, lboxShowFiles);

            int counter = 0;
            foreach (var excelFile in dtc) // Fylle opp en liste med DataTables
            {
                conStr = string.Format(Excel03ConString, excelFile);

                //Get the name of the First Sheet.
                using (OleDbConnection con = new OleDbConnection(conStr))
                {
                    using (OleDbCommand cmd = new OleDbCommand())
                    {
                        try
                        {
                            cmd.Connection = con;
                            con.Open();
                            DataTable dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                            // Ruller gjennom alle arkene for å finne Forsiden eller #00
                            for (var sheets = 0; sheets < dtExcelSchema.Rows.Count; sheets++)
                            {
                                //sheetName = dtExcelSchema.Rows[sheets]["TABLE_NAME"].ToString();
                                if (dtExcelSchema.Rows[sheets]["TABLE_NAME"].ToString().Contains("Forside") || dtExcelSchema.Rows[sheets]["TABLE_NAME"].ToString().Contains("#00"))
                                {
                                    sheetName = dtExcelSchema.Rows[sheets]["TABLE_NAME"].ToString();
                                    break;
                                }
                            }

                            con.Close();
                        }
                        catch (OleDbException e)
                        {
                            MessageBox.Show("Får ikke åpnet en av Excel-filene. Virker som den er åpen et annet sted...", e.Message);
                            break;
                        }
                    }
                }

                //Read Data from the First Sheet.
                using (OleDbConnection con = new OleDbConnection(conStr))
                {
                    using (OleDbCommand cmd = new OleDbCommand())
                    {
                        using (OleDbDataAdapter oda = new OleDbDataAdapter())
                        {

                            DataTable dt = new DataTable();
                            //sheetName = "Forside";
                            cmd.CommandText = "SELECT * From [" + sheetName + "]";
                            cmd.Connection = con;
                            con.Open();
                            oda.SelectCommand = cmd;
                            oda.Fill(dt);

                            var excelFrontPage = new excelFrontPage();
                            excelFrontPage.frontPageDataTable = dt;
                            excelFrontPage.seriesName = dt.Rows[0][0].ToString();
                            excelFrontPage.excelFileName = dtc[counter];
                            counter++;

                            for (int i = 4; i < 16; i++)
                            {
                                excelFrontPage.numEpisodesList.Add(dt.Rows[2][i].ToString());
                            }
                            allProductions.productions.Add(excelFrontPage);
                            con.Close();

                        }
                    }
                }
            }
            return allProductions;
        }

        // Laster inn dir-innhold i en liste i minnet
        public static List<string> getDubtoolFolderContent()
        {
            List<string> dtc = new List<string>();
            DirectoryInfo dinfo = new DirectoryInfo(Utils.dubToolDir);

            if (dinfo.Exists)
            {
                FileInfo[] Files = dinfo.GetFiles("*.xls");

                foreach (FileInfo file in Files)
                {
                    if (file.Extension == ".xls")
                    {
                        dtc.Add(dinfo.ToString() + file.Name.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Ser ut som det er noen uvedkommende filer i mappa...", "Fare!");
                    }

                }
            }
            else
            {
                MessageBox.Show("Kan ikke finne Dubtool-mappe...", "Fare!");
            }
            return dtc;
        }
    }
}
