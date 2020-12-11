using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using HRM.DAL.Util;
using WorkFlow.DAL;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.Logic;
using WorkFlow.DAL.Service;

namespace OnePortal.Helper
{
    public class NewTravelProcess
    {
        TransactionTokenService _transactionTokenService = new TransactionTokenService();

        public void TravelAuthorization(XElement definition, FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee, Guid id, Step step, XElement definition_step)
        {
            definition.Element("form").Add(new XElement("departure", collection["departure"].ToString()));
            definition.Element("form").Add(new XElement("destination", collection["destination"].ToString()));
            definition.Element("form").Add(new XElement("travel_class", collection["travel_class"].ToString()));
            definition.Element("form").Add(new XElement("start_date", collection["start_date"].ToString()));
            definition.Element("form").Add(new XElement("end_date", collection["end_date"].ToString()));
            definition.Element("form").Add(new XElement("purpose_of_travel", collection["purpose_of_travel"].ToString()));


            var second_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "2");
            //sets the current employee as the person to file the travel advance form
            second_step.Element("next_step_position_id").SetValue(employee.emp_number);
            var third_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "3");
            third_step.Element("position").SetValue(employee.emp_number);

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

            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
        }

        public void TravelAdvance(XElement definition, FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee, Guid id, Step step, XElement definition_step)
        {
            //get the type of travel
            var travel_expenses = Util.GetTravelExpenses(Convert.ToInt32(definition.Element("form").Element("travel_class").Value));
            if (travel_expenses.Any())
            {
                if (definition.Element("form").Element("expenses") == null)
                {
                    definition.Element("form").Add(new XElement("expenses"));
                }
                definition.Element("form").Element("expenses").RemoveAll();
                foreach (var expense in travel_expenses)
                {
                    //check if the dates are null
                    if ((collection[expense.expense_id + "_start_date"] != null && collection[expense.expense_id + "_start_date"].ToString() != "") && (collection[expense.expense_id + "_end_date"] != null && collection[expense.expense_id + "_end_date"].ToString() != "") && (collection[expense.expense_id + "_no_of_days"] != null && collection[expense.expense_id + "_no_of_days"].ToString() != ""))
                    {
                        var element = new XElement("expense");
                        element.Add(new XElement("id", expense.expense_id));
                        element.Add(new XElement("name", expense.bpm_travel_expenses.name));
                        element.Add(new XElement("rate", expense.amount));
                        element.Add(new XElement("start_date", collection[expense.expense_id + "_start_date"].ToString()));
                        element.Add(new XElement("end_date", collection[expense.expense_id + "_end_date"].ToString()));
                        element.Add(new XElement("no_of_days", collection[expense.expense_id + "_no_of_days"].ToString()));
                        definition.Element("form").Element("expenses").Add(element);
                    }
                }
            }
            //check for hotel
            if (collection["hotel"] != null)
            {
                if ((collection["h_start_date"] != null && collection["h_start_date"].ToString() != "") && (collection["h_end_date"] != null && collection["h_end_date"].ToString() != "") & (collection["h_no_of_days"] != null && collection["h_no_of_days"].ToString() != ""))
                {
                    var element = new XElement("expense");
                    element.Add(new XElement("name", "Hotel"));
                    element.Add(new XElement("rate", collection["hotel"].ToString()));
                    element.Add(new XElement("start_date", collection["h_start_date"].ToString()));
                    element.Add(new XElement("end_date", collection["h_end_date"].ToString()));
                    element.Add(new XElement("no_of_days", collection["h_no_of_days"].ToString()));
                    definition.Element("form").Element("expenses").Add(element);
                }
            }

            //check for transportation
            if (collection["transportation"] != null && collection["transportation"].ToString() != "")
            {
                if (collection["t_start_date"] != null && collection["t_end_date"] != null && collection["t_no_of_days"] != null)
                {
                    var element = new XElement("expense");
                    element.Add(new XElement("name", "Transportation"));
                    element.Add(new XElement("rate", collection["transportation"].ToString()));
                    element.Add(new XElement("start_date", collection["t_start_date"].ToString()));
                    element.Add(new XElement("end_date", collection["t_end_date"].ToString()));
                    element.Add(new XElement("no_of_days", collection["t_no_of_days"].ToString()));
                    definition.Element("form").Element("expenses").Add(element);
                }
            }

            //check for other expenses
            if ((collection["other"] != null && collection["other"].ToString() != "") && (collection["other_amount"] != null && collection["other_amount"].ToString() != ""))
            {
                var others = collection["other"].ToString().Split(',');
                var other_amounts = collection["other_amount"].ToString().Split(',');
                //var other_starts= collection["o_start_date"].ToString().Split(',');
                //var other_ends = collection["o_end_date"].ToString().Split(',');
                if (others.Count() == other_amounts.Count())
                {
                    for (var i = 0; i < others.Count(); i++)
                    {
                        if (others[i].Trim() == "") continue;
                        var element = new XElement("expense");
                        //element.Add(new XElement("id", i+1));
                        element.Add(new XElement("name", others[i]));
                        element.Add(new XElement("rate", other_amounts[i]));
                        if (collection["o_start_date"] != null)
                        {
                            var other_starts = collection["o_start_date"].ToString().Split(',');
                            element.Add(new XElement("start_date", other_starts[i]));
                        }
                        else
                        {
                            element.Add(new XElement("start_date", ""));
                        }

                        if (collection["o_start_date"] != null)
                        {
                            var other_ends = collection["o_end_date"].ToString().Split(',');
                            element.Add(new XElement("end_date", other_ends[i]));
                        }
                        else
                        {
                            element.Add(new XElement("end_date", ""));
                        }

                        if (collection["o_no_of_days"] != null)
                        {
                            var days = collection["o_no_of_days"].ToString().Split(',');
                            element.Add(new XElement("no_of_days", days[i]));
                        }
                        else
                        {
                            element.Add(new XElement("no_of_days", "1"));
                        }

                        definition.Element("form").Element("expenses").Add(element);
                    }
                }
            }
            if (definition.Element("form").Element("account_name") == null)
                definition.Element("form").Add(new XElement("account_name", collection["account_name"].ToString()));
            if (definition.Element("form").Element("account_number") == null)
                definition.Element("form").Add(new XElement("account_number", collection["account_number"].ToString()));
            if (definition.Element("form").Element("bank_name") == null)
                definition.Element("form").Add(new XElement("bank_name", collection["bank_name"].ToString()));
            if (definition.Element("form").Element("conversion_rate") == null)
                definition.Element("form").Add(new XElement("conversion_rate", collection["conversion_rate"].ToString())); 

            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
        }

        public void ReimbursementAuthorization(XElement definition, FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee, Guid id, Step step, XElement definition_step)
        {
            definition.Element("form").Add(new XElement("purpose_of_reimbursement", collection["purpose_of_reimbursement"]));


            var second_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "2");
            //sets the current employee as the person to fill the travel advance form
            second_step.Element("next_step_position_id").SetValue(employee.emp_number);
            var third_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "3");
            third_step.Element("position").SetValue(employee.emp_number);
            //set the current employee as the person to complete the process
            var seven_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "7");
            seven_step.Element("next_step_position_id").SetValue(employee.emp_number);
            var last_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "8");
            last_step.Element("position").SetValue(employee.emp_number);
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

            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
        }

        public void Reimbursement(XElement definition, FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee, Guid id, Step step, XElement definition_step)
        {
            if (definition.Element("form") != null && definition.Element("form").Element("expenses") == null)
            {
                definition.Element("form").Add(new XElement("expenses"));
            }
            definition.Element("form").Element("expenses").RemoveAll();

            /*
             * Get form elements from the FormCollection
             */
            var units = collection["unit"].Split(',');
            var quantities = collection["quantity"].Split(',');
            var descriptions = collection["description"].Split(',');
            var amounts = collection["unit_cost"].Split(',');
            /*
             * Empty the items element in case of rejected forms
             * Write the item xml element
             * and put them/it inside items tag
             */
            for (var i = 0; i < units.Count(); i++)
            {
                var item = new XElement("expense");
                item.Add(new XAttribute("id", i + 1));
                item.Add(new XElement("unit", units[i]));
                item.Add(new XElement("quantity", quantities[i]));
                item.Add(new XElement("description", descriptions[i]));
                item.Add(new XElement("est_unit_cost", amounts[i]));

                definition.Element("form").Element("expenses").Add(item);
            }
            
            if (definition.Element("form").Element("account_name") == null)
                definition.Element("form").Add(new XElement("account_name", collection["account_name"]));
            if (definition.Element("form").Element("account_number") == null)
                definition.Element("form").Add(new XElement("account_number", collection["account_number"]));
            if (definition.Element("form").Element("bank_name") == null)
                definition.Element("form").Add(new XElement("bank_name", collection["bank_name"]));
            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
        }

        public void AdvanceRequest(XElement definition, FormCollection collection, HRM.DAL.Models.admin_hrm_employee employee, Guid id, Step step, XElement definition_step)
        {
            definition.Element("form").Add(new XElement("purpose_of_advance", collection["purpose_of_advance"]));

            var second_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "2");
            //sets the current employee as the person to fill the travel advance form
            second_step.Element("next_step_position_id").SetValue(employee.emp_number);
            var third_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "3");
            third_step.Element("position").SetValue(employee.emp_number);
            //set the current employee as the person to complete the process
            var seven_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "7");
            seven_step.Element("next_step_position_id").SetValue(employee.emp_number);
            var last_step = definition.Element("steps").Elements("step").FirstOrDefault(e => e.Element("code").Value == "8");
            last_step.Element("position").SetValue(employee.emp_number);
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

            //mark step as completed
            definition_step.Element("status").SetValue(1);
            definition.Element("next_step").SetValue(step.Definition.Element("next_step").Value);
        }
    }
}