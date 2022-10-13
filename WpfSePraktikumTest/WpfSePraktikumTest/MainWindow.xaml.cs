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
using TiledCS;
namespace WpfSePraktikumTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player player;

        public MainWindow()
        {
            InitializeComponent();
            parseMap();
            player = new Player(200, 200, canv);
            
        }

        private void canv_KeyDown(object sender, KeyEventArgs e)
        {
            Window? source = e.Source as Window;
            if (source != null)
            {
                if (e.Key == Key.Left)
                {
                    player.velocity.X = -5;

                }
                if (e.Key == Key.Right)
                {
                    player.velocity.X = 5;

                }
                if (e.Key == Key.Up)
                {
                    player.Jump(10);

                }
                if (e.Key == Key.Down)
                {
                    player.velocity.Y = 5;

                }

            }
        }
        public void parseMap()
        {
            var map = new TiledMap("C:\\Users\\t2brozz\\RiderProjects\\mci\\WpfSePraktikumTest\\WpfSePraktikumTest\\bilder\\map.tmx");
            var tileset = new TiledTileset("C:\\Users\\t2brozz\\RiderProjects\\mci\\WpfSePraktikumTest\\WpfSePraktikumTest\\bilder\\tileset.tsx");
            var tilesetImage = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri("C:\\Users\\t2brozz\\RiderProjects\\mci\\WpfSePraktikumTest\\WpfSePraktikumTest\\bilder\\tileset.png"));
            ImageBrush imgBrsh = new ImageBrush(bitmapImage);
            // Retrieving objects or layers can be done using Linq or a for loop
            var myLayer = map.Layers.First(l => l.name == "Kachelebene 1");
            int tileSize = tileset.TileHeight;
            int cols = myLayer.width;
            int rows = myLayer.width;
            for (int i = 0; i < myLayer.data.Length; i++)
            {
                if ((int)myLayer.data[i] != 0)
                {
                    int posx = i % cols;
                    int posy = i / cols;
                    int val = (int)myLayer.data[i];
                    int tileX = val % tileset.Columns;
                    int tileY = val / tileset.Columns;
                    imgBrsh.ViewboxUnits = BrushMappingMode.Absolute;

                    imgBrsh.Viewbox = new Rect(tileX * tileSize-tileSize, tileY * tileSize, tileSize, tileSize);
                    Rectangle rect = new Rectangle();
                    rect.Tag = "wand";
                    rect.Width = tileSize;
                    rect.Height = tileSize;
                    rect.Fill = imgBrsh;
                    canv.Children.Add(rect);
                    Canvas.SetLeft(rect,posx * tileSize);
                    Canvas.SetTop(rect,posy * tileSize);
                }
            }
            // Since they are classes and not structs, you can do null checks to figure out if an object exists or not
        }
    }
}
