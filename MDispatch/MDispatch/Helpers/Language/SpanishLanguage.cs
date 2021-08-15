using System;
namespace MDispatch.Helpers.Language
{
    public class SpanishLanguage : ILanguage
    {
        public string WelcomeText => "Bienvenidos";
        public string WelcomeDescriptionText => "Inicia sesión para continuar";
        public string PlaceholderEmail => "Correo electrónico habla a";
        public string PlaceholderPassword => "Contraseña";
        public string BtnLogInText => "Iniciar sesión";
        public string ForGotPasswordText => "Забыл пароль?";

        public string PasswordChangeRequestTitel => "Solicitud de cambio de contraseña";
        public string PlaceholderEmailChangeRequest => "Introduzca su correo electrónico aquí";
        public string NotCorectEmail => "El formato de correo electrónico ingresado es incorrecto";
        public string PlaceholderNameChangeRequest => "Ingrese su nombre completo aquí";
        public string PasswordChangeRequestBtnText => "Solicitar un cambio de contraseña";

        public string SuccessfulPasswordChangeRequest => "Datos de restablecimiento de contraseña enviados a usted por correo:";

        public string InspectionTodayAlert => "Usted ya ha superado la prueba de hoy";
        public string AskErrorAlert => "¡No ha completado todos los campos obligatorios, puede continuar con la verificación solo después de completar los campos obligatorios!";
        public string PassTheDeviceAlert => "Por favor lleve el dispositivo al cliente";
        public string TechnicalWorkServiceAlert => "Los trabajos de mantenimiento";
        public string GiveMoneyAlert => "Dar dinero para la entrega al conductor";
        public string PaymentForDeliveryAlert => "Debes ingresar el monto del pago para la entrega";
        public string NotNetworkAlert => "Sin red";
        public string NoVehiclesAlert => "No hay vehículos en el pedido.\n\nPara pasar la inspección, pídale al despachador que agregue un automóvil.";
        public string NoAvtorisationAlert => "Не авторизован";
        public string NoDataAlert => "No hay datos";
        public string AnswersSaved => "Las respuestas a las preguntas guardan";
        public string VideoSavedSuccessfully => "Grabación de video guardada con éxito";
        public string PaymentSaved => "Foto de la forma de pago guardada";
        public string FutureDispatcherProblem => "En un futuro próximo, el despachador verá el problema.";
        public string BOLIsSent => "Se envía una copia del BOL por correo";
        public string InformationDeliverySaved => "Información entrega salvó";
        public string InformationPaymentSaved => "Información de pago guardada";
        public string InformationPikedUpSaved => "Cargando información guardada";
        public string FeedbackSaved => "Revisión guardada";
        public string PaymmantMethodSaved => "Métodos de pago guardados";

        public string ScanPlateNumber => "Escanear matrícula";
        public string TitleSetPlateTruckAlert => "Por favor escriba el número de camión";
        public string TitleSetPlateTrailerAlert => "Por favor escriba el número del tráiler";
        public string PlaceholderSetPlateTruckAlert => "Introduzca Truck Nº";
        public string PlaceholderSetPlateTrailerkAlert => "Ingrese el tráiler No";
        public string CancelBtnText => "Cancelación";
        public string SendBtnText => "Enviar mensaje";

        public string TitleInfoPage => "Información";
        public string TitleVehicleInfo => "Vehículo";
        public string TitlePikedUpInfo => "Información de recibo";
        public string TitleDeliveryInfo => "información sobre la entrega";
        public string TitlePaymentInfo => "Información del pago";
        public string ContatInfo => "Contacto: ";
        public string PhoneInfo => "Teléfono: ";
        public string PaymentInfo => "Pago: ";
        public string Instructions => "Instrucciones";
        public string ReedInstructionsBtnText => "He leído las instrucciones";

        public string TitleSettingsPage => "Ajustes";
        public string DocumentInfo => "Documentación";
        public string ShowDocumentBtnText => "Mostrar documentos";
        public string LastInspactionInfo => "Última inspección: ";
        public string TruckPlateInfo => "Numero de camion: ";
        public string TrailerPalateInfo => "Número de remolque: ";
        public string TitleDocumentsTrailerTruckNumber => "Documentos de número de remolque y camión";
        public string NumberTruckPlateInfo => "№ camión:";
        public string NumberTrailerPalateInfo => "№ remolque";
        public string Application => "Aplicación";
        public string CurrentVersion => "Versión actual: ";
        public string LastUpdateAvailable => "Última actualización disponible: ";
        public string SignOutBtnText => "Apagarse";
        public string LanguageText => "Lenguaje";

