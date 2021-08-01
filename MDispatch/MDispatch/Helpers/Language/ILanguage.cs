using MDispatch.ViewModels.InspectionMV.Servise.Models;

namespace MDispatch.Helpers.Language
{
    public interface ILanguage
    {
        string WelcomeText { get; }
        string WelcomeDescriptionText { get; }
        string PlaceholderEmail { get; }
        string PlaceholderPassword { get; }
        string BtnLogInText { get; }
        string ForGotPasswordText { get; }

        string PasswordChangeRequestTitel { get; }
        string PlaceholderEmailChangeRequest { get; }
        string NotCorectEmail { get; }
        string PlaceholderNameChangeRequest { get; }
        string PasswordChangeRequestBtnText { get; }

        string SuccessfulPasswordChangeRequest { get; }

        string InspectionTodayAlert { get; }
        string AskErrorAlert { get; }
        string PassTheDeviceAlert { get; }
        string TechnicalWorkServiceAlert { get; }
        string GiveMoneyAlert { get; }
        string PaymentForDeliveryAlert { get; }
        string NotNetworkAlert { get; }
        string NoVehiclesAlert { get; }
        string NoAvtorisationAlert { get; }
        string NoDataAlert { get; }
        string AnswersSaved { get; }
        string VideoSavedSuccessfully { get; }
        string PaymentSaved { get; }
        string FutureDispatcherProblem { get; }
        string BOLIsSent { get; }
        string InformationDeliverySaved { get; }
        string InformationPaymentSaved { get; }
        string InformationPikedUpSaved { get; }
        string FeedbackSaved { get; }
        string PaymmantMethodSaved { get; }

        string ScanPlateNumber { get; }

        string TitleSetPlateTruckAlert { get; }
        string TitleSetPlateTrailerAlert { get; }
        string PlaceholderSetPlateTruckAlert { get; }
        string PlaceholderSetPlateTrailerkAlert { get; }
        string CancelBtnText { get; }
        string SendBtnText { get; }

        string TitleInfoPage { get; }
        string TitlePikedUpInfo { get; }
        string TitleVehicleInfo { get; }
        string AskBlockRateDriverTitle { get; }
        string TitleDeliveryInfo { get; }
        string TitlePaymentInfo { get; }
        string ContatInfo { get; }
        string PhoneInfo { get; }
        string PaymentInfo { get; }
        string Instructions { get; }
        string ReedInstructionsBtnText { get; }

        string TitleSettingsPage { get; }
        string DocumentInfo { get; }
        string ShowDocumentBtnText { get; }
        string LastInspactionInfo { get; }
        string TruckPlateInfo { get; }
        string TrailerPalateInfo { get; }
        string TitleDocumentsTrailerTruckNumber { get; }
        string NumberTruckPlateInfo { get; }
        string NumberTrailerPalateInfo { get; }
        string Application { get; }
        string CurrentVersion { get; }
        string LastUpdateAvailable { get; }
        string SignOutBtnText { get; }
        string LanguageText { get; }

        string NamePageTabActive { get; }
        string NamePageTabDelivery { get; }
        string NamePageTabArchived { get; }

        string TitleInspectionDriverAlert { get; }
        string YesBtnText { get; }
        string NoBtnText { get; }
        string TakeBtnText { get; }

        string ContinuingInspectionDelivery { get; }
        string ContinuingInspectionPickedUp { get; }
        string StartInspectionDelivery { get; }
        string StartInspectionPickedUp { get; }

        string HintPhotoItemsPage { get; }
        string HintPhotoInspactionPage { get; }
        string HintPhotoInTruckPage { get; }
        string HintPhotoSeatBeltsPage { get; }

        string TitleAskQuestionPage { get; }
        string ItemNextPage { get; }

        string AskBlockWeatherTitle { get; }
        string ClearAnswer { get; }
        string RainAnswer { get; }
        string SnowAnswer { get; }
        string DustAnswer { get; }

        string AskBlockLightBrightnessTitle { get; }
        string HighAnswer { get; }
        string LowAnswer { get; }

        string AskBlockSafeLoctionTitle { get; }

        string AskBlockFarFromTrailerTitle { get; }
        string PlaceholderFarFromTrailerAnswer { get; }

        string AskBlockGaveKeysTitle { get; }
        string PlaceholderGaveKeysAnswer { get; }

        string AskBlockEnoughSpaceTitle { get; }

        string AskBlockAnyoneRushingTitle { get; }

        string AskBlockNamePersonTitle { get; }
        string PlaceholderNamePersonAnswer { get; }

        string AskBlockTypeCarTitle { get; }

        string AskBlockPlateTitle { get; }
        string PlaceholderPlateAnswer { get; }

        string AskBlockConditionTitle { get; }
        string CleanAnswer { get; }
        string DirtyAnswer { get; }
        string WetAnswer { get; }
        string Snow1Answer { get; }
        string ExtraDirtyAnswer { get; }

        string AskBlockAdditionalItemsTitle { get; }

        string HintAddDamagePage { get; }
        string BackMainBtnText { get; }
        string NextInspactionBtnText { get; }

        string RetakeBtnText { get; }
        string AddDamagBtnText { get; }
        string NextBtnText { get; }
        string AddPhotoText { get; }

        string AskBlockJumpedVehicleTitle { get; }

        string AskBlockMusteMileageTitle { get; }
        string PlaceholderMusteMileage { get; }

