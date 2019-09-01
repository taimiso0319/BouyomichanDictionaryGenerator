using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace BouyomichanDictionaryGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DICTIONARY_NAME = "ReplaceTag.dic";
        private const string BOUYOMI_EXE = "BouyomiChan.exe";
        private const string SOUND_DIR = "Sound";
        //private ObservableCollection<SoundSettingData> list = new ObservableCollection<SoundSettingData>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //listView.DataContext = this.list;
        }

        private void ExploreBouyomiPathButton_Click(object sender, RoutedEventArgs e)
        {
            // フォルダー参照ダイアログのインスタンスを生成
            var dlg = new CommonOpenFileDialog("フォルダ選択");
            dlg.IsFolderPicker = true;

            var ret = dlg.ShowDialog();
            if (ret == CommonFileDialogResult.Ok)
            {
                string directoryPath = dlg.FileName;
                BouyomiPathTextBox.Text = directoryPath;
                ListingItems(directoryPath);
            }
        }

        private void ListingItems(string directoryPath)
        {
            if (!File.Exists(directoryPath + "/" + BOUYOMI_EXE)) { return; }
            if (Directory.Exists(directoryPath + "/" + SOUND_DIR))
            {
                List<string> seFileList = GetSoundFilePaths(directoryPath + "/" + SOUND_DIR);
            }
            string[] dicLines = ReadDictionary(directoryPath + "/" + DICTIONARY_NAME);
            foreach(string dicLine in dicLines)
            {
                SoundSettingData soundSetting = new SoundSettingData();
                soundSetting.SetSettingString(dicLine);
                listView.Items.Add(soundSetting);
            }
        }

        private List<string> GetSoundFilePaths(string directoryPath)
        {
            return Directory.GetFiles(directoryPath).
                Where(p => p.EndsWith(".wav") || p.EndsWith(".mp3") || p.EndsWith(".wma")).ToList<string>();
        }

        private string[] ReadDictionary(string directoryPath)
        {
            return File.ReadLines(directoryPath).Where(l => l.Contains("(Sound")).ToArray();
            //foreach(string line in lines)
            //{
            //    Debug.WriteLine(line);
            //    string[] test = line.Split('	');
            //    foreach(string s in test)
            //    {
            //        Debug.WriteLine(s);
            //    }
            //}
        }

        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            Button playButton = sender as Button;
            SoundSettingData data = playButton.DataContext as SoundSettingData;
            Debug.WriteLine(data.soundName);
        }

        public class SoundSettingData
        {
            private char separator = '	';
            public int priority { get; set; }
            public bool isEnglishWord { get; set; }
            public string searchLetter { get; set; }
            public string soundName { get; set; }
            public string extension { get; set; }
            public bool isSoundW { get; set; }

            public void SetSettingString(string str)
            {
                string[] sep = str.Split(separator);
                priority = int.Parse(sep[0]);
                isEnglishWord = sep[1] == "E";
                searchLetter = sep[2];
                isSoundW = sep[3].StartsWith("(SoundW");
                int length = sep[3].Length;
                if (isSoundW)
                {
                    soundName = sep[3].Substring(8, length - 9);
                }
                else
                {
                    soundName = sep[3].Substring(7, length - 8);
                }
                extension = System.IO.Path.GetExtension(soundName);
                
            }

            public string GetSettingString()
            {
                return priority + separator +
                    (isEnglishWord ? "E" : "N") + separator +
                    searchLetter + separator +
                    (isSoundW ? "(SoundW " : "(Sound ") + soundName + extension + ")";
            }
        }
    }
}
