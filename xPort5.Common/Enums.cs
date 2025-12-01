using System;

namespace xPort5.Common
{
    /// <summary>
    /// Common enumerations used throughout the xPort5 application.
    /// Migrated from xPort5.DAL.Common.cs
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Status enumeration for entities
        /// </summary>
        public enum Status
        {
            Inactive = -1,
            Draft = 0,
            Active,
            Power
        }

        /// <summary>
        /// Workflow status enumeration
        /// </summary>
        public enum Workflow
        {
            Cancelled = 1,
            Queuing,
            Retouch,
            Printing,
            ProofingOutgoing,
            ProofingIncoming,
            Ready,
            Dispatch,
            InTransit,
            Completed
        }

        /// <summary>
        /// Order type enumeration
        /// </summary>
        public enum OrderType
        {
            UploadFile = 1,
            DirectPrint,
            PsFile,
            Others
        }

        /// <summary>
        /// Platform enumeration
        /// </summary>
        public enum Platform
        {
            PC = 1,
            Mac
        }

        /// <summary>
        /// Priority enumeration
        /// </summary>
        public enum Priority
        {
            Rush = 1,
            Express,
            Regular
        }

        /// <summary>
        /// Software enumeration
        /// </summary>
        public enum Software
        {
            PageMaker = 1,
            FreeHand = 3,
            Illustrator = 5,
            PhotoShop = 7,
            QuarkXpress = 9,
            CorelDraw = 11,
            MsWord = 13,
            Others = 15
        }

        /// <summary>
        /// Delivery method enumeration
        /// </summary>
        public enum DeliveryMethod
        {
            PickUp = 1,
            DeliverTo
        }

        /// <summary>
        /// Edit mode enumeration for forms
        /// </summary>
        public enum EditMode
        {
            Add,
            Edit,
            Read
        }

        /// <summary>
        /// Content type enumeration for file handling
        /// </summary>
        public enum ContentType
        {
            Image,      // Supports *.Jpg/*.jpeg
            PdfFile,
            PlainText,  // Supports *.txt
            MSExcel,    // Supports *.xls/*.xlsx
            MSWord,     // Supports *.doc/*.docx
            Video       // Supports *.mp4
        }

        /// <summary>
        /// User group enumeration for authorization
        /// </summary>
        public enum UserGroup
        {
            Owner,
            Administrator,
            Manager,
            Supervisor,
            Senior,
            Junior,
            Guest
        }

        /// <summary>
        /// User type enumeration
        /// </summary>
        public enum UserType
        {
            Staff,
            Customer,
            Supplier
        }
    }
}
