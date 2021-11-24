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

namespace HOI4Tool
{
    /// <summary>
    /// Interaktionslogik für Insignieneditor.xaml
    /// </summary>
    public partial class Insignieneditor : Page
    {
        private Icon _currentSelectedIcon;
        public ArmyIconsTxt armeeIcons;
        public SpriteTypes theatreSelectorGFX;
        public Insignieneditor()
        {
#warning Backupfunktion muss noch implantiert werden!
            InitializeComponent();

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
                        if(!string.IsNullOrEmpty(paradoxType.Grafikdateiname))
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
                                lblMeldung.Visibility = Visibility.Visible;
                                lblMeldung.Content = "Es wurden keine Daten für Armeen in der Konfigdatei gefunden!";
                            }
                        }
                        else
                        {
                            lblMeldung.Visibility = Visibility.Visible;
                            lblMeldung.Content = "Keine Grafikdatei für " + paradoxType.ParadoxCategory.ToString() + " vorhanden.";
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(armeeIcons != null)
            {
                ParadoxCategory typ = (ParadoxCategory)comboBoxTyp.SelectedIndex;

                // Hier noch ne Sicherung einbauen, falls es aus irgendeinem Grund mehere Gruppen gibt.
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
            List<Icon> iconsToDelete = new List<Icon>();

            ParadoxCategory typ = (ParadoxCategory)comboBoxTyp.SelectedIndex;

#warning Hier noch ne Sicherung einbauen, falls es aus irgendeinem Grund mehere Gruppen gibt.
            foreach (ParadoxType ptyp in armeeIcons.ParadoxTypes)
            {
                if (ptyp.ParadoxCategory == typ)
                {
                    foreach (DataGridCellInfo cellInfo in dataGridInsignien.SelectedCells)
                    {
                        Row r = (Row)cellInfo.Item;
                        iconsToDelete.Add(ptyp.Icons[cellInfo.Column.DisplayIndex + 6 * r.No]);
                    }

                    foreach (Icon icon in iconsToDelete)
                    {
                        ptyp.Icons.Remove(icon);
                    }

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
            if(dataGridInsignien.SelectedCells.Count == 1)
            {
                DataGridCellInfo cellInfo = dataGridInsignien.SelectedCells[0];
                Row r = (Row)cellInfo.Item;
                _currentSelectedIcon = r.Icons[cellInfo.Column.DisplayIndex];
                gridIconProperties.DataContext = _currentSelectedIcon;
            }
            else
            {
                gridIconProperties.DataContext = null;
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
    }
}
