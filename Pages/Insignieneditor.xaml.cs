using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Pdoxcl2Sharp;
using HOI4Tool.Properties;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter

namespace HOI4Tool
{
    /// <summary>
    /// Interaktionslogik für Insignieneditor.xaml
    /// </summary>
    public partial class Insignieneditor : Page
    {
        private Icon _currentSelectedIcon;
        private List<Icon> _tempIconList = new List<Icon>();
        public ArmyIconsTxt armeeIcons;
        public SpriteTypes theatreSelectorGFX;

        public Insignieneditor()
        {
            InitializeComponent();
            stackPanelButtons.DataContext = this;

            try
            {
                if(Settings.Default.IsSetupOk)
                {
                    if(Settings.Default.IsBackupOk)
                    {
                        // Erst einmal Army-Icon-Datei von Paradox einlesen
                        string completeConfigPath = Settings.Default.PathArmyIcons + Settings.Default.FileArmyIconConfig;
                        string completeGraphicsPath = Settings.Default.PathArmyIcons + Settings.Default.FileArmyIconGraphics;

                        if (File.Exists(completeConfigPath))
                        {
                            if (File.Exists(completeGraphicsPath))
                            {
                                // *******************************************************************
                                // ********** Paradox Konfigurationsdatei komplett einlesen ********** 
                                // *******************************************************************
                                using (FileStream fs = new FileStream(completeConfigPath, FileMode.Open))
                                {
                                    armeeIcons = Pdoxcl2Sharp.ParadoxParser.Parse(fs, new ArmyIconsTxt());
                                }

                                // *********************************************************
                                // ********** Paradox GFX-Datei komplett einlesen ********** 
                                // *********************************************************
                                using (FileStream fs = new FileStream(Settings.Default.PathInterface + Settings.Default.FileTheatreSelector, FileMode.Open))
                                {
                                    theatreSelectorGFX = Pdoxcl2Sharp.ParadoxParser.Parse(fs, new SpriteTypes());
                                }

                                // ***********************************************************************************************
                                // ********** In die Konfigurationsstruktur werden nun die zugehörigen Grafiken geladen ********** 
                                // ***********************************************************************************************
                                foreach (ParadoxType paradoxType in armeeIcons.ParadoxTypes)
                                {
                                    if (!string.IsNullOrEmpty(paradoxType.Grafikdateiname))
                                    {
                                        if (paradoxType.Icons.Count > 0)
                                        {
                                            // zugehörige Grafikdatei laden
                                            DDSFile ddsDatei = new DDSFile(Settings.Default.PathArmyIcons, paradoxType.Grafikdateiname);
                                            if (ddsDatei.Status == "OK")
                                            {
                                                if (ddsDatei.HeightInPixel == Settings.Default.InsigniaHeight)
                                                {
                                                    int x2 = Settings.Default.InsigniaWidth + Settings.Default.InsigniaGap;
                                                    if (x2 > 0)
                                                    {
                                                        if ((ddsDatei.WidthInPixel / x2) == paradoxType.Icons.Count)
                                                        {
                                                            int x = 0;
                                                            foreach (Icon icon in paradoxType.Icons)
                                                            {
                                                                icon.Bmp = ddsDatei.GetCustomBitmap(x);
                                                                icon.BmpSource = ddsDatei.Bitmap2BitmapImage(icon.Bmp);
                                                                x += Settings.Default.InsigniaWidth + Settings.Default.InsigniaGap;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Die Anzahl der Icons in der Grafikdatei " + completeGraphicsPath + " passt nicht mit der Anzahl Icons in " + completeConfigPath + " zusammen.");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Division durch Null -.- ! Bitte eine Korrekte Breite bzw. Zwischenraum in den Einstellungen konfigurieren!");
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Die Grafikdatei " + completeGraphicsPath + " passt nicht mit den Einstellungen zusammen. (Der X-Wert der Grafik passt nicht.)");
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Die Grafikdatei " + completeGraphicsPath + " konnte nicht geladen werden.");
                                            }
                                        }
                                        else
                                        {
                                            //lblMeldung.Visibility = Visibility.Visible;
                                            //lblMeldung.Content = "Es wurden keine Daten für Armeen in der Konfigdatei gefunden!";
                                        }
                                    }
                                    else
                                    {
                                        //lblMeldung.Visibility = Visibility.Visible;
                                        //lblMeldung.Content = "Keine Grafikdatei für " + paradoxType.ParadoxCategory.ToString() + " vorhanden.";
                                    }
                                }
                            }
                            else
                            {
#warning Hier vielleicht die EInstellungen automatisch aufrufen.
                                MessageBox.Show($@"{completeGraphicsPath} wurde nicht gefunden. Bitte die Einstellungen überprüfen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
#warning Hier vielleicht die EInstellungen automatisch aufrufen.
                            MessageBox.Show($@"{completeConfigPath} wurde nicht gefunden. Bitte die Einstellungen überprüfen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bevor diese Funktion verwendet werden kann, muss ein Backup (unter Einstellungen) gemacht werden.", "Backup noch nicht vorhanden.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Bevor diese Funktion verwendet werden kann, muss das Setup konfiguriert und gespeichert werden.", "Setup noch nicht geprüft.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Fehler :-(", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public int NoCopiedIcons
        {
            get
            {
                return this._tempIconList.Count;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(armeeIcons != null)
            {
                ParadoxCategory typ = (ParadoxCategory)comboBoxTyp.SelectedIndex;

#warning Hier noch ne Sicherung einbauen, falls es aus irgendeinem Grund mehere Gruppen gibt.
                foreach (ParadoxType ptyp in armeeIcons.ParadoxTypes)
                {
                    if(ptyp.ParadoxCategory == typ)
                    {
                        dataGridInsignien.ItemsSource = null;
                        dataGridInsignien.ItemsSource = ptyp.IconGrid;
                    }
                }
            }
        }

        private void cmdSpeichern_Click(object sender, RoutedEventArgs e)
        {
            // ********** Konfigdatei neu schreiben **********
            using (FileStream fs = new FileStream(Settings.Default.PathArmyIcons + Settings.Default.FileArmyIconConfig, FileMode.Create))

            using (ParadoxSaver saver = new ParadoxSaver(fs))
            {
                armeeIcons.Write(saver);
            }

            // ********** Grafikdateien neu schreiben **********
            List<Bitmap> iconListe = new List<Bitmap>();
            List<Bitmap> iconListeSelected = new List<Bitmap>();
            DDSFile ddsFile = null;
            Bitmap tmpBmp = null;
            foreach (ParadoxType typ in armeeIcons.ParadoxTypes)
            {
                iconListe.Clear();
                iconListeSelected.Clear();
                foreach (Icon icon in typ.Icons)
                {
                    if(icon.Bmp != null)
                    {
                        iconListe.Add(icon.Bmp);

                        tmpBmp = new Bitmap(icon.Bmp, new System.Drawing.Size(icon.Bmp.Width + 2, icon.Bmp.Height + 2));
                        iconListeSelected.Add(tmpBmp);
                    }
                }

                switch (typ.ParadoxCategory)
                {
                    case ParadoxCategory.Army:
                        // Neue DDS-Datei erzeugen. Es werden alle Bitmaps (Icons/Insignien) aus den Icon-Objekten übergeben.
                        // Der Konstruktor der DDSFile-Klasse setzt diese dann zusammen. Anschließend braucht man nur noch die
                        // Save-Methode aufrufen, um die Datei zu speichern.
                        ddsFile = new DDSFile(Settings.Default.PathArmyIcons, Settings.Default.FileArmyIconGraphics, iconListe);
                        MessageBox.Show(ddsFile.Save());

                        ddsFile = new DDSFile(Settings.Default.PathArmyIcons, Settings.Default.FileArmyIconGraphicsSelected, iconListeSelected, true);
                        MessageBox.Show(ddsFile.Save());

                        SetNoOfFrames(typ);
                        break;

                    case ParadoxCategory.ArmyGroup:
                        ddsFile = new DDSFile(Settings.Default.PathArmyIcons, Settings.Default.FileArmyGroupIconGraphics, iconListe);
                        MessageBox.Show(ddsFile.Save());

                        ddsFile = new DDSFile(Settings.Default.PathArmyIcons, Settings.Default.FileArmyGroupIconGraphicsSelected, iconListeSelected, true);
                        MessageBox.Show(ddsFile.Save());

                        SetNoOfFrames(typ);
                        break;

                    case ParadoxCategory.Navy:
                        ddsFile = new DDSFile(Settings.Default.PathArmyIcons, Settings.Default.FileNavyIconGraphics, iconListe);
                        MessageBox.Show(ddsFile.Save());

                        ddsFile = new DDSFile(Settings.Default.PathArmyIcons, Settings.Default.FileNavyIconGraphicsSelected, iconListeSelected, true);
                        MessageBox.Show(ddsFile.Save());

                        SetNoOfFrames(typ);
                        break;

                    default:
                        break;
                }
            }

            // ********** Zum Schluss noch die Summe der Insignien in das GFX-Dateiobjekt speichern und die Datei neu schreiben **********
            using (FileStream fs = new FileStream(Settings.Default.PathInterface + Settings.Default.FileTheatreSelector, FileMode.Create))

            using (ParadoxSaver saver = new ParadoxSaver(fs))
            {
                theatreSelectorGFX.Write(saver);
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            ParadoxType prdxType = this.GetSelectedParadoxType();

            if(prdxType != null)
            {
                List<Icon> iconsToDelete = this.GetSelectedIcons();
                foreach (Icon icon in iconsToDelete)
                {
                    prdxType.Icons.Remove(icon);
                }
            }

            ComboBox_SelectionChanged(null, null); // Schade! Das mit der ObservableCollection klappt noch nicht ganz ohne explizite Aktualisierung :-(
        }

        // Es muss die korrekte Anzahl von Icons/Insignien in die GFX-Datei geschrieben werden. (Sonst werden die Icons im Spiel nicht richtig dargestellt.)
        private void SetNoOfFrames(ParadoxType paraTyp)
        {
            foreach (FrameAnimatedSpriteType frameAniSprTyp in theatreSelectorGFX.FrameAnimatedSpritetypes)
            {
                // Das hier ist die Verknüpfung zwischen den Dateien army_icons.txt und theatreselector.gfx über den GFX-Namen
                if(paraTyp.Gfx == frameAniSprTyp.Name | (paraTyp.Gfx + "_selected") == frameAniSprTyp.Name)
                {
                    frameAniSprTyp.NoOfFrames = paraTyp.Icons.Count.ToString();
                    //MessageBox.Show(frameAniSprTyp.Name + " no. of frames auf " + paraTyp.Icons.Count.ToString() + " geändert.");
                }
            }
        }

        private void dataGridInsignien_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            { 
                if (dataGridInsignien.SelectedCells.Count == 1)
                {
                    DataGridCellInfo cellInfo = dataGridInsignien.SelectedCells[0];
                    Row r = (Row)cellInfo.Item;
                    if(r.Icons[cellInfo.Column.DisplayIndex] != null)
                    {
                        _currentSelectedIcon = r.Icons[cellInfo.Column.DisplayIndex];
                        gridIconProperties.DataContext = this._currentSelectedIcon;
                    }
                    else
                    {
                        throw new Exception("Es wurde versucht eine leere Zelle zu markieren!");
                    }
                }
                else
                {
                    gridIconProperties.DataContext = null;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Fehler :-(", MessageBoxButton.OK, MessageBoxImage.Error);
                dataGridInsignien.UnselectAllCells(); 
            }
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            this._currentSelectedIcon.Availables[0].NOT[0].Tags.Remove(listBoxNotTags.SelectedItem.ToString());
            gridIconProperties.DataContext = null;
            gridIconProperties.DataContext = this._currentSelectedIcon;
        }

        private void listBoxAvailableTags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Wenn es noch keine Tags gibt im NOT-Bereich, dann muss erst einmal ein NOT-Objekt
            // erstellt werden dem man die Tags hinzufügen kann.
            if(this._currentSelectedIcon.Availables[0].NOT.Count == 0)
            {
                this._currentSelectedIcon.Availables[0].NOT.Add(new Not());
            }
            this._currentSelectedIcon.Availables[0].NOT[0].Tags.Add(listBoxAvailableTags.SelectedItem.ToString());
            gridIconProperties.DataContext = null;
            gridIconProperties.DataContext = this._currentSelectedIcon;
        }

        private void cmdExchange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridInsignien.SelectedCells.Count == 2)
                {
                    ParadoxCategory typ = (ParadoxCategory)comboBoxTyp.SelectedIndex;

#warning Hier noch ne Sicherung einbauen, falls es aus irgendeinem Grund mehere Gruppen gibt.
                    // Hier wird das richtige Objekt vom Typ ParadoxType gesucht, je nachdem welche Insignien
                    // gerade barabeitet werden (Armee oder Armeegruppe oder Flotte...). Mit dem richtigen
                    // ParadoxTyp-Objekt kann dann die richtige Iconliste bearbeitet werden.
                    foreach (ParadoxType ptyp in armeeIcons.ParadoxTypes)
                    {
                        if (ptyp.ParadoxCategory == typ)
                        {
                            // Speichert die beiden Indizes der Icons, die miteinander getauscht werden sollen. 
                            List<int> indizes = new List<int>();

                            foreach (DataGridCellInfo cellInfo in dataGridInsignien.SelectedCells)
                            {
                                Row row = (Row)cellInfo.Item;
                                // Berechnet den Index in der Icon-Liste. Hier stehen alle Icons, die
                                // im Grid angezeigt in Reihe.
                                indizes.Add(cellInfo.Column.DisplayIndex + 6 * row.No);
                            }

                            // Die beiden Indizes aufsteigend sortieren, damit der Austausch (siehe unten)
                            // funktioniert. Es wird davon ausgegangen, dass zuerst das Icon mit dem kleineren
                            // Index ausgewählt wurde. Dies führt aber ohne eine Sortierung zu einem
                            // fehlerhaften Tausch der Icons, wenn zuerst ein Icon mit höherem Index
                            // ausgewählt wird.
                            indizes.Sort();

#warning Hier noch eine Art Transaktionssicherheit einbauen, falls es mittendrin einen Fehler gibt und ein Listenelement bereits gelöscht oder eingefügt wurde.
                            ptyp.Icons.Insert(indizes[0], ptyp.Icons[indizes[1]]);
                            ptyp.Icons.RemoveAt(indizes[1] + 1);

                            ptyp.Icons.Insert(indizes[1] + 1, ptyp.Icons[indizes[0] + 1]);
                            ptyp.Icons.RemoveAt(indizes[0] + 1);
                        }
                    }

                    ComboBox_SelectionChanged(null, null); // Schade! Das mit der ObservableCollection klappt noch nicht ganz ohne explizite Aktualisierung :-(
                }
                else
                {
                    MessageBox.Show("Für diese Funktion müssen genau 2 Insignien markiert sein. (STRG + Mausklick) um mehrere auszuwählen.",
                                    "Tauschen nicht möglich",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Exclamation);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Fehler :-(", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Returns a list with object of type icon. Everey icon represent an insignia with all properties and graphics.
        /// Depending on what is selected in the combobox control of the editor, this method will find every selected icon
        /// in of the respective paradoxtype (army, armygroup or fleet). The icons of each paradoxtyp are represented in
        /// the datagrid.
        /// </summary>
        /// <returns></returns>
        private List<Icon> GetSelectedIcons()
        {
            List<Icon> iconList = new List<Icon>();
            ParadoxType prdxType = this.GetSelectedParadoxType();

            if(prdxType != null)
            {
                // go through all selected items in the dataGrid
                foreach (DataGridCellInfo cellInfo in dataGridInsignien.SelectedCells)
                {
                    Row row = (Row)cellInfo.Item;
                    // calculate the internal index of the icon list which belongs to the current paradoxType
                    int iconIndex = cellInfo.Column.DisplayIndex + 6 * row.No;


                    iconList.Add(prdxType.Icons[iconIndex]);
                }
            }
            else
            {
                MessageBox.Show("Der ParadoxTyp wurde anhand der Auswahl in der ComboBox nicht gefunden.", "Typ nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return iconList;
        }

        /// <summary>
        /// Returns the ParadoxType object from ArmyIconsTxt which is currently, 
        /// indirect selected via the combobox. (ParadoxTypes are army, armygroups, fleets etc.)
        /// </summary>
        /// <returns></returns>
        private ParadoxType GetSelectedParadoxType()
        {
#warning Hier noch ne Sicherung einbauen, falls es aus irgendeinem Grund mehere Typen gibt.
            // Save the currently selected paradox category in typ
            ParadoxCategory currentlySelectedTyp = (ParadoxCategory)comboBoxTyp.SelectedIndex;

            // Looking for the correct paradoxtype. Via the type we gain access to the icon list.
            foreach (ParadoxType ptyp in armeeIcons.ParadoxTypes)
            {
                if (ptyp.ParadoxCategory == currentlySelectedTyp)
                {
                    return ptyp;
                }
            }

            // Type not found for some reason.
            return null;
        }

        private void cmdCopy_Click(object sender, RoutedEventArgs e)
        {
            List<Icon> selectedIconList = this.GetSelectedIcons();
            this._tempIconList.Clear();

            try
            {
                foreach (Icon ico in selectedIconList)
                {
                    this._tempIconList.Add(ico.Clone());
                }

#warning Hier noch ein INotification einbauen 
                stackPanelButtons.DataContext = null;
                stackPanelButtons.DataContext = this;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Fehler beim Kopieren", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            ParadoxType pType = this.GetSelectedParadoxType();

            foreach(Icon icon in this._tempIconList)
            {
                pType.Icons.Add(icon);
            }

            // Gridansicht aktualisieren
            dataGridInsignien.ItemsSource = null;
            dataGridInsignien.ItemsSource = pType.IconGrid;
        }
    }
}
