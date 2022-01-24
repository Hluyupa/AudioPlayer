using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Bo4kaBass.Model;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using TagLib;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Bo4kaBass.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        //gfhgfhgfghfhgfhgf
        //gfgfdgfxfxfdz
        //Переменная для статуса Играть/Пауза
        private bool PlayPauseStatus;

        //Класс, позволяющий работать с аудиофайлами
        private MediaPlayer MediaPlayer;

        //Команда для выбора папки
        public RelayCommand SelectDirectory { get; set; }

        //Команда для кнопки Играть/Пауза
        public RelayCommand PlayPauseCommand { get; set; }

        //Команда для переключения на предыдущий трек
        public RelayCommand PreviousMusic { get; set; }

        //Команда для переключения на следующий трек
        public RelayCommand NextMusic { get; set; }

        //Установка свойства отображения для textBox при отстуствии mp3-файлов
        private string noFilesToPlayed;
        public string NoFilesToPlayed
        {
            get 
            {
                return noFilesToPlayed; 
            }
            set 
            { 
                noFilesToPlayed = value;
                OnPropertyChanged(); 
            }
        }


        //Путь сменяемого изображения для кнопки остановки и виозобновления воспроизведение аудиозаписи
        private string playPauseImageSource;
        public string PlayPauseImageSource
        {
            get 
            {
                return playPauseImageSource; 
            }
            set 
            { 
                playPauseImageSource = value;
                OnPropertyChanged();
            }
        }

        //Звуковая шкала для громкости трека
        private double volumeMusic;
        public double VolumeMusic
        {
            get 
            {
                return volumeMusic; 
            }
            set 
            {
                volumeMusic = value;
                MediaPlayer.Volume = volumeMusic/100;
                OnPropertyChanged("VolumeMusic");
            }
        }

        //Текущая позиция проигрывания 
        private string timeMusicPosition;
        public string TimeMusicPosition
        {
            get 
            {
                return timeMusicPosition; 
            }
            set 
            {
                timeMusicPosition = value;
                OnPropertyChanged();
            }
        }

        //Длительность трека
        private string durationMusic;
        public string DurationMusic
        {
            get 
            {
                return durationMusic; 
            }
            set 
            {
                durationMusic = value; 
                OnPropertyChanged();
            }
        }

        //Текущя позиция трека и шкала перемотки
        private int sliderPositionMusic;
        public int SliderPositionMusic
        {
            get 
            {
                return sliderPositionMusic; 
            }
            set
            {
                if (value != 0)
                {
                    //Проверка на перемотку вперёд
                    if (value < sliderPositionMusic)
                    {
                        MediaPlayer.Position = TimeSpan.FromSeconds(sliderPositionMusic);
                    }
                    else
                    {
                        sliderPositionMusic = value;
                    }

                    //Проверка на перемотку назад
                    if (value > sliderPositionMusic + 1)
                    {
                        MediaPlayer.Position = TimeSpan.FromSeconds(sliderPositionMusic);
                    }
                    else
                    {
                        sliderPositionMusic = value;
                    }
                }
                else
                {
                    sliderPositionMusic = value;
                }
                OnPropertyChanged();
            }
        }

        //Конец трека (секунды)
        private int sliderFinishPositionMusic;
        public int SliderFinishPositionMusic
        {
            get
            {
                return sliderFinishPositionMusic;
            }
            set
            {
                sliderFinishPositionMusic = value;
                OnPropertyChanged();
            }
        }

        //Выбранная музыка для проигрывания
        private MetaDataMP3File selectedMusic;
        public MetaDataMP3File SelectedMusic
        {
            get 
            {
                return selectedMusic; 
            }
            set
            {
                selectedMusic = value;

                MediaPlayer.Stop();
                MediaPlayer.Open(new Uri(selectedMusic.Path));
                PlayPauseImageSource = "/Resources/Pause.png";
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                timer.Start();

                MediaPlayer.Play();
                MediaPlayer.MediaOpened += CheckDurationMusic_mediaOpened;
                PlayPauseStatus = true;
                OnPropertyChanged();
            }
        }

        private void CheckDurationMusic_mediaOpened(object sender, EventArgs e)
        {
            DurationMusic = MediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            SliderFinishPositionMusic = Convert.ToInt32(MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Включение следующего трека

            if (TimeMusicPosition == DurationMusic)
            {
                if (MetaDataMP3List.IndexOf(SelectedMusic) == MetaDataMP3List.Count - 1)
                {
                    SelectedMusic = MetaDataMP3List[0];
                }
                else
                {
                    SelectedMusic = MetaDataMP3List[MetaDataMP3List.IndexOf(SelectedMusic) + 1];
                }
                
            }

            TimeMusicPosition = MediaPlayer.Position.ToString(@"mm\:ss");
            SliderPositionMusic = Convert.ToInt32(MediaPlayer.Position.TotalSeconds);
            

        }


        //Лист с метаданными (исполнитель, название и т.д) треков
        private ObservableCollection<MetaDataMP3File> metaDataMP3List;
        public ObservableCollection<MetaDataMP3File> MetaDataMP3List
        {
            get 
            { 
                return metaDataMP3List; 
            }
            set 
            { 
                metaDataMP3List = value;
                
                OnPropertyChanged();
            }
        }

        //Путь до папки с mp3-файлами и чтение метаданных из каждого файла
        //Есть баг с выбором папки без mp3-файла. Крашится прога из-за отстуствия mp3-файла.
        private string directoryPath;
        public string DirectoryPath
        {
            get
            {
                return directoryPath;
            }
            set
            {
                directoryPath = value;
                
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                FileList = new ObservableCollection<FileInfo>(directoryInfo.GetFiles("*.mp3"));
                if (FileList.Count == 0)
                {
                    MessageBox.Show("В папке не найдено mp3-файлов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    NoFilesToPlayed = "Visible";
                }
                else
                {
                    NoFilesToPlayed = "Collapsed";
                }
                MetaDataMP3List.Clear();
                //Получение метаданных из каждого mp3-файла
                foreach (var item in FileList)
                {
                    

                    var audioFile = TagLib.File.Create(item.FullName);

                   

                    MetaDataMP3List.Add(new MetaDataMP3File
                    {
                        
                        Title = audioFile.Tag.Title,
                        Artist = audioFile.Tag.FirstAlbumArtist,
                        Album = audioFile.Tag.Album,
                        Year = audioFile.Tag.Year.ToString(),
                        Comment = audioFile.Tag.Comment,
                        Bitrate = audioFile.Properties.AudioBitrate.ToString(),
                        Duration = audioFile.Properties.Duration.ToString(),
                        Image = null,
                        Path = item.FullName
                    });
                    if (audioFile.Tag.Title == null)
                    {
                        MetaDataMP3List.Last().Title = item.Name;
                    }

                    if (audioFile.Tag.Pictures.Length > 0)
                    {
                        IPicture picture = audioFile.Tag.Pictures[0];
                        MetaDataMP3List.Last().Image = picture.Data.Data;
                    }

                    else
                    {


                        BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/Лого.png"));
                        byte[] data;
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        using (MemoryStream ms = new MemoryStream())
                        {
                            encoder.Save(ms);
                            data = ms.ToArray();
                        }
                        MetaDataMP3List.Last().Image = data;
                    }
                }
                

                OnPropertyChanged("DirectoryPath");
            }
        }

        //Получение списка mp3-файлов 
        private ObservableCollection<FileInfo> fileList;
        public ObservableCollection<FileInfo> FileList
        {
            get
            {
                return fileList;
            }
            set
            {
                fileList = value;
                OnPropertyChanged("FileList");
            }
        }

        public MainWindowVM()
        {
            PlayPauseCommand = new RelayCommand(playPauseCommand);
            SelectDirectory = new RelayCommand(selectDirectory);
            PreviousMusic = new RelayCommand(previousMusic);
            NextMusic = new RelayCommand(nextMusic);
            MediaPlayer = new MediaPlayer();
            TimeMusicPosition = "00:00";
            DurationMusic = "99:99";
            PlayPauseImageSource = "/Resources/Play.png";
            VolumeMusic = 50;
            PlayPauseStatus = false;
            MetaDataMP3List = new ObservableCollection<MetaDataMP3File>();
            try
            {
                NoFilesToPlayed = "Collapsed";
                using (StreamReader streamReader = new StreamReader("DirectoryPath.txt"))
                {
                    DirectoryPath = streamReader.ReadToEnd();
                }
            }
            catch 
            {
                NoFilesToPlayed = "Visible";
                MessageBox.Show("Выберите папку с файлами для вопроизведения.", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void nextMusic(object obj)
        {
            int index = MetaDataMP3List.IndexOf(SelectedMusic);
            if (index == MetaDataMP3List.Count - 1)
            {
                SelectedMusic = MetaDataMP3List[0];
            }
            else
            {
                SelectedMusic = MetaDataMP3List[MetaDataMP3List.IndexOf(SelectedMusic) + 1];
            }
        }

        private void previousMusic(object obj)
        {
            int index = MetaDataMP3List.IndexOf(SelectedMusic);
            if (index == 0)
            {
                SelectedMusic = MetaDataMP3List[MetaDataMP3List.Count - 1];
            }
            else
            {
                SelectedMusic = MetaDataMP3List[MetaDataMP3List.IndexOf(SelectedMusic) - 1];
            }
        }

        private void playPauseCommand(object obj)
        {
            PlayPauseStatus = !PlayPauseStatus;
            if (!PlayPauseStatus)
            {
                MediaPlayer.Pause();
                PlayPauseImageSource = "/Resources/PLay.png";
            }
            else
            {
                MediaPlayer.Play();
                PlayPauseImageSource = "/Resources/Pause.png";
            }
        }

        //Реализация метода для выбора папки с mp3-файлами
        private void selectDirectory(object obj)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryPath = folderBrowserDialog.SelectedPath;
                using (StreamWriter streamWriter = new StreamWriter("DirectoryPath.txt", false))
                {
                    streamWriter.WriteLine(DirectoryPath);
                }
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
