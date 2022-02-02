using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bo4kaBass.ViewModel
{
    public class CreatePlayListWindowVM
    {
        public RelayCommand AddPosterPlayList { get; set; }
        public CreatePlayListWindowVM()
        {
            AddPosterPlayList = new RelayCommand(addPosterPlayList);
        }

        private void addPosterPlayList(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
