﻿namespace MDispatch.Helpers.Language
{
    public class EnglishLanguage : ILanguage
    {
        public string WelcomeText => "Welcome Back";
        public string WelcomeDescriptionText => "Log in to continue";
        public string PlaceholderEmail => "Email";
        public string PlaceholderPassword => "Password";
        public string BtnLogInText => "Log in";
        public string ForGotPasswordText => "Forgot password?";

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
        public string TitleSetPlateTruckAlert => "Please write the number plate of the truck";
        public string TitleSetPlateTrailerAlert => "Please write the number plate of the trailer";
        public string PlaceholderSetPlateTruckAlert => "Enter № plate truck";
        public string PlaceholderSetPlateTrailerkAlert => "Enter № plate trailer";
        public string CancelBtnText => "Cancel";
        public string SendBtnText => "Send";

        public string TitleInfoPage => "Info";
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

        public string NamePageTabActive => "Active";
        public string NamePageTabDelivery => "Delivery";
        public string NamePageTabArchived => "Archived";

        public string TitleInspectionDriverAlert => "To take an order, you need to pass the inspection of the truck and the trailer, pass the inspection of the truck?";
        public string YesBtnText => "Yes";
        public string NoBtnText => "No";

        public string ContinuingInspectionDelivery => "Continuing inspection Delivery";
        public string ContinuingInspectionPickedUp => "Continuing inspection Picked up";
        public string StartInspectionDelivery => "Start Inspection Delivery";
        public string StartInspectionPickedUp => "Start Inspection Picked up";

        public string HintPhotoItemsPage => "Please take a picture of the items being transported with the car";
        public string HintPhotoInspactionPage => "Try to photograph the item exactly on the layout or damage close";
        public string HintPhotoInTruckPage => "Try to photograph the item exactly on the layout";
        public string HintPhotoSeatBeltsPage => "Take a picture of one of the safety belt, but the same one that has already been photographed";

        public string TitleAskQuestionPage => "Question inspection";
        public string ItemNextPage => "Next";

        public string AskBlockWeatherTitle => "Weather conditions";
        public string ClearAnswer => "Clear";
        public string RainAnswer => "Rain";
        public string SnowAnswer => "Snow";
        public string DustAnswer => "Dust";

        public string AskBlockLightBrightnessTitle => "Light brightness";
        public string HighAnswer => "High";
        public string LowAnswer => "Low";

        public string AskBlockSafeLoctionTitle => "Safe delivery location";

        public string AskBlockFarFromTrailerTitle => "How far from trailer?";
        public string PlaceholderFarFromTrailerAnswer => "Please enter how far are you from the trailer";

        public string AskBlockGaveKeysTitle => "Enter the name of the person who gave you the keys to the car";
        public string PlaceholderGaveKeysAnswer => "Enter full name";

        public string AskBlockEnoughSpaceTitle => "Enough space to take pictures?";

        public string AskBlockAnyoneRushingTitle => "Anyone Rushing you to perform the inspection?";

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

        public string HintAddDamagePage => "Tap on the place with the damage to add damage, that would remove touch on the damage";
        public string BackMainBtnText => "Back Main";
        public string NextInspactionBtnText => "Next inspection";

        public string RetakeBtnText => "Retake";
        public string AddDamagBtnText => "Add Damag";
        public string NextBtnText => "Next";
        public string AddPhotoText => "Add Photo";

        public string AskBlockJumpedVehicleTitle => "Did you jumped the vehicle to start?";

        public string AskBlockMusteMileageTitle => "Exact mileage after loading (Must type miles)";
        public string PlaceholderMusteMileage => "Mileage";

        public string AskBlockImperfectionsWileLoadingTitle => "Did you notice any mechanical imperfections wile loading?";
        public string PlaceholderMechanicalDefects => "Enter Mechanical defects";

        public string AskBlockMethodExitTitle => "What method of exit did you use";
        public string DoorAnswer => "Door";
        public string WindowAnswer => "Window";
        public string SunroofAnswer => "Sunroof";
        public string ConvertibleAnswer => "Convertible";

        public string AskBlockHelpYouLoadTitle => "Did someone help you load it";
        public string AskBlockLoadTheVehicleTitle => "Did someone load the vehicle for you?";
        public string PlaceholderName => "Enter name";

        public string AskBlockDamageAnythingTitle => "Did you Damage anything at the pick up";
        public string PlaceholderIfAny => "Enter damage, if any";

        public string AskBlockUsedWinchTitle => "Have you used winch";

        public string TitleHelloCustomerPage => "Hello customer";
        public string ThankYouForUsingOurCompany => "Thank you for using our company.";
        public string ThankYouForUsingOurCompany1 => "Press the “Start” button to continue!";
        public string StartBtnText => "Start";

        public string AskBlockFullNameTitle => "Your Full Name";
        public string PlaceholderFullName => "Enter Full Name";

        public string AskBlockYourPhoneTitle => "Your phone";
        public string PlaceholderYourPhone => "Enter Your phone";

        public string AskBlockManyKesTitle => "How many keys are driver been given";
        public string PlaceholderManyKes => "Enter the number of keys you gave";

        public string AskBlockGivenToDriverTitle => "Any titles been given to driver?";
        public string IDontKnowBtnText => "I don't know";

        public string ContinuetnBtnText => "Continue";

        public string TitleBillOfLandingPage => "BILL OF LADING";

        public string TitleOriginInfo => "Origin";
        public string TitleDestinatiinInfo => "Destination";
        public string TitleYourSignature => "Your signature";
        public string SaveBtnText => "Save";
    }
}