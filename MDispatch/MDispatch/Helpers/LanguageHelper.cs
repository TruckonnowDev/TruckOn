using MDispatch.Helpers.Language;
using MDispatch.Models.Enum;
using Plugin.Settings;

namespace MDispatch.Helpers
{
    public static class LanguageHelper
    {
        private static ILanguage Language { get; set; }

        public static string WelcomeText => Language.WelcomeText;
        public static string WelcomeDescriptionText => Language.WelcomeDescriptionText;
        public static string PlaceholderEmail => Language.PlaceholderEmail;
        public static string PlaceholderPassword => Language.PlaceholderPassword;
        public static string BtnLogInText => Language.BtnLogInText;
        public static string ForGotPasswordText => Language.ForGotPasswordText;
        public static string WithoutTranslationPlaceholderEmail => WithoutTranslationLanguage.PlaceholderEmail;

        public static string PasswordChangeRequestTitel => Language.PasswordChangeRequestTitel;
        public static string PlaceholderEmailChangeRequest => Language.PlaceholderEmailChangeRequest;
        public static string NotCorectEmail => Language.NotCorectEmail;
        public static string PlaceholderNameChangeRequest => Language.PlaceholderNameChangeRequest;
        public static string PasswordChangeRequestBtnText => Language.PasswordChangeRequestBtnText;

        public static string SuccessfulPasswordChangeRequest => Language.SuccessfulPasswordChangeRequest;

        public static string InspectionTodayAlert => Language.InspectionTodayAlert;
        public static string AskErrorAlert => Language.AskErrorAlert;
        public static string PassTheDeviceAlert => WithoutTranslationLanguage.PassTheDeviceAlert;
        public static string TechnicalWorkServiceAlert => Language.TechnicalWorkServiceAlert;
        public static string GiveMoneyAlert => Language.GiveMoneyAlert;
        public static string PaymentForDeliveryAlert => Language.PaymentForDeliveryAlert;
        public static string NotNetworkAlert => Language.NotNetworkAlert;
        public static string NoVehiclesAlert => Language.NoVehiclesAlert;
        public static string NoAvtorisationAlert => Language.NoAvtorisationAlert;
        public static string NoDataAlert => Language.NoDataAlert;
        public static string AnswersSaved => Language.AnswersSaved;
        public static string VideoSavedSuccessfully => Language.VideoSavedSuccessfully;
        public static string PaymentSaved => Language.PaymentSaved;
        public static string FutureDispatcherProblem => Language.FutureDispatcherProblem;
        public static string BOLIsSent => Language.BOLIsSent;
        public static string InformationDeliverySaved => Language.InformationDeliverySaved;
        public static string InformationPaymentSaved => Language.InformationPaymentSaved;
        public static string InformationPikedUpSaved => Language.InformationPikedUpSaved;
        public static string FeedbackSaved => Language.FeedbackSaved;
        public static string PaymmantMethodSaved => Language.PaymmantMethodSaved;
        public static string WithoutTranslationAskErrorAlert => WithoutTranslationLanguage.AskErrorAlert;

        public static string ScanPlateNumber => Language.ScanPlateNumber;
        public static string TitleSetPlateTruckAlert => Language.TitleSetPlateTruckAlert;
        public static string TitleSetPlateTrailerAlert => Language.TitleSetPlateTrailerAlert;
        public static string PlaceholderSetPlateTruckAlert => Language.PlaceholderSetPlateTruckAlert;
        public static string PlaceholderSetPlateTrailerkAlert => Language.PlaceholderSetPlateTrailerkAlert;
        public static string CancelBtnText => Language.CancelBtnText;
        public static string SendBtnText => Language.SendBtnText;

        public static string TitleInfoPage => Language.TitleInfoPage;
        public static string TitleVehicleInfo => Language.TitleVehicleInfo;
        public static string TitlePikedUpInfo => Language.TitlePikedUpInfo;
        public static string TitleDeliveryInfo => Language.TitleDeliveryInfo;
        public static string TitlePaymentInfo => Language.TitlePaymentInfo;
        public static string ContatInfo => Language.ContatInfo;
        public static string PhoneInfo => Language.PhoneInfo;
        public static string PaymentInfo => Language.PaymentInfo;
        public static string Instructions => Language.Instructions;
        public static string ReedInstructionsBtnText => Language.ReedInstructionsBtnText;
        public static string WithoutTranslationContatInfo => WithoutTranslationLanguage.ContatInfo;
        public static string WithoutTranslationPhoneInfo => WithoutTranslationLanguage.PhoneInfo;
        public static string WithoutTranslationPaymentInfo => WithoutTranslationLanguage.PaymentInfo;

