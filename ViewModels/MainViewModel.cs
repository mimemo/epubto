// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   ���C�������N���X
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace epubto.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using epubto.Common;
    using epubto.Models;
    using System.Windows;
    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using Reactive.Bindings.Notifiers;
    using Reactive.Bindings.Helpers;
    using Reactive.Bindings.ObjectExtensions;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using epubto.Services;
    using System.Windows.Media;
    using System.Windows.Data;
    using System.ComponentModel;

    /// <summary>���C�������N���X </summary>
    public class MainViewModel
    {
        /// <summary>�R���X�g���N�^ Initializes a new instance of the <see cref="MainViewModel"/> class. </summary>
        public MainViewModel()
        {
            this.KindlegenMenuCommand.Subscribe(_ => this.ChangeMode(0));
            this.AceByDaisyMenuCommand.Subscribe(_ => this.ChangeMode(1));
            this.AboutMenuCommand.Subscribe(_ => this.ChangeMode(2));
            this.CurrentViewTitle = this.CurrentViewIndex.Select(x => this.IndexToTitle(x)).ToReactiveProperty();

            // wpf-Webbrowser���őO�ʂɂ����Ⴄ�����
            this.CurrentViewIndex.Subscribe(x =>
            {

                if (x == 1)
                {
                    AceViewModel.IsShowBrowser.TurnOn();
                }
                else
                {
                    AceViewModel.IsShowBrowser.TurnOff();
                }
            });
        }

        public ReactiveProperty<string> CurrentViewTitle { get; set; } = new ReactiveProperty<string>(string.Empty);

        private string IndexToTitle(int x)
        {
            switch (x)
            {
                case 0:
                    {
                        return "kindlegen";
                    }
                case 1:
                    {
                        return "Ace by DAISY";
                    }
                case 2:
                    {
                        return "About";
                    }
                default:
                    {
                        return "Unknown";
                    }
            }

        }



        /// <summary>
        /// �I�𒆂̉��
        /// </summary>
        public ReactiveProperty<int> CurrentViewIndex { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// �\���X�V
        /// </summary>
        /// <param name="v">���[�h</param>
        private void ChangeMode(int mode)
        {
            this.CurrentViewIndex.Value = mode;
            this.SideMenuIsOpen.TurnOff();
        }


        /// <summary>
        /// �I�𒆂�View�C���f�b�N�X
        /// </summary>
        public ReactiveProperty<int> SelectedIndex { get; set; } = new ReactiveProperty<int>(0);



        public BooleanNotifier SideMenuIsOpen { get; set; } = new BooleanNotifier(false);



        /// <summary> The change message command.</summary>
        public ReactiveCommand KindlegenMenuCommand { get; set; } = new ReactiveCommand();
        public ReactiveCommand AceByDaisyMenuCommand { get; set; } = new ReactiveCommand();
        public ReactiveCommand AboutMenuCommand { get; set; } = new ReactiveCommand();




    }
}