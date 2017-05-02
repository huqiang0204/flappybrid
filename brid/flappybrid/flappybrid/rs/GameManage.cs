using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace brid.rs
{
    class GameManage
    {
        static GameManage gm;
        public static GameManage Main { get { if (gm == null) gm = new GameManage();return gm; } }
        private GameManage()
        {

        }
        Border[] background;
        Window mainwindow;
        Canvas main_can;
        Button startgame;
        public void Inital(Window parent)
        {
            main_can = new Canvas();
            mainwindow = parent;
            parent.Content = main_can;
            background = new Border[2];
            for (int i = 0; i < 2; i++)
            {
                background[i] = new Border();
                background[i].Width = 528;
                background[i].Height = 350;
                background[i].Background = Resource.Main.Background;
                main_can.Children.Add(background[i]);
            }
            background[1].Margin = new Thickness(525,0,0,0);
            parent.SizeChanged += Resize;
            CompositionTarget.Rendering += Update;
            lasttime = DateTime.Now.Millisecond;
            SceneManage.Main.Inital(main_can);
            startgame = new Button();
            startgame.Width = 80;
            startgame.Height = 30;
            startgame.Content = "开始游戏";
            startgame.Margin = new Thickness(247.5f,170,0,0);
            startgame.Click += (o, e) => { GameStart();startgame.Visibility = Visibility.Collapsed; };
            main_can.Children.Add(startgame);
            main_can.PointerPressed += (o, e) => { PointerPressd = true; };
            main_can.PointerReleased += (o, e) => { PointerPressd = false; };
            ScaleTransform st = new ScaleTransform();
            st.ScaleX = mainwindow.Bounds.Width / 525;
            st.ScaleY = mainwindow.Bounds.Height / 350;
            st.CenterX = 0.5f;
            st.CenterY = 0.5f;
            main_can.RenderTransform = st;
        }
        void Resize(object o,object e)
        {
            (main_can.RenderTransform as ScaleTransform).ScaleX = mainwindow.Bounds.Width / 525;
            (main_can.RenderTransform as ScaleTransform).ScaleY = mainwindow.Bounds.Height / 350;
        }
        int lasttime;
        void Update(object o,object e)
        {
            int t = DateTime.Now.Millisecond;
            int st = t - lasttime;
            lasttime = t;
            if (st < 0)
                st += 1000;
            Update(st);
        }
        void Update(int slicetime)
        {
            if (!playing)
                return;
            AnimationManage.Manage.Update(slicetime);
            if( !SceneManage.Main.Update(slicetime))
            {
                playing = false;
                startgame.Visibility = Visibility.Visible;
            }
            float speed = slicetime * 0.005f;
            for(int i=0;i<2;i++)
            {
                var m = background[i].Margin;
                m.Left -= speed;
                if (m.Left < -525)
                    m.Left += 525;
                background[i].Margin = m;
            }
        }
        bool playing=false;
        public void GameStart()
        {
            playing = true;
            SceneManage.Main.Reset();
        }
        public bool PointerPressd { get; private set; }
    }
    class SceneManage
    {
        const double width = 30;
        const double height = 300;
        static SceneManage sm;
        public static SceneManage Main { get { if (sm == null) sm = new SceneManage();return sm; } }
        Canvas parent;
        List<Border> lb;
        List<Animat> la;
        Border brid;
        bool gameover;
        public void Inital(Canvas p)
        {
            parent = p;
            lb = new List<Border>();
            la = new List<Animat>();
            for(int i=0;i<30;i++)
            {
                Border bor = new Border();
               var rt = new RotateTransform();
                rt.CenterX = 15;
                rt.CenterY = 150;
                bor.RenderTransform = rt;
                bor.Width = width;
                bor.Height = height;
                if ((i & 1) == 0)
                    bor.Background = Resource.Main.PipeUp;
                else bor.Background = Resource.Main.PipeDown;
                lb.Add(bor);
                var ani = AnimationManage.Manage.CreateAnimat(bor);
                bor.Visibility = Visibility.Collapsed;
                bor.DataContext = 0;
                la.Add(ani);
                parent.Children.Add(bor);
            }
            brid = new Border();
            brid.Width = 34;
            brid.Height = 24;
            var rtf = new RotateTransform();
            rtf.CenterX = 17;
            rtf.CenterY = 12;
            brid.RenderTransform = rtf;
            brid.Background = Resource.Main.Brid[0];
            parent.Children.Add(brid);
            Reset();
        }
        public bool Update(int time)
        {
            if(gameover)
            {
                if (BridOverDown(time))
                    return false;
            }
            else
            {
                UpdateBrid(time);
                float speed = time * 0.08f;
                bool c = false;
                bool create = false;
                int count = 0;
                for (int i = 0; i < la.Count; i++)
                {
                    if (la[i].Target.Visibility == Visibility.Visible)
                    {
                        var t = la[i].Target.Margin;
                        t.Left -= speed;
                        la[i].Target.Margin = t;
                        var s = la[i].StartPosition;
                        s.X-= speed;
                        la[i].StartPosition = s;
                        s = la[i].EndPosition;
                        s.X -= speed;
                        la[i].EndPosition= s;
                        if (!c)
                        if (t.Left < 250)
                        {
                            Vector p = new Vector(t.Left + 15, t.Top + 150);
                            Vector[] v = MathF.RotateVector2(edge, p, 0);
                            c = MathF.PToP2(v, brid_edge);
                        }
                        if(t.Left<-100)
                        {
                            la[i].Target.Visibility = Visibility.Collapsed;
                            create = true;
                        }
                    }
                    count++;
                }
                if (count < 2)
                    return false;
                if (c)
                {
                    gameover = true;
                    (brid.RenderTransform as RotateTransform).Angle = 90;
                    for (int i = 0; i < 30; i++)
                        la[i].Pause();
                }
                if(create)
                    CreateNext();
            }
            return true;
        }
        public double DecayRate = 0.996f;
        double dr;
        int anitime;

        void UpdateBrid(int time)
        {
            if (GameManage.Main.PointerPressd)
                dr = 1;
            else
            {
                if (dr > 0.0000001f)
                {
                    int c = time;
                    double v = DecayRate;
                    while (c > 1)
                    {
                        v *= v;
                        c >>= 1;
                    }
                    if (c > 0)
                        v *= DecayRate;
                    dr *= v;
                   
                }
                if(dr>0.5)
                {
                    anitime += time;
                    if (anitime >= 300)
                        anitime -= 300;
                    int c = anitime / 100;
                    brid.Background = Resource.Main.Brid[c];
                }
                else
                brid.Background = Resource.Main.Brid[0];
            }
            double ss = 0.5f-dr;
            ss *= 8;
            double a = ss-4;
            a *= 4.5;
            var t = brid.Margin;
            t.Top += ss;
            if(t.Top>0 &t.Top<350)
            {
                brid.Margin = t;
                var p = new Vector(t.Left + 17, t.Top + 12);
                brid_edge = MathF.RotateVector2(edge_b, p, a);
                (brid.RenderTransform as RotateTransform).Angle = a;
            }
        }
        bool BridOverDown(int time)
        {
            var t = brid.Margin;
            t.Left = 20;
            t.Top +=time*0.2f;
            if (t.Top >= 350)
                return true;
            brid.Margin = t;
            return false;
        }
        public void Reset()
        {
            gameover = false;
            for (int i = 0; i < 14; i++)
            {
                var ani = la[i];
                ani.Target.Visibility = Visibility.Visible;
                ani.Target.Margin = new Thickness(pp[i].x,pp[i].y,0,0);
                (ani.Target.RenderTransform as RotateTransform).Angle = 0;
            }
            brid.Margin = new Thickness(20,160,0,0);
            next = 14;
        }
        int next;
        void CreateNext()
        {
            if (next >= pp.Length)
                return;
            int c=0;
            for(int i=0;i<30;i++)
            {
                if(la[i].Target.Visibility==Visibility.Collapsed)
                {
                    var ani = la[i];
                    ani.Target.Visibility = Visibility.Visible;
                    ani.Target.Margin = new Thickness(pp[next].x, pp[next].y, 0, 0);
                    (ani.Target.RenderTransform as RotateTransform).Angle = pp[next].angle;
                    int a = pp[next].type;
                    if (a > 0)
                        movs[a](ani);
                    next++;
                    c++;
                    if (c >= 2)
                        break;
                }
            }
        }
        static Position[] pp = new Position[] {
            new Position { x=200,y=-200},new Position { x=200,y=200},
            new Position { x=300,y=-170},new Position { x=300,y=230},
            new Position { x=400,y=-125},new Position { x=400,y=275},
            new Position { x=500,y=-200},new Position { x=500,y=200},
            new Position { x=600,y=-180},new Position { x=600,y=220},
            new Position { x=700,y=-160},new Position { x=700,y=240},
            new Position { x=800,y=-140},new Position { x=800,y=260},
              new Position { x=600,y=-160,type=1},new Position { x=600,y=240,type=1},
               new Position { x=600,y=-180,type=1},new Position { x=600,y=220,type=1},
                new Position { x=600,y=-200,type=1},new Position { x=600,y=200,type=1},
                 new Position { x=600,y=-220,type=1},new Position { x=600,y=180,type=1},
                new Position { x=600,y=-240,type=1},new Position { x=600,y=160,type=1},
                new Position { x=600,y=-240,type=2},new Position { x=600,y=160,type=2},
                  new Position { x=600,y=-220,type=2},new Position { x=600,y=180,type=2},
                   new Position { x=600,y=-200,type=2},new Position { x=600,y=200,type=2},
                    new Position { x=600,y=-180,type=2},new Position { x=600,y=220,type=2},
                  new Position { x=600,y=-160,type=2},new Position { x=600,y=240,type=2},
        };

        #region move
        //5.710593f,150.7481343f
        static Vector[] edge = new Vector[] {
             new Vector(175f,150.7481343f),new Vector(5f,150.7481343f),
            new Vector(355f,150.7481343f),new Vector(185f,150.7481343f) };
        static Vector[] edge_b = new Vector[] {
            new Vector(125f,20.80865f),new Vector(55f,20.80865f),
            new Vector(305f,20.80865f),new Vector(235f,20.80865f) };
        Vector[] brid_edge;
        static Action<Animat>[] movs = new Action<Animat>[] {
            null,MoveUpEX,MoveDownEX,MoveUp,MoveDown
        };
        static void MoveDownEX(Animat ani)
        {
            ani.OnPlayOver = MoveUpEX;
            var t = ani.Target.Margin;
            Vector v =new Vector( t.Left + 15,t.Top+150);
            ani.StartPosition = v;
            v.Y += 100;
            ani.EndPosition = v;
            ani.Time = 1000;
            ani.Play();
        }
        static void MoveUpEX(Animat ani)
        {
            ani.OnPlayOver = MoveDownEX;
            var t = ani.Target.Margin;
            Vector v = new Vector(t.Left + 15, t.Top + 150);
            ani.StartPosition = v;
            v.Y -= 100;
            ani.EndPosition = v;
            ani.Time = 1000;
            ani.Play();
        }
        static void MoveDown(Animat ani)
        {
            var t = ani.Target.Margin;
            Vector v = new Vector(t.Left + 15, t.Top + 150);
            ani.StartPosition = v;
            v.Y += 100;
            ani.EndPosition = v;
            ani.Time = 1000;
            ani.Play();
        }
        static void MoveUp(Animat ani)
        {
            var t = ani.Target.Margin;
            Vector v = new Vector(t.Left + 15, t.Top + 150);
            ani.StartPosition = v;
            v.Y -= 100;
            ani.EndPosition = v;
            ani.Time = 1000;
            ani.Play();
        }
        #endregion
    }
    struct Position
    {
        public float x;
        public float y;
        public int angle;
        public int type;
    }
}