        public static string TitleSettingsPage => Language.TitleSettingsPage;
        public static string DocumentInfo => Language.DocumentInfo;
        public static string ShowDocumentBtnText => Language.ShowDocumentBtnText;
        public static string LastInspactionInfo => Language.LastInspactionInfo;
        public static string TruckPlateInfo => Language.TruckPlateInfo;
        public static string TrailerPalateInfo => Language.TrailerPalateInfo;
        public static string TitleDocumentsTrailerTruckNumber => Language.TitleDocumentsTrailerTruckNumber;
        public static string NumberTruckPlateInfo => Language.NumberTruckPlateInfo;
        public static string NumberTrailerPalateInfo => Language.NumberTrailerPalateInfo;
        public static string Application => Language.Application;
        public static string CurrentVersion => Language.CurrentVersion;
        public static string LastUpdateAvailable => Language.LastUpdateAvailable;
        public static string SignOutBtnText => Language.SignOutBtnText;
        public static string LanguageText => Language.LanguageText;

        public static string NamePageTabActive => Language.NamePageTabActive;
        public static string NamePageTabDelivery => Language.NamePageTabDelivery;
        public static string NamePageTabArchived => Language.NamePageTabArchived;

        public static string TitleInspectionDriverAlert => Language.TitleInspectionDriverAlert;
        public static string YesBtnText => Language.YesBtnText;
        public static string NoBtnText => Language.NoBtnText;
        public static string TakeBtnText => Language.TakeBtnText;
        public static string WithoutTranslationYesBtnText => WithoutTranslationLanguage.YesBtnText;
        public static string WithoutTranslationNoBtnText => WithoutTranslationLanguage.NoBtnText;
        public static string WithoutTranslationContinuingBtnText => WithoutTranslationLanguage.ContinuingBtnText;

        public static string ContinuingInspectionDelivery => Language.ContinuingInspectionDelivery;
        public static string ContinuingInspectionPickedUp => Language.ContinuingInspectionPickedUp;
        public static string StartInspectionDelivery => Language.StartInspectionDelivery;
        public static string StartInspectionPickedUp => Language.StartInspectionPickedUp;

        public static string HintPhotoItemsPage => Language.HintPhotoItemsPage;
        public static string HintPhotoInspactionPage => Language.HintPhotoInspactionPage;
        public static string HintPhotoInTruckPage => Language.HintPhotoInTruckPage;
        public static string HintPhotoSeatBeltsPage => Language.HintPhotoSeatBeltsPage;

        public static string TitleAskQuestionPage => Language.TitleAskQuestionPage;
        public static string WithoutTranslationTitleAskQuestionPage => WithoutTranslationLanguage.TitleAskQuestionPage;
        public static string ItemNextPage => Language.ItemNextPage;
        public static string WithoutTranslationAskBlockFullNameTitle => WithoutTranslationLanguage.AskBlockFullNameTitle;

        public static string AskBlockWeatherTitle => Language.AskBlockWeatherTitle;
        public static string ClearAnswer => Language.ClearAnswer;
        public static string RainAnswer => Language.RainAnswer;
        public static string SnowAnswer => Language.SnowAnswer;
        public static string DustAnswer => Language.DustAnswer;

        public static string AskBlockLightBrightnessTitle => Language.AskBlockLightBrightnessTitle;
        public static string HighAnswer => Language.HighAnswer;
        public static string LowAnswer => Language.LowAnswer;

        public static string AskBlockSafeLoctionTitle => Language.AskBlockSafeLoctionTitle;

        public static string AskBlockFarFromTrailerTitle => Language.AskBlockFarFromTrailerTitle;
        public static string PlaceholderFarFromTrailerAnswer => Language.PlaceholderFarFromTrailerAnswer;

        public static string AskBlockGaveKeysTitle => Language.AskBlockGaveKeysTitle;
        public static string PlaceholderGaveKeysAnswer => Language.PlaceholderGaveKeysAnswer;

        public static string AskBlockEnoughSpaceTitle => Language.AskBlockEnoughSpaceTitle;

        public static string AskBlockAnyoneRushingTitle => Language.AskBlockAnyoneRushingTitle;

