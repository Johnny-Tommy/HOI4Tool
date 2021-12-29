using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
// u.a. INotifyPropertyChanged
using System.ComponentModel;
// u.a. CallerMemberName
using System.Runtime.CompilerServices;
using HOI4Tool.Properties;

namespace HOI4Tool
{
    public class FileManager
    {
        private List<Directory> _directories;
        private bool _isCheckOk;

        public FileManager()
        {
            this._directories = new List<Directory>();
            this._isCheckOk = false;
        }


        public bool Backup()
        {
            try
            {
                // Prüfen, ob es bereits ein Backup gibt. Für's erste ein simpler Verzeichnischeck.
                List<string> unterVerzeichnisse = new List<string>(System.IO.Directory.EnumerateDirectories(Settings.Default.PathBackup));
                if (unterVerzeichnisse.Count == 0)
                {
                    // Keine Verzeichnisse gefunden. Backup starten...
                    string backupPathComplete = Settings.Default.PathBackup + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "\\";
                    System.IO.Directory.CreateDirectory(backupPathComplete);
                    foreach (Directory dir in this._directories)
                    {
                        foreach (File datei in dir.Files)
                        {
                            System.IO.File.Copy(dir.CompletePath + datei.Filename, backupPathComplete + datei.Filename);
                        }
                    }
                    System.Windows.MessageBox.Show("Die Dateien wurden im Verzeichnis " + backupPathComplete + " gesichert.",
                                                    "Backup komplett",
                                                    System.Windows.MessageBoxButton.OK,
                                                    System.Windows.MessageBoxImage.Information);
                    return true;
                }
                else if (unterVerzeichnisse.Count == 1)
                {
                    System.Windows.MessageBox.Show("Es gibt bereits ein Backup im Verzeichnis: " + Settings.Default.PathBackup,
                                                    "Backup vorhanden",
                                                    System.Windows.MessageBoxButton.OK,
                                                    System.Windows.MessageBoxImage.Information);
                    // Prüfen, ob alle Dateien vorhanden sind
                    return true;
                }
                else
                {
#warning Hier nicht eine eigene Exception einbauen für eine weitere Fehlerbehandlung...
                    System.Windows.MessageBox.Show("Es gibt mehr als ein Backupverzeichnis!",
                                                    "Fehler",
                                                    System.Windows.MessageBoxButton.OK,
                                                    System.Windows.MessageBoxImage.Error);
                    return false;
                }
            }
            catch(Exception err)
            {
#warning Hier nicht eine eigene Exception einbauen für eine weitere Fehlerbehandlung...
                System.Windows.MessageBox.Show(err.Message,
                                                "Fehler",
                                                System.Windows.MessageBoxButton.OK,
                                                System.Windows.MessageBoxImage.Error);
                return false;
            }
        }

        public void AddDirectory(string absolutePath, string settingname, string labelname)
        {
            if(absolutePath.Substring(absolutePath.Length - 1, 1) != "\\")
            {
                absolutePath += "\\";
            }

            this._directories.Add(new Directory(absolutePath, settingname, labelname));
        }

        public List<Directory> Directories
        {
            get
            {
                return this._directories;
            }
        }

        public bool IsCheckOk
        {
            get
            {
                return this._isCheckOk;
            }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Prüft, ob alle Verzeichnisse und zugehörigen Dateien gefunden werden.
        /// Wenn ein Objekt nicht gefunden werden kann, wird das jeweilige found-Flag 
        /// auf false gesetzt. Gibt nur true zurück, wenn alle Dateien und Verzeichnisse
        /// gefunden worden sind.
        /// </summary>
        public bool CheckAll()
        {
            bool tmp = true;

            foreach(Directory dir in this._directories)
            {
                if (!dir.Check()) tmp = false;
                foreach(File file in dir.Files)
                {
                    if(!file.Check(dir.CompletePath)) tmp = false;
                }
            }

            // Wenn eine einzige Datei / Verzeichnis nicht gefunden wurde, stimmt
            // die gesamte Prüfung nicht. (Hier sollte also nicht gespeichert werden.)
            this._isCheckOk = tmp;

            return tmp;
        }

        public class Directory : INotifyPropertyChanged
        {
            private string _settingName;
            private string _labelName;
            private string _completePath;
            private List<File> _files;
            private bool _found;
            
            public Directory(string directoryCompletePath, string settingName, string labelName)
            {
                this._completePath = directoryCompletePath;
                this._settingName = settingName;
                this._labelName = labelName;
                this._files = new List<File>();
                this._found = true;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public bool Found
            {
                get
                {
                    return this._found;
                }
            }

            public string LabelName
            {
                get
                {
                    return this._labelName;
                }
            }

            public string SettingName
            {
                get
                {
                    return this._settingName;
                }
            }

            public string CompletePath 
            { 
                get
                {
                    return this._completePath;
                }
                set
                {
                    this._completePath = value;
                    if (value.Substring(value.Length - 1, 1) != "\\")
                    {
                        this._completePath += "\\";
                    }
                    PropertyInfo pi = Properties.Settings.Default.GetType().GetProperty(this._settingName);
                    pi.SetValue(Properties.Settings.Default, this._completePath);
                    // XAML-Bindingengine melden, dass sich die Daten geändert haben.
                    OnPropertyChanged();
                }
            } 

            public List<File> Files 
            { 
                get
                {
                    return this._files;
                } 
            }

            public void AddFile(string filename, string settingname, string labelname)
            {
                this._files.Add(new File(filename, settingname, labelname));
            }

            public bool Check()
            {
                if(System.IO.Directory.Exists(this.CompletePath))
                {
                    this._found = true;
                    return true;
                }
                else
                {
                    this._found = false;
                    return false;
                }
            }
        }

        public class File : INotifyPropertyChanged
        {
            private string _fileName;
            private string _settingName;
            private string _labelName;
            private bool _found;

            public File(string fileName, string settingName, string labelName)
            {
                this._fileName = fileName;
                this._settingName = settingName;
                this._labelName = labelName;
                this._found = true;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public string Filename
            {
                get
                {
                    return this._fileName;
                }
                set
                {
                    this._fileName = value;
                    PropertyInfo pi = Properties.Settings.Default.GetType().GetProperty(this._settingName);
                    pi.SetValue(Properties.Settings.Default, this._fileName);
                    // XAML-Bindingengine melden, dass sich die Daten geändert haben.
                    OnPropertyChanged();
                }
            }

            public bool Found
            {
                get
                {
                    return this._found;
                }
            }

            public string SettingName 
            {
                get
                {
                    return this._settingName;
                } 
            }

            public string LabelName
            {
                get
                {
                    return this._labelName;
                }
            }

            public bool Check(string path)
            {
                if(System.IO.File.Exists(path + this._fileName))
                {
                    this._found = true;
                    return true;
                }
                else
                {
                    this._found = false;
                    return false;
                }
            }
        }
    }
}
