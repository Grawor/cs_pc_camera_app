using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AForge.Video;             // AForge.NETライブラリから読込
using AForge.Video.DirectShow;  // AForge.NETライブラリから読込 

using OpenCvSharp;              // OpenCVSharp


namespace PcCameraApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // フィールド
        public bool DeviceExist = false;                // デバイス有無
        public FilterInfoCollection videoDevices;       // カメラデバイスの一覧
        public VideoCaptureDevice videoSource = null;   // カメラデバイスから取得した映像

        // Loadイベント（Formの立ち上げ時に実行）
        private void Form1_Load(object sender, EventArgs e)
        {
            this.getCameraInfo();
        }

        // カメラ情報の取得
        public void getCameraInfo()
        {
            try
            {
                // 端末で認識しているカメラデバイスの一覧を取得
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                comboBox1.Items.Clear();

                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                foreach (FilterInfo device in videoDevices)
                {
                    // カメラデバイスの一覧をコンボボックスに追加
                    comboBox1.Items.Add(device.Name);
                    comboBox1.SelectedIndex = 0;
                    DeviceExist = true;
                }
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                comboBox1.Items.Add("Deviceが存在していません。");
            }
        }

        // 開始or停止ボタン
        private void button1_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("ボタンクリック");

            if (button1.Text == "開始")
            {

                if (DeviceExist)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
                    videoSource.NewFrame += new NewFrameEventHandler(videoRendering);
                    this.CloseVideoSource();

                    videoSource.Start();

                    button1.Text = "停止";
                    timer1.Enabled = true;
                }
                else
                {
                    label1.Text = "デバイスが存在していません。";
                }
            }
            else
            {
                if (videoSource.IsRunning)
                {
                    timer1.Enabled = false;
                    this.CloseVideoSource();
                    label1.Text = "停止中";
                    button1.Text = "開始";

                }
            }
        }
        // 描画処理
        private void videoRendering(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = img;

        }
        // 停止の初期化
        private void CloseVideoSource()
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }
        // フレームレートの取得
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = videoSource.FramesReceived.ToString() + "FPS";
        }
    }
}
