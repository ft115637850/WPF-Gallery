using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace WpfGallery
{
    [TemplatePart(Name = "PART_Pane1", Type = typeof(ImgPanel))]
    [TemplatePart(Name = "PART_Pane1ImagePlaceholder", Type = typeof(Image))]
    [TemplatePart(Name = "PART_Pane1border", Type = typeof(DropShadowEffect))]
    [TemplatePart(Name = "PART_Pane2", Type = typeof(ImgPanel))]
    [TemplatePart(Name = "PART_Pane2ImagePlaceholder", Type = typeof(Image))]
    [TemplatePart(Name = "PART_Pane2border", Type = typeof(DropShadowEffect))]
    [TemplatePart(Name = "PART_Pane3", Type = typeof(ImgPanel))]
    [TemplatePart(Name = "PART_Pane3ImagePlaceholder", Type = typeof(Image))]
    [TemplatePart(Name = "PART_Pane3border", Type = typeof(DropShadowEffect))]
    public class Gallery : Control
    {
        #region Static Fields
        public static DependencyProperty ImgsSrcProperty;

        static Gallery()
        {
            ImgsSrcProperty = DependencyProperty.Register(
                            "ImgsSrc",
                            typeof(List<BitmapSource>),
                            typeof(Gallery),
                            new FrameworkPropertyMetadata(new List<BitmapSource>(), new PropertyChangedCallback(OnImgsSrcChanged)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(typeof(Gallery)));
        }

        private static void OnImgsSrcChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Gallery colorPicker = (Gallery)sender;
            colorPicker.ImgsSrc = (List<BitmapSource>)e.NewValue;
        }
        #endregion

        #region Instance Fields
        private RelayCommand rotateCommand;
        #endregion

        #region Instance Propertie
        public List<BitmapSource> ImgsSrc
        {
            get { return (List<BitmapSource>)GetValue(ImgsSrcProperty); }
            set { SetValue(ImgsSrcProperty, value); }
        }

        internal List<ImgPanel> Panels
        {
            set;
            get;
        }

        public RelayCommand RotateCommand
        {
            get
            {
                if (rotateCommand == null)
                {
                    rotateCommand = new RelayCommand(Rotate);
                }
                return rotateCommand;
            }
        }
        #endregion

        #region Constructor
        public Gallery()
        {
            this.DataContext = this;
        }
        #endregion

        #region Public Instance Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var pane1 = GetTemplateChild("PART_Pane1") as ImgPanel;
            var pane2 = GetTemplateChild("PART_Pane2") as ImgPanel;
            var pane3 = GetTemplateChild("PART_Pane3") as ImgPanel;
            if (pane1 != null && pane2 != null && pane3 != null)
            {
                //TO DO: Command
                //pane1.MouseDown += ImagePlaceholder_MouseDown;
                //pane2.MouseDown += ImagePlaceholder_MouseDown;
                //pane3.MouseDown += ImagePlaceholder_MouseDown;
                Panels = new List<ImgPanel>
                        {
                            pane1,
                            pane2,
                            pane3
                        };
            }

            //this.Unloaded += (s, a) => 
            //{
            //    pane1.MouseDown -= ImagePlaceholder_MouseDown;
            //    pane2.MouseDown -= ImagePlaceholder_MouseDown;
            //    pane3.MouseDown -= ImagePlaceholder_MouseDown;
            //};
        }

        //public void Dispose()
        //{
        //    foreach (var pane in Panels)
        //    {
        //        pane.MouseDown -= ImagePlaceholder_MouseDown;
        //    }
        //}
        #endregion

        #region Private Instance Methods
        private void Rotate(object position)
        {
            var panelPosition = (ImagePanelPosition)position;
            switch (panelPosition)
            {
                case ImagePanelPosition.Left:
                    this.AnticlockwiseNav();
                    break;
                case ImagePanelPosition.Middle:
                    //Raise event
                    break;
                case ImagePanelPosition.Right:
                    this.ClockWiseNav();
                    break;
            }
        }

        private void ClockWiseNav()
        {
            var sb = new Storyboard();
            foreach (var panel in Panels)
            {
                var animations = panel.GetClockWiseNavAnimations();
                foreach (var animation in animations)
                {
                    sb.Children.Add(animation);
                }

                panel.ClockWiseNav();
            }

            sb.Begin(this);
        }

        private void AnticlockwiseNav()
        {
            var sb = new Storyboard();
            foreach (var panel in Panels)
            {
                var animations = panel.GetAnticlockwiseNavAnimations();
                foreach (var animation in animations)
                {
                    sb.Children.Add(animation);
                }

                panel.AnticlockwiseNav();
            }

            sb.Begin(this);
        }

        #endregion
        
    }
}
