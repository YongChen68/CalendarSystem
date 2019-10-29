using System;
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

        [DataMember]
        [Lift.Database.DbIgnore]
        public string FileEncrpCode
        {
            get
            {
                if (FileNameSource != null)
                {
                    string fileEncrpCode = FileNameSource.Split(';')[3].Replace("sha1==", "").Replace("'", "");
                    return fileEncrpCode;
                }
                else
                {
                    return null;
                }
            }
            set { }
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
                    // str = "document.getElementById('" + DetailRecordId.ToString().Trim() + "').style.display='block'";

                    //str = "ShowWOBigPicture('" + DetailRecordId.ToString().Trim() + "');";
                    //return "<img src='data:image/jpeg;base64," + Convert.ToBase64String(pic) +
                    ////   "' style = \"width:30%;cursor:zoom-in\"  " +

                    //      "' onclick =" + str +
                    //             ">";
                    //  strBuilder.Append("<a href='data:image/jpeg;base64," + Convert.ToBase64String(pic) + "' data-toggle=\"lightbox\" data-title=\"A random title\" data-footer=\"A custom footer text\">");
                    //  strBuilder.Append("<a href= javascript:alert('test');" + " data-toggle=\"lightbox\" data-title=\"A random title\" data-footer=\"A custom footer text\">");
                    //   strBuilder.Append("<a id= '"+ DetailRecordId  + "'  onclick=\"ShowWOBigPicture('" + DetailRecordId + "');\" data-toggle=\"lightbox\" data-title=\"A random title\" data-footer=\"A custom footer text\">");
                    // strBuilder.Append(" <img src='data:image/jpeg;base64," + Convert.ToBase64String(pic) + "'></a>");

                    strBuilder.Append(" <img src='data:image/jpeg;base64," + Convert.ToBase64String(pic) + "'" + " onclick =\"ShowWOBigPicture('" + DetailRecordId + "');" + "\">");


                    //  strBuilder.Append(" <img src='data:image/jpeg;base64," + Convert.ToBase64String(pic) + ">");

                    return strBuilder.ToString();
                    //    "onclick = alert('test') ;" +
                    //  "onclick = \"document.getElementById(\"" + DetailRecordId + "\").style.display='block'" +
                    //"</img>";

                    //    "' id='" + DetailRecordId + "' " + "style = 'width:30%;cursor:zoom-in'  " +
                    //   "\">";
                    //   "onclick = 'document.getElementById('" + DetailRecordId + "').style.display='block'" + "\">";
                    // return "src=\"data:image/jpeg;base64," + Convert.ToBase64String(pic);

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