using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using BioLis_30i.DTOs;

namespace BioLis_30i.Services
{
    public class CsvExportService
    {
        public void ExportResultsToCsv(List<GenericResult> results)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.FileName = $"BioLis30i_Results_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            // Write header
                            writer.WriteLine("Message ID,Test Code,Test Name,Result,Units,Reference Range,Observation Date/Time,Status");

                            // Write data
                            foreach (var result in results)
                            {
                                writer.WriteLine($"{result.MessageId}," +
                                              $"{result.TestCode}," +
                                              $"{result.TestName}," +
                                              $"{result.Result}," +
                                              $"{result.Units}," +
                                              $"{result.ReferenceRange}," +
                                              $"{result.ObservationDateTime:yyyy-MM-dd HH:mm:ss}," +
                                              $"{result.ResultStatus}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting to CSV: {ex.Message}", ex);
            }
        }
    }
} 