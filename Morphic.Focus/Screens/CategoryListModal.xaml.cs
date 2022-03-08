using Morphic.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Morphic.Focus.Screens
{
    /// <summary>
    /// Interaction logic for CategoryList.xaml
    /// </summary>
    public partial class CategoryListModal : Window
    {
        private string _categoryName = string.Empty;

        AppEngine _engine;
        public AppEngine Engine { get { return _engine; } }
        public CategoryListModal(string category)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _engine = AppEngine.Instance;
            }

            InitializeComponent();

            CategoryName = category;

            this.DataContext = this;
        }

        public string CategoryName { get => _categoryName; set => _categoryName = value; }

        public Category SelectedCategory => Engine.CategoryCollection.Categories.Where(p => p.Name == CategoryName).First();

        /// <summary>
        /// Allow user to drag the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    this.DragMove();
            //}
        }
    }
}
