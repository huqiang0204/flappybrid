using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace brid.rs
{
    class Resource
    {
        static Resource rs;
        public static Resource Main { get { if (rs == null) rs = new Resource();return rs; } }
        private Resource()
        {
            Uri u = new Uri("Pack://application:,,,/rs/brid.png");
            BitmapImage bi = new BitmapImage(u);
            Background = new ImageBrush(bi);
            Background.Viewbox= new Rect(0, 0, 0.61987f, 1f);
            Brid = new ImageBrush[3];
            Brid[0]= Background.Clone();
            Brid[0].Viewbox = new Rect(0.6587473f, 0.919921f, 0.0734341f, 0.046875f);
            Brid[1] = Background.Clone();
            Brid[1].Viewbox = new Rect(0.7796976f, 0.919921f, 0.0734341f, 0.046875f);
            Brid[2] = Background.Clone();
            Brid[2].Viewbox = new Rect(0.9006479f, 0.919921f, 0.0734341f, 0.046875f);
            PipeUp = Background.Clone();
            PipeUp.Viewbox = new Rect(0.697264f,0.02539f,0.112311f,0.625f);
            PipeDown = Background.Clone();
            PipeDown.Viewbox = new Rect(0.8185745f, 0.02539f, 0.112311f, 0.625f);
        }
        public ImageBrush Background { get; private set; }
        public ImageBrush[] Brid { get; private set; }
        public ImageBrush PipeUp { get; private set; }
        public ImageBrush PipeDown { get; private set; }
    }
}
