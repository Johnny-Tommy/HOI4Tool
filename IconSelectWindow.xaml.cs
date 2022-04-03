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
using System.Windows.Shapes;

using HOI4Tool.Properties;
using System.IO;

namespace HOI4Tool
{
    /// <summary>
    /// Interaktionslogik für IconSelectWindow.xaml
    /// </summary>
    public partial class IconSelectWindow : Window
    {
        // Gibt an, ob man sich im "Verschiebemodus" befindet.
        // Dies ist der Fall, wenn man die linke Maustaste mitten im Rechteck gedrück hält (nicht die Außenränder!).
        protected bool isDragging = false;
        // Gibt an, ob man sich im "Größenänderungsmodus" befindet.
        // Dies ist der Fall, wenn man die linke Maustaste auf dem Rand des Rechtecks gedrückt hält.
        protected bool isResizing = false;
        // Das ist die Position an der die linke Maustaste auf dem Rechtseck gedrückt wird.
        private Point clickPosition;
        // Diese Variable speichert, in welche Richtung das Rechteck vergrößert wird. Dies hängt davon ab,
        // an welcher Seite des Rechtecks die linke Maustaste gedrückt und gehalten wird.
        private Direction direction = Direction.none;
        // Objekt, welches das aktuell geladene Bild darstellt.
        BitmapImage bmpImage;

        private CroppedBitmap _croppedBmp;
        private ParadoxType _pdxType;

        public IconSelectWindow(string pathToImage, ParadoxType pdxType)
        {
            InitializeComponent();

            this._pdxType = pdxType;

            // Ausgewähltes Bild laden und der Objektvariablen zuweisen. Dieses wird in ein Brush umgewandelt um es
            // im Canvas-Objekt als Hintergrund zu verwenden. Der Hintergrund ist quasi die Arbeitsfläche auf der man
            // einen Bildausschnitt auswählen kann.
            bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.StreamSource = new System.IO.FileStream(pathToImage, System.IO.FileMode.Open);
            bmpImage.EndInit();
            ImageBrush imgBrush = new ImageBrush(bmpImage);
            myCanvas.Background = imgBrush;
            // Wichtig: Hier das Canvas-Objekt den original Pixelmaßen des geladenen Bitmaps anpassen!
            // WPF versucht von Hause aus das Bitmap den Gegebenheiten der Desktopumgebung anzupassen (DPI).
            // Außerdem ist die reguläre Maßeinheit Punkt und nicht Pixel.
            myCanvas.Width = bmpImage.PixelWidth;
            myCanvas.Height = bmpImage.PixelHeight;

            windowIconSelect.DataContext = this;
        }

        private enum Direction
        {
            none,
            up,
            down,
            left,
            right
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle draggableControl = sender as Rectangle;

            if (draggableControl != null)
            {
                // ----------------------------------
                // Auswahlrahmen verschieben / ziehen
                // ----------------------------------

                if (isDragging)
                {
                    double newXvalue = e.GetPosition(myCanvas).X - clickPosition.X; // Von der linken Rechteckseite aus betrachtet.
                    double newYvalue = e.GetPosition(myCanvas).Y - clickPosition.Y; // Von der oberen Rechteckseite aus betrachtet.

                    // Verschieben wenn die Grenzen mit dem Auswahlrechteck von Canvas nicht überschritten werden!
                    if (newXvalue >= 0 && newXvalue + draggableControl.Width <= myCanvas.Width)
                    {
                        Canvas.SetLeft(draggableControl, newXvalue);
                    }

                    if (newYvalue >= 0 && newYvalue + draggableControl.Height <= myCanvas.Height)
                    {
                        Canvas.SetTop(draggableControl, newYvalue);
                    }
                }
                else if (isResizing)
                {
                    // ---------------
                    // Größe verändern
                    // ---------------

                    if (Cursor == Cursors.SizeWE)
                    {
                        if (direction == Direction.right)
                        {                            
                            // von rechts gezogen
                            draggableControl.Width = e.GetPosition(myCanvas).X - Canvas.GetLeft(draggableControl);
                        }

                        if (direction == Direction.left)
                        {
                            // von links gezogen
                            draggableControl.Width += Canvas.GetLeft(draggableControl) - e.GetPosition(myCanvas).X;
                            Canvas.SetLeft(draggableControl, e.GetPosition(myCanvas).X);
                        }
                    }
                    else
                    {
                        if (direction == Direction.down)
                        {
                            // von unten gezogen
                            draggableControl.Height = Math.Round(e.GetPosition(myCanvas).Y - Canvas.GetTop(draggableControl));
                        }

                        if (direction == Direction.up)
                        {
                            // von oben gezogen
                            draggableControl.Height += Canvas.GetTop(draggableControl) - e.GetPosition(myCanvas).Y;
                            Canvas.SetTop(draggableControl, e.GetPosition(myCanvas).Y);
                        }
                    }
                }
                else
                {
                    // --------------------------------------------------------------------
                    // Cursoraussehen verändern, je nachdem wo sich der Mauszeiger befindet
                    // --------------------------------------------------------------------

                    if (e.GetPosition(draggableControl).X >= 0 && e.GetPosition(draggableControl).X <= 1 || Math.Ceiling(e.GetPosition(draggableControl).X) == Math.Ceiling(draggableControl.Width))
                    {
                        //  
                        //  <-->
                        //  
                        Cursor = Cursors.SizeWE; 
                        // Left or Right?
                        direction = e.GetPosition(draggableControl).X >= 0 && e.GetPosition(draggableControl).X <= 1 ? Direction.left : Direction.right;
                    }
                    else if (e.GetPosition(draggableControl).Y >= 0 && e.GetPosition(draggableControl).Y <= 1 ||  
                        (Math.Ceiling(e.GetPosition(draggableControl).Y) >= Math.Floor(draggableControl.Height) && e.GetPosition(draggableControl).Y <= draggableControl.Height + 1))
                    {
                        //  /|\
                        //   |
                        //  \|/
                        Cursor = Cursors.SizeNS; 
                        // Up or Down?
                        direction = e.GetPosition(draggableControl).Y >= 0 && e.GetPosition(draggableControl).Y <= 1 ? Direction.up : Direction.down;                                    
                    }
                    else
                    {
                        Cursor = Cursors.SizeAll;
                        direction = Direction.none;
                    }

                    lblMessage.Content = direction.ToString();                  
                }
            }