        string AskBlockImperfectionsWileLoadingTitle { get; }
        string PlaceholderMechanicalDefects { get; }

        string AskBlockMethodExitTitle { get; }
        string DoorAnswer { get; }
        string WindowAnswer { get; }
        string SunroofAnswer { get; }
        string ConvertibleAnswer { get; }

        string AskBlockHelpYouLoadTitle { get; }
        string AskBlockLoadTheVehicleTitle { get; }
        string PlaceholderName { get; }

        string AskBlockDamageAnythingTitle { get; }
        string PlaceholderIfAny { get; }

        string AskBlockUsedWinchTitle { get; }

        string TitleHelloCustomerPage { get; }
        string ThankYouForUsingOurCompany { get; }
        string ThankYouForUsingOurCompany1 { get; }
        string StartBtnText { get; }

        string AskBlockFullNameTitle { get; }
        string PlaceholderFullName { get; }

        string AskBlockYourPhoneTitle { get; }
        string PlaceholderYourPhone { get; }

        string ContinuetnBtnText { get; }

        string TitleBillOfLandingPage { get; }

        string TitleOriginInfo { get; }
        string TitleDestinatiinInfo { get; }
        string TitleYourSignature { get; }

        string SaveBtnText { get; }

        string ThankYouInspactionText { get; }

        string AskBlockAccountPasswordTitle { get; }
        string PlaceholderAccountPassword { get; }

        string AskBlockDriverPayTitle { get; }
        string DescriptionDriverPayTitle { get; }

        string TypeInfo { get; }
        string ColorInfo { get; }
        string HintDamegePickedUp { get; }
        string HintDamegeDelivery { get; }
        string SeeInspactinPhoneBtnText { get; }

        string AskBlockSendBOLTitle { get; }
        string SendBOLBtnText { get; }

        string TitlePhotoInspactionPickedUp { get; }
        string TitlePhotoInspactionDelivery { get; }

        string TitleAlertSendEmailBOL { get; }

        string DescriptionDiscount { get; }

        string TitleFeedBackPage { get; }

        string AskBlockSatisfiedServiceTitle { get; }

        string AskBlockUseCompanyAgainTitle { get; }
        string MaybeBtnText { get; }

        string AskBlockPromotionTitle { get; }

        string AskBlockDriverPerformTitle { get; }

        string AskBlockYourTitle { get; }

        string AskBlockManyKeysTotalTitle { get; }
        string PlaceholderManyKeysTotal { get; }

        string AskBlockAdditionalDocumentationTitle { get; }

        string AskBlockAdditionalPartsTitle { get; }

        string AskBlockCarLokedTitle { get; }

        string AskBlockKeysLocationTitle { get; }
        string TruckAnswer { get; }
        string VehicleAnswer { get; }
        string TrailerAnswer { get; }

        string AskBlockRateCustomerTitle { get; }

        string ComleteInspactinBtnText { get; }

        string TitleBlockInspaction { get; }
        string TimeInspactionText { get; }
        string NeedInspectionText { get; }
        string HoursText { get; }
        string CanPassText { get; }
        string BestTimePassText { get; }
        string PassNowText { get; }

        string TakePictureProblem { get; }
        string PictureOneSafetyBelt { get; }

        string AskBlockSafeDeliveryLocationTitle { get; }
        string ParkingLotAnswer { get; }
        string DrivewayAnswer { get; }
        string GravelAnswer { get; }
        string SidewalklAnswer { get; }
        string StreetAnswer { get; }
        string MiddleStreetAnswer { get; }

        string AskBlockTruckEmergencyBrakeTitle { get; }

        string AskBlockMeetClientTitle { get; }

        string AskBlockTruckLockedTitle { get; }

        string AskBlockPictureIdPersonTitle { get; }

        string AskBlockTrailerLockedTitle { get; }

        string AskBlockAnyoneRushingPerformTitle { get; }

        string AskBlockWhileVehicleBeingTransportedTitle { get; }

        string PlaceholderBodyFlaws { get; }

        string AskBlockVehicleStartsTitle { get; }
        string AskBlockVehicleStarts1Title { get; }
        string JumpAnswer { get; }
        string CablesAnswer { get; }
        string RolledOutAnswer { get; }

        string AskBlockDoesVehicleDrivesTitle { get; }

        string AskBlockAnyoneHelpingUnloadTitle { get; }

        string AskBlockSomeoneElseUnloadedVehicleTitle { get; }

        string AskBlockVehicleParkedSafeLocationTitle { get; }

        string AskBlockTimeOfDeliveryTitle { get; }

        string InfoKeysGiveDriver { get; }
        string AskBlockDeliveryCustomerInspectCarTitle { get; }
        string IConfirmTheInspectionBtnText { get; }

        string HintAddDamageForUser { get; }

        string AskBlockInspectedVehicleAdditionalImperfectionsTitle { get; }
        string FoundIssueBtnText { get; }

        string AskBlockBilingPayTitle { get; }
        string AskBlockClientSignatureBlockTitle { get; }
        string AskBlockClientNameTitle { get; }
        string AskBlockClientSignatureTitle { get; }

        string AskBlockLikeRecive20fromYourNextCarTransportTitle { get; }

        string VehicleInspectionPikedUp { get; }
        string VehicleInspectionDelivery { get; }
        string ThereAreNoVehiclesInThisOrder { get; }

    }
}
