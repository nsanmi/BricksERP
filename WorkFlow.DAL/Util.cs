using HRM.DAL.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.Logic;

namespace WorkFlow.DAL
{
    public static class Util
    {
        readonly static workFlowEntities _context = new workFlowEntities();

        public static IQueryable<bpm_process> GetProcesses()
        {
            return _context.bpm_process.Where(e=>e.status==1);
        }


        public static string GetXELementValue(XElement element,string name)
        {
            return element.Element(name) != null || element.Element(name).Value != null ? element.Element(name).Value.ToString() : "";
        }

        public static string FormatXELementDate(XElement element, string name)
        {
            try
            {
                return element.Element(name) != null || element.Element(name).Value != null ? DateTime.Parse(element.Element(name).Value).ToString("yyyy-MM-dd HH:mm tt") : "";
            }
            catch (Exception)
            {
                return "";
            }
            
           
        }
        public static IQueryable<bpm_vendor_category> GetVendorCategories()
        {
            return _context.bpm_vendor_category.AsQueryable();
        }

        public static List<bpm_vendor> GetVendors()
        {
           return  _context.bpm_vendor.ToList();
        }

        public static bpm_vendor GetVendor(int id)
        {
            return _context.bpm_vendor.FirstOrDefault(e=>e.id==id);
        }

        public static int GetWorkflowPercentage(string workflow)
        {
            XElement definition = XElement.Parse(workflow);
            var next_value = Convert.ToDouble(definition.Element("next_step").Value);
            var steps = Convert.ToDouble(definition.Element("steps").Elements("step").Count());
            double result = ((next_value ) / steps);
            return (int)(((next_value) / steps) * 100);
        }

        public static IQueryable<bpm_travel_type> GetTravelTypes()
        {
            return _context.bpm_travel_type.AsQueryable();
        }

        public static IQueryable<bpm_link_travel_expense> GetTravelExpenses(int travel_type_id)
        {
            return _context.bpm_link_travel_expense.Where(e => e.travel_type_id == travel_type_id && e.status == 1);
        }

        public static string GenerateTCode()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        public static string Encrypt(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
                    0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        /*
         * Modified By Johnbosco
         * 
         */
        public static void AddWorkflowDocument(bpm_workflow_document document)
        {
            try
            {
                /*
                 * Modification starts here
                 */
                var existingdocument = _context.bpm_workflow_document.FirstOrDefault(m =>
                    m.name.Equals(document.name) && m.workflow_id == document.workflow_id);
                if (existingdocument == null)
                {
                    document.id = Guid.NewGuid();
                    document.created_at = DateTime.Now;
                    _context.bpm_workflow_document.Add(document);
                }
                else
                {
                    existingdocument.location = document.location;
                    _context.Entry(existingdocument).State = EntityState.Modified;
                }

                _context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        Utils.Log(message + " - AddWorkflowDocument");
                        Utils.LogError(raise);
                    }
                }
                
            }
        }


        public static string GetStepPerformDate(XElement document,int step)
        {
            var step_element = document.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.ToString());
            var date_element = step_element.Element("created_at");
            if (date_element == null || date_element.Value=="")
            {
                return "";
            }
            else
            {
                var values = date_element.Value.ToString().Split('-');
                var date = new DateTime(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2].Substring(0, 2)));
                return date.ToString("MM/dd/yyyy");
            }
        }

      

        public static bpm_vendor GetSelectedVendor(XElement document)
        {
            try
            {
                var vendor_element = document.Element("vendors").Elements("vendor").FirstOrDefault(e => e.Attribute("selected").Value == "1");
                if (vendor_element != null)
                {
                    var vendor = GetVendor(Convert.ToInt32(vendor_element.Element("id").Value));
                    if (vendor != null)
                    {
                        return vendor;
                    }
                }
            }
            catch (Exception)
            {

            }
           

            return null;
        }

        public static IQueryable<bpm_workflow_document> GetWorkflowDocuments(Guid workflow_id)
        {
            return  _context.bpm_workflow_document.Where(e => e.workflow_id == workflow_id);

        }

        public static bpm_workflow_document GetWorkflowDocument(Guid id)
        {
            return _context.bpm_workflow_document.FirstOrDefault(e => e.id == id);
        }

        public static IQueryable<bpm_item> GetItems()
        {
            return _context.bpm_item.AsQueryable();
        }

        public static double GetTransactionCost(Guid workflow_id)
        {
            double amount = 0;

            var bLWorkflow = new BLWorkflow(workflow_id);

            var workflow = bLWorkflow.GetWorkflow();
            var definition = bLWorkflow.GetDefinition();
            try
            {
                //procurements
                if (workflow.process_id == Guid.Parse("94E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317") || workflow.process_id == Guid.Parse("2D008467-EA12-4821-A2F2-7568B76039F1") || workflow.process_id == Guid.Parse("10471746-0236-4F79-BE27-8AA1CBFF2D92") || workflow.process_id == Guid.Parse("396E4066-81F1-4311-B076-FC3C53BB3178"))
                {
                    foreach (var item in definition.Element("form").Element("items").Elements("item"))
                    {
                        amount += Convert.ToDouble(item.Element("quantity").Value) * Convert.ToDouble(item.Element("unit_price").Value);
                    }
                }
                //payment
                else if (workflow.process_id == Guid.Parse("84E9FEBB-6C8E-42E2-B1A2-4DEE7F6E7317"))
                {
                    foreach (var item in definition.Element("form").Element("items").Elements("item"))
                    {
                        amount += Convert.ToDouble(item.Element("quantity").Value) * Convert.ToDouble(item.Element("amount").Value);
                    }
                }
                //travel
                else if (workflow.process_id == Guid.Parse("A9BE0559-94E4-4A6E-AE46-74A1344DF7EF") || workflow.process_id == Guid.Parse("A9BE0559-94E4-4A6E-AE46-74A1344DF7DF"))
                {
                    foreach (var item in definition.Element("form").Element("expenses").Elements("expense"))
                    {
                        amount += Convert.ToDouble(item.Element("no_of_days").Value) * Convert.ToDouble(item.Element("rate").Value);
                    }
                }
            }
            catch (Exception)
            {

            }
            return amount;
        }

        public static int GetProgramApprover(int project_id,int sub_unit)
        {
            var approval = _context.bpm_workflow_program_approval.FirstOrDefault(e => e.project_id == project_id && e.job_title == sub_unit);
            if (approval != null) return approval.emp_number;
            return 0;
        }

        /*
         * Added by Johnbosco
         * This is a functionality for vendors category search
         */
        public static List<bpm_vendor_category> GetVendorCategory()
        {
            return _context.bpm_vendor_category.ToList();
        }

        public static List<bpm_vendor> GetVendorsByCategory(int category, string term = "")
        {
            var vendorCategory = _context.bpm_lnk_vendor_category.Where(m => m.category_id == category);
            return vendorCategory.Select(m => m.bpm_vendor).Where(m => m.name.Contains(term)).ToList();
        }
    }
}