            // **************************************** Ausgaben der Koordinaten ****************************************
            this.lblMousePosX.Content = e.GetPosition(myCanvas).X.ToString();
            this.lblMousePosXrounded.Content = Math.Round(e.GetPosition(myCanvas).X).ToString();
            this.lblMousePosY.Content = e.GetPosition(myCanvas).Y.ToString();
            this.lblMousePosYrounded.Content = Math.Round(e.GetPosition(myCanvas).Y).ToString();

            this.lblCropFrameSizeX.Content = draggableControl.Width.ToString();
            this.lblCropFrameSizeY.Content = draggableControl.Height.ToString();
            // **********************************************************************************************************
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            isResizing = false;
            Rectangle draggableControl = sender as Rectangle; // oder as Shape
            draggableControl.ReleaseMouseCapture();
            // Use the coordinates of the rectangle shape to create an preview of the new icon.
            Int32Rect rechteck = new Int32Rect((int)Canvas.GetLeft(draggableControl), (int)Canvas.GetTop(draggableControl), (int)draggableControl.Width, (int)draggableControl.Height);
            this._croppedBmp = new CroppedBitmap(bmpImage, rechteck);
            PreviewImage.Source = this._croppedBmp;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var draggableControl = sender as Shape;
            clickPosition = e.GetPosition(draggableControl);
            if (Cursor == Cursors.SizeAll)
            {
                isDragging = true;
            }
            else
            {
                isResizing = true;
            }
            draggableControl.CaptureMouse();

            this.lblClickPosX.Content = this.clickPosition.X.ToString();
            this.lblClickPosY.Content = this.clickPosition.Y.ToString();
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void cmdOriginalSize_Click(object sender, RoutedEventArgs e)
        {
            this.croppingFrame.Width = Settings.Default.InsigniaWidth;
            this.croppingFrame.Height = Settings.Default.InsigniaHeight;
        }

#warning Besser designen!
        // "Save" the new Images as new Icon. This Icon will be added to the respective List of Icon in the Paradox type.
        // Furthermore it will be appear in the Icongrid on the left side of the insignia editor.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Icon icon = new Icon();
            icon.Bmp = this.GetBitmap(this._croppedBmp);
            icon.Bmp.SetResolution(96, 96);
            icon.BmpSource = this.Bitmap2BitmapImage(icon.Bmp);
            this._pdxType.Icons.Add(icon);

            this.Close();
        }

        private System.Drawing.Bitmap GetBitmap(BitmapSource source)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            
            System.Drawing.Bitmap resizedBmp = new System.Drawing.Bitmap(bmp, new System.Drawing.Size(Settings.Default.InsigniaWidth, Settings.Default.InsigniaHeight));
            return resizedBmp;
        }

        private BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            img.StreamSource = ms;
            img.EndInit();

            return img;
        }
    }
}
