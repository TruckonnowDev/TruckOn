using BaceModel.ModelInspertionDriver.Trailer;
using BaceModel.ModelInspertionDriver.Truck;

namespace BaceModel.ModelInspertionDriver
{
    public class HelperTransport
    {
        public static ITransportVehicle GetTransportVehicle(string typeTruk)
        {
            ITransportVehicle transportVehicle = null;
            switch (typeTruk)
            {
                case "PickUpDually4TiresInTheBackMustHave4Door": transportVehicle = new PickUpDually4TiresInTheBackMustHave4Door(); break;
                case "PickupFourWheel": transportVehicle = new PickupFourWheel(); break;
                case "FourDoorTruckChassisOpenFrame": transportVehicle = new FourDoorTruckChassisOpenFrame(); break;
                case "BoxTruckSingleWheel": transportVehicle = new BoxTruckSingleWheel(); break;
                case "BoxTruckWheelDually": transportVehicle = new BoxTruckWheelDually(); break;
                case "CargiVanChasissDuallyOrSingleWheel": transportVehicle = new CargiVanChasissDuallyOrSingleWheel(); break;
                case "CargoVanMustHaveWithDually": transportVehicle = new CargoVanMustHaveWithDually(); break;
                case "PickUpDuallyChassisOpenFrameInTheBackMustHave2": transportVehicle = new PickUpDuallyChassisOpenFrameInTheBackMustHave2(); break;
                case "PickUpDuallyChassisMustHave2Door": transportVehicle = new PickUpDuallyChassisMustHave2Door(); break;
                case "PickUpDuallyChassisMustHave4Door": transportVehicle = new PickUpDuallyChassisMustHave4Door(); break;
                case "PickUpDuallyFlatbed4Door": transportVehicle = new PickUpDuallyFlatbed4Door(); break;
                case "PickUpDuallyFlatbedMustHave2Door": transportVehicle = new PickUpDuallyFlatbedMustHave2Door(); break;

                case "Bumperpool2CarEeclosedTraile": transportVehicle = new Bumperpool2CarEeclosedTraile(); break;
                case "EnclosedTrailerTwoVehicles": transportVehicle = new EnclosedTrailerTwoVehicles(); break;
                case "GooseneckTrailerTwoVehicles": transportVehicle = new GooseneckTrailerTwoVehicles(); break;
                case "a1CarGooseneckTrailer": transportVehicle = new A1CarGooseneckTrailer(); break;
                case "A3CarEnclosedGooseneckTrailer": transportVehicle = new A3CarEnclosedGooseneckTrailer(); break;
                case "Bumperpook24_34ftFlatbed": transportVehicle = new Bumperpook24_34ftFlatbed(); break;
                case "Bumperpool1CarEnclosedTrailer": transportVehicle = new Bumperpool1CarEnclosedTrailer(); break;
                case "Bumperpool16_20FtFlatbed": transportVehicle = new Bumperpool16_20FtFlatbed(); break;
                case "CarEnclosedTrailer": transportVehicle = new CarEnclosedTrailer(); break;
                case "Gooseneck3CarLowTrailer": transportVehicle = new Gooseneck3CarLowTrailer(); break;
                case "Gooseneck24_34FtFlatbed": transportVehicle = new Gooseneck24_34FtFlatbed(); break;
                case "Gooseneck3CarWedgeTrailer": transportVehicle = new Gooseneck3CarWedgeTrailer(); break;
                case "Gooseneck5CarOpenTrailer": transportVehicle = new Gooseneck5CarOpenTrailer(); break;
                case "Gooseneck6CarOpenTrailer": transportVehicle = new Gooseneck6CarOpenTrailer(); break;
                case "BumperPool2CarTrailer": transportVehicle = new BumperPool2CarTrailer(); break;

                case "Deffalt": transportVehicle = new DeffalteTransport(); break;
            }
            return transportVehicle;
        }
    }
}