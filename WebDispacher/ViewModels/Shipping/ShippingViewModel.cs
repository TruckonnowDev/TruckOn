using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DaoModels.DAO.Models;

namespace WebDispacher.ViewModels
{
    public class ShippingViewModel
    {
        public string Id { get; set; }
        public string IdOrder { get; set; }
        public string InternalLoadID { get; set; }
        public string CurrentStatus { get; set; }
        public string LastUpdated { get; set; }
        public string CDReference { get; set; }
        public string UrlReqvest { get; set; }

        /*      ORDER INFORMATION       */

        public string DispatchDate { get; set; }
        public string PickupExactly { get; set; }
        public string DeliveryEstimated { get; set; }
        public string ShipVia { get; set; }
        public string Condition { get; set; }
        
        public string PriceListed { get; set; }
        
        public string TotalPaymentToCarrier { get; set; }
        public string OnDeliveryToCarrier { get; set; }
        public string CompanyOwesCarrier { get; set; }
        public string Description { get; set; }
        public string BrokerFee { get; set; }

        /*      CONTACT INFORMATION        */

        public string ContactC { get; set; }
        public string PhoneC { get; set; }
        public string FaxC { get; set; }
        public string IccmcC { get; set; }

        /*      PICKUP INFORMATION      */

        public string NameP { get; set; }
        public string ContactNameP { get; set; }

        public string AddresP { get; set; }
        
        [MinLength(2)]
        [MaxLength(2)]
        public string StateP { get; set; }
        
        [MinLength(5)]
        [MaxLength(5)]
        public string ZipP { get; set; }

        public string CityP { get; set; }
        
        [MinLength(4)]
        public string PhoneP { get; set; }
        
        public string EmailP { get; set; }

        /*      DELIVERY INFORMATION        */

        public string NameD { get; set; }
        public string ContactNameD { get; set; }

        public string AddresD { get; set; }
        
        [MinLength(2)]
        [MaxLength(2)]
        public string StateD { get; set; }
        
        [MinLength(5)]
        [MaxLength(5)]
        public string ZipD { get; set; }
        
        public string CityD { get; set; }

        [MinLength(4)]
        public string PhoneD { get; set; }
        
        public string EmailD { get; set; }

             //DISPATCH INSTRUCTIONS       

        public string Titl1DI { get; set; }
        public List<VehiclwInformation> VehiclwInformations { get; set; }
        /*


        public AskFromUser AskFromUser { get; set; }
        public Ask2 Ask2 { get; set; }
        public AskForUserDelyveryM askForUserDelyveryM { get; set; }
        public List<DamageForUser> DamageForUsers { get; set; }
*/
              

        public int IdDriver { get; set; }

        public string DataPaid { get; set; }
        public string DataCancelOrder { get; set; }
        public string DataFullArcive { get; set; }
        public bool IsProblem { get; set; }
        public bool IsInstructinRead  { get; set; }
    }
}