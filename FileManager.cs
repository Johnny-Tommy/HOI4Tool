using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

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
        /// auf false gesetzt.
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

        public class Directory
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
                    PropertyInfo pi = Properties.Settings.Default.GetType().GetProperty(this._settingName);
                    pi.SetValue(Properties.Settings.Default, this._completePath);
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

        public class File
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
