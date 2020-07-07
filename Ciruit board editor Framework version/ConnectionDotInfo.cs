using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ciruit_board_editor_Framework_version
{
    class ConnectionDotInfo
    {
        private string pictureTag;
        private List<PictureBox> pbList = new List<PictureBox>();

        public ConnectionDotInfo(string pictureTag)
        {
            this.pictureTag = pictureTag;
        }

        public void AddDotToList(PictureBox pb)
        {
            pbList.Add(pb);
        }

        public List<PictureBox> GetPictureBoxList()
        {
            return pbList;
        }

        public string GetTag()
        {
            return pictureTag;
        }


    }
}
