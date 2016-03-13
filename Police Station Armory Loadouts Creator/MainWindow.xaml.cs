namespace Police_Station_Armory_Loadouts_Creator
{
    // System
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    // Microsoft
    using Microsoft.Win32;

    // this
    using Police_Station_Armory_Loadouts_Creator.Types;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Loadout CurrentLoadout = null;

        public MainWindow()
        {
            InitializeComponent();
            loadoutNameDescLbl.Content = @"The name displayed in the loadouts menu.";
            loadoutTextureNameDescLbl.Content = @"The file name of the image that will be displayed as the
icon in the loadouts menu.
The format of file has to 
be a PNG File(.png).";
            Logger.LogTrivial<MainWindow>("Initialized MainWindow");
        }

        private void loadLoadoutBttn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();

                // Set filter for file extension and default file extension 
                fileDialog.DefaultExt = ".xml";
                fileDialog.Filter = "XML Files (*.xml)|*.xml";

                // Display OpenFileDialog by calling ShowDialog method 
                bool? result = fileDialog.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    string filename = System.IO.Path.GetFullPath(fileDialog.FileName);
                    CurrentLoadout = Loadout.GetFromXML(filename);
                    loadoutNameTxtBox.Text = CurrentLoadout.Name;
                    loadoutTextureNameTxtBox.Text = CurrentLoadout.TextureFileName;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException<MainWindow>("Unexpected exception trying to load a Loadout file", ex);
                ResetValues();
                MessageBox.Show("Error loading the XML file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetValues()
        {
            CurrentLoadout = null;
            loadoutNameTxtBox.Text = "<insert your loadout name here>";
            loadoutTextureNameTxtBox.Text = "<insert your texture name here>";
            loadoutNameDescLbl.Content = @"The name displayed in the loadouts menu.";
            loadoutTextureNameDescLbl.Content = @"The file name of the image that will be displayed as the
icon in the loadouts menu.
The format of file has to 
be a PNG File(.png).";
        }
    }
}
