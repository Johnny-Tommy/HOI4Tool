using System.Reflection;

namespace HOI4Tool
{
    public class CustomAssemblyInfo
    {
        private Assembly _assembly;
        private AssemblyName _assemblyName;

        public CustomAssemblyInfo()
        {
            this._assembly = typeof(MainWindow).Assembly;
            this._assemblyName = this._assembly.GetName();
        }

        // *** VERSION ***
        // Major: Erste Stelle des AssemblyVersionAttribute 
        // Minor: Zweite Stelle des AssemblyVersionAttribute
        // Build: Tage seit dem Jahr 2000 (steigt also mit jedem Tag um 1)
        // MinorRevision: Anzahl Sekunden seit Mitternacht geteilt durch 2
        // MajorRevision: Immer 0 ???

        public string ProductName => this._assemblyName.Name;
        public string FullAssemblyName => this._assemblyName.FullName;
        public string FullAssemblyVersion => $"{this._assemblyName.Version.Major.ToString()}.{this._assemblyName.Version.Minor.ToString()}.{this._assemblyName.Version.Build.ToString()}.{this._assemblyName.Version.MinorRevision.ToString()}";
        public string AssemblyBuild => this._assemblyName.Version.Build.ToString();
        public string AssemblyMinorRevision => this._assemblyName.Version.MinorRevision.ToString();
        public string AdditionalInfo => this._assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        public string Autor => this._assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string FullProgramVersionWithName => this.ProductName + " " + this.FullProgramVersion;
        public string FullProgramVersion
        {
            get
            {
                string[] teilstring = this._assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version.Split('.');

                // Sicherstellen, dass die Versionsnummer aus 4 Abschnitten, getrennt durch einen Punkt besteht.
                if(teilstring.Length == 4)
                {
                    string version = teilstring[0] + "." + teilstring[1];
                    version += "-" + this.AdditionalInfo;
                    version += " (Build: " + this.AssemblyBuild + "." + this.AssemblyMinorRevision + ")";
                    return version;
                }
                else
                {
                    return "Versionsattribut fehlerhaft.";
                }
            }
        }
    }
}