        public string NamePageTabActive => "Activo";
        public string NamePageTabDelivery => "Entregado";
        public string NamePageTabArchived => "Archivado";

        public string TitleInspectionDriverAlert => "Para aceptar un pedido, ¿necesita someterse a una inspección técnica de un camión y un remolque, para someterse a una inspección técnica de un camión?";
        public string YesBtnText => "Sí";
        public string NoBtnText => "No";
        public string TakeBtnText => "Fotografiar";

        public string ContinuingInspectionDelivery => "Continúa la inspección de entrega";
        public string ContinuingInspectionPickedUp => "Inspección constante de la carga";
        public string StartInspectionDelivery => "Iniciar entrega de inspección";
        public string StartInspectionPickedUp => "Recibo de inspección de inicio";

        public string HintPhotoItemsPage => "Tome una foto de los artículos transportados por la máquina.";
        public string HintPhotoInspactionPage => "Trate de fotografiar la parte exactamente como en la maqueta o el daño está cerca";
        public string HintPhotoInTruckPage => "Intenta fotografiar el detalle exactamente en la maqueta.";
        public string HintPhotoSeatBeltsPage => "Tome una foto de uno de los cinturones de seguridad, pero el que ya está abrochado";

        public string TitleAskQuestionPage => "Preguntas de inspección";
        public string ItemNextPage => "Siguiente";

        public string AskBlockWeatherTitle => "Tiempo";
        public string ClearAnswer => "Soleado";
        public string RainAnswer => "Lluvia";
        public string SnowAnswer => "Nieve";
        public string DustAnswer => "Polvo";

        public string AskBlockLightBrightnessTitle => "Brillo de luz";
        public string HighAnswer => "Elevado";
        public string LowAnswer => "Bajo";

        public string AskBlockSafeLoctionTitle => "Lugar de entrega segura";

        public string AskBlockFarFromTrailerTitle => "¿Qué tan lejos del remolque?";
        public string PlaceholderFarFromTrailerAnswer => "Por favor, indique qué tan lejos está del remolque";

        public string AskBlockGaveKeysTitle => "Por favor ingrese qué tan lejos está del tráiler";
        public string PlaceholderGaveKeysAnswer => "Por favor, introduzca el nombre completo";

        public string AskBlockEnoughSpaceTitle => "¿Espacio suficiente para fotos?";

        public string AskBlockAnyoneRushingTitle => "¿Alguien tiene prisa por hacer una inspección?";

        public string AskBlockNamePersonTitle => "Ingrese el nombre de la persona";
        public string PlaceholderNamePersonAnswer => "Por favor, introduzca el nombre completo";

        public string AskBlockTypeCarTitle => "Tipo de vehiculo";

        public string AskBlockPlateTitle => "Número de vehículo";
        public string PlaceholderPlateAnswer => "Ingrese el número de vehículo";

        public string AskBlockConditionTitle => "Condición del vehículo";
        public string CleanAnswer => "Limpio";
        public string DirtyAnswer => "Sucio";
        public string WetAnswer => "Mojado";
        public string Snow1Answer => "Nieve";
        public string ExtraDirtyAnswer => "Muy sucio";

        public string AskBlockAdditionalItemsTitle => "Cualquier artículo personal o adicional en el vehículo.";

        public string HintAddDamagePage => "Haga clic en el lugar dañado para agregar una marca de daño.";
        public string BackMainBtnText => "De vuelta a casa";
        public string NextInspactionBtnText => "Próxima inspección";

        public string RetakeBtnText => "Volver a fotografiar";
        public string AddDamagBtnText => "Agregar daño";
        public string NextBtnText => "Siguiente";
        public string AddPhotoText => "Añadir foto";

        public string AskBlockJumpedVehicleTitle => "¿Saltó del coche para marcharse?";

        public string AskBlockMusteMileageTitle => "Millaje exacto después de la carga (debe ingresar millas)";
        public string PlaceholderMusteMileage => "Kilometraje";

        public string AskBlockImperfectionsWileLoadingTitle => "¿Ha notado algún defecto mecánico durante la carga?";
        public string PlaceholderMechanicalDefects => "Ingrese defectos mecánicos";

        public string AskBlockMethodExitTitle => "¿Qué método de salida usaste?";
        public string DoorAnswer => "Una puerta";
        public string WindowAnswer => "Ventana";
        public string SunroofAnswer => "Techo corredizo";
        public string ConvertibleAnswer => "Cabriolé";

