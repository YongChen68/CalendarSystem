﻿using System;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using static System.Net.Mime.MediaTypeNames;


namespace Generics.Utils
{
    public class Product
    {
        [DataMember]
        public string WorkOrderNumber { get; set; }

        [DataMember]
        public string Item { get; set; }

        [DataMember]
        public string Size { get; set; }

        [DataMember]
        public string Quantity { get; set; }

        [DataMember]
        public string SubQty { get; set; }

        [DataMember]
        public string System { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
    public class Installer
    {
        [DataMember]
        public string SeniorInstaller { get; set; }

        [DataMember]
        public string CrewNames { get; set; }

    }

    public class InstallerWithLessInfo
    {
        [DataMember]
        public string ParentRecordid { get; set; }

        [DataMember]
        public string DetailedRecordid { get; set; }

        [DataMember]
        public string Userid { get; set; }

        [DataMember]
        public string Account { get; set; }


        [DataMember]
        public string id { get; set; }
    }



    public class InstallerInfo
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Branch { get; set; }
        [DataMember]
        public string Department { get; set; }
        [DataMember]
        public string Telephone { get; set; }
        [DataMember]
        public string recordid { get; set; }
        [DataMember]
        public string WorkPhoneNumber { get; set; }
        [DataMember]
        public string InstallerLevel { get; set; }
        [DataMember]
        public string WorkOrderNumber { get; set; }
        [DataMember]
        public string email { get; set; }
        [DataMember]
        public string DetailRecordId { get; set; }
        [DataMember]
        public string UserId { get; set; }


    }



    public class InstallerInfoWithImage
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Branch { get; set; }
        [DataMember]
        public string Department { get; set; }
        [DataMember]
        public string Telephone { get; set; }
        [DataMember]
        public string recordid { get; set; }
        [DataMember]
        public string WorkPhoneNumber { get; set; }
        [DataMember]
        public string InstallerLevel { get; set; }
        [DataMember]
        public string WorkOrderNumber { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        [Lift.Database.DbIgnore]
        public string picString
        {
            get
            {
                if (pic != null)
                {
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.Append(" data:image/jpeg;base64," + Convert.ToBase64String(pic));
                    return strBuilder.ToString();


                }
                else
                {
                    return null;
                }
            }
            set { }
        }

        //[DataMember]
        //[Lift.Database.DbIgnore]
        //public string smallpicString
        //{
        //    get
        //    {
        //        if (pic != null)
        //        {
        //            StringBuilder strBuilder = new StringBuilder();

        //            strBuilder.Append(" <img src='data:image/jpeg;base64," + Convert.ToBase64String(pic) + "'" + " onclick =\"ShowWOBigPicture('" + DetailRecordId + "');" + "\">");

        //            return strBuilder.ToString();


        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    set { }
        //}

        [DataMember]
        public byte[] pic
        {
            get; set;
        }


    }

    public class CalledLog
    {
        [DataMember]
        public String DateCalled
        {
            //get {
            //    string str = string.Empty;
            //    str = DateCalled;
            //    return str;

            //}
            //set { }
            get; set;
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        //   public string start { get { return FormatDateTime(ScheduledDate); } set { } }
        public string CalledLogDate
        {
            get
            {
                string str = string.Empty;
                if ((DateCalled != null) && (DateCalled.ToString().Length > 0))
                {
                    //   str = FormatDateTime(Convert.ToDateTime(WoodDropOffDate.Split(' ')[0]));
                    str = Convert.ToDateTime(DateCalled.ToString().Split(' ')[0]).ToString("MM/d/yyyy");
                }
                return str;
            }
            set { }
        }


        [DataMember]
        [Lift.Database.DbIgnore]
        public string StrCalledLogTime
        {
            get
            {
                string str = string.Empty;
                if ((DateCalled != null) && (DateCalled.ToString().Length > 0))
                {
                    str = Convert.ToDateTime(DateCalled).ToString("M/dd/yyyy HH:mm:ss").Split(' ')[1];
                }
                return str;
            }
            set { }
        }

        [DataMember]
        public string DetailRecordId { get; set; }

        [DataMember]
        public string ParentRecordId { get; set; }

        [DataMember]
        public string CalledMessage { get; set; }


        [DataMember]
        public string Notes3 { get; set; }

        [DataMember]
        public string id { get; set; }


    }


