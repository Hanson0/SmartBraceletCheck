using AILinkFactoryAuto.Task.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AILinkFactoryAuto.Task.SmartBracelet.Property
{
    public class ReadMapProperties : CommonProperties
    {
        private string mapFilePath;


        [Category("MAP"), Description("MapFilePath")]
        public string MapFilePath
        {
            get { return mapFilePath; }
            set { mapFilePath = value; }
        }




    }
}