        public static string AskBlockNamePersonTitle => Language.AskBlockNamePersonTitle;
        public static string PlaceholderNamePersonAnswer => Language.PlaceholderNamePersonAnswer;

        public static string AskBlockTypeCarTitle => Language.AskBlockTypeCarTitle;

        public static string AskBlockPlateTitle => Language.AskBlockPlateTitle;
        public static string PlaceholderPlateAnswer => Language.PlaceholderPlateAnswer;

        public static string AskBlockConditionTitle => Language.AskBlockConditionTitle;
        public static string CleanAnswer => Language.CleanAnswer;
        public static string DirtyAnswer => Language.DirtyAnswer;
        public static string Snow1Answer => Language.Snow1Answer;
        public static string WetAnswer => Language.WetAnswer;
        public static string ExtraDirtyAnswer => Language.ExtraDirtyAnswer;

        public static string AskBlockAdditionalItemsTitle => Language.AskBlockAdditionalItemsTitle;

        public static string HintAddDamagePage => Language.HintAddDamagePage;
        public static string NextInspactionBtnText => Language.NextInspactionBtnText;
        public static string BackMainBtnText => Language.BackMainBtnText;

        public static string RetakeBtnText => Language.RetakeBtnText;
        public static string NextBtnText => Language.NextBtnText;
        public static string AddDamagBtnText => Language.AddDamagBtnText;
        public static string AddPhotoText => Language.AddPhotoText;

        public static string AskBlockJumpedVehicleTitle => Language.AskBlockJumpedVehicleTitle;

        public static string AskBlockMusteMileageTitle => Language.AskBlockMusteMileageTitle;
        public static string PlaceholderMusteMileage => Language.PlaceholderMusteMileage;

        public static string AskBlockImperfectionsWileLoadingTitle => Language.AskBlockImperfectionsWileLoadingTitle;
        public static string PlaceholderMechanicalDefects => Language.PlaceholderMechanicalDefects;

        public static string AskBlockMethodExitTitle => Language.AskBlockMethodExitTitle;
        public static string DoorAnswer => Language.DoorAnswer;
        public static string WindowAnswer => Language.WindowAnswer;
        public static string SunroofAnswer => Language.SunroofAnswer;
        public static string ConvertibleAnswer => Language.ConvertibleAnswer;

        public static string AskBlockHelpYouLoadTitle => Language.AskBlockHelpYouLoadTitle;
        public static string AskBlockLoadTheVehicleTitle => Language.AskBlockLoadTheVehicleTitle;
        public static string PlaceholderName => Language.PlaceholderName;
        public static string WithoutTranslationPlaceholderName => WithoutTranslationLanguage.PlaceholderName;

        public static string AskBlockDamageAnythingTitle => Language.AskBlockDamageAnythingTitle;
        public static string PlaceholderIfAny => Language.PlaceholderIfAny;

        public static string AskBlockUsedWinchTitle => Language.AskBlockUsedWinchTitle;

        public static string TitleHelloCustomerPage => WithoutTranslationLanguage.TitleHelloCustomerPage;
        public static string ThankYouForUsingOurCompany => WithoutTranslationLanguage.ThankYouForUsingOurCompany;
        public static string ThankYouForUsingOurCompany1 => WithoutTranslationLanguage.ThankYouForUsingOurCompany1;
        public static string StartBtnText => WithoutTranslationLanguage.StartBtnText;

        public static string AskBlockFullNameTitle => Language.AskBlockFullNameTitle;
        public static string PlaceholderFullName => Language.PlaceholderFullName;
        public static string WithoutTranslatioPlaceholderFullName => WithoutTranslationLanguage.PlaceholderFullName;

        public static string AskBlockYourPhoneTitle => Language.AskBlockYourPhoneTitle;
        public static string PlaceholderYourPhone => Language.PlaceholderYourPhone;
        public static string WithoutTranslatioAskBlockYourPhoneTitle => WithoutTranslationLanguage.AskBlockYourPhoneTitle;
        public static string WithoutTranslatioPlaceholderYourPhone => WithoutTranslationLanguage.PlaceholderYourPhone;

        public static string AskBlockManyKesTitle => WithoutTranslationLanguage.AskBlockManyKesTitle;
        public static string PlaceholderManyKes => WithoutTranslationLanguage.PlaceholderManyKes;

        public static string AskBlockGivenToDriverTitle => WithoutTranslationLanguage.AskBlockGivenToDriverTitle;
        public static string IDontKnowBtnText => WithoutTranslationLanguage.IDontKnowBtnText;