    public class Notes
    {
        [DataMember]
        public String NotesDate
        {
            get; set;
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        //   public string start { get { return FormatDateTime(ScheduledDate); } set { } }
        public string StrNotesDate
        {
            get
            {
                string str = string.Empty;
                if ((NotesDate != null) && (NotesDate.ToString().Length > 0))
                {
                    //   str = FormatDateTime(Convert.ToDateTime(WoodDropOffDate.Split(' ')[0]));
                    str = Convert.ToDateTime(NotesDate.ToString().Split(' ')[0]).ToString("MM/d/yyyy");
                }
                return str;
            }
            set { }
        }


        [DataMember]
        [Lift.Database.DbIgnore]
        public string StrNotesTime
        {
            get
            {
                string str = string.Empty;
                if ((NotesDate != null) && (NotesDate.ToString().Length > 0))
                {
                    str = Convert.ToDateTime(NotesDate).ToString("M/dd/yyyy HH:mm:ss").Split(' ')[1];
                }
                return str;
            }
            set { }
        }

        [DataMember]
        public string DetailRecordId { get; set; }

        [DataMember]
        public string ParentRecordId { get; set; }

        [DataMember]
        public string Category { get; set; }


        [DataMember]
        public string GeneralNotes { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string WorkOrderNumber { get; set; }


    }
    public class WindowsCustomer
    {
        [DataMember]
        public string WorkOrderNumber { get; set; }

        [DataMember]
        public string CustomerName { get; set; }


        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string ShippingType { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Branch_display { get; set; }

        [DataMember]
        public int TotalWindows { get; set; }

        [DataMember]
        public int TotalDoors { get; set; }

        [DataMember]
        public int TotalPatioDoors { get; set; }

        [DataMember]
        public decimal TotalPrice { get; set; }


    }


    public class SubTrades
    {


        [DataMember]
        public string SubTrade { get; set; }


        [DataMember]
        public string Status { get; set; }


    }

    public class DocumentFile
    {
        [DataMember]
        public Int64 DetailRecordId
        {
            get; set;
        }

        [DataMember]
        public string id
        {
            get; set;
        }

        [DataMember]
        public byte[] FileSource
        {
            get; set;
        }


        [DataMember]
        [Lift.Database.DbIgnore]
        public string DocumentFileStr
        {
            get
            {

                StringBuilder strBuilder = new StringBuilder();
                // strBuilder.Append(" data:application/pdf;base64," + Convert.ToBase64String(FileSource));

                strBuilder.Append(" data:" + FileNameMime + ";base64," + Convert.ToBase64String(FileSource));

             //    strBuilder.Append(Convert.ToBase64String(FileSource));
                //  strBuilder.Append(" <embed src='data:image/jpeg;base64," + Convert.ToBase64String(FileSource) + "'" + " onclick =\"ShowWOBigPicture('" + DetailRecordId + "');" + "\">");
                return strBuilder.ToString();

            }
            set { }

        }

        [DataMember]
        public string FileNameSource
        {
            get; set;
        }
        [DataMember]
        [Lift.Database.DbIgnore]
        public string FileNameMime
        {
            get
            {
                if (FileNameSource != null)
                {
                    string fileType = FileNameSource.Split(';')[1].Replace("mime-type=", "").Replace("'", "");
                    return fileType;
                }
                else
                {
                    return null;
                }
            }
            set { }
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        public string FileName
        {
            get
            {
                if (FileNameSource != null)
                {
                    string fileName = FileNameSource.Split(';')[0].Replace("file-name=", "").Replace("'", "");
                    return fileName;
                }
                else
                {
                    return null;
                }
            }
            set { }
        }

    }

    public class JobReview
    {

        //string fileName;
        //string fileType;
        //string fileEncrpCode;

        [DataMember]
        public string WorkOrderNumber { get; set; }

        [DataMember]
        public string JobComplete
        {
            get; set;
        }
        [DataMember]
        public string WindowProductReady
        {
            get; set;
        }
        [DataMember]
        public string CodelProductReady
        {
            get; set;
        }

        [DataMember]
        public string InstallMaterialMissing
        {
            get; set;
        }
        [DataMember]
        public string Whatwasmissing
        {
            get; set;
        }
        [DataMember]
        public string CentraQuality
        {
            get; set;
        }

        [DataMember]
        public string CentraRating
        {
            get; set;
        }

        [DataMember]
        public string CentraStarRating
        {
            get; set;
        }

        [DataMember]
        public string CodelQuality
        {
            get; set;
        }
        [DataMember]
        public string CodelStarRating
        {
            get; set;
        }
        [DataMember]
        public string CodelRating
        {
            get; set;
        }
        [DataMember]
        public string ContractQuality
        {
            get; set;
        }


        [DataMember]
        public string ContractRating
        {
            get; set;
        }

        [DataMember]
        public string ContractStarRating
        {
            get; set;
        }
        [DataMember]
        public string RemeasureQuality
        {
            get; set;
        }
        [DataMember]
        public string RemeasureRating
        {
            get; set;
        }
        [DataMember]
        public string RemeasureStarRating
        {
            get; set;
        }
        [DataMember]
        public string Notes
        {
            get; set;
        }

        [DataMember]
        public Int64 RecordId
        {
            get; set;
        }

        [DataMember]
        public DateTime CreatedAt
        {
            get; set;
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        //   public string start { get { return FormatDateTime(ScheduledDate); } set { } }
        public string StrCreatedAt
        {
            get
            {
                string str = string.Empty;
                if ((CreatedAt != null) && (CreatedAt.ToString().Length > 0))
                {
                    //   str = FormatDateTime(Convert.ToDateTime(WoodDropOffDate.Split(' ')[0]));
                    str = Convert.ToDateTime(CreatedAt.ToString().Split(' ')[0]).ToString("MM/d/yyyy");
                }
                return str;
            }
            set { }
        }
        [DataMember]
        public string CreatedBy
        {
            get; set;
        }
    }
    public class DocumentLibrary
    {

        //string fileName;
        //string fileType;
        //string fileEncrpCode;

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public string FileNameSource
        {
            get; set;
        }
        [DataMember]
        [Lift.Database.DbIgnore]
        public string FileNameMime
        {
            get
            {
                if ((FileNameSource != null) && (FileNameSource.Length!=0))
                {
                    string fileType = FileNameSource.Split(';')[1].Replace("mime-type=", "").Replace("'", "");
                    return fileType;
                }
                else
                {
                    return null;
                }
            }
            set { }
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        public string FileName
        {
            get
            {
                if (FileNameSource != null)
                {
                    string fileName = FileNameSource.Split(';')[0].Replace("file-name=", "").Replace("'", "");
                    return fileName;
                }
                else
                {
                    return null;
                }
            }
            set { }
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        public string FileEncrpCode
        {
            //get
            //{
            //    if (FileNameSource != null)
            //    {
            //        string fileEncrpCode = FileNameSource.Split(';')[3].Replace("sha1==", "").Replace("'", "");
            //        return fileEncrpCode;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //set { }
            get; set;
        }





        [DataMember]
        public Int64 DetailRecordId
        {
            get; set;
        }
    }

    public class WOPicture
    {
        [DataMember]
        public string PictureName { get; set; }

        [DataMember]
        public byte[] pic
        {
            get; set;
        }

        [DataMember]
        public Int64 DetailRecordId
        {
            get; set;
        }

        [DataMember]
        [Lift.Database.DbIgnore]
        public string smallpicString
        {
            get
            {
                if (pic != null)
                {
                    StringBuilder strBuilder = new StringBuilder();
      
                    strBuilder.Append(" <img src='data:image/jpeg;base64," + Convert.ToBase64String(pic) + "'" + " onclick =\"ShowWOBigPicture('" + DetailRecordId + "');" + "\">");

                    return strBuilder.ToString();
                  

                }
                else
                {
                    return null;
                }
            }
            set { }
        }
        [DataMember]
        [Lift.Database.DbIgnore]
        public string picString
        {
            get
            {
                if (pic != null)
                {
                    StringBuilder strBuilder = new StringBuilder();
                    strBuilder.Append(" data:image/jpeg;base64," + Convert.ToBase64String(pic));
                    return strBuilder.ToString();


                }
                else
                {
                    return null;
                }
            }
            set { }
        }

    }
    public class UnavailableHR
    {
        [DataMember]
        public string DateUnavailable { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Branch { get; set; }


        [DataMember]
        public string Reason { get; set; }

    }

    public class Datetime
    {
    }

    public class Projections
    {
        [DataMember]
        public int NumberOfInstallers { get; set; }

        [DataMember]
        public string NumberOfMinAvailable { get; set; }

        [DataMember]
        public string Branch { get; set; }

        [DataMember]
        public DateTime ProjectionsDate { get; set; }

    }

}