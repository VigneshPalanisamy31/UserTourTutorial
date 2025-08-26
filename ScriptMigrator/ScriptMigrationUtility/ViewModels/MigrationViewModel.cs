using ScriptMigrationUtility.Models;
using ScriptMigrationUtility.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Serialization;

namespace ScriptMigrationUtility.ViewModels
{
    public class MigrationViewModel : BaseViewModel
    {
        private string _selectedDirectory;
        public string SelectedDirectory
        {
            get => _selectedDirectory;
            set { _selectedDirectory = value; OnPropertyChanged(); }
        }
        public ObservableCollection<StepModel> Steps { get; set; } = new();
        public ICommand BrowseCommand { get; }
        public ICommand ScanCommand { get; }
        public ICommand ConvertCommand { get; }

        public MigrationViewModel()
        {
            BrowseCommand = new RelayCommand(BrowseFolder);
            ScanCommand = new RelayCommand(ScanDirectory);
            ConvertCommand = new RelayCommand(ConvertSteps, () => Steps.Any());
        }

        private void BrowseFolder()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SelectedDirectory = dialog.SelectedPath;
            }
        }
        private void ScanDirectory()
        {
            Steps.Clear();
            if (!Directory.Exists(SelectedDirectory)) return;

            foreach (var file in Directory.GetFiles(SelectedDirectory, "*.xml"))
            {
                var stepList = XmlHelper.LoadStepsFromXml(file);
                foreach (var step in stepList)
                {
                    step.SourceFile = Path.GetFileName(file); 
                    Steps.Add(step);
                }
            }
        }

        private void ConvertSteps()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedDirectory) || !Directory.Exists(SelectedDirectory))
                {
                    System.Windows.MessageBox.Show("Please select a valid directory first.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string migratedDir = Path.Combine(SelectedDirectory, "Migrated");
                Directory.CreateDirectory(migratedDir);
                var groupedByFile = Steps.GroupBy(s => s.SourceFile);
                foreach (var group in groupedByFile)
                {
                    try
                    {
                        var finalSteps = new List<BaseStep>();
                        foreach (var step in group)
                        {
                            if (step.IsSelected)
                            {
                                finalSteps.Add(new NewStepModel(step)); 
                            }
                            else
                            {
                                finalSteps.Add(step); 
                            }
                        }
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(group.Key);
                        string extension = Path.GetExtension(group.Key);
                        string outputFile = Path.Combine(migratedDir, fileNameWithoutExt + "_migrated" + extension);
                        var serializer = new XmlSerializer(typeof(List<BaseStep>),
                            new Type[] { typeof(StepModel), typeof(NewStepModel) });

                        using (var writer = new StreamWriter(outputFile))
                        {
                            serializer.Serialize(writer, finalSteps);
                        }
                    }
                    catch (Exception innerEx)
                    {
                        System.Windows.MessageBox.Show($"Migration failed for {Path.GetFileName(group.Key)}: {innerEx.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                System.Windows.MessageBox.Show($"Migration completed successfully!\nMigrated files saved in:\n{migratedDir}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Migration failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
