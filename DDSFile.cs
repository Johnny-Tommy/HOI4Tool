using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Drawing.Imaging;

// u.a. BitmapSource
using System.Windows.Media.Imaging; 
// System.Drawing.Common muss als Nugetpaket installiert werden.
// Stellt u.a. die Bitmap Klasse zur Verfügung.
using System.Drawing;
// .\BCnEncoder.NET\BCnEncTests\bin\Debug\netcoreapp3.1\BCnEncoder.dll
using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.Shared.ImageFiles;
using BCnEncoder.Shared;
// ReadOnlyMemory2D
//using Microsoft.Toolkit.HighPerformance.Memory;
using Microsoft.Toolkit.HighPerformance;
using HOI4Tool.Properties;

namespace HOI4Tool
{
    public class DDSFile
    {
        private DdsFile _Dds { get; set; }
        private Bitmap _Bmp { get; set; }
        private BitmapSource _BitmapSource { get; set; }
        private string _Path { get; set; }
        private string _Filename { get; set; }
        private string _Status { get; set; } = "OK";
        public DdsFile DDS => _Dds;
        public Bitmap BMP => _Bmp;
        public BitmapSource BMPSource => _BitmapSource;
        public string Path => _Path;
        public string Filename => _Filename;
        public string CompletePath => Path + Filename;
        public int WidthInPixel => this.Status != "OK" ? 0 : (int)_Dds.header.dwWidth;
        public int HeightInPixel => this.Status != "OK" ? 0 : (int)_Dds.header.dwHeight;
        public string Status => _Status;

        // ########## KONSTRUKTOREN ##########
        /// <summary>
        /// Dieser Konstruktor erstellt ein DDS-Fileobjekt aus einer vorhanden DDS-Datei auf einem Datenträger.
        /// </summary>
        /// <param name="PathToDDSFile"></param>
        /// <param name="DDSFilename"></param>
        public DDSFile(string PathToDDSFile, string DDSFilename)
        {
            try
            {
                _Path = PathToDDSFile.Substring(PathToDDSFile.Length - 1, 1) == @"\" ? PathToDDSFile : PathToDDSFile + @"\";
                _Filename = DDSFilename;
                FileStream fs = File.OpenRead(CompletePath);
                _Dds = DdsFile.Load(fs); 
                if(_Dds.header.ddsPixelFormat.DxgiFormat != DxgiFormat.DxgiFormatUnknown)
                {
                    _Bmp = Dds2Bitmap(CompletePath);
                    _BitmapSource = Bitmap2BitmapImage(_Bmp);
                }
                else
                {
                    this._Status = "unbekanntes DXGI-Format";
                }
            }
            catch (Exception err)
            {
                this._Bmp = null;
                this._BitmapSource = null;
                this._Status = err.Message;
            }
        }
        /// <summary>
        /// Dieser Konstruktor erstellt eine DDS-Datei anhand von einer Liste, die Bitmapdateien enthält. Es kann noch angegeben werden,
        /// ob diese horizontal,- oder vertikal aneinander gereiht werden. Standardmäßig wird die Lücke zwischen den Icons mit berücksichtigt.
        /// Die kann aber ggf. über den Schalter includeHoiIconGap ausgeschaltet werden.
        /// </summary>
        /// <param name="bmpListe"></param>
        public DDSFile(string PathToDDSFile, string DDSFilename, List<Bitmap> bmpListe, bool selectedIcons = false, AddBmpDirection direction = AddBmpDirection.horizontal, bool includeHoiIconGap = true)
        {
            _Path = PathToDDSFile.Substring(PathToDDSFile.Length - 1, 1) == @"\" ? PathToDDSFile : PathToDDSFile + @"\";
            _Filename = DDSFilename;

            if (direction == AddBmpDirection.horizontal)
            {
                int widthTotal = 0;
                int height = Settings.Default.InsigniaHeight;

                if (includeHoiIconGap)
                {
                    widthTotal = bmpListe.Sum(bmp => bmp.Width) + bmpListe.Count * Settings.Default.InsigniaGap;
                }
                else
                {
                    widthTotal = bmpListe.Sum(bmp => bmp.Width);
                }

                if (selectedIcons)
                {
                    //widthTotal += bmpListe.Count * 2;
                    height += 2;
                }

                this._Bmp = new Bitmap(widthTotal, height);
                using (var bmpHull = Graphics.FromImage(this._Bmp))
                {
                    int x = selectedIcons ? 1 : 0;
                    foreach (var image in bmpListe)
                    {
                        Bitmap bmpTemp = new Bitmap(image);
                        bmpHull.DrawImage(bmpTemp, x, 0);
                        x = x + Settings.Default.InsigniaGap + Settings.Default.InsigniaWidth;
                        if (selectedIcons) x += 2;
                    }
                }

                ColorRgba32[,] farbArray = new ColorRgba32[this.BMP.Height, this.BMP.Width];

                for (int y = 0; y < this.BMP.Height; y++)
                {
                    for (int x = 0; x < this.BMP.Width; x++)
                    {
                        Color pixel = this.BMP.GetPixel(x, y);
                        farbArray[y, x] = new ColorRgba32(pixel.R, pixel.G, pixel.B, pixel.A);
                        if(selectedIcons) farbArray[y, x] = new ColorRgba32(0, 255, 0, 0);
                    }
                }

                ReadOnlyMemory2D<ColorRgba32> tmp2D = new ReadOnlyMemory2D<ColorRgba32>(farbArray);

                BcEncoder encoder = new BcEncoder();

                //encoder.OutputOptions.DdsPreferDxt10Header = true;
                //encoder.OutputOptions.GenerateMipMaps = true;
                encoder.OutputOptions.Quality = CompressionQuality.Balanced;
                encoder.OutputOptions.Format = CompressionFormat.Rgba;
                encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;

                this._Dds = encoder.EncodeToDds(tmp2D);
            }
            else
            {
                throw new NotImplementedException("Die Funktion, BMPs vertikal aneinander zu reihen, ist noch nicht fertiggestellt.");
            }
        }

        public enum AddBmpDirection
        {
            horizontal,
            vertical
        }

        private Bitmap Dds2Bitmap(string completePath)
        {
            BcDecoder decoder = new BcDecoder();
            ColorRgba32[] rgba32 = decoder.Decode(_Dds);

            Bitmap bmp = new Bitmap(this.WidthInPixel, this.HeightInPixel);
            int argb = 0;
            for (int y = 0; y < this.HeightInPixel; y++)
            {
                for (int x = 0; x < this.WidthInPixel; x++)
                {
                    System.Drawing.Color c = System.Drawing.Color.FromArgb(rgba32[argb].a, rgba32[argb].r, rgba32[argb].g, rgba32[argb].b);
                    bmp.SetPixel(x, y, c);
                    argb++;
                }
            }

            return bmp;
        }

        internal BitmapImage Bitmap2BitmapImage(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            img.StreamSource = ms;
            img.EndInit();

            return img;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">X-Koordinate der linken unteren Ecke</param>
        /// <param name="y">Y-Koordinate der linken unteren Ecke</param>
        /// <returns></returns>
        public Bitmap GetCustomBitmap(int x, int y = 0)
        {            
            return this.BMP.Clone(new Rectangle(x, y, Settings.Default.InsigniaWidth, Settings.Default.InsigniaHeight), System.Drawing.Imaging.PixelFormat.Format32bppArgb);            
        }


        public string Save()
        {
            using FileStream fsAusgabe = File.OpenWrite(this.CompletePath);
            this._Dds.Write(fsAusgabe);
            return this.CompletePath + " gespeichert.";
        }
    }
}
