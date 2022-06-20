using MDispatch.ViewModels.InspectionMV.Servise.Models;

namespace MDispatch.Helpers.Language
{
    public class RussianLanguage : ILanguage
    {
        public string WelcomeText => "Добро пожаловать";
        public string WelcomeDescriptionText => "Авторизуйтесь, чтобы продолжить";
        public string PlaceholderEmail => "Эл. адрес";
        public string PlaceholderPassword => "Пароль";
        public string BtnLogInText => "Авторизоваться";
        public string ForGotPasswordText => "Забыл пароль?";

        public string PasswordChangeRequestTitel => "Запрос на изменение пароля";
        public string PlaceholderEmailChangeRequest => "Введите сюда вашу почту";
        public string NotCorectEmail => "Введенный формат электронной почты неверен";
        public string PlaceholderNameChangeRequest => "Введите здесь свое полное имя";
        public string PasswordChangeRequestBtnText => "Запросить изменение пароля";

        public string SuccessfulPasswordChangeRequest => "Данные для сброса пароля, отправленные вам по почте:";

        public string InspectionTodayAlert => "Вы уже прошли осмотр сегодня";
        public string AskErrorAlert => "Вы не заполнили все обязательные поля, продолжить проверку можно только после заполнения обязательных полей !!";
        public string PassTheDeviceAlert => "Пожалуйста, передайте устройство клиенту";
        public string TechnicalWorkServiceAlert => "Технические работы по сервису";
        public string GiveMoneyAlert => "Отдать деньги за доставку водителю";
        public string PaymentForDeliveryAlert => "Необходимо ввести сумму оплаты за доставку";
        public string NotNetworkAlert => "Нет сети";
        public string NoVehiclesAlert => "В заказе нет транспортных средств.\n\nЧтобы пройти техосмотр, попросите диспетчера добавить автомобиль.";
        public string NoAvtorisationAlert => "Не авторизован";
        public string NoDataAlert => "Нет данных";
        public string AnswersSaved => "Ответы на вопросы сохранены";
        public string VideoSavedSuccessfully => "Видеозапись успешно сохранены";
        public string PaymentSaved => "Фотография способа оплаты сохранена";
        public string FutureDispatcherProblem => "В ближайшее время диспетчер увидит проблему";
        public string BOLIsSent => "Копия BOL отправляется на почту";
        public string InformationDeliverySaved => "Информация о доставке сохранена";
        public string InformationPaymentSaved => "Информация о платеже сохранена";
        public string InformationPikedUpSaved => "Информация о погрузке сохранена";
        public string FeedbackSaved => "Отзыв сохранен";
        public string PaymmantMethodSaved => "Способы оплаты сохранены";

        public string ScanPlateNumber => "Сканирование Номерной знак";
        public string TitleSetPlateTruckAlert => "Напишите, пожалуйста, номер грузовика";
        public string TitleSetPlateTrailerAlert => "Напишите, пожалуйста, номер прицепа";
        public string PlaceholderSetPlateTruckAlert => "Введите № грузовика";
        public string PlaceholderSetPlateTrailerkAlert => "Введите № прицепа";
        public string CancelBtnText => "Отмена";
        public string SendBtnText => "Отправить";

        public string TitleInfoPage => "Информация";
        public string TitleVehicleInfo => "Транспорное средство";
        public string TitlePikedUpInfo => "Информация о получении";
        public string TitleDeliveryInfo => "Информация о доставке";
        public string TitlePaymentInfo => "Платежная информация";
        public string ContatInfo => "Контакт: ";
        public string PhoneInfo => "Телефон: ";
        public string PaymentInfo => "Оплата: ";
        public string Instructions => "Инструкции";
        public string ReedInstructionsBtnText => "Я прочитал инструкцию";

        public string TitleSettingsPage => "Настройки";
        public string DocumentInfo => "Документы";
        public string ShowDocumentBtnText => "Показать документы";
        public string LastInspactionInfo => "Последний осмотр: ";
        public string TruckPlateInfo => "Номер грузовика: ";
        public string TrailerPalateInfo => "Номер прицепа: ";
        public string TitleDocumentsTrailerTruckNumber => "Документы по номеру прицепа и грузовика";
        public string NumberTruckPlateInfo => "№ грузовика:";
        public string NumberTrailerPalateInfo => "№ прицепа";
        public string Application => "Приложение";
        public string CurrentVersion => "Текущая версия: ";
        public string LastUpdateAvailable => "Доступно последнее обновление: ";
        public string SignOutBtnText => "Выйти";
        public string LanguageText => "Язык";

