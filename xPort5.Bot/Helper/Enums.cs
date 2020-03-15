using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xFilm5.Bot
{
    public class Enums
    {
        public enum Status
        {
            Cancelled = -1,
            Draft = 0,
            Active,
            Power
        }

        public enum OrderStatus
        {
            Cancel = -1,
            Opening = 0,
            FollowUp,
            Complete,
            Complain,
            Blacklist
        }

        public enum DateType
        {
            NormalDay,
            PublicHoliday,
            CompanyHoliday,
            MorningShiftOnly,
            AfternoonShiftOnly
        }

        public enum DiscountType
        {
            None,
            ItemDiscount,
            OrderDiscount
        }

        public enum DiscountCriteriaType
        {
            Weekday,
            Qty,
            Month,
            DayOfMonth,
            TimeRange
        }

        public enum DiscountAmountType
        {
            None,
            Fixed,
            Percentage
        }

        public enum ContractorCostType
        {
            None,
            Fixed,
            Percent
        }

        public enum PaymentStatus
        {
            Opening = 0,
            Queued,
            Processed,
            Closed
        }

        public enum PaymentType
        {
            Cash,
            Cheque
        }

        public enum InspectionLocation
        {
            LivingRoom,
            MasterBedroom,
            Bedroom,
            Others
        }

        public enum MachineType
        {
            Window,
            Split,
            WindowSplit,
            AllDirectionSplit
        }

        public enum EditMode
        {
            Add,
            Edit,
            Read
        }

        public enum UserRole
        {
            Customer,
            Supplier,
            Technician,
            CS,
            Supervisor,
            Manager,
            SuperUser
        }

        public enum SearchPattern
        {
            None,
            Lookup,
            FirstChar,
            Days,
            Filter
        }

        public enum DbAction
        {
            None,
            InsRec,
            UpdRec,
            DelRec
        }

        public enum DeliveryMethod
        {
            PickUp = 1,
            DeliverTo
        }
    }
}
