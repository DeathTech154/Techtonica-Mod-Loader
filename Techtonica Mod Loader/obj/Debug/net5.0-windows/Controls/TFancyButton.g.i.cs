﻿#pragma checksum "..\..\..\..\Controls\TFancyButton.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "62BD71D5A37F33F2C0E96BA8384F212452F7E80B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Techtonica_Mod_Loader.Controls;


namespace Techtonica_Mod_Loader.Controls {
    
    
    /// <summary>
    /// TFancyButton
    /// </summary>
    public partial class TFancyButton : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\..\..\Controls\TFancyButton.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Techtonica_Mod_Loader.Controls.TFancyButton thisControl;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.10.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Techtonica Mod Loader;V0.0.1.20;component/controls/tfancybutton.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\TFancyButton.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.10.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.thisControl = ((Techtonica_Mod_Loader.Controls.TFancyButton)(target));
            
            #line 10 "..\..\..\..\Controls\TFancyButton.xaml"
            this.thisControl.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.OnMouseLeftButtonUp);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\..\Controls\TFancyButton.xaml"
            this.thisControl.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.OnMouseRightButtonUp);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

