using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WorkFlow.DAL;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.Logic;
using WorkFlow.DAL.Service;

namespace OnePortal.Helper
{
    public class Procurement
    {
        TransactionTokenService _transactionTokenService = new TransactionTokenService();
        public XElement Run()
        {
     
            return null;
        }

        public void AddItemsToPayment(XElement definition,Step step,XElement definition_step,FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee,Guid id)
        {
            
            
            //var step_type = step.Definition.Element("type").Value;
           // var definition = bLWorkflow.GetDefinition();
            //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

            var units = collection["unit"].ToString().Split(',');
            var quantities = collection["quantity"].ToString().Split(',');
            var itemnames = collection["itemname"].ToString().Split(',');
            var descriptions = collection["description"].ToString().Split(',');
            var amounts = collection["amount"].ToString().Split(',');

            for (var i = 0; i < units.Count(); i++)
            {
                var item = new XElement("item");
                item.Add(new XAttribute("id", i + 1));
                item.Add(new XElement("unit", units[i]));
                item.Add(new XElement("quantity", quantities[i]));
                item.Add(new XElement("itemname", itemnames[i]));
                item.Add(new XElement("description", descriptions[i]));
                item.Add(new XElement("amount", amounts[i]));


                definition.Element("form").Element("items").Add(item);
            }


            var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
            {
                t_code = Util.Encrypt(Util.GenerateTCode()),
                transaction_id = id,
                created_by = employee.emp_number
            });

            //var form_id = form.Attribute("id").Value;
            if (definition.Element("form").Element("signatures") == null)
            {
                definition.Element("form").Add(new XElement("signatures"));
            }

            var signature_element = new XElement("signature");
            signature_element.Add(new XElement("signature_id", token_id));
            signature_element.Add(new XElement("step", step.Id));
            signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
            signature_element.Add(new XElement("emp_number", employee.emp_number));
            signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
            definition.Element("form").Element("signatures").Add(signature_element);



            definition.Element("form").Add(new XElement("justification", collection["justification"].ToString()));
            //mark step as completed




            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);