        public static string ContinuetnBtnText => Language.ContinuetnBtnText;

        public static string TitleBillOfLandingPage => Language.TitleBillOfLandingPage;
        public static string WithoutTranslatioTitleBillOfLandingPage => WithoutTranslationLanguage.TitleBillOfLandingPage;

        public static string TitleOriginInfo => Language.TitleOriginInfo;
        public static string TitleDestinatiinInfo => Language.TitleDestinatiinInfo;
        public static string TitleYourSignature => Language.TitleYourSignature;
        public static string SaveBtnText => Language.SaveBtnText;
        public static string WithoutTranslatioTitleOriginInfo => WithoutTranslationLanguage.TitleOriginInfo;
        public static string WithoutTranslatioTitleDestinatiinInfo => WithoutTranslationLanguage.TitleDestinatiinInfo;
        public static string WithoutTranslatioTitleYourSignature => WithoutTranslationLanguage.TitleYourSignature;
        public static string WithoutTranslatioSaveBtnText => WithoutTranslationLanguage.SaveBtnText;

        public static string ThankYouInspactionText => Language.ThankYouInspactionText;
        public static string WithoutTranslationThankYouInspactionText => WithoutTranslationLanguage.ThankYouInspactionText;

        public static string AskBlockAccountPasswordTitle => Language.AskBlockAccountPasswordTitle;
        public static string PlaceholderAccountPassword => Language.PlaceholderAccountPassword;

        public static string AskBlockDriverPayTitle => Language.AskBlockDriverPayTitle;
        public static string DescriptionDriverPayTitle => Language.DescriptionDriverPayTitle;

        public static string TypeInfo => Language.TypeInfo;
        public static string ColorInfo => Language.ColorInfo;
        public static string HintDamegePickedUp => Language.HintDamegePickedUp;
        public static string HintDamegeDelivery => Language.HintDamegeDelivery;
        public static string SeeInspactinPhoneBtnText => Language.SeeInspactinPhoneBtnText;
        public static string WithoutTranslationTypeInfo => WithoutTranslationLanguage.TypeInfo;
        public static string WithoutTranslationColorInfo => WithoutTranslationLanguage.ColorInfo;
        public static string WithoutTranslationHintDamegePickedUp => WithoutTranslationLanguage.HintDamegePickedUp;
        public static string WithoutTranslationHintDamegeDelivery => WithoutTranslationLanguage.HintDamegeDelivery;
        public static string WithoutTranslationSeeInspactinPhoneBtnText => WithoutTranslationLanguage.SeeInspactinPhoneBtnText;

        public static string AskBlockSendBOLTitle => Language.AskBlockSendBOLTitle;
        public static string WithoutTranslationAskBlockSendBOLTitle => WithoutTranslationLanguage.AskBlockSendBOLTitle;
        public static string SendBOLBtnText => Language.SendBOLBtnText;
        public static string WithoutTranslationSendBOLBtnText => WithoutTranslationLanguage.SendBOLBtnText;

        public static string TitlePhotoInspactionPickedUp => Language.TitlePhotoInspactionPickedUp;
        public static string WithoutTranslationTitlePhotoInspactionPickedUp => WithoutTranslationLanguage.TitlePhotoInspactionPickedUp;
        public static string TitlePhotoInspactionDelivery => Language.TitlePhotoInspactionDelivery;
        public static string WithoutTranslationTitlePhotoInspactionDelivery => WithoutTranslationLanguage.TitlePhotoInspactionDelivery;

        public static string TitleAlertSendEmailBOL => Language.TitleAlertSendEmailBOL;
        public static string WithoutTranslationTitleAlertSendEmailBOL => WithoutTranslationLanguage.TitleAlertSendEmailBOL;

        public static string DescriptionDiscount => Language.DescriptionDiscount;
        public static string WithoutTranslationDescriptionDiscount => WithoutTranslationLanguage.DescriptionDiscount;

        public static string TitleFeedBackPage => Language.TitleFeedBackPage;
        public static string WithoutTranslationTitleFeedBackPage => WithoutTranslationLanguage.TitleFeedBackPage;

        public static string AskBlockSatisfiedServiceTitle => Language.AskBlockSatisfiedServiceTitle;
        public static string WithoutTranslationAskBlockSatisfiedServiceTitle => WithoutTranslationLanguage.AskBlockSatisfiedServiceTitle;

