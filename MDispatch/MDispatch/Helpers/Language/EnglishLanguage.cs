namespace MDispatch.Helpers.Language
{
    public class EnglishLanguage : ILanguage
    {
        public string WelcomeText => "Welcome Back";
        public string WelcomeDescriptionText => "Log in to continue";
        public string PlaceholderEmail => "Email";
        public string PlaceholderPassword => "Password";
        public string BtnLogInText => "Log in";
        public string ForGotPasswordText => "Forgot password?";
        public string WrongScanPicture => "Please scan car plate that is registered.";
        public string PasswordChangeRequestTitel => "Password Change Request";
        public string PlaceholderEmailChangeRequest => "Enter your mail here";
        public string NotCorectEmail => "The entered mail format is not correct";
        public string PlaceholderNameChangeRequest => "Enter your full name here";
        public string PasswordChangeRequestBtnText => "Request Password Changes";

        public string SuccessfulPasswordChangeRequest => "Password reset data sent to you by mail:";

        public string InspectionTodayAlert => "You have already passed inspection today";
        public string AskErrorAlert => "You did not fill in all the required fields, you can continue the inspection only when filling in the required fields !!";
        public string PassTheDeviceAlert => "Please pass the device to the client";
        public string TechnicalWorkServiceAlert => "Technical work on the service";
        public string GiveMoneyAlert => "Give money for delivery to the driver";
        public string PaymentForDeliveryAlert => "You must enter the amount of payment for delivery";
        public string NotNetworkAlert => "Not Network";
        public string NoVehiclesAlert => "There are no vehicles in the order.\n\nIn order to pass the inspection, ask the dispatcher to add a vehicle.";
        public string NoAvtorisationAlert => "Not Authorized";
        public string NoDataAlert => "No Data";
        public string AnswersSaved => "Answers to questions saved";
        public string VideoSavedSuccessfully => "Video capture saved successfully";
        public string PaymentSaved => "Payment method photo saved";
        public string FutureDispatcherProblem => "In the near future the dispatcher see the problem";
        public string BOLIsSent => "A copy of BOL is sent to the mail";
        public string InformationDeliverySaved => "Information about delivery saved";
        public string InformationPaymentSaved => "Information about Paymmant saved";
        public string InformationPikedUpSaved => "Information about Picked Up saved";
        public string FeedbackSaved => "Feedback saved";
        public string PaymmantMethodSaved => "Paymmant method saved";

        public string ScanPlateNumber => "Scan Plate number";
        public string TitleSetPlateTruckAlert => "Please input or take a picture of Truck plate number";
        public string TitleSetPlateTrailerAlert => "Please write the number plate of the trailer";
        public string PlaceholderSetPlateTruckAlert => "Enter № plate truck";
        public string PlaceholderSetPlateTrailerkAlert => "Enter № plate trailer";
        public string CancelBtnText => "Cancel";
        public string SendBtnText => "Send";

        public string TitleInfoPage => "Info";
        public string TitleVehicleInfo => "Vehicle";
        public string TitlePikedUpInfo => "Pickup information";
        public string TitleDeliveryInfo => "Delivery information";
        public string TitlePaymentInfo => "Payment information";
        public string ContatInfo => "Contact: ";
        public string PhoneInfo => "Phone: ";
        public string PaymentInfo => "Payment: ";
        public string Instructions => "Instructions";
        public string ReedInstructionsBtnText => "I have read the instructions";

        public string TitleSettingsPage => "Settings";
        public string DocumentInfo => "Documents";
        public string ShowDocumentBtnText => "Show documents";
        public string LastInspactionInfo => "Last inspection: ";
        public string TruckPlateInfo => "Truck plate: ";
        public string TrailerPalateInfo => "Trailer plate: ";
        public string TitleDocumentsTrailerTruckNumber => "Documents by trailer and truck number";
        public string NumberTruckPlateInfo => "№ plate truck:";
        public string NumberTrailerPalateInfo => "№ plate trailer:";
        public string Application => "Application";
        public string CurrentVersion => "Current version: ";
        public string LastUpdateAvailable => "Last update available: ";
        public string SignOutBtnText => "Sign out";
        public string LanguageText => "Language";

        public string NamePageTabActive => "Active";
        public string NamePageTabDelivery => "Delivery";
        public string NamePageTabArchived => "Archived";

        public string TitleInspectionDriverAlert => "To take an order, you need to pass the inspection of the truck and the trailer, pass the inspection of the truck?";
        public string YesBtnText => "Yes";
        public string NoBtnText => "No";
        public string TakeBtnText => "Take";

        public string ContinuingInspectionDelivery => "Continuing inspection Delivery";
        public string ContinuingInspectionPickedUp => "Continuing inspection Picked up";
        public string StartInspectionDelivery => "Inspection of the Delivered Car";
        public string StartInspectionPickedUp => "Inspection of the Picked Up Car";

        public string HintPhotoItemsPage => "Please take a picture of the items being transported with the car";
        public string HintPhotoInspactionPage => "Try to photograph the item exactly on the layout or damage close";
        public string HintPhotoInTruckPage => "Try to photograph the item exactly on the layout";
        public string HintPhotoSeatBeltsPage => "Take a picture of one of the safety belt, but the same one that has already been photographed";

        public string TitleAskQuestionPage => "Customer Information";
        public string ItemNextPage => "Next";

        public string AskBlockWeatherTitle => "Weather conditions";
        public string ClearAnswer => "Clear";
        public string RainAnswer => "Rain";
        public string SnowAnswer => "Snow";
        public string DustAnswer => "Dust";

        public string AskBlockLightBrightnessTitle => "Light brightness";
        public string HighAnswer => "High";
        public string LowAnswer => "Low";

        public string AskBlockSafeLoctionTitle => "delivery location";

        public string AskBlockFarFromTrailerTitle => "How many ft far from trailer?";
        public string PlaceholderFarFromTrailerAnswer => "Please enter how far are you from the trailer";

        public string AskBlockGaveKeysTitle => "Enter the name of the person who gave you the keys to the car";
        public string PlaceholderGaveKeysAnswer => "Enter full name";

        public string AskBlockEnoughSpaceTitle => "Enough space to take pictures?";

        public string AskBlockAnyoneRushingTitle => "Anyone is rushing you to perform the inspection?";

        public string AskBlockNamePersonTitle => "Enter the name of the person";
        public string PlaceholderNamePersonAnswer => "Enter full name";

        public string AskBlockTypeCarTitle => "Type car";

        public string AskBlockPlateTitle => "Plate #";
        public string PlaceholderPlateAnswer => "Enter plate #";

        public string AskBlockConditionTitle => "Condition of the vehicle";
        public string CleanAnswer => "Clean";
        public string DirtyAnswer => "Dirty";
        public string WetAnswer => "Wet";
        public string Snow1Answer => "Snow";
        public string ExtraDirtyAnswer => "Extra dirty";

        public string AskBlockAdditionalItemsTitle => "Any personal or additional items with or in vehicle";

        public string HintAddDamagePage => "Click on the damaged spot to add a damage mark";
        public string BackMainBtnText => "Back";
        public string NextInspactionBtnText => "Next inspection";

        public string RetakeBtnText => "Retake";
        public string AddDamagBtnText => "Add Damag";
        public string NextBtnText => "Next";
        public string AddPhotoText => "Add Photo";

        public string AskBlockJumpedVehicleTitle => "Did you jumped the vehicle to start?";

        public string AskBlockMusteMileageTitle => "Exact mileage after loading (Must type miles)";
        public string PlaceholderMusteMileage => "Mileage";

        public string AskBlockImperfectionsWileLoadingTitle => "Did you notice any mechanical imperfections while loading?";
        public string PlaceholderMechanicalDefects => "Enter Mechanical defects";

        public string AskBlockMethodExitTitle => "What method of exit did you use?";
        public string DoorAnswer => "Door";
        public string WindowAnswer => "Window";
        public string SunroofAnswer => "Sunroof";
        public string ConvertibleAnswer => "Convertible";

        public string AskBlockHelpYouLoadTitle => "Did someone help you load it?";
        public string AskBlockLoadTheVehicleTitle => "Did someone load the vehicle for you?";
        public string PlaceholderName => "Enter name";

        public string AskBlockDamageAnythingTitle => "Did you damage anything at the pick up?";
        public string PlaceholderIfAny => "Enter damage, if any";

        public string AskBlockUsedWinchTitle => "Have you used winch?";

        public string TitleHelloCustomerPage => "Hello customer";
        public string ThankYouForUsingOurCompany => "Thank you for using our company.";
        public string ThankYouForUsingOurCompany1 => "Press the “Start” button to continue!";
        public string StartBtnText => "Start";

        public string AskBlockFullNameTitle => "Your Full Name";
        public string PlaceholderFullName => "Enter Full Name";

        public string AskBlockYourPhoneTitle => "Your phone";
        public string PlaceholderYourPhone => "Enter Your phone";

        public string ContinuetnBtnText => "Continue";

        public string TitleBillOfLandingPage => "BOLL";

        public string TitleOriginInfo => "Origin";
        public string TitleDestinatiinInfo => "Destination";
        public string TitleYourSignature => "Your signature";
        public string SaveBtnText => "Save";

        public string ThankYouInspactionText => "Thank you. Please pass the device to the driver.";

        public string AskBlockAccountPasswordTitle => "Enter your account password to continue";
        public string PlaceholderAccountPassword => "Enter password";

        public string AskBlockDriverPayTitle => "Did the driver pay for the carriage?";
        public string DescriptionDriverPayTitle => "In the near future the dispatcher see the problem";

        public string TypeInfo => "Type: ";
        public string ColorInfo => "Color: ";

        public string HintDamegePickedUp => "Circles Yellow — pickup damages;";
        public string HintDamegeDelivery => "Circles Green — delivery damages;";
        public string SeeInspactinPhoneBtnText => "Click to see inpection photo";

        public string AskBlockSendBOLTitle => "Send BOL on Email";
        public string SendBOLBtnText => "Send BOL";

        public string TitlePhotoInspactionPickedUp => "Photo inspection Picked Up";
        public string TitlePhotoInspactionDelivery => "Photo inspection Delivery";

        public string TitleAlertSendEmailBOL => "Do you need a copy of inspection?";

        public string DescriptionDiscount => "Would you like to get 10% off from next load just for giving us a good revue and filling out a short survey?";

        public string TitleFeedBackPage => "Feedback";

        public string AskBlockSatisfiedServiceTitle => "How are you satisfied with a service?";

        public string AskBlockUseCompanyAgainTitle => "Would you use our company again";
        public string MaybeBtnText => "Maybe";

        public string AskBlockPromotionTitle => "Would you like to get an notification if we have any promotion";

        public string AskBlockDriverPerformTitle => "How did the driver perform";

        public string AskBlockYourTitle => "Your Email";

        public string AskBlockManyKeysTotalTitle => "How many keys total you been given?";
        public string PlaceholderManyKeysTotal => "Enter the number of keys";

        public string AskBlockAdditionalDocumentationTitle => "Any additional documentation been given after loading";

        public string AskBlockAdditionalPartsTitle => "Any additional parts been given to you";

        public string AskBlockCarLokedTitle => "Car locked?";

        public string AskBlockKeysLocationTitle => "Keys location";
        public string TruckAnswer => "Truck";
        public string VehicleAnswer => "Vehicle";
        public string TrailerAnswer => "Trailer";

        public string AskBlockRateCustomerTitle => "Rate a customer";

        public string ComleteInspactinBtnText => "Complete inspection";

        public string TitleBlockInspaction => "Inspection Driver";
        public string TimeInspactionText => "Time to inspection: ";
        public string NeedInspectionText => "Need for inspection: ";
        public string HoursText => "Hours";
        public string CanPassText => "You can pass the inspection after";
        public string BestTimePassText => "best time to pass inspection";
        public string PassNowText => "Pass inspection now";

        public string TakePictureProblem => "Please take a picture of the problem areas of the vehicle";
        public string PictureOneSafetyBelt => "Take a picture of one of the safety belt, but the same one that has already been photographed";

        public string AskBlockSafeDeliveryLocationTitle => "delivery location";
        public string ParkingLotAnswer => "Parking lot";
        public string DrivewayAnswer => "Driveway";
        public string GravelAnswer => "Gravel";
        public string SidewalklAnswer => "Sidewalk";
        public string StreetAnswer => "Street";
        public string MiddleStreetAnswer => "Middle of the street";

        public string AskBlockTruckEmergencyBrakeTitle => "Truck on emergency brake?";

        public string AskBlockMeetClientTitle => "Did you meet the client?";

        public string AskBlockTruckLockedTitle => "Truck locked?";

        public string AskBlockPictureIdPersonTitle => "Please take a picture of the person's ID, who is receiving the delivery";

        public string AskBlockTrailerLockedTitle => "everything locks on the trailer? ";

        public string AskBlockAnyoneRushingPerformTitle => "Anyone rushing you to perform the delivery?";

        public string AskBlockWhileVehicleBeingTransportedTitle => "Did you notice any imperfections on body while vehicle being transported?";

        public string PlaceholderBodyFlaws => "Enter body flaws";

        public string AskBlockVehicleStartsTitle => "Does the vehicle starts?";
        public string AskBlockVehicleStarts1Title => "How did you start the car?";
        public string JumpAnswer => "Jump";
        public string CablesAnswer => "Cables";
        public string RolledOutAnswer => "Rolled out";

        public string AskBlockDoesVehicleDrivesTitle => "Does the vehicle drives?";

        public string AskBlockAnyoneHelpingUnloadTitle => "Anyone helping you to unload?";

        public string AskBlockSomeoneElseUnloadedVehicleTitle => "Did someone else unloaded the vehicle for you?";

        public string AskBlockVehicleParkedSafeLocationTitle => "Vehicle parked in the safe location?";

        public string AskBlockTimeOfDeliveryTitle => "Time of delivery";

        public string InfoKeysGiveDriver => "Please remove all your delivery items and keep keys to yourself";
        public string AskBlockDeliveryCustomerInspectCarTitle => "Ask the delivery customer to inspect the car? After inspecting the car, press the confirm button";
        public string IConfirmTheInspectionBtnText => "I confirm the inspection";

        public string HintAddDamageForUser => "Click on the damaged area to add damage, which would remove the touch of the damage (After adding the damage, the application will return to the camera)";

        public string AskBlockInspectedVehicleAdditionalImperfectionsTitle => "iu examined the vehicle for any additional deficiencies other than those listed upon receipt?";
        public string FoundIssueBtnText => "Found an issue";

        public string AskBlockBilingPayTitle => "Biling pay";
        public string AskBlockClientSignatureBlockTitle => "Your name and signature dear client";
        public string AskBlockClientNameTitle => "Customer name";
        public string AskBlockClientSignatureTitle => "Customer signature";

        public string AskBlockLikeRecive20fromYourNextCarTransportTitle => "Would you like to recive 20% off from your next car transport?";

        public string AskBlockRateDriverTitle => "Rate driver";

        public string VehicleInspectionPikedUp => "Vehicle inspection: Piked Up";
        public string VehicleInspectionDelivery => "Vehicle inspection: Delivery";
        public string ThereAreNoVehiclesInThisOrder => "There are no vehicles in this order";

        public string TitelSelectPickPhoto => "Choose an option";
        public string SelectPhoto => "Gallery";
        public string SelectGalery => "Camera";

        public string SelectBackToRootBage => "Return to order screen";
        public string SelectLoadGalery => "Upload photo to gallery";
        public string SelectLoadFolderOffline => "Upload photo to offline folder";
        public string SelectLoadFolderOfflineAndGalery => "Upload photo to offline folder and gallery";
    }
}