            //return definition;
        }


        public void AddItemsToProcure(XElement definition, Step step, XElement definition_step, FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee, Guid id)
        {


            //var step_type = step.Definition.Element("type").Value;
            // var definition = bLWorkflow.GetDefinition();
            //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

            var units = collection["unit"].ToString().Split(',');
            var quantities = collection["quantity"].ToString().Split(',');
            var itemnames = collection["itemname"].ToString().Split(',');
            var descriptions = collection["description"].ToString().Split(',');
            var amounts = collection["unit_cost"].ToString().Split(',');

            for (var i = 0; i < units.Count(); i++)
            {
                var item = new XElement("item");
                item.Add(new XAttribute("id", i + 1));
                item.Add(new XElement("unit", units[i]));
                item.Add(new XElement("quantity", quantities[i]));
                item.Add(new XElement("itemname", itemnames[i]));
                item.Add(new XElement("description", descriptions[i]));
                item.Add(new XElement("unit_cost", amounts[i]));


                definition.Element("form").Element("items").Add(item);
            }


            var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
            {
                t_code = Util.Encrypt(Util.GenerateTCode()),
                transaction_id = id,
                created_by = employee.emp_number
            });

            //var form_id = form.Attribute("id").Value;
            if (definition.Element("form").Element("signatures") == null)
            {
                definition.Element("form").Add(new XElement("signatures"));
            }

            var signature_element = new XElement("signature");
            signature_element.Add(new XElement("signature_id", token_id));
            signature_element.Add(new XElement("step", step.Id));
            signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
            signature_element.Add(new XElement("emp_number", employee.emp_number));
            signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
            definition.Element("form").Element("signatures").Add(signature_element);



            definition.Element("form").Add(new XElement("justification", collection["justification"].ToString()));
            //mark step as completed




            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);

            //return definition;
        }

        public void AddUserToSteps(int[] steps,XElement definition,int emp_number,int next_step)
        {
            foreach (var st in steps)
            {
                try
                {

                    var step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == st.ToString());
                    step.Element("position").SetValue(emp_number);
                    step.Element("position").Attribute("pos_type").SetValue("emp");

                }
                catch (Exception)
                {

                }

            }
            definition.Element("next_step").SetValue(next_step);
        }




        public void CreateVPRF(XElement definition, Step step, XElement definition_step, FormCollection collection)
        {
            //var bLWorkflow = new BLWorkflow(workflow.id);
            //var step = bLWorkflow.GetCurrentStep();
            //var step_type = step.Definition.Element("type").Value;
            //var definition = bLWorkflow.GetDefinition();
            //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

            definition.Element("form").Add(new XElement("reason_for_purchase", collection["reason_for_purchase"].ToString()));
            definition.Element("form").Add(new XElement("final_delivery_location", collection["final_delivery_location"].ToString()));
            definition.Element("form").Add(new XElement("date_received_by", collection["date_received_by"].ToString()));
            //date_received_by

            if (definition.Element("form").Element("payments") == null)
            {
                definition.Element("form").Add(new XElement("payments"));
            }
            var payment_element = new XElement("payment");
            payment_element.Add(new XElement("id", definition.Element("form").Element("payments").Elements("payment").Count() + 1));
            payment_element.Add(new XElement("vprf_amount", collection["vprf_amount"].ToString()));
            payment_element.Add(new XElement("p_date", DateTime.Now.ToString("yyyy-MM-dd")));
            payment_element.Add(new XElement("status", "Not paid"));

            definition.Element("form").Element("payments").Add(payment_element);
            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);

            //return definition;
        }

        public XElement ReturnToStep(int step_id,XElement definition)
        {
            definition.Element("next_step").SetValue(step_id);
            //get the highest step in the workflow
            var h_step_id =Convert.ToInt32(definition.Element("steps").Elements("step").LastOrDefault().Element("code").Value);

            for(var i=step_id;i <= h_step_id; i++)
            {
                var def_step = definition.Element("steps").Elements("step").FirstOrDefault(e=>e.Element("code").Value==i.ToString());
                def_step.Element("created_at").SetValue(null);
                def_step.Element("created_by").SetValue(null);
                def_step.Element("status").SetValue(null);
            }

            return definition;
        }

        public string Approve(XElement definition, Step step, XElement definition_step, FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee, Guid id,bpm_workflow workflow)
        {
            //var bLWorkflow = new BLWorkflow(workflow.id);
            //var step = bLWorkflow.GetCurrentStep();
            //var step_type = step.Definition.Element("type").Value;
            //var definition = bLWorkflow.GetDefinition();
            //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);

            //#if !DEBUG
            var approval_token = collection["approval_token"];
            if (approval_token != null)
            {
                var a_token = _transactionTokenService.GetApprovalToken(approval_token.ToString());
                if (a_token != null && a_token.consumed == 0)
                {
                    var rs = collection["approval"].ToString();
                    if (rs == step.Definition.Element("approve_value").Value)
                    {
                        //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
                        definition_step.Element("status").SetValue(1);

                        var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
                        {
                            t_code = Util.Encrypt(Util.GenerateTCode()),
                            transaction_id = id,
                            created_by = employee.emp_number
                        });
                        if (definition.Element("form").Element("signatures") == null)
                        {
                            definition.Element("form").Add(new XElement("signatures"));
                        }
                        var signature_element = new XElement("signature");
                        signature_element.Add(new XElement("signature_id", token_id));
                        signature_element.Add(new XElement("step", step.Id));
                        signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
                        signature_element.Add(new XElement("emp_number", employee.emp_number));
                        signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
                        definition.Element("form").Element("signatures").Add(signature_element);

                        if (definition_step.Element("form_values").Element("signatures") == null)
                        {
                            definition_step.Element("form_values").Add(new XElement("signatures"));
                        }

                        foreach (var form in definition_step.Element("form_values").Elements("form"))
                        {
                            var form_id = form.Attribute("id").Value;
                            definition_step.Element("form_values").Element("signatures").Add(signature_element);
                            //definition_step.Element("form_values").Elements("form").FirstOrDefault(e => e.Attribute("id").Value == form_id).Element("signatures").Add(signature_element);
                            //definition.Element("forms").Elements("form").FirstOrDefault(e => e.Attribute("id").Value == form_id).Element("signatures").Add(signature_element);
                        }
                        definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
                        definition_step.Element("status").SetValue(1);
                    }
                    else if (rs == "dismissed")
                    {
                        workflow.status = 24;
                    }
                    else
                    {
                        if (definition.Element("current_step") == null)
                        {
                            definition.Add(new XElement("current_step", step.Definition.Element("next_step").Value));
                        }
                        else
                        {
                            definition.Element("current_step").SetValue(step.Definition.Element("code").Value);
                        }
                        definition.Element("next_step").SetValue(collection["fall_back"].ToString());
                    }

                    a_token.consumed = 1;
                    _transactionTokenService.UpdateApprovalToken(a_token);
                }
                else
                {
                    return "Your token is expired. Please click refresh to generate a new one.";
                    //return RedirectToAction("Step", new { id = id });
                }
            }
            else
            {
                return "Invalid operation. Please review and re submit.";
                //return RedirectToAction("Step", new { id = id });
            }
            //#endif


            //var result = collection["approval"].ToString();
            //if (result == step.Definition.Element("approve_value").Value)
            //{
            //    //var definition_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == step.Definition.Element("code").Value);
            //    definition_step.Element("status").SetValue(1);

            //    var token_id = _transactionTokenService.AddToken(new bpm_transaction_token
            //    {
            //        t_code = Util.Encrypt(Util.GenerateTCode()),
            //        transaction_id = id,
            //        created_by = employee.emp_number
            //    });

            //    var signature_element = new XElement("signature");
            //    signature_element.Add(new XElement("signature_id", token_id));
            //    signature_element.Add(new XElement("step", step.Id));
            //    signature_element.Add(new XElement("name", string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
            //    signature_element.Add(new XElement("emp_number", employee.emp_number));
            //    signature_element.Add(new XElement("created_at", DateTime.Now.ToShortDateString()));
            //    definition.Element("form").Element("signatures").Add(signature_element);

            //    foreach (var form in definition_step.Element("form_values").Elements("form"))
            //    {

            //        var form_id = form.Attribute("id").Value;
            //        definition_step.Element("form_values").Element("signatures").Add(signature_element);
            //        //definition_step.Element("form_values").Elements("form").FirstOrDefault(e => e.Attribute("id").Value == form_id).Element("signatures").Add(signature_element);
            //        //definition.Element("forms").Elements("form").FirstOrDefault(e => e.Attribute("id").Value == form_id).Element("signatures").Add(signature_element);

            //    }
            //    definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
            //    definition_step.Element("status").SetValue(1);
            //}
            //else if (result == "dismissed")
            //{
            //    //workflow.status = 24;
            //}
            //else
            //{
            //    definition.Element("next_step").SetValue(step.Definition.Element("fallback_step").Value);
            //}


            return "success";
            //return definition;
        }


        public void CreatePO(XElement definition, Step step, FormCollection collection, XElement definition_step)
        {
            var account_name = collection["account_name"];
            var account_number = collection["account_number"];
            var bank_name = collection["bank_name"];
            var term_of_payment = collection["term_of_payment"];

            definition.Element("form").Add(new XElement("account_name", account_name!=null ? account_name.ToString() : ""));
            definition.Element("form").Add(new XElement("account_number", account_number !=null ? account_number.ToString() : ""));
            definition.Element("form").Add(new XElement("bank_name", bank_name !=null ? bank_name.ToString() : ""));
            definition.Element("form").Add(new XElement("term_of_payment", term_of_payment!=null ? term_of_payment.ToString() : ""));


            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
        }

        public void UpdateCost(XElement definition, Step step, FormCollection collection, XElement definition_step)
        {
            foreach (var item in definition.Element("form").Element("items").Elements("item"))
            {
                var item_id = item.Attribute("id").Value;
                item.Element("itemname").SetValue(collection["itemname_" + item_id].ToString());
                item.Element("quantity").SetValue(collection["quantity_" + item_id].ToString());
                item.Element("unit").SetValue(collection["unit_" + item_id].ToString());
                item.Element("description").SetValue(collection["description_" + item_id].ToString());
                //update the estimated prices
                item.Add(new XElement("est_unit_price", collection["est_unit_price_" + item_id].ToString()));
                item.Add(new XElement("est_total_price", collection["est_total_price_" + item_id].ToString()));
            }

            definition.Element("form").Add(new XElement("conversion_rate", collection["conversion_rate"].ToString()));

            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
        
        }
        
    }
}