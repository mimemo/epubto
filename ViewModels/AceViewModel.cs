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

    /// <summary>���C�������N���X </summary>
    public class AceViewModel
    {
        /// <summary>�R���X�g���N�^ Initializes a new instance of the <see cref="AceViewModel"/> class. </summary>
        public AceViewModel()
        {
            this.CurrentItemViewIndex.Value = 0;

            this.Items.ObserveAddChanged().Delay(TimeSpan.FromSeconds(0.1)).Subscribe(_ => this.UpData());

            // �A�C�e��View���[�h�̃��X�g�I���B���I�����(-1)�ł���΋����I��0�Ƃ���
            this.CurrentItemViewIndex.Subscribe(x => this.CurrentItemViewIndex.Value = x == -1 ? 0 : x);

            // �����P��ǉ�
            var sampleepub = new EpubItem("sample.epub", 1);
            sampleepub.CurState.Value = EpubItem.State.Completed;

            this.Items.AddOnScheduler(sampleepub);

            this.EnableAceCheck.Value = AceUtil.CanRun;

            this.AceVersionTxt.Value = string.IsNullOrEmpty(AceUtil.Version) ? string.Empty : string.Format("{0}\r\n{1}", AceUtil.Path, AceUtil.Version);
            this.NodeVersionTxt.Value = string.Format("{0}", AceUtil.NodeVersion);

            // �㕔�^�u�I���j���[
            this.InViewIsSelected0.Where(x => x).Subscribe(_ =>
            {
                this.InViewSelectedIndex.Value = 0;
                AceViewModel.IsShowBrowser.TurnOn();
            });
            this.InViewIsSelected1.Where(x => x).Subscribe(_ =>
            {
                this.InViewSelectedIndex.Value = 1;
                AceViewModel.IsShowBrowser.TurnOff();
            });
            this.InViewIsSelected2.Where(x => x).Subscribe(_ =>
            {
                this.InViewSelectedIndex.Value = 2;
                AceViewModel.IsShowBrowser.TurnOff();
            });
        }


        #region Public Properties

        /// <summary> The change message command.</summary>

        public ReactiveProperty<int> CurrentViewIndex { get; set; } = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> CurrentItemViewIndex { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// �S�̓I�ȏ����e�L�X�g�ŕ\��
        /// </summary>
        /// <value> �\���p�̃e�L�X�g </value>
        public string StatusText { get; set; }

        /// <summary>
        /// �\�����[�h
        /// </summary>
        public ReactiveProperty<int> Mode { get; set; } = new ReactiveProperty<int>(0);

        /// <summary>
        /// AirSpace ���Ή�
        /// </summary>
        public static BooleanNotifier IsShowBrowser { get; set; } = new BooleanNotifier(false);

        /// <summary>
        /// �����Ώۃf�[�^
        /// </summary>
        public ReactiveCollection<EpubItem> Items { get; set; } = new ReactiveCollection<EpubItem>();

        /// <summary>
        ///  �I�𒆂̃A�C�e��
        /// </summary>
        public ReactiveProperty<EpubItem> SelectedItem { get; set; } = new ReactiveProperty<EpubItem>();

        public int ItemCount { get; set; } = -1;

        /// <summary>
        /// Ace by DAISY
        /// </summary>
        public BooleanNotifier EnableAceCheck { get; set; } = new BooleanNotifier(false);

        public ReactiveProperty<string> AceVersionTxt { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> NodeVersionTxt { get; } = new ReactiveProperty<string>();

        /// <summary>
        /// View���̃^�u�I�Ȃ��
        /// </summary>
        public ReactiveProperty<int> InViewSelectedIndex { get; set; } = new ReactiveProperty<int>(0);
        public BooleanNotifier InViewIsSelected0 { get; set; } = new BooleanNotifier(true);
        public BooleanNotifier InViewIsSelected1 { get; set; } = new BooleanNotifier(false);
        public BooleanNotifier InViewIsSelected2 { get; set; } = new BooleanNotifier(false);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// �X�V
        /// </summary>
        public void UpData()
        {
            // ���݂�Items�̐����A�z�萔�ƈ�v�����炢�����ɍX�V�J�n
            if (this.ItemCount == this.Items.Count)
            {
                // �������s
                foreach (var item in this.Items)
                {
                    item.DoProcessAce();
                }
            }

            this.ItemCount = -1;
        }

        /// <summary>�f�[�^��ǉ�</summary>
        /// <param name="files">�Ώۃt�@�C���p�X�̃��X�g</param>
        public void AddData(IEnumerable<string> files)
        {
            foreach (var item in files.Select((value, index) => new { value, index }))
            {
                this.Items.AddOnScheduler(new EpubItem(item.value, item.index + 1));
            }
        }

        /// <summary>�f�[�^���N���A</summary>
        public void ClearData()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// �f�[�^�����Z�b�g����
        /// </summary>
        public void ResetData()
        {
            // �e�s�����Z�b�g�B�Ō�ɍX�V�i�m��j�ׂ̈Ƀ��X�g�ɂ��Ă���
            var resets = (from item in this.Items select item.Clear()).ToList();
        }

        #endregion

    }
}