        public string AskBlockHelpYouLoadTitle => "Alguien te ayudó a descargar esto";
        public string AskBlockLoadTheVehicleTitle => "¿Alguien cargó el auto por ti?";
        public string PlaceholderName => "Introduzca su nombre";

        public string AskBlockDamageAnythingTitle => "¿Dañó algo mientras cargaba?";
        public string PlaceholderIfAny => "Indique daño, si lo hubiera";

        public string AskBlockUsedWinchTitle => "¿Ha utilizado un cabrestante?";

        public string TitleHelloCustomerPage => "Hola cliente";
        public string ThankYouForUsingOurCompany => "Gracias por utilizar nuestra empresa..";
        public string ThankYouForUsingOurCompany1 => "Haga clic en el botón Inicio para continuar.";
        public string StartBtnText => "Comienzo";

        public string AskBlockFullNameTitle => "Tu nombre completo";
        public string PlaceholderFullName => "Por favor ingrese su nombre completo";

        public string AskBlockYourPhoneTitle => "Tu número de teléfono";
        public string PlaceholderYourPhone => "Ingresa tu teléfono";

        public string AskBlockManyKesTitle => "Cuántas llaves se le dieron al conductor";
        public string PlaceholderManyKes => "Ingrese el número de claves que le da";

        public string AskBlockGivenToDriverTitle => "¿Se ha otorgado algún título al conductor?";
        public string IDontKnowBtnText => "No sé";

        public string ContinuetnBtnText => "Continuar";

        public string TitleBillOfLandingPage => "GUÍA DE CARGA";

        public string TitleOriginInfo => "De donde";
        public string TitleDestinatiinInfo => "Destino";
        public string TitleYourSignature => "Tu firma";
        public string SaveBtnText => "Salvar";

        public string ThankYouInspactionText => "Gracias. Entregue el dispositivo al conductor.";

        public string AskBlockAccountPasswordTitle => "Ingrese la contraseña de su cuenta para continuar";
        public string PlaceholderAccountPassword => "Introducir la contraseña";

        public string AskBlockDriverPayTitle => "¿Hizo una tarifa de conductor por el transporte?";
        public string DescriptionDriverPayTitle => "En un futuro próximo, el despachador verá el problema.";

        public string TypeInfo => "Un tipo: ";
        public string ColorInfo => "Color: ";

        public string HintDamegePickedUp => "Círculos amarillos: daño durante la carga;";
        public string HintDamegeDelivery => "Los círculos son verdes: daños en la entrega;";
        public string SeeInspactinPhoneBtnText => "Haga clic para ver una foto de la inspección";

        public string AskBlockSendBOLTitle => "Correo electrónico BOL";
        public string SendBOLBtnText => "Enviar BOL";

        public string TitlePhotoInspactionPickedUp => "Foto inspección Cargando";
        public string TitlePhotoInspactionDelivery => "Foto de inspección de entrega";

        public string TitleAlertSendEmailBOL => "¿Necesitas una copia de la verificación?";

        public string DescriptionDiscount => "¿Le gustaría obtener un 10% de descuento en su próxima descarga solo por darnos una buena reseña y completar una pequeña encuesta?";

        public string TitleFeedBackPage => "Отзыв";

        public string AskBlockSatisfiedServiceTitle => "¿Qué tan satisfecho está con el servicio?";

        public string AskBlockUseCompanyAgainTitle => "¿Volvería a utilizar nuestra empresa?";
        public string MaybeBtnText => "Quizás";

        public string AskBlockPromotionTitle => "¿Le gustaría ser notificado si tenemos una promoción?";

        public string AskBlockDriverPerformTitle => "¿Cómo se comportó el conductor?";

        public string AskBlockYourTitle => "Su dirección de correo electrónico";

        public string AskBlockManyKeysTotalTitle => "¿Cuántas llaves te dieron?";
        public string PlaceholderManyKeysTotal => "Ingrese el número de llaves";

        public string AskBlockAdditionalDocumentationTitle => "Cualquier documentación adicional se proporciona al descargar";

        public string AskBlockAdditionalPartsTitle => "Se le ha transmitido cualquier detalle adicional";

        public string AskBlockCarLokedTitle => "¿Está cerrado el coche?";

        public string AskBlockKeysLocationTitle => "Ubicación clave";
        public string TruckAnswer => "Camión";
        public string VehicleAnswer => "Vehículo";
        public string TrailerAnswer => "Remolque";

        public string AskBlockRateCustomerTitle => "Califica al cliente";

