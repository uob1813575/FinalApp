﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalApp.Views.Pages.MainMasterDetail {
    public class MainMasterDetailPageMenuItem {
        public MainMasterDetailPageMenuItem() {
            TargetType = typeof(MainMasterDetailPageDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public Type TargetType { get; set; }
    }
}
