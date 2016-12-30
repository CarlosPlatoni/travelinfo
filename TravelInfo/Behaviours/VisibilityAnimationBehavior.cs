﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace TravelInfo.Behaviours
{
    public static class VisibilityAnimationBehavior
    {
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible", typeof(bool), typeof(VisibilityAnimationBehavior), new PropertyMetadata(true, IsVisibleChanged));
        public static bool GetIsVisible(DependencyObject Target) { return (bool)Target.GetValue(IsVisibleProperty); }
        public static void SetIsVisible(DependencyObject Target, bool Value) { Target.SetValue(IsVisibleProperty, Value); }

        static void IsVisibleChanged(DependencyObject Source, DependencyPropertyChangedEventArgs Arguments)
        {
            bool OldValue = (bool)Arguments.OldValue;
            bool NewValue = (bool)Arguments.NewValue;

            DependencyObject ParentObject = Source as DependencyObject;
            if (ParentObject == null)
                return;

            if (NewValue == true && OldValue != true)
            {
                Storyboard TransitionStoryboard = new Storyboard();
                Storyboard.SetTarget(TransitionStoryboard, ParentObject);
                FadeInThemeAnimation fadein = new FadeInThemeAnimation();
                fadein.Duration = new Duration(new TimeSpan(0,0,20));
                TransitionStoryboard.Children.Add(fadein);

                TransitionStoryboard.Begin();
            }
            else if (NewValue == false && OldValue != false) 
            {
                Storyboard TransitionStoryboard = new Storyboard();
                Storyboard.SetTarget(TransitionStoryboard, ParentObject);
                FadeInThemeAnimation fadeout = new FadeInThemeAnimation();
                fadeout.Duration = new Duration(new TimeSpan(0,0,20));
                TransitionStoryboard.Children.Add(fadeout);

                TransitionStoryboard.Begin();
            }
        }
    }
}
