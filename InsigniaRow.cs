using System.Collections.Generic;
using System.Windows.Media.Imaging; // u.a. BitmapSource

namespace HOI4Tool
{
    public class InsigniaRow
    {
        private List<BitmapSource> _Insignia = new List<BitmapSource>();
        public InsigniaRow(int InsigniaPerRow)
        {
            for (int i = 0; i < InsigniaPerRow; i++)
            {
                _Insignia.Add(null);
            }
        }
        public List<BitmapSource> Insignia
        {
            get
            {
                return _Insignia;
            }
        }
        public bool IsCompleteRow
        {
            get
            {
                for (int i = 0; i < _Insignia.Count; i++)
                {
                    if (_Insignia[i] == null) return false;
                }
                return true;
            }
        }

        public int AddInsignia(BitmapSource bmp_src)
        {
            for (int i = 0; i < _Insignia.Count; i++)
            {
                if (_Insignia[i] == null)
                {
                    _Insignia[i] = bmp_src;
                    return i;
                }
            }
            return _Insignia.Count;
        }
    }
}
