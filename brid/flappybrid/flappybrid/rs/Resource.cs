using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace brid.rs
{
    class Resource
    {
        static Resource rs;
        public static Resource Main { get { if (rs == null) rs = new Resource();return rs; } }
        private Resource()
        {
            Uri u = new Uri("ms-appx:///rs/bk.png");
            BitmapImage bi = new BitmapImage(u);
            Background = new ImageBrush();
            Background.ImageSource = bi;
            u = new Uri("ms-appx:///rs/up.png");
            bi = new BitmapImage(u);
            PipeUp = new ImageBrush();
            PipeUp.ImageSource = bi;
            u = new Uri("ms-appx:///rs/down.png");
            bi = new BitmapImage(u);
            PipeDown = new ImageBrush();
            PipeDown.ImageSource = bi;
            Brid = new ImageBrush[3];
            for(int i=0;i<3;i++)
            {
                u = new Uri(string.Format("ms-appx:///rs/b{0}.png", i.ToString()));
                bi = new BitmapImage(u);
                Brid[i] = new ImageBrush();
                Brid[i].ImageSource = bi;
            }
        }
        public ImageBrush Background { get; private set; }
        public ImageBrush[] Brid { get; private set; }
        public ImageBrush PipeUp { get; private set; }
        public ImageBrush PipeDown { get; private set; }
    }
}
