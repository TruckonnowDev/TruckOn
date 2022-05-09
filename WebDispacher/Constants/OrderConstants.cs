namespace WebDispacher.Constants
{
    public static class OrderConstants
    {
        public const string OrderStatusNewLoad = "NewLoad";
        public const string OrderStatusArchived = "Archived";
        public const string OrderStatusAssigned = "Assigned";
        public const string OrderStatusDeleted = "Deleted";
        public const string OrderStatusPickedUp = "Picked up";
        public const string OrderStatusArchivedBilled = "Archived,Billed";
        public const string OrderStatusArchivedPaid = "Archived,Paid";
        public const string OrderStatusDeletedBilled = "Deleted,Billed";
        public const string OrderStatusDeletedPaid = "Deleted,Paid";
        public const string OrderStatusDeliveredPaid = "Delivered,Paid";
        public const string OrderStatusDeliveredBilled = "Delivered,Billed";
        public const string ActionAssign = "Assign";
        public const string ActionUnAssign = "Unassign";
        public const string ActionSolved = "Solved";
        public const string ActionCreate = "Create";
        public const string ActionSaveOrder = "SaveOrder";
        public const string ActionRemoveVech = "RemoveVech";
        public const string ActionAddVech = "AddVech";
        public const string ActionArchivedOrder = "ArchivedOrder";
        public const string ActionDeletedOrder = "DeletedOrder";
        public const string SuccessfullyRemovedVehicle = "Vehicle information removed successfully";
        public const string SuccessfullyAddedVehicle = "Vehicle information Added successfully";
        public const string UnAuthorizedUserCannotChangeOrder = "Unauthorized user cannot change order";
        public const string ErrorRemovedVehicle = "Vehicle information not removed (ERROR)";
        public const string ErrorAddedVehicle = "Vehicle information not Added (ERROR)";
    }
}