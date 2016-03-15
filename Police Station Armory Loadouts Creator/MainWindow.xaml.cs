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

        public List<ItemControl> items = new List<ItemControl>();
        public ItemControl currentItem = null;

        public MainWindow()
        {
            InitializeComponent();
            loadoutNameDescLbl.Content = @"The name displayed in the loadouts menu.";
            loadoutTextureNameDescLbl.Content = @"The file name of the image that will be displayed as the
icon in the loadouts menu.
The format of file has to 
be a PNG File(.png).";
            loadoutItemsDescLbl.Content = @"Here you will choose which 
items the loadout will have.
Select one of the avalaible 
items in the left list, 
then to add it check 
the 'Add item' checkbox, 
and finally in the right list 
choose the components
(scopes, bigger 
magazines, suppresors...)
the selected item will have.";

            itemComponentsListView.IsEnabled = false;
            foreach (WeaponComponent comp in Enum.GetValues(typeof(WeaponComponent)))
            {
                CheckBox checkbox = new CheckBox() { Margin = new Thickness(5, 5, 0, 0), Content = comp.ToString(), IsChecked = false, IsEnabled = false };
                checkbox.Checked += ComponentCheckBox_CheckChanged;
                checkbox.Unchecked += ComponentCheckBox_CheckChanged;
                itemComponentsListView.Items.Add(checkbox);
            }

            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                ItemControl i = new ItemControl(type);
                items.Add(i);
                
                itemsListBox.Items.Add(i.Type);
            }
            itemsListBox.SelectionChanged += ItemsListBox_SelectionChanged;
            addItemCheckBox.Checked += AddItemCheckBox_CheckChanged;
            addItemCheckBox.Unchecked += AddItemCheckBox_CheckChanged;

            Logger.LogTrivial<MainWindow>("Initialized MainWindow");
        }


        private void ComponentCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            CheckBox senderCheckBox = ((CheckBox)sender);
            WeaponComponent checkboxComponent = (WeaponComponent)Enum.Parse(typeof(WeaponComponent), senderCheckBox.Content.ToString());
            currentItem.SetComponentActive(checkboxComponent, senderCheckBox.IsChecked.HasValue ? senderCheckBox.IsChecked.Value : false);
        }


        private void AddItemCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (addItemCheckBox.IsChecked.HasValue && addItemCheckBox.IsChecked.Value == true)
            {
                itemComponentsListView.IsEnabled = true;
                foreach (UIElement item in itemComponentsListView.Items)
                {
                    item.IsEnabled = true;
                }
            }
            else if (!addItemCheckBox.IsChecked.HasValue || addItemCheckBox.IsChecked.Value == false)
            {
                itemComponentsListView.IsEnabled = false;
                foreach (UIElement item in itemComponentsListView.Items)
                {
                    item.IsEnabled = false;
                }
            }
            currentItem.IsAdded = addItemCheckBox.IsChecked.HasValue ? addItemCheckBox.IsChecked.Value : false;
        }
        // TODO: add some visual(a tick...) to easily see if an Item is added
        private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentItem = items.FirstOrDefault(i => i.Type.ToString() == itemsListBox.SelectedItem.ToString());
            if (currentItem == null)
                return;
            addItemCheckBox.IsEnabled = true;
            addItemCheckBox.IsChecked = currentItem.IsAdded;
            foreach (CheckBox checkB in itemComponentsListView.Items)
            {
                checkB.IsChecked = currentItem.IsComponentActive((WeaponComponent)Enum.Parse(typeof(WeaponComponent), checkB.Content.ToString()));
            }
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

                    foreach (Item item in CurrentLoadout.Items)
                    {
                        ItemControl itemControl = new ItemControl(item.Type);
                        for (int i = 0; i < itemControl.ActiveComponents.Length; i++)
                        {
                            itemControl.ActiveComponents[i] = item.Components.Contains(itemControl.Components[i]);
                        }
                        itemControl.IsAdded = true;
                        items[items.FindIndex(i => i.Type == item.Type)] = itemControl;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException<MainWindow>("Unexpected exception trying to load a Loadout file", ex);
                ResetValues();
                MessageBox.Show("Error loading the Loadout XML file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveLoadoutBttn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();

                // Set filter for file extension and default file extension 
                saveDialog.DefaultExt = ".xml";
                saveDialog.Filter = "XML Files (*.xml)|*.xml";

                bool? result = saveDialog.ShowDialog();

                if(result == true)
                {
                    string filename = System.IO.Path.GetFullPath(saveDialog.FileName);
                    Loadout loadout = new Loadout()
                    {
                        Name = loadoutNameTxtBox.Text,
                        TextureFileName = loadoutTextureNameTxtBox.Text,
                    };
                    List<Item> loadoutItems = new List<Item>();
                    foreach (ItemControl item in items)
                    {
                        if (item.IsAdded)
                            loadoutItems.Add(item.ToItem());
                    }
                    loadout.Items = loadoutItems;
                    Loadout.WriteToXML(filename, loadout);
                    CurrentLoadout = loadout;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException<MainWindow>("Unexpected exception trying to save a Loadout in a file", ex);
                ResetValues();
                MessageBox.Show("Error saving the Loadout XML file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            loadoutItemsDescLbl.Content = @"Here you will choose which 
items the loadout will have.
Select one of the avalaible 
items in the left list, 
then to add it check 
the 'Add item' checkbox, 
and finally in the right list 
choose the components
(scopes, bigger 
magazines, suppresors...)
the selected item will have.";
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
    }




    public class ItemControl
    {
        public ItemType Type;
        public bool IsAdded = false;

        public WeaponComponent[] Components;
        public bool[] ActiveComponents;

        public ItemControl(ItemType type)
        {
            Type = type;
            int count = Common.GetEnumCount<WeaponComponent>();
            Components = new WeaponComponent[count];
            int index = 0;
            foreach (WeaponComponent w in Enum.GetValues(typeof(WeaponComponent)))
            {
                Components[index] = w;
                index++;
            }
            ActiveComponents = new bool[count];
            for (int i = 0; i < index; i++)
            {
                ActiveComponents[i] = false;
            }
        }

        public int IndexOfComponent(WeaponComponent component)
        {
            return Array.IndexOf(Components, component);
        }

        public bool IsComponentActive(WeaponComponent component)
        {
            return ActiveComponents[IndexOfComponent(component)];
        }

        public void SetComponentActive(WeaponComponent component, bool active)
        {
            ActiveComponents[IndexOfComponent(component)] = active;
        }

        public Item ToItem()
        {
            return new Item() { Type = this.Type, Components = this.Components.Where(c => IsComponentActive(c)).ToArray(),};
        }
    }
}
