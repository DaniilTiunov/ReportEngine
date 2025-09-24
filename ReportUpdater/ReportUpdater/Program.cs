using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ReportUpdater
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdaterForm());
        }
    }

    public class UpdaterForm : Form
    {
        private ProgressBar progressBar;
        private Label labelStatus;
        private TextBox textLocalPath;
        private TextBox textUpdatePath;
        private Button buttonUpdate;

        public UpdaterForm()
        {
            Width = 500;
            Height = 220;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Обновление Смета КИП";

            Label labelLocal = new Label() { Left = 10, Top = 10, Width = 120, Text = "Локальная папка:" };
            Controls.Add(labelLocal);
            textLocalPath = new TextBox() { Left = 140, Top = 10, Width = 320 };
            textLocalPath.Text = AppDomain.CurrentDomain.BaseDirectory;
            Controls.Add(textLocalPath);

            Label labelUpdate = new Label() { Left = 10, Top = 40, Width = 120, Text = "Папка обновлений:" };
            Controls.Add(labelUpdate);
            textUpdatePath = new TextBox() { Left = 140, Top = 40, Width = 320 };
            textUpdatePath.Text = @"\\172.16.10.16\Share\Output\ReportEngineRelease";
            Controls.Add(textUpdatePath);

            buttonUpdate = new Button() { Left = 10, Top = 75, Width = 450, Height = 30, Text = "Обновить" };
            buttonUpdate.Click += ButtonUpdate_Click;
            Controls.Add(buttonUpdate);

            labelStatus = new Label() { Left = 10, Top = 115, Width = 450, Text = "Ожидание..." };
            Controls.Add(labelStatus);

            progressBar = new ProgressBar() { Left = 10, Top = 145, Width = 450, Height = 25 };
            Controls.Add(progressBar);
        }

        private async void ButtonUpdate_Click(object sender, EventArgs e)
        {
            buttonUpdate.Enabled = false;
            string localPath = textLocalPath.Text.Trim(); 
            string updatePath = textUpdatePath.Text.Trim();

            if (!Directory.Exists(updatePath))
            {
                MessageBox.Show("Папка с обновлениями не найдена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonUpdate.Enabled = true;
                return;
            }
            if (!Directory.Exists(localPath))
            {
                MessageBox.Show("Локальная папка не найдена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonUpdate.Enabled = true;
                return;
            }
            await Task.Run(() => RunUpdate(localPath, updatePath));
        }

        private void RunUpdate(string localPath, string updatePath)
        {
            // 1. Закрываем приложение
            Invoke((Action)(() => labelStatus.Text = "Останавливаю ReportEngine..."));
            KillProcess("ReportEngine");

            // 2. Получаем список файлов
            var files = Directory.GetFiles(updatePath, "*", SearchOption.AllDirectories);
            int total = files.Length;
            int count = 0;

            foreach (var file in files)
            {
                string relativePath = Path.GetRelativePath(updatePath, file);
                string targetPath = Path.Combine(localPath, relativePath);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                    File.Copy(file, targetPath, overwrite: true);
                }
                catch (Exception ex)
                {
                    Invoke((Action)(() =>
                    {
                        labelStatus.Text = $"Ошибка копирования: {relativePath} ({ex.Message})";
                    }));
                    continue;
                }

                count++;
                int percent = (int)((count / (double)total) * 100);

                Invoke((Action)(() =>
                {
                    progressBar.Value = percent;
                    labelStatus.Text = $"Копирование файлов... {count}/{total}";
                }));
            }

            // 3. Запускаем приложение
            Invoke((Action)(() => labelStatus.Text = "Запускаю обновлённое приложение..."));
            try
            {
                Process.Start(Path.Combine(localPath, "ReportEngine.exe"));
            }
            catch (Exception ex)
            {
                Invoke((Action)(() => labelStatus.Text = $"Ошибка запуска: {ex.Message}"));
            }

            // 4. Закрываем апдейтер
            Invoke((Action)(() => Close()));
        }

        private void KillProcess(string processName)
        {
            foreach (var proc in Process.GetProcessesByName(processName))
            {
                try
                {
                    proc.Kill();
                    proc.WaitForExit();
                }
                catch { }
            }
            System.Threading.Thread.Sleep(1000);
        }
    }
}
