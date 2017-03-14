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
        public static DependencyProperty AnimationDurationProperty;
        public static DependencyProperty PositionProperty;
        static ImgPanel()
        {
            PositionProperty = DependencyProperty.Register(
                            "Position",
                            typeof(ImagePanelPosition),
                            typeof(ImgPanel),
                            null);
            AnimationDurationProperty = DependencyProperty.Register(
                            "AnimationDuration",
                            typeof(TimeSpan),
                            typeof(ImgPanel),
                            new FrameworkPropertyMetadata(TimeSpan.FromSeconds(0.5), null));
        }
        #endregion

        #region Instance Fields
        private Int32AnimationUsingKeyFrames zIndexAnimation;
        private DoubleAnimation translateXAnimation;
        private DoubleAnimation translateYAnimation;
        private DoubleAnimation scaleXAnimation;
        private DoubleAnimation scaleYAnimation;
        private DoubleAnimation borderAnimation;
        private List<AnimationTimeline> animationList;
        #endregion

        #region Instance Propertie
        public ImagePanelPosition Position
        {
            get { return (ImagePanelPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public Image image { get; set; }

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

        public bool IsNewImageComing(bool isClockWise)
        {
            if ((isClockWise && this.Position == ImagePanelPosition.Left) || (!isClockWise && this.Position == ImagePanelPosition.Right))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public List<AnimationTimeline> GetClockWiseNavAnimations()
        {
            switch (this.Position)
            {
                case ImagePanelPosition.Left:
                    this.UpdateZIndexAnimationForNewComing();
                    this.UpdatePositionAnimationForNewRightComing();
                    break;
                case ImagePanelPosition.Middle:
                    this.UpdateZIndexAnimationForGoBackwards();
                    this.UpdatePostionAnimationForMiddleToLeft();
                    break;
                case ImagePanelPosition.Right:
                    this.UpdateZIndexAnimationForGoForefront();
                    this.UpdatePositionAnimationForRightToMiddle();
                    break;
            }

            return this.animationList;
        }
        
        public List<AnimationTimeline> GetAnticlockwiseNavAnimations()
        {
            switch (this.Position)
            {
                case ImagePanelPosition.Left:
                    this.UpdateZIndexAnimationForGoForefront();
                    this.UpdatePositionAnimationForLeftToMiddle();
                    break;
                case ImagePanelPosition.Middle:
                    this.UpdateZIndexAnimationForGoBackwards();
                    this.UpdatePositionAnimationForMiddleToRight();
                    break;
                case ImagePanelPosition.Right:
                    this.UpdateZIndexAnimationForNewComing();
                    this.UpdatePositionAnimationForNewLeftComing();
                    break;
            }

            return this.animationList;
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
            this.translateXAnimation.Duration = new Duration(this.AnimationDuration);
            Storyboard.SetTarget(this.translateXAnimation, this);
            Storyboard.SetTargetProperty(this.translateXAnimation, new PropertyPath("RenderTransform.Children[0].X"));

            this.translateYAnimation = new DoubleAnimation();
            this.translateYAnimation.Duration = new Duration(this.AnimationDuration);
            Storyboard.SetTarget(this.translateYAnimation, this);
            Storyboard.SetTargetProperty(this.translateYAnimation, new PropertyPath("RenderTransform.Children[0].Y"));

            this.scaleXAnimation = new DoubleAnimation();
            this.scaleXAnimation.Duration = new Duration(this.AnimationDuration);
            Storyboard.SetTarget(this.scaleXAnimation, this);
            Storyboard.SetTargetProperty(this.scaleXAnimation, new PropertyPath("RenderTransform.Children[1].ScaleX"));

            this.scaleYAnimation = new DoubleAnimation();
            this.scaleYAnimation.Duration = new Duration(this.AnimationDuration);
            Storyboard.SetTarget(this.scaleYAnimation, this);
            Storyboard.SetTargetProperty(this.scaleYAnimation, new PropertyPath("RenderTransform.Children[1].ScaleY"));

            var shadow = this.FindName(this.Name + "border") as DropShadowEffect;
            this.borderAnimation = new DoubleAnimation();
            this.borderAnimation.Duration = new Duration(this.AnimationDuration);
            Storyboard.SetTarget(this.borderAnimation, shadow);
            Storyboard.SetTargetProperty(this.borderAnimation, new PropertyPath(DropShadowEffect.ShadowDepthProperty));

            this.animationList = new List<AnimationTimeline>() 
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

        #region Private Methods
        private void UpdateZIndexAnimationForNewComing()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = -2},
                        new SplineInt32KeyFrame(){ KeyTime = this.AnimationDuration, Value = -1}
                    };

            this.scaleXAnimation.From = 0.4;
            this.scaleXAnimation.To = 0.7;

            this.scaleYAnimation.From = 0.4;
            this.scaleYAnimation.To = 0.7;

            this.borderAnimation.From = 0;
            this.borderAnimation.To = 1;
        }

        private void UpdateZIndexAnimationForGoBackwards()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = 1},
                        new SplineInt32KeyFrame(){ KeyTime = this.AnimationDuration, Value = 0}
                    };

            this.scaleXAnimation.From = 1;
            this.scaleXAnimation.To = 0.7;

            this.scaleYAnimation.From = 1;
            this.scaleYAnimation.To = 0.7;

            this.borderAnimation.From = 2;
            this.borderAnimation.To = 1;
        }

        private void UpdateZIndexAnimationForGoForefront()
        {
            this.zIndexAnimation.KeyFrames = new Int32KeyFrameCollection 
                    {
                        new SplineInt32KeyFrame(){ KeyTime = TimeSpan.FromSeconds(0), Value = -1},
                        new SplineInt32KeyFrame(){ KeyTime = this.AnimationDuration, Value = 1}
                    };

            this.scaleXAnimation.From = 0.7;
            this.scaleXAnimation.To = 1;

            this.scaleYAnimation.From = 0.7;
            this.scaleYAnimation.To = 1;

            this.borderAnimation.From = 1;
            this.borderAnimation.To = 2;
        }

        private void UpdatePositionAnimationForNewRightComing()
        {
            this.translateXAnimation.From = 1000;
            this.translateXAnimation.To = 500;

            this.translateYAnimation.From = -100;
            this.translateYAnimation.To = -10;
        }        

        private void UpdatePostionAnimationForMiddleToLeft()
        {
            this.translateXAnimation.From = 0;
            this.translateXAnimation.To = -300;

            this.translateYAnimation.From = 0;
            this.translateYAnimation.To = -10;
        }
        
        private void UpdatePositionAnimationForRightToMiddle()
        {
            this.translateXAnimation.From = 500;
            this.translateXAnimation.To = 0;

            this.translateYAnimation.From = -10;
            this.translateYAnimation.To = 0;
        }

        private void UpdatePositionAnimationForLeftToMiddle()
        {
            this.translateXAnimation.From = -300;
            this.translateXAnimation.To = 0;

            this.translateYAnimation.From = -10;
            this.translateYAnimation.To = 0;
        }

        private void UpdatePositionAnimationForMiddleToRight()
        {
            this.translateXAnimation.From = 0;
            this.translateXAnimation.To = 500;

            this.translateYAnimation.From = 0;
            this.translateYAnimation.To = -10;
        }

        private void UpdatePositionAnimationForNewLeftComing()
        {
            this.translateXAnimation.From = -500;
            this.translateXAnimation.To = -300;

            this.translateYAnimation.From = -50;
            this.translateYAnimation.To = -10;
        }
        #endregion
    }
}
