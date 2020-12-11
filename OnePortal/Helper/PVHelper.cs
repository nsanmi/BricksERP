using OnePortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WorkFlow.DAL;
using WorkFlow.DAL.Logic;
using WorkFlow.DAL.Service;

namespace OnePortal.Helper
{
    public class PVHelper
    {
        public PVWorkflow GetWorkflowPVSummary(Guid workflow_id)
        {
            var pv_summary = new PVWorkflow();

            pv_summary.WorkflowId = workflow_id;
            var bLWorkflow = new BLWorkflow(workflow_id);

            var workflow = bLWorkflow.GetWorkflow();
            pv_summary.title = new WorkflowService().GetWorkflow(workflow_id).title;
            pv_summary.workflow = workflow;
            var definition = bLWorkflow.GetDefinition();
            //procurement
            if (workflow.process_id.ToString().ToUpper() == "94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317" || workflow.process_id.ToString().ToUpper() == "10471746-0236-4F79-BE27-8AA1CBFF2D92" || workflow.process_id.ToString().ToUpper() == "396E4066-81F1-4311-B076-FC3C53BB3178" || workflow.process_id.ToString().ToUpper() == "2D008467-EA12-4821-A2F2-7568B76039F1")
            {

               



                var form = definition.Element("form");
                if (form.Element("justification") != null)
                {
                    pv_summary.Purpose = form.Element("justification").Value;
                }

                if (form.Element("conversion_rate") != null)
                {
                    pv_summary.ConversionRate = Convert.ToDecimal(form.Element("conversion_rate").Value);
                }
                else
                {
                    pv_summary.ConversionRate = 1;
                }
                decimal total = 0;
                if (form.Element("payments") != null && form.Element("payments").Elements("payment").Any())
                {
                    foreach (var item in form.Element("payments").Elements("payment"))
                    {
                        if (item.Element("status") != null || item.Element("status").Value == "Not paid")
                        {
                            total = Convert.ToDecimal(item.Element("vprf_amount").Value);
                        }

                    }
                }
               
                pv_summary.Total = total;

                var vendor_element = definition.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Attribute("selected").Value == "1");

                if (vendor_element != null)
                {
                    var vendor_id = Convert.ToInt32(vendor_element.Element("id").Value);
                    var vendor = Util.GetVendors().FirstOrDefault(e => e.id == vendor_id);
                    pv_summary.Recipient = vendor.name;
                    pv_summary.VendorBank = vendor.bank_name;
                    pv_summary.AccountName = vendor.account_name;
                    pv_summary.VendorAccountNumber = vendor.account_number;
                }
            }else if (workflow.process_id.ToString().ToUpper() == "A9BE0559-94E4-4A6E-AE46-74A1344DF7DF")
            {
                var form = definition.Element("form");
                //get all the expenses
                decimal total = 0;

                if (form.Element("expenses") != null && form.Element("expenses").Elements("expense").Any())
                {
                    foreach (var element in form.Element("expenses").Elements("expense"))
                    {
                        try
                        {
                            total += (Convert.ToDecimal(element.Element("no_of_days").Value) * Convert.ToDecimal(element.Element("rate").Value));

                        }
                        catch (Exception)
                        {
                        }

                    }
                }
                pv_summary.Total = total;
                pv_summary.Recipient = definition.Element("steps").Elements("step").FirstOrDefault(e=>e.Element("code").Value=="1").Element("created_by_name").Value;

                pv_summary.VendorBank = "";
                if (form.Element("bank_name") != null)
                {
                    pv_summary.VendorBank = form.Element("bank_name").Value;
                }

                pv_summary.VendorAccountNumber = "";
                if (form.Element("account_no") != null)
                {
                    pv_summary.VendorAccountNumber = form.Element("account_no").Value;
                }

                pv_summary.AccountName = "";
                if (form.Element("account_name") != null)
                {
                    pv_summary.AccountName = form.Element("account_name").Value;
                }

                pv_summary.ConversionRate = 1;
                pv_summary.Purpose = form.Element("purpose_of_travel").Value;

            }
            return pv_summary;
        }
    }
}