        public string NamePageTabActive => "Активные";
        public string NamePageTabDelivery => "Доставленые";
        public string NamePageTabArchived => "Архивированные";

        public string TitleInspectionDriverAlert => "Чтобы принять заказ, нужно пройти техосмотр грузового автомобиля и прицепа, пройти техосмотр грузового автомобиля?";
        public string YesBtnText => "Да";
        public string NoBtnText => "Нет";
        public string TakeBtnText => "Сфотографировать";

        public string ContinuingInspectionDelivery => "Продолжение осмотра при доставке";
        public string ContinuingInspectionPickedUp => "Постоянное осмотр Погрузки";
        public string StartInspectionDelivery => "Начать Инспекцию на Доставке";
        public string StartInspectionPickedUp => "Начать осмотр Полученния";

        public string HintPhotoItemsPage => "Пожалуйста, сфотографируйте предметы, которые перевозятся на машине.";
        public string HintPhotoInspactionPage => "Попробуйте сфотографировать деталь точно как на макете или повреждение близко";
        public string HintPhotoInTruckPage => "Попробуйте сфотографировать деталь точно на макете";
        public string HintPhotoSeatBeltsPage => "Сфотографируйте один из ремней безопасности, но тот, который уже был застегнут";

        public string TitleAskQuestionPage => "Вопросы осмотра";
        public string ItemNextPage => "Дальше";

        public string AskBlockWeatherTitle => "Погодные условия";
        public string ClearAnswer => "Солнечно";
        public string RainAnswer => "Дождь";
        public string SnowAnswer => "Снег";
        public string DustAnswer => "Пыль";

        public string AskBlockLightBrightnessTitle => "Яркость света";
        public string HighAnswer => "Высокая";
        public string LowAnswer => "Низкая";

        public string AskBlockSafeLoctionTitle => "Безопасное место доставки";

        public string AskBlockFarFromTrailerTitle => "Как далеко вы от трейлера?";
        public string PlaceholderFarFromTrailerAnswer => "Укажите в ft.";

        public string AskBlockGaveKeysTitle => "Пожалуйста, укажите кто отдал вам машину и ключи";
        public string PlaceholderGaveKeysAnswer => "Введите полное имя";

        public string AskBlockEnoughSpaceTitle => "Достаточно места для фотографий?";

        public string AskBlockAnyoneRushingTitle => "Кто-нибудь спешит провести осмотр?";

        public string AskBlockNamePersonTitle => "Введите имя человека";
        public string PlaceholderNamePersonAnswer => "Введите полное имя";

        public string AskBlockTypeCarTitle => "Тип автомобиля";

        public string AskBlockPlateTitle => "Номер транспортного средства";
        public string PlaceholderPlateAnswer => "Введите номер транспортного средства";

        public string AskBlockConditionTitle => "Состояние автомобиля";
        public string CleanAnswer => "Чистый";
        public string DirtyAnswer => "Грязный";
        public string WetAnswer => "Влажный";
        public string Snow1Answer => "Снег";
        public string ExtraDirtyAnswer => "Очень грязный";

        public string AskBlockAdditionalItemsTitle => "Любые личные или дополнительные предметы в автомобиле.";

        public string HintAddDamagePage => "Нажмите на поврежденное место, чтобы добавить отметку повреждения.";
        public string BackMainBtnText => "Назад на Главную";
        public string NextInspactionBtnText => "Следующий осмотр";

        public string RetakeBtnText => "Перефотографировать";
        public string AddDamagBtnText => "Добавить урон";
        public string NextBtnText => "Следующий";
        public string AddPhotoText => "Добавить фото";

        public string AskBlockJumpedVehicleTitle => "Вы использовали кабель чтобы запустить авто?";

        public string AskBlockMusteMileageTitle => "Точный пробег после загрузки (необходимо ввести мили)";
        public string PlaceholderMusteMileage => "Пробег";

        public string AskBlockImperfectionsWileLoadingTitle => "Вы заметили какие-либо механические дефекты при загрузке?";
        public string PlaceholderMechanicalDefects => "Введите механические дефекты";

        public string AskBlockMethodExitTitle => "Какой метод выхода вы использовали";
        public string DoorAnswer => "Дверь";
        public string WindowAnswer => "Окно";
        public string SunroofAnswer => "Люк на крыше";
        public string ConvertibleAnswer => "Кабриолет";

        public string AskBlockHelpYouLoadTitle => "Кто-то помог вам загрузить машину?";
        public string AskBlockLoadTheVehicleTitle => "Кто-то загрузил автомобиль за вас?";
        public string PlaceholderName => "Введите имя";

