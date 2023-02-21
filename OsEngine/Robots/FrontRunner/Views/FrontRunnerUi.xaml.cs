using OsEngine.OsTrader.Gui;
using OsEngine.Robots.FrontRunner.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using OsEngine.Robots.FrontRunner.Models;


namespace OsEngine.Robots.FrontRunner.Views
{
    /// <summary>
    /// Логика взаимодействия для FrontRunnerUi.xaml
    /// </summary>
    public partial class FrontRunnerUi : Window
    {
        protected ViewModels.VM vm;

        public FrontRunnerUi(Models.FrontRunner frontRunner)
        {
            InitializeComponent();
            vm = new ViewModels.VM(frontRunner);
            DataContext = vm;
        }
    }
}
