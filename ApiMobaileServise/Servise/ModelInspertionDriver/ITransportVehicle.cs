﻿using System.Collections.Generic;

namespace ApiMobaileServise.Servise.ModelInspertionDriver
{
    public interface ITransportVehicle
    {
        int CountPhoto { get; set; }
        string Type { get; set; }
        bool IsNextInspection { get; set; }
        List<string> NamePatern { get; set; }
        List<string> Layouts { get; set; }
        string PlateTruck { get; set; }
        string PlateTraler { get; set; }
        string TypeTransportVehicle { get; set; }
    }
}