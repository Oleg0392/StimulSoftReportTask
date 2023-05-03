using System;
using System.Windows;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;
using System.IO;
using Microsoft.Win32;

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonSaveAsJson_Click(object sender, RoutedEventArgs e)
        {
            if (StiWpfViewerControl1.Report == null)
            {
                MessageBox.Show("The report was not loaded!","Error. Report does not exists",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files |*.json";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                string JsonReport = StiWpfViewerControl1.Report.SaveDocumentJsonToString();
                using (var writer = new StreamWriter(File.OpenWrite(saveFileDialog.FileName)))
                {
                    try { await writer.WriteAsync(JsonReport); }
                    catch(Exception exception) { MessageBox.Show(exception.Message,"File writing failed.", MessageBoxButton.OK, MessageBoxImage.Error); }
                }
            }
        }

        private void ButtonLoadJsonReport_Click(object sender, RoutedEventArgs e)
        {
            var report = new StiReport();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files |*.json";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                string JsonReport = string.Empty;
                using (var reader = new StreamReader(File.OpenRead(openFileDialog.FileName)))
                {
                    try { JsonReport = reader.ReadToEnd(); }
                    catch(Exception exception) { MessageBox.Show(exception.Message, "File reading failed.", MessageBoxButton.OK, MessageBoxImage.Error); return; }
                }
                report.LoadDocumentFromJson(JsonReport);
                StiWpfViewerControl1.Report = report;
            }            
        }

        private async void ButtonExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            if (StiWpfViewerControl1.Report == null)
            {
                MessageBox.Show("The report was not loaded!", "Error. Report does not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            var report = new StiReport();
            foreach (StiPage page in StiWpfViewerControl1.Report.RenderedPages)
            {
                report.RenderedPages.Add(page);
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files |*.pdf";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                try { await report.ExportDocumentAsync(StiExportFormat.Pdf, File.OpenWrite(saveFileDialog.FileName)); }
                catch(Exception exception) { MessageBox.Show(exception.Message, "Report exporting failed.", MessageBoxButton.OK, MessageBoxImage.Error); }
            }                
            
        }

        #region unuse code
        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            var report = new StiReport();
            report.Load(@"Reports\SimpleList.mrt");
            report.Render();
            StiWpfViewerControl1.Report = report;
        }

        private void ButtonDialog_Click(object sender, RoutedEventArgs e)
        {
            var report = new StiReport();
            report.Load(@"Reports\SimpleList.mrt");
            report.ShowWithWpf();
        }
        #endregion
    }
}
