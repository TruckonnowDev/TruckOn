using MDispatch.Models;
using System.Collections.Generic;
using System.Drawing;

namespace MDispatch.Service.Inspection
{
    public interface IInspectionService
    {
        int ReCurentStatus(string token, string id, ref string description, string status);
        int SendBolEmaile(string token, string idship, ref string description, string email);
        int SendCouponEmaile(string token, ref string description, string email);
        int SaveSigPikedUp(string token, Photo photoSig, string idShip, ref string description);
        int GetShipping(string token, string id, ref string description, ref Shipping shipping);
        int InspectionStatus(string token, string idShipping, string statusInspection, ref string description, ref Shipping shipping);
        int GetShippingPhoto(string token, string id, ref string description, ref Shipping shipping);
        int SaveAsk(string token, string id, Ask ask, ref string description);
        int SaveAsk(string token, string id, Ask2 ask, ref string description);
        int SaveInTruck(string token, string idVe, List<Photo> photo, ref string description);
        int SaveStrap(string token, string id, List<Photo> photo, ref string description);
        int SaveAsk(string token, Feedback feedback, ref string description);
        int SaveAsk(string token, string id, Ask1 ask1, ref string description);
        int SaveAsk(string token, string id, AskFromUser askForUser, ref string description);
        int SaveAsk(string token, string id, AskDelyvery askDelyvery, ref string description);
        int SetInstaraction(string token, string idShiping, ref string description);
        int CheckProblem(string token, string idShiping, ref bool isProplem);
        int SetProblem(string token, string idShiping);
        int SaveAsk(string token, string id, AskForUserDelyveryM askForUserDelyveryM, ref string description);
        int SavePhoto(string token, string id, PhotoInspection photoInspection, ref string description);
        byte[] OptimizeNResize(Bitmap original_image, double width, double heidth);
        int SaveDamageForuser(string token, string idVech, string idShiping, List<DamageForUser> damageForUsers, ref string description);
        int SavePhotPay(string token, string idShiping, int type, Photo photo, ref string description);
        int SaveVideoRecount(string token, string idShiping, int type, Video video, ref string description);
        int SaveMethodPay(string token, string idShiping, string payMethod, string countPay, ref string description);





    }
}
