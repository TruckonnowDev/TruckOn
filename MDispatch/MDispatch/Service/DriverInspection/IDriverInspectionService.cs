using MDispatch.Models;
using MDispatch.Vidget.VM;

namespace MDispatch.Service.DriverInspection
{
    public interface IDriverInspectionService
    {
        int CheckInspectionDriver(string token, ref string description, ref bool isInspection, ref int indexPhoto, ref TruckCar truckCar);
        int SaveInspactionDriver(string token, ref string description, string idDriver, Photo photo, int indexPhoto, string typeTransportVehicle);
        int SetInspectionDriver(string token, ref string description, InspectionDriver inspectionDriver, string idDriver);
        int UpdateInspectionDriver(string token, ref string description, string idDriver);
        int SetPlate(string token, string plateTruck, string plateTrailer, string nowCheck, ref string description, ref bool isPlate, ref TruckCar truckCar);
        int GetLastInspaction(string token, string idDriver, ref string latsInspection, ref string plateTruck, ref string plateTrailer, ref string description);
        int CheckPlate(string token, ref string description, ref string plateTruckAndTrailer);
    }
}