        public string ComleteInspactinBtnText => "Completa la inspección";

        public string TitleBlockInspaction => "Inspección del conductor";
        public string TimeInspactionText => "Tiempo antes de la inspección: ";
        public string NeedInspectionText => "La necesidad de inspección: ";
        public string HoursText => "Horas";
        public string CanPassText => "Puede pasar por el examen después";
        public string BestTimePassText => "El mejor momento para pasar por la inspección";
        public string PassNowText => "Obtenga una inspección ahora";

        public string TakePictureProblem => "Tome una foto de las áreas problemáticas del vehículo.";
        public string PictureOneSafetyBelt => "Toma una foto de uno de los cinturones de seguridad, pero el que ya ha sido fotografiado";

        public string AskBlockSafeDeliveryLocationTitle => "Lugar de entrega segura";
        public string ParkingLotAnswer => "Estacionamiento";
        public string DrivewayAnswer => "Camino";
        public string GravelAnswer => "Grava";
        public string SidewalklAnswer => "Acera";
        public string StreetAnswer => "Calle";
        public string MiddleStreetAnswer => "Medio de la calle";

        public string AskBlockTruckEmergencyBrakeTitle => "¿Está el camión en el freno de emergencia?";

        public string AskBlockMeetClientTitle => "Has conocido a un cliente";

        public string AskBlockTruckLockedTitle => "¿Está bloqueado el camión?";

        public string AskBlockPictureIdPersonTitle => "Tome una foto de la identificación de la persona que realiza el envío";

        public string AskBlockTrailerLockedTitle => "¿Están todas las cerraduras del remolque cerradas?";

        public string AskBlockAnyoneRushingPerformTitle => "¿Alguien tiene prisa con la entrega?";

        public string AskBlockWhileVehicleBeingTransportedTitle => "¿Ha notado alguna imperfección en la carrocería al transportar el automóvil?";

        public string PlaceholderBodyFlaws => "Introduzca defectos corporales";

        public string AskBlockVehicleStartsTitle => "¿Ha arrancado el coche?";
        public string AskBlockVehicleStarts1Title => "¿Cómo arrancaste el coche?";
        public string JumpAnswer => "Saltar";
        public string CablesAnswer => "Cables";
        public string RolledOutAnswer => "Desplegado";

        public string AskBlockDoesVehicleDrivesTitle => "¿Se está moviendo el auto?";

        public string AskBlockAnyoneHelpingUnloadTitle => "¿Alguien te está ayudando a descargar?";

        public string AskBlockSomeoneElseUnloadedVehicleTitle => "¿Alguien más descargó el auto por ti?";

        public string AskBlockVehicleParkedSafeLocationTitle => "¿Está aparcado el coche en un lugar seguro?";

        public string AskBlockTimeOfDeliveryTitle => "Tiempo de entrega";

        public string InfoKeysGiveDriver => "Retire todos los artículos de envío y guarde las llaves con usted";
        public string AskBlockDeliveryCustomerInspectCarTitle => "¿Pedirle al cliente que inspeccione el coche? Después de inspeccionar el automóvil, presione el botón confirmar.";
        public string IConfirmTheInspectionBtnText => "Confirmo la inspección";

        public string HintAddDamageForUser => "Haga clic en el área dañada para agregar una etiqueta de daño (después de agregar el daño, la aplicación volverá a la cámara)";

        public string AskBlockInspectedVehicleAdditionalImperfectionsTitle => "¿Ha examinado el vehículo en busca de deficiencias adicionales a las enumeradas al recibirlo?";
        public string FoundIssueBtnText => "Encontré un problema";

        public string AskBlockBilingPayTitle => "Pago de facturación";
        public string AskBlockClientSignatureBlockTitle => "Tu nombre y firma querido cliente";
        public string AskBlockClientNameTitle => "Nombre del cliente";
        public string AskBlockClientSignatureTitle => "Firma del cliente";

        public string AskBlockLikeRecive20fromYourNextCarTransportTitle => "¿Quieres conseguir un 20% de descuento en tu próximo transporte?";

        public string AskBlockRateDriverTitle => "Conductor de tasa";

        public string VehicleInspectionPikedUp => "Inspeccion de vehiculo: Piked Up";
        public string VehicleInspectionDelivery => "Inspeccion de vehiculo: Delivery";
        public string ThereAreNoVehiclesInThisOrder => "No hay vehículos en este orden.";

        public string TitelSelectPickPhoto => "Escoge una opción";
        public string SelectPhoto => "Galería";
        public string SelectGalery => "Cámara";
    }
}