        public string AskBlockDamageAnythingTitle => "Вы что-нибудь повредили при погрузке?";
        public string PlaceholderIfAny => "Укажите повреждения, если таковые имеются";

        public string AskBlockUsedWinchTitle => "Вы использовали лебедку?";

        public string TitleHelloCustomerPage => "Привет клиент";
        public string ThankYouForUsingOurCompany => "Спасибо, что пользуетесь нашей компанией.";
        public string ThankYouForUsingOurCompany1 => "Чтобы продолжить, нажмите кнопку «Пуск»!";
        public string StartBtnText => "Пуск";

        public string AskBlockFullNameTitle => "Ваше полное имя";
        public string PlaceholderFullName => "Введите полное имя";

        public string AskBlockYourPhoneTitle => "Ваш телефон";
        public string PlaceholderYourPhone => "Введите свой телефон";

        public string AskBlockManyKesTitle => "Сколько ключей отдано водителю";
        public string PlaceholderManyKes => "Введите количество ключей, которое вы даете";

        public string AskBlockGivenToDriverTitle => "Какие-нибудь титулы были присвоены водителю?";
        public string IDontKnowBtnText => "Я не знаю";

        public string ContinuetnBtnText => "Продолжить";

        public string TitleBillOfLandingPage => "КОНОСАМЕНТ";

        public string TitleOriginInfo => "Источник";
        public string TitleDestinatiinInfo => "Назначения";
        public string TitleYourSignature => "Ваша подпись";
        public string SaveBtnText => "Сохранить";

        public string ThankYouInspactionText => "Спасибо. Пожалуйста, передайте устройство водителю.";

        public string AskBlockAccountPasswordTitle => "Введите пароль своей учетной записи, чтобы продолжить";
        public string PlaceholderAccountPassword => "Введите пароль";

        public string AskBlockDriverPayTitle => "Клиент оплатил перевозку?";
        public string DescriptionDriverPayTitle => "В ближайшее время диспетчер увидит проблему";

        public string TypeInfo => "Тип: ";
        public string ColorInfo => "Цвет: ";

        public string HintDamegePickedUp => "Желтые кружки - повреждения на погрузке;";
        public string HintDamegeDelivery => "Круги зеленые - повреждения при доставке;";
        public string SeeInspactinPhoneBtnText => "Нажмите, чтобы увидеть фото инспекции";

        public string AskBlockSendBOLTitle => "Отправить BOL по электронной почте";
        public string SendBOLBtnText => "Отправить BOL";

        public string TitlePhotoInspactionPickedUp => "Фото осмотр Погрузки";
        public string TitlePhotoInspactionDelivery => "Фото осмотр Доставка";

        public string TitleAlertSendEmailBOL => "Вам нужна копия проверки?";

        public string DescriptionDiscount => "Хотели бы вы получить 10% скидки на следующую загрузку только за то, что дали нам хороший обзор и заполнили небольшой опрос?";

        public string TitleFeedBackPage => "Отзыв";

        public string AskBlockSatisfiedServiceTitle => "Насколько вы удовлетворены обслуживанием?";

        public string AskBlockUseCompanyAgainTitle => "Вы бы снова использовали нашу компанию?";
        public string MaybeBtnText => "Может быть";

        public string AskBlockPromotionTitle => "Хотели бы вы получать уведомление, если у нас будет какая-либо акция?";

        public string AskBlockDriverPerformTitle => "Как вел себя водитель?";

        public string AskBlockYourTitle => "Ваш адрес электронной почты";

        public string AskBlockManyKeysTotalTitle => "Сколько всего ключей вам дали?";
        public string PlaceholderManyKeysTotal => "Введите количество ключей";

        public string AskBlockAdditionalDocumentationTitle => "Любая дополнительная документация была предоставлена вам после загрузки?";

        public string AskBlockAdditionalPartsTitle => "Любые дополнительные детали были переданы вам?";

        public string AskBlockCarLokedTitle => "Автомобиль заблокирован?";

        public string AskBlockKeysLocationTitle => "Расположение ключей";
        public string TruckAnswer => "Грузовик";
        public string VehicleAnswer => "Транспортное средство";
        public string TrailerAnswer => "Трейлер";

        public string AskBlockRateCustomerTitle => "Оцените клиента";

        public string ComleteInspactinBtnText => "Завершить Осмотр";