        public static string AskBlockUseCompanyAgainTitle => Language.AskBlockUseCompanyAgainTitle;
        public static string WithoutTranslationAskBlockUseCompanyAgainTitle => WithoutTranslationLanguage.AskBlockUseCompanyAgainTitle;
        public static string MaybeBtnText => Language.MaybeBtnText;
        public static string WithoutTranslationMaybeBtnText => WithoutTranslationLanguage.MaybeBtnText;

        public static string AskBlockPromotionTitle => Language.AskBlockPromotionTitle;

        public static string AskBlockDriverPerformTitle => Language.AskBlockDriverPerformTitle;
        public static string WithoutTranslationAskBlockDriverPerformTitle => WithoutTranslationLanguage.AskBlockDriverPerformTitle;

        public static string AskBlockYourTitle => Language.AskBlockYourTitle;
        public static string WithoutTranslationAskBlockYourTitle => WithoutTranslationLanguage.AskBlockYourTitle;

        public static string AskBlockManyKeysTotalTitle => Language.AskBlockManyKeysTotalTitle;
        public static string PlaceholderManyKeysTotal => Language.PlaceholderManyKeysTotal;

        public static string AskBlockAdditionalDocumentationTitle => Language.AskBlockAdditionalDocumentationTitle;

        public static string AskBlockAdditionalPartsTitle => Language.AskBlockAdditionalPartsTitle;

        public static string AskBlockCarLokedTitle => Language.AskBlockCarLokedTitle;

        public static string AskBlockKeysLocationTitle => Language.AskBlockKeysLocationTitle;
        public static string TruckAnswer => Language.TruckAnswer;
        public static string TrailerAnswer => Language.TrailerAnswer;
        public static string VehicleAnswer => Language.VehicleAnswer;

        public static string AskBlockRateCustomerTitle => Language.AskBlockRateCustomerTitle;

        public static string ComleteInspactinBtnText => Language.ComleteInspactinBtnText;

        public static string TitleBlockInspaction => Language.TitleBlockInspaction;
        public static string TimeInspactionText => Language.TimeInspactionText;
        public static string NeedInspectionText => Language.NeedInspectionText;
        public static string HoursText => Language.HoursText;
        public static string CanPassText => Language.CanPassText;
        public static string BestTimePassText => Language.BestTimePassText;
        public static string PassNowText => Language.PassNowText;

        public static string TakePictureProblem => Language.TakePictureProblem;
        public static string PictureOneSafetyBelt => Language.PictureOneSafetyBelt;

        public static string AskBlockSafeDeliveryLocationTitle => Language.AskBlockSafeDeliveryLocationTitle;
        public static string ParkingLotAnswer => Language.ParkingLotAnswer;
        public static string DrivewayAnswer => Language.DrivewayAnswer;
        public static string GravelAnswer => Language.GravelAnswer;
        public static string SidewalklAnswer => Language.SidewalklAnswer;
        public static string StreetAnswer => Language.StreetAnswer;
        public static string MiddleStreetAnswer => Language.MiddleStreetAnswer;

        public static string AskBlockTruckEmergencyBrakeTitle => Language.AskBlockTruckEmergencyBrakeTitle;

        public static string AskBlockMeetClientTitle => Language.AskBlockMeetClientTitle;

        public static string AskBlockTruckLockedTitle => Language.AskBlockTruckLockedTitle;

        public static string AskBlockPictureIdPersonTitle => Language.AskBlockPictureIdPersonTitle;

        public static string AskBlockTrailerLockedTitle => Language.AskBlockTrailerLockedTitle;

        public static string AskBlockAnyoneRushingPerformTitle => Language.AskBlockAnyoneRushingPerformTitle;

        public static string AskBlockWhileVehicleBeingTransportedTitle => Language.AskBlockWhileVehicleBeingTransportedTitle;

        public static string PlaceholderBodyFlaws => Language.PlaceholderBodyFlaws;

        public static string AskBlockVehicleStartsTitle => Language.AskBlockVehicleStartsTitle;
        public static string AskBlockVehicleStarts1Title => Language.AskBlockVehicleStarts1Title;
        public static string JumpAnswer => Language.JumpAnswer;
        public static string CablesAnswer => Language.CablesAnswer;
        public static string RolledOutAnswer => Language.RolledOutAnswer;

        public static string AskBlockDoesVehicleDrivesTitle => Language.AskBlockDoesVehicleDrivesTitle;

