﻿using System.Collections.Generic;

namespace BaceModel.ModelInspertionDriver.Truck
{
    public class PickUpDuallyChassisOpenFrameInTheBackMustHave2 : ITransportVehicle
    {
        public int CountPhoto { get; set; } = 23;
        public string Type { get; set; } = "PickUpDuallyChassisOpenFrameInTheBackMustHave2";
        public bool IsNextInspection { get; set; } = true;
        public List<string> NamePatern { get; set; }
        public string PlateTruck { get; set; }
        public string PlateTraler { get; set; }
        public string TypeTransportVehicle { get; set; } = "Truck";
        public List<string> Layouts { get; set; }

        public PickUpDuallyChassisOpenFrameInTheBackMustHave2()
        {
            NamePatern = new List<string>()
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16",
                "17",
                "18",
                "19",
            };
            Layouts = new List<string>()
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16",
                "17",
                "18",
                "19",
            };
        }
    }
}