        public string TitleBlockInspaction => "Осмотр водителя";
        public string TimeInspactionText => "Время до осмотра: ";
        public string NeedInspectionText => "Необходимость осмотра: ";
        public string HoursText => "Часов";
        public string CanPassText => "Вы можете пройти осмотр после";
        public string BestTimePassText => "Лучшее время для прохождения Осмотра";
        public string PassNowText => "Пройти осмотр сейчас";

        public string TakePictureProblem => "Пожалуйста, сфотографируйте проблемные места автомобиля.";
        public string PictureOneSafetyBelt => "Сфотографируйте один из ремней безопасности, но тот, который уже был сфотографирован";

        public string AskBlockSafeDeliveryLocationTitle => "Безопасное место доставки";
        public string ParkingLotAnswer => "Стоянка";
        public string DrivewayAnswer => "Дорога";
        public string GravelAnswer => "Гравий";
        public string SidewalklAnswer => "Тротуар";
        public string StreetAnswer => "Улица";
        public string MiddleStreetAnswer => "Середина улицы";

        public string AskBlockTruckEmergencyBrakeTitle => "Грузовик на аварийном тормозе?";

        public string AskBlockMeetClientTitle => " Встретились ли вы с клиентом?";

        public string AskBlockTruckLockedTitle => "Машина заблокирована?";

        public string AskBlockPictureIdPersonTitle => "Пожалуйста, сфотографируйте права человека принимающего доставку";

        public string AskBlockTrailerLockedTitle => "Все замки на прицепе заблокированы?";

        public string AskBlockAnyoneRushingPerformTitle => "Кто то торопит вас с доставкой?";

        public string AskBlockWhileVehicleBeingTransportedTitle => "Заметили ли вы какие-либо недостатки на кузове при транспортировке автомобиля?";

        public string PlaceholderBodyFlaws => "Введите недостатки кузова";

        public string AskBlockVehicleStartsTitle => "Автомобиль завелся?";
        public string AskBlockVehicleStarts1Title => "Как ты завел машину?";
        public string JumpAnswer => "Портативный Аккумулятор";
        public string CablesAnswer => "Кабели";
        public string RolledOutAnswer => "Выкатил";

        public string AskBlockDoesVehicleDrivesTitle => "Автомобиль движется?";

        public string AskBlockAnyoneHelpingUnloadTitle => "Кто-то помогает вам разгрузиться?";

        public string AskBlockSomeoneElseUnloadedVehicleTitle => "Кто-то разгрузил авто за вас?";

        public string AskBlockVehicleParkedSafeLocationTitle => "Автомобиль припаркован в безопасном месте?";

        public string AskBlockTimeOfDeliveryTitle => "Время доставки";

        public string InfoKeysGiveDriver => "Пожалуйста, разгрузите авто и все предметы доставки, оставьте ключи при себе до оплаты!";
        public string AskBlockDeliveryCustomerInspectCarTitle => "Попросите клиента осмотреть машину";
        public string IConfirmTheInspectionBtnText => "Подтверждаю осмотр";

        public string HintAddDamageForUser => "Нажмите на поврежденную область, чтобы добавить метку повреждение(после добавления повреждений приложение вернется к камере)";

        public string AskBlockInspectedVehicleAdditionalImperfectionsTitle => "Осматривали ли вы автомобиль на предмет каких-либо дополнительных недостатков, кроме перечисленных при получении?";
        public string FoundIssueBtnText => "Нашли проблему";

        public string AskBlockBilingPayTitle => "Оплата биллинга";
        public string AskBlockClientSignatureBlockTitle => "Ваше имя и подпись дорогой клиент";
        public string AskBlockClientNameTitle => "Имя клиента";
        public string AskBlockClientSignatureTitle => "Подпись клиента";

        public string AskBlockLikeRecive20fromYourNextCarTransportTitle => "Хотите получить скидку 20% на следующий транспорт?";

        public string AskBlockRateDriverTitle => "Оценить водителя";

        public string VehicleInspectionPikedUp => "Начать Осмотр на Получении";
        public string VehicleInspectionDelivery => "Начать Осмотр на Доставке";
        public string ThereAreNoVehiclesInThisOrder => "В этом заказе нет транспортных средств";

        public string TitelSelectPickPhoto => "Выберете вариант";
        public string SelectPhoto => "Камера";
        public string SelectGalery => "Галерея";

        public string SelectBackToRootBage => "Вернуться на экран заказов";
        public string SelectLoadGalery => "Загрузить фото в галерею";
        public string SelectLoadFolderOffline => "Загрузить фото в офлайн папку";
        public string SelectLoadFolderOfflineAndGalery => "Загрузить фото в офлайн папку и галерею";
    }
}