        public static string AskBlockVehicleParkedSafeLocationTitle => Language.AskBlockVehicleParkedSafeLocationTitle;

        public static string AskBlockAnyoneHelpingUnloadTitle => Language.AskBlockAnyoneHelpingUnloadTitle;

        public static string AskBlockSomeoneElseUnloadedVehicleTitle => Language.AskBlockSomeoneElseUnloadedVehicleTitle;

        public static string AskBlockTimeOfDeliveryTitle => Language.AskBlockTimeOfDeliveryTitle;

        public static string InfoKeysGiveDriver => Language.InfoKeysGiveDriver;
        public static string AskBlockDeliveryCustomerInspectCarTitle => Language.AskBlockDeliveryCustomerInspectCarTitle;
        public static string IConfirmTheInspectionBtnText => Language.IConfirmTheInspectionBtnText;

        public static string HintAddDamageForUser => Language.HintAddDamageForUser;
        public static string WithoutTranslationHintAddDamageForUser => WithoutTranslationLanguage.HintAddDamageForUser;

        public static string AskBlockInspectedVehicleAdditionalImperfectionsTitle => Language.AskBlockInspectedVehicleAdditionalImperfectionsTitle;
        public static string FoundIssueBtnText => Language.FoundIssueBtnText;
        public static string WithoutTranslationFoundIssueBtnText => WithoutTranslationLanguage.FoundIssueBtnText;
        public static string WithoutTranslationAskBlockInspectedVehicleAdditionalImperfectionsTitle => WithoutTranslationLanguage.AskBlockInspectedVehicleAdditionalImperfectionsTitle;

        public static string AskBlockBilingPayTitle => Language.AskBlockBilingPayTitle;
        public static string WithoutTranslationAskBlockBilingPayTitle => WithoutTranslationLanguage.AskBlockBilingPayTitle;
        public static string AskBlockClientSignatureBlockTitle => Language.AskBlockClientSignatureBlockTitle;
        public static string WithoutTranslationAskBlockClientSignatureBlockTitle => WithoutTranslationLanguage.AskBlockClientSignatureBlockTitle;
        public static string AskBlockClientNameTitle => Language.AskBlockClientNameTitle;
        public static string WithoutTranslationAskBlockClientNameTitle => WithoutTranslationLanguage.AskBlockClientNameTitle;
        public static string AskBlockClientSignatureTitle => Language.AskBlockClientSignatureTitle;
        public static string WithoutTranslationAskBlockClientSignatureTitle => WithoutTranslationLanguage.AskBlockClientSignatureTitle;

        public static string AskBlockLikeRecive20fromYourNextCarTransportTitle => Language.AskBlockLikeRecive20fromYourNextCarTransportTitle;
        public static string WithoutTranslationAskBlockLikeRecive20fromYourNextCarTransportTitle => WithoutTranslationLanguage.AskBlockLikeRecive20fromYourNextCarTransportTitle;

        public static string AskBlockRateDriverTitle => Language.AskBlockRateDriverTitle;
        public static string WithoutTranslationAskBlockRateDriverTitle => WithoutTranslationLanguage.AskBlockRateDriverTitle;

        public static string VehicleInspectionPikedUp => Language.VehicleInspectionPikedUp;
        public static string VehicleInspectionDelivery => Language.VehicleInspectionDelivery;
        public static string ThereAreNoVehiclesInThisOrder => Language.ThereAreNoVehiclesInThisOrder;

        public static string TitelSelectPickPhoto => Language.TitelSelectPickPhoto;
        public static string SelectPhoto => Language.SelectPhoto;
        public static string SelectGalery => Language.SelectGalery;
        
        public static string SelectBackToRootBage => Language.SelectBackToRootBage;
        public static string SelectLoadGalery => Language.SelectLoadGalery;
        public static string SelectLoadFolderOffline => Language.SelectLoadFolderOffline;
        public static string SelectLoadFolderOfflineAndGalery => Language.SelectLoadFolderOfflineAndGalery;

        public static void InitLanguage()
        {
            switch(CrossSettings.Current.GetValueOrDefault("Language" , (int)LanguageType.Russian))
            {
                case (int)LanguageType.English:
                    {
                        Language = new EnglishLanguage();
                        break;
                    }
                case (int)LanguageType.Russian:
                    {
                        Language = new RussianLanguage();
                        break;
                    }
                case (int)LanguageType.Spanish:
                    {
                        Language = new SpanishLanguage();
                        break;
                    }
            }
        }
    }
}
