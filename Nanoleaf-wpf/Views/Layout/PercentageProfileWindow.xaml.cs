﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Wpf.Views.Layout
{
    /// <summary>
    /// Interaction logic for PercentageProfileWindow.xaml
    /// </summary>
    public partial class PercentageProfileWindow : Window
    {
        private PercentageProfile _profile;

        public PercentageProfileWindow()
        {
            InitializeComponent();

            LayoutDisplay.SetWithAndHeight((int)LayoutDisplay.Width, (int)LayoutDisplay.Height);
            LayoutDisplay.DrawLayout();
            LayoutDisplay.EnableClick();
            LayoutDisplay.DisableColorTimer();

            if (UserSettings.Settings.ActiveDevice.PercentageProfile != null)
            {
                var serialized = JsonConvert.SerializeObject(UserSettings.Settings.ActiveDevice.PercentageProfile); //Deep copy the profile when editing
                _profile = JsonConvert.DeserializeObject<PercentageProfile>(serialized);
            }
            else
            {
                _profile = new PercentageProfile();
            }
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (LayoutDisplay.SelectedPanelIds.Count > 0)
            {
                var newStep = new PercentageStep();

                foreach (var panelId in LayoutDisplay.SelectedPanelIds)
                {
                    newStep.PanelIds.Add(panelId);
                }                

                _profile.Steps.Add(newStep);

                BuildStepList();

                LayoutDisplay.LockPanels(LayoutDisplay.SelectedPanelIds);

                LayoutDisplay.ClearSelectedPanels();
            }
        }

        private void BuildStepList()
        {
            StepList.Children.Clear();

            for (var i = 0; i < _profile.Steps.Count; i++)
            {
                StepList.Children.Add(new StepItemUserControl(this, i + 1, _profile.Steps[i]));
            }
        }

        public void HighlightPanles(HashSet<int> panelIds)
        {
            LayoutDisplay.HighlightPanels(panelIds);
        }

        public void UnhighlightPanles(HashSet<int> panelIds)
        {
            LayoutDisplay.UnhighlightPanels(panelIds);
        }

        public void DeleteStep(PercentageStep step)
        {
            LayoutDisplay.UnlockPanels(step.PanelIds);

            _profile.Steps.Remove(step);
            BuildStepList();
        }
    }
}
