using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace PauseResumeDemo
{
    public partial class PlayPauseDemo : Form
    {
        private BackgroundWorker _worker;
        readonly ManualResetEvent _busy = new ManualResetEvent(false);

        public PlayPauseDemo()
        {
            InitializeComponent();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (btnPause.Text.ToLower() == "pause")
            {
                PauseWorker();
                btnPause.Text = "Resume";
            }
            else
            {
                StartWorker();
                btnPause.Text = "Pause";
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnPause.Enabled = true;
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.WorkerReportsProgress = true;
            _worker.DoWork += DoWork;
            _worker.ProgressChanged += ProgressChanged;
            _worker.RunWorkerCompleted += WorkCompleted;
            StartWorker();
        }

        private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnPause.Enabled = false;
            btnPause.Text = "Pause";
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;

            lblProgress.Text = string.Format("{0}% complete!", e.ProgressPercentage);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                _busy.WaitOne();
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                var progress = i;
                _worker.ReportProgress(++progress);
                Thread.Sleep(80);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblProgress.Text = "Idle...";
            btnPause.Enabled = false;
            btnStop.Enabled = false;
            btnPause.Text = "Pause";

        }

        void StartWorker()
        {
            if (!_worker.IsBusy)
            {
                _worker.RunWorkerAsync();
            }
            // Unblock the worker 
            _busy.Set();
            lblProgress.Text = "In Progress...";
        }

        void PauseWorker()
        {
            // Block the worker
            _busy.Reset();
            lblProgress.Text += " Paused...";
        }

        void CancelWorker()
        {
            if (_worker.IsBusy)
            {
                _worker.CancelAsync();
                lblProgress.Text = "Idle...";
                progressBar.Value = 0;

                // Unblock worker so it can see that
                _busy.Set();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            CancelWorker();
        }
    }
}
