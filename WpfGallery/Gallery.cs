using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImgsSrcChanged)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(typeof(Gallery)));
        }

        private static void OnImgsSrcChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Gallery gallery = (Gallery)sender;
            gallery.ImgsSrc = (List<BitmapSource>)e.NewValue;
            gallery.Panels[0].image.Source = gallery.ImgsSrc[0];
            gallery.Panels[1].image.Source = gallery.ImgsSrc[1];
            gallery.Panels[2].image.Source = gallery.ImgsSrc[2];
        }
        #endregion

        #region Instance Fields
        private RelayCommand rotateCommand;
        private int middleImageIndex = 1;
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
        
        #region Public Instance Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var pane1 = this.GetTemplateChild("PART_Pane1") as ImgPanel;
            var pane2 = this.GetTemplateChild("PART_Pane2") as ImgPanel;
            var pane3 = this.GetTemplateChild("PART_Pane3") as ImgPanel;
            if (pane1 != null && pane2 != null && pane3 != null)
            {
                pane1.image = pane1.FindName("PART_Pane1ImagePlaceholder") as Image;
                pane2.image = pane1.FindName("PART_Pane2ImagePlaceholder") as Image;
                pane3.image = pane1.FindName("PART_Pane3ImagePlaceholder") as Image;
                
                Panels = new List<ImgPanel>
                        {
                            pane1,
                            pane2,
                            pane3
                        };
            }
        }
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

                if (panel.IsNewImageComing(true))
                {
                    var newImageIndex = (this.middleImageIndex + 2) % this.ImgsSrc.Count;
                    panel.image.Source = this.ImgsSrc[newImageIndex];
                    this.middleImageIndex = newImageIndex - 1;
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

                if (panel.IsNewImageComing(false))
                {
                    var newImageIndex = (this.middleImageIndex - 2 + this.ImgsSrc.Count) % this.ImgsSrc.Count;
                    panel.image.Source = this.ImgsSrc[newImageIndex];
                    this.middleImageIndex = newImageIndex + 1;
                }
                panel.AnticlockwiseNav();
            }

            sb.Begin(this);
        }

        #endregion
        
    }
}
