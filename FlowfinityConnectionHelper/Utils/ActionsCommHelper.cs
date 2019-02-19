using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowfinityConnectionHelper.Utils
{

    internal class ExecuteReturnValue
    {
        internal bool ReturnValue { get; set; }
        internal string RecordId { get; set; }

        public override string ToString()
        {
            return string.Format("RecordId:{0}, Status:{1}", RecordId, ReturnValue.ToString());
        }
    }
    public class ActionsCommHelperwithLogging : ActionsCommHelper
    {
        internal new ExecuteReturnValue Send(FASR.OperationCall[] ocall, string transactionid)
        {
            Lift.LiftManager.Logger.Write("Actions Comm Helper", "Entering ActionsCommHelperwithLogging.Send({0}, {1})", ocall.Length.ToString(), transactionid);
            ExecuteReturnValue returnValue = base.Send(ocall, transactionid);
            Lift.LiftManager.Logger.Write("Actions Comm Helper", "Leaving ActionsCommHelperwithLogging.Send({0}, {1})={2}", ocall.Length.ToString(), transactionid, returnValue.ToString());
            return returnValue;
        }
    }
    public class ActionsCommHelper
    {
        internal ExecuteReturnValue Send(FASR.OperationCall[] ocall, string transactionid)
        {
            ExecuteReturnValue ret = new ExecuteReturnValue() { ReturnValue = false, RecordId = null };
            if (ocall == null)
                throw new Exception("Operation call array is empty");

            FASR.ExecuteRequest request = new FASR.ExecuteRequest();

            request.OperationCalls = ocall;
            request.ExternalTransactionId = transactionid;

            string serverURL = Lift.LiftManager.ConfigProvider.GetValue("wsdl_url");
            FASR.ExecuteResponse response = Lift.LiftManager.RemoteClient.ExecuteRequest<FASR.IntegrationClient, FASR.ExecuteRequest, FASR.ExecuteResponse>(request,
                new Lift.II.RemoteClientInfo()
                {
                    Url = serverURL,
                    User = Lift.LiftManager.ConfigProvider.GetValue("wsdl_user"),
                    Password = Lift.LiftManager.ConfigProvider.GetValue("wsdl_password")
                }
                );

            if (response.Transaction.Status == FASR.TransactionStatus.SUCCESS)  {
                ret = new ExecuteReturnValue() { ReturnValue = true, RecordId = null };
            }

            return ret;
        }
    }
}