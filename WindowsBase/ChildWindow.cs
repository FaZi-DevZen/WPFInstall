﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicTeachingInstall.WindowsBase
{
    /// <summary>
    /// 子窗体模板
    /// </summary>
    public class ChildWindow : Window, INotifyPropertyChanged
    {

        ResourceDictionary style1;
        ControlTemplate childWindowTemplate;
        static ChildWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow)));
        }
        public ChildWindow()
        {
            this.DataContext = this;
            style1 = new ResourceDictionary();
            style1.Source = new Uri("MusicTeachingInstall;component/WindowsBase/ChildWindowStyle.xaml", UriKind.Relative);
            this.Style = (System.Windows.Style)style1["ChildWindowStyle"];
        }


        //bool closeStoryBoardCompleted = false;

        /// <summary>
        /// 窗体关闭动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    if (!closeStoryBoardCompleted)
        //    {
        //        Grid gd = childWindowTemplate.FindName("WindowGrid", this) as Grid;
        //        Storyboard s = new Storyboard();
        //        ScaleTransform stf1 = new ScaleTransform();
        //        ScaleTransform stf2 = new ScaleTransform();
        //        DoubleAnimation da1 = new DoubleAnimation(1, 0.8, new Duration(TimeSpan.Parse("0:0:0.3")));
        //        da1.AutoReverse = false;
        //        Storyboard.SetTargetProperty(da1, new PropertyPath("RenderTransform.ScaleX"));
        //        DoubleAnimation da2 = new DoubleAnimation(1, 0.8, new Duration(TimeSpan.Parse("0:0:0.3")));
        //        da2.AutoReverse = false;
        //        Storyboard.SetTargetProperty(da2, new PropertyPath("RenderTransform.ScaleY"));
        //        DoubleAnimation da3 = new DoubleAnimation(1, 0.8, new Duration(TimeSpan.Parse("0:0:0.3")));
        //        Storyboard.SetTargetProperty(da3, new PropertyPath(Grid.OpacityProperty));
        //        da3.AutoReverse = false;
        //        s.Children.Add(da1);
        //        s.Children.Add(da2);
        //        s.Children.Add(da3);
        //        s.Completed += s_Completed;
        //        s.Begin(gd);
        //        e.Cancel = true;
        //    }
        //}

        //void s_Completed(object sender, EventArgs e)
        //{
        //    closeStoryBoardCompleted = true;
        //    this.Close();
        //}


        public override void OnApplyTemplate()
        {
            childWindowTemplate = (ControlTemplate)style1["ChildWindowTemplate"];

            Border borderTitle = (Border)childWindowTemplate.FindName("borderTitle", this);

            borderTitle.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                WindowMove(e);
            };
            Button minBtn = (Button)childWindowTemplate.FindName("btnMin", this);
            minBtn.Click += delegate
            {
                this.WindowState = WindowState.Minimized;
            };
            Button closeBtn = (Button)childWindowTemplate.FindName("btnClose", this);

            closeBtn.Click += delegate
            {
                this.Close();
            };
        }
        public void WindowMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                 this.DragMove();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return PropertyChanged;
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get
            {
                return PropertyChanging;
            }
        }

        public void VerifyPropertyName(string propertyName)
        {
            var myType = GetType();

            if (!string.IsNullOrEmpty(propertyName)
                && myType.GetProperty(propertyName) == null)
            {
                var descriptor = this as ICustomTypeDescriptor;

                if (descriptor != null)
                {
                    if (descriptor.GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Any(property => property.Name == propertyName))
                    {
                        return;
                    }
                }

                throw new ArgumentException("Property not found", propertyName);
            }
        }
        public virtual void RaisePropertyChanging(
            string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }
        public virtual void RaisePropertyChanged(
           string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool Set<T>(
           string propertyName,
           ref T field,
           T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);
            field = newValue;

            RaisePropertyChanged(propertyName);

            return true;
        }
        protected bool Set<T>(
            ref T field,
            T newValue,
             string propertyName = null)
        {
            return Set(propertyName, ref field, newValue);
        }
    }
}
