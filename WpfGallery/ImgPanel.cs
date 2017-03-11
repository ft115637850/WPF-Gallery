using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WpfGallery
{
    internal enum ImagePanelPosition
    {
        Left,
        Middle,
        Right
    }

    internal class ImgPanel : DockPanel
    {
        #region Static Fields
        public static DependencyProperty PositionProperty;
        static ImgPanel()
        {
            PositionProperty = DependencyProperty.Register(
                            "Position",
                            typeof(ImagePanelPosition),
                            typeof(ImgPanel),
                            new FrameworkPropertyMetadata(ImagePanelPosition.Middle, new PropertyChangedCallback(OnPositionChanged)));
        }

        private static void OnPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ImgPanel panel = (ImgPanel)sender;
            panel.Position = (ImagePanelPosition)e.NewValue;
            /*
            switch (panel.Position)
            {
                case ImagePanelPosition.Left:
                    panel.translate.X = -300;
                    panel.translate.Y = -10;
                    panel.scale.ScaleX = 0.7;
                    panel.scale.ScaleY = 0.7;
                    break;
                case ImagePanelPosition.Middle:
                    panel.translate.X = 1;
                    panel.translate.Y = 1;
                    panel.scale.ScaleX = 1;
                    panel.scale.ScaleY = 1;
                    break;
                case ImagePanelPosition.Right:
                    panel.translate.X = 500;
                    panel.translate.Y = -10;
                    panel.scale.ScaleX = 0.7;
                    panel.scale.ScaleY = 0.7;
                    break;
            }*/
        }

        #endregion

        #region Instance Fields
        private Int32AnimationUsingKeyFrames zIndexAnimation;
        private DoubleAnimation translateXAnimation;
        private DoubleAnimation translateYAnimation;
        private DoubleAnimation scaleXAnimation;
        private DoubleAnimation scaleYAnimation;
        private DoubleAnimation borderAnimation;
        #endregion

        #region Instance Propertie
        public ImagePanelPosition Position
        {
            get { return (ImagePanelPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion

        #region Public Instance Methods
        public void ClockWiseNav()
        {
            switch (this.Position)
            {
                case ImagePanelPosition.Left:
                    this.Position = ImagePanelPosition.Right;
                    break;
                case ImagePanelPosition.Middle:
                    this.Position = ImagePanelPosition.Left;
                    break;
                case ImagePanelPosition.Right:
                    this.Position = ImagePanelPosition.Middle;
                    break;
            }
        }

        public void AnticlockwiseNav()
        {
            switch (this.Position)
            {
                case ImagePanelPosition.Left:
                    this.Position = ImagePanelPosition.Middle;
                    break;
                case ImagePanelPosition.Middle:
                    this.Position = ImagePanelPosition.Right;
                    break;
                case ImagePanelPosition.Right:
                    this.Position = ImagePanelPosition.Left;
                    break;
            }
        }
        
        public List<AnimationTimeline> GetClockWiseNavAnimations()
        {
            switch (this.Position)
            {
                case ImagePanelPosition.Left:
                    this.UpdateAnimationForNewRightComing();
                    break;
                case ImagePanelPosition.Middle:
                    this.UpdateAnimationForMiddleToLeft();
                    break;
                case ImagePanelPosition.Right:
                    this.UpdateAnimationForRightToMiddle();
                    break;
            }

            return new List<AnimationTimeline>() 
            { 
                this.zIndexAnimation,
                this.translateXAnimation,
                this.translateYAnimation,
                this.scaleXAnimation,
                this.scaleYAnimation,
                this.borderAnimation
            };
        }
        
        public List<AnimationTimeline> GetAnticlockwiseNavAnimations()
        {
            switch (this.Position)
            {
                case ImagePanelPosition.Left:
                    this.UpdateAnimationForLeftToMiddle();
                    break;
                case ImagePanelPosition.Middle:
                    this.UpdateAnimationForMiddleToRight();
                    break;
                case ImagePanelPosition.Right:
                    this.UpdateAnimationForNewLeftComing();
                    break;
            }

            return new List<AnimationTimeline>() 
            { 
                this.zIndexAnimation,
                this.translateXAnimation,
                this.translateYAnimation,
                this.scaleXAnimation,
                this.scaleYAnimation,
                this.borderAnimation
            };
        }
        #endregion

        #region Protected Instance Methods
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.RenderTransform = new TransformGroup()
            {
                Children =
                {
                    new TranslateTransform()
                    {
                        X = this.Position == ImagePanelPosition.Left ? -300 : (this.Position == ImagePanelPosition.Middle ? 0 : 500),
                        Y = this.Position == ImagePanelPosition.Middle ? 0 : -10
                    },
                    new ScaleTransform()
                    {
                        ScaleX = this.Position == ImagePanelPosition.Middle ? 1 : 0.7,
                        ScaleY = this.Position == ImagePanelPosition.Middle ? 1 : 0.7
                    }
                }
            };

            this.zIndexAnimation = new Int32AnimationUsingKeyFrames();
            Storyboard.SetTarget(this.zIndexAnimation, this);
            Storyboard.SetTargetProperty(this.zIndexAnimation, new PropertyPath(ImgPanel.ZIndexProperty));

            this.translateXAnimation = new DoubleAnimation();
            this.translateXAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(this.translateXAnimation, this);
            Storyboard.SetTargetProperty(this.translateXAnimation, new PropertyPath("RenderTransform.Children[0].X"));

            this.translateYAnimation = new DoubleAnimation();
            this.translateYAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(this.translateYAnimation, this);
            Storyboard.SetTargetProperty(this.translateYAnimation, new PropertyPath("RenderTransform.Children[0].Y"));

            this.scaleXAnimation = new DoubleAnimation();
            this.scaleXAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(this.scaleXAnimation, this);
            Storyboard.SetTargetProperty(this.scaleXAnimation, new PropertyPath("RenderTransform.Children[1].ScaleX"));

            this.scaleYAnimation = new DoubleAnimation();
            this.scaleYAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(this.scaleYAnimation, this);
            Storyboard.SetTargetProperty(this.scaleYAnimation, new PropertyPath("RenderTransform.Children[1].ScaleY"));

            var shadow = this.FindName(this.Name + "border") as DropShadowEffect;
            this.borderAnimation = new DoubleAnimation();
            this.borderAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(this.borderAnimation, shadow);
            Storyboard.SetTargetProperty(this.borderAnimation, new PropertyPath(DropShadowEffect.ShadowDepthProperty));
 
        }

        #endregion     

        #region Private Methods
        private void UpdateAnimationForNewRightComing()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = -2},
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0.8), Value = -1}
                    };

            this.translateXAnimation.From = 1000;
            this.translateXAnimation.To = 500;

            this.translateYAnimation.From = -100;
            this.translateYAnimation.To = -10;

            this.scaleXAnimation.From = 0.4;
            this.scaleXAnimation.To = 0.7;

            this.scaleYAnimation.From = 0.4;
            this.scaleYAnimation.To = 0.7;

            this.borderAnimation.From = 0;
            this.borderAnimation.To = 1;
        }

        private void UpdateAnimationForMiddleToLeft()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = 1},
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0.8), Value = 0}
                    };

            this.translateXAnimation.From = 0;
            this.translateXAnimation.To = -300;

            this.translateYAnimation.From = 0;
            this.translateYAnimation.To = -10;

            this.scaleXAnimation.From = 1;
            this.scaleXAnimation.To = 0.7;

            this.scaleYAnimation.From = 1;
            this.scaleYAnimation.To = 0.7;

            this.borderAnimation.From = 2;
            this.borderAnimation.To = 1;
        }

        private void UpdateAnimationForRightToMiddle()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = -1},
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0.8), Value = 1}
                    };

            this.translateXAnimation.From = 500;
            this.translateXAnimation.To = 0;

            this.translateYAnimation.From = -10;
            this.translateYAnimation.To = 0;

            this.scaleXAnimation.From = 0.7;
            this.scaleXAnimation.To = 1;

            this.scaleYAnimation.From = 0.7;
            this.scaleYAnimation.To = 1;

            this.borderAnimation.From = 1;
            this.borderAnimation.To = 2;
        }

        private void UpdateAnimationForLeftToMiddle()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = -1},
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0.8), Value = 1}
                    };

            this.translateXAnimation.From = -300;
            this.translateXAnimation.To = 0;

            this.translateYAnimation.From = -10;
            this.translateYAnimation.To = 0;

            this.scaleXAnimation.From = 0.7;
            this.scaleXAnimation.To = 1;

            this.scaleYAnimation.From = 0.7;
            this.scaleYAnimation.To = 1;

            this.borderAnimation.From = 1;
            this.borderAnimation.To = 2;
        }

        private void UpdateAnimationForMiddleToRight()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = 1},
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0.8), Value = 0}
                    };

            this.translateXAnimation.From = 0;
            this.translateXAnimation.To = 500;

            this.translateYAnimation.From = 0;
            this.translateYAnimation.To = -10;

            this.scaleXAnimation.From = 1;
            this.scaleXAnimation.To = 0.7;

            this.scaleYAnimation.From = 1;
            this.scaleYAnimation.To = 0.7;

            this.borderAnimation.From = 2;
            this.borderAnimation.To = 1;
        }

        private void UpdateAnimationForNewLeftComing()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = -2},
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0.8), Value = -1}
                    };

            this.translateXAnimation.From = -500;
            this.translateXAnimation.To = -300;

            this.translateYAnimation.From = -50;
            this.translateYAnimation.To = -10;

            this.scaleXAnimation.From = 0.4;
            this.scaleXAnimation.To = 0.7;

            this.scaleYAnimation.From = 0.4;
            this.scaleYAnimation.To = 0.7;

            this.borderAnimation.From = 0;
            this.borderAnimation.To = 1;
        }
        #endregion
    }
}
