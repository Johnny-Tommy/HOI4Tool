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

        public IconSelectWindow(string pathToImage)
        {
            InitializeComponent();

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
                if (isDragging)
                {
                    double newXvalue = e.GetPosition(myCanvas).X - clickPosition.X; // Von der linken Rechteckseite aus betrachtet.
                    double newYvalue = e.GetPosition(myCanvas).Y - clickPosition.Y; // Von der oberen Rechteckseite aus betrachtet.

                    // Verschieben wenn die Grenzen mit dem Auswahlrechteck von Canvas nicht überschritten werden.
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
                    // Größe verändern
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
                            draggableControl.Height = e.GetPosition(myCanvas).Y - Canvas.GetTop(draggableControl);
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
                    // Cursoraussehen verändern, je nachdem wo sich der Mauszeiger befindet
                    if (e.GetPosition(draggableControl).X == 0 || e.GetPosition(draggableControl).X == draggableControl.Width)
                    {
                        Cursor = Cursors.SizeWE;
                        direction = e.GetPosition(draggableControl).X == 0 ? Direction.left : Direction.right;
                    }
                    else if (e.GetPosition(draggableControl).Y == 0 || e.GetPosition(draggableControl).Y == draggableControl.Height)
                    {
                        Cursor = Cursors.SizeNS;
                        direction = e.GetPosition(draggableControl).Y == 0 ? Direction.up : Direction.down;
                    }
                    else
                    {
                        Cursor = Cursors.SizeAll;
                        direction = Direction.none;
                    }
                }
            }

            //lblAusgabe.Content = "x (Maus) = " + e.GetPosition(myCanvas).X.ToString()
            //                  + " y (Maus) = " + e.GetPosition(myCanvas).Y.ToString()
            //                  + " RECHTECK: [Left = " + Canvas.GetLeft(draggableControl)
            //                  + " Top = " + Canvas.GetTop(draggableControl)
            //                  + " Width (Rahmen) = " + draggableControl.Width.ToString()
            //                  + " Height (Rahmen) = " + draggableControl.Height.ToString()
            //                  + "] ClickPos = " + clickPosition;
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            isResizing = false;
            Rectangle draggableControl = sender as Rectangle; // oder as Shape
            draggableControl.ReleaseMouseCapture();
            Int32Rect rechteck = new Int32Rect((int)Canvas.GetLeft(draggableControl), (int)Canvas.GetTop(draggableControl), (int)draggableControl.Width, (int)draggableControl.Height);
            CroppedBitmap croppedBmp = new CroppedBitmap(bmpImage, rechteck);
            myImage.Source = croppedBmp;
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
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }
}
