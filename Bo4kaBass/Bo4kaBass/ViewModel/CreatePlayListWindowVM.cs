using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bo4kaBass.ViewModel
{
    public class CreatePlayListWindowVM : INotifyPropertyChanged
    {
        //Название плейлиста
        private string playListName;
        public string PlayListName
        {
            get 
            {
                return playListName; 
            }
            set 
            {
                playListName = value;
                OnPropertyChanged(); 
            }
        }

        //Путь до постера плейлиста
        private string sourcePosterPlayList;
        public string SourcePosterPlayList 
        {
            get 
            {
                return sourcePosterPlayList;
            }
            set 
            {
                sourcePosterPlayList = value;
                OnPropertyChanged();
            }
        }

        //Команда для создания плейлиста
        public RelayCommand CreatePlayList { get; set; }
        //Команда для добавления обложки плейлиста
        public RelayCommand AddPosterPlayList { get; set; }
        public CreatePlayListWindowVM()
        {
            CreatePlayList = new RelayCommand(createPlayList);
            AddPosterPlayList = new RelayCommand(addPosterPlayList);
            SourcePosterPlayList = "/Resources/Лого.png";
        }

        private void createPlayList(object obj)
        {
           DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\PlayLists");
           if (directoryInfo.Exists == false)
           {
               directoryInfo.Create();
           }
            directoryInfo.CreateSubdirectory(PlayListName);
            FileInfo imageFile = new FileInfo(SourcePosterPlayList);
            imageFile.CopyTo(Path.Combine(@"C:\PlayLists\" + PlayListName, Path.GetFileName(SourcePosterPlayList)), true);
        }

        private void addPosterPlayList(object obj)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmaps|*.bmp|" +
                  "PNG files|*.png|" +
                  "JPEG files|*.jpg|" +
                  "Image files|*.bmp;*.jpg;*.png|" +
                  "All files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            SourcePosterPlayList = openFileDialog.FileName;
            

        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
