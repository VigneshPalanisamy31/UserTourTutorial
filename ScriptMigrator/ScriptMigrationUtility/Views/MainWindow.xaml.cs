using ScriptMigrationUtility.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ScriptMigrationUtility.ViewModels;

namespace ScriptMigrationUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MigrationViewModel();
        }
    }
}
