﻿#pragma checksum "..\..\..\Pages\WiFiPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "58DE26E5ECFF36ACA6F2682217DA222719EF7578F1E6B35C1F8385C93BB61EED"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace FrontEnd {
    
    
    /// <summary>
    /// WiFiPage
    /// </summary>
    public partial class WiFiPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 69 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox userInput;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox watermarkedTxt;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton IsHidden;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton WPA_WPA2;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton WEP;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton None;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button GetWifiInfoButton;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PasswordInput;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox watermarkedPassword;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Password;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\Pages\WiFiPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label Password_Копировать;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/FrontEnd;component/pages/wifipage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\WiFiPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.userInput = ((System.Windows.Controls.TextBox)(target));
            
            #line 69 "..\..\..\Pages\WiFiPage.xaml"
            this.userInput.LostFocus += new System.Windows.RoutedEventHandler(this.userInput_LostFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            this.watermarkedTxt = ((System.Windows.Controls.TextBox)(target));
            
            #line 71 "..\..\..\Pages\WiFiPage.xaml"
            this.watermarkedTxt.GotFocus += new System.Windows.RoutedEventHandler(this.watermarkedTxt_GotFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.IsHidden = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 4:
            this.WPA_WPA2 = ((System.Windows.Controls.RadioButton)(target));
            
            #line 96 "..\..\..\Pages\WiFiPage.xaml"
            this.WPA_WPA2.Checked += new System.Windows.RoutedEventHandler(this.EncryptionOption_Checked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.WEP = ((System.Windows.Controls.RadioButton)(target));
            
            #line 98 "..\..\..\Pages\WiFiPage.xaml"
            this.WEP.Checked += new System.Windows.RoutedEventHandler(this.EncryptionOption_Checked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.None = ((System.Windows.Controls.RadioButton)(target));
            
            #line 100 "..\..\..\Pages\WiFiPage.xaml"
            this.None.Checked += new System.Windows.RoutedEventHandler(this.EncryptionOption_Checked);
            
            #line default
            #line hidden
            return;
            case 7:
            this.GetWifiInfoButton = ((System.Windows.Controls.Button)(target));
            
            #line 102 "..\..\..\Pages\WiFiPage.xaml"
            this.GetWifiInfoButton.Click += new System.Windows.RoutedEventHandler(this.GetWifiInfoButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.PasswordInput = ((System.Windows.Controls.TextBox)(target));
            
            #line 105 "..\..\..\Pages\WiFiPage.xaml"
            this.PasswordInput.LostFocus += new System.Windows.RoutedEventHandler(this.userInput_LostFocus);
            
            #line default
            #line hidden
            return;
            case 9:
            this.watermarkedPassword = ((System.Windows.Controls.TextBox)(target));
            
            #line 107 "..\..\..\Pages\WiFiPage.xaml"
            this.watermarkedPassword.GotFocus += new System.Windows.RoutedEventHandler(this.watermarkedTxt_GotFocus);
            
            #line default
            #line hidden
            return;
            case 10:
            this.Password = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            this.Password_Копировать = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

