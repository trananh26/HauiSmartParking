using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_parking
{
    public class RecognitionResult : IDisposable
    {
        #region Properties

        public bool Success { get; set; }
        public string PlateNumber { get; set; }
        public string FormattedText { get; set; }
        public string ErrorMessage { get; set; }

        public Bitmap PlateImage { get; set; }
        public Bitmap GrayImage { get; set; }
        public Bitmap ColorImage { get; set; }

        public List<Rectangle> UpperCharacters { get; set; }
        public List<Rectangle> LowerCharacters { get; set; }

        #endregion

        #region Methods

        public void Dispose()
        {
            PlateImage?.Dispose();
            GrayImage?.Dispose();
            ColorImage?.Dispose();
        }

        #endregion

    }

}
