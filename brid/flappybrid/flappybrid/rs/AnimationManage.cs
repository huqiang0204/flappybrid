using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Xaml;

namespace brid.rs
{
    public class AnimationManage
    {
        static AnimationManage am;
        public static AnimationManage Manage { get { if (am == null) am = new AnimationManage(); return am; } }
        //public void Add
        List<Animat> Actions;
        AnimationManage()
        {
            Actions = new List<Animat>();
        }
        public void Update(int timeslice)
        {
            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i] != null)
                    Actions[i].Update(timeslice);
            }
        }
        public Animat CreateAnimat(FrameworkElement target)
        {
            var ani = new Animat(target);
            Actions.Add(ani);
            return ani;
        }
        public void ReleaseAnimat(Animat ani)
        {
            Actions.Remove(ani);
        }
        public void ReleaseAll()
        {
            Actions.Clear();
        }
    }
    public class Animat
    {
        public object DataContext { get; set; }
        public Animat(FrameworkElement t)
        {
            Target = t;
            playing = false;
        }
        public FrameworkElement Target { get; private set; }
        public Vector StartPosition { get; set; }
        public Vector EndPosition { get; set; }
        public Vector OffsetStart { get; set; }
        public Vector OffsetEnd { get; set; }
        float time,time1;
        public float Delay { get; set; }
        public float Time { get { return time; } set { SurplusTime = time = value; } }
        public float OffsetTime { get { return time1; } set { remaintime = time1 = value; } }
        public bool OffsetLoop { get; set; }
        float SurplusTime;
        float remaintime;
        public bool playing { get; private set; }
        public Action<Animat> OnPlayOver;
        public void Play()
        {
            playing = true;
        }
        public void Pause()
        {
            playing = false;
        }
        public void Update(int timeslice)
        {
            if (playing)
            {
                if (Delay > 0)
                { Delay -= timeslice; }
                else
                {
                    SurplusTime -= timeslice;
                    if (SurplusTime <= 0)
                    {
                        playing = false;
                        Target.Margin = new Thickness(EndPosition.X-Target.Width*0.5f,
                            EndPosition.Y-Target.Height*0.5f, 0, 0);
                        if (OnPlayOver != null)
                            OnPlayOver(this);
                    }
                    else
                    {
                        float r = 1 - SurplusTime / time;
                        Vector v = EndPosition - StartPosition;
                        Thickness t = new Thickness();
                        t.Left = v.X * r + StartPosition.X-Target.Width*0.5f;
                        t.Top = v.Y * r + StartPosition.Y-Target.Height*0.5f;
                        Target.Margin = t;
                    }
                }
            }
        }
    }
}
