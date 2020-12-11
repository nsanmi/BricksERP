using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WorkFlow.DAL.Logic
{
    public static class StepHelper
    {
        public static XElement GenerateStepDefinition(XElement form,XElement display,XElement input,string form_id)
        {
            var form_definition = new XElement("form");
            form_definition.Add(new XAttribute("id",form_id));
           
            //get all the items that belongs to a collection
            foreach(var item in display.Elements("item"))
            {
                //get the id of the item which is an attribute
                var item_id = item.Attribute("id").Value;
                //get all the input items for this particular item
                XElement input_item = null;
                if (input != null)
                    input_item = input.Elements("item").FirstOrDefault(e => e.Attribute("id").Value == item_id);
                //get the entered values stored for this item
                var form_items = form.Elements("item").Where(e => e.Attribute("id").Value == item_id);

                foreach(var form_item in form_items)
                {
                    var item_element = new XElement("item");
                    item_element.Add(new XAttribute("id", form_item.Attribute("id").Value));
                    foreach (var el in form_item.Elements())
                    {

                        //check if the element is in display
                        XElement display_element = null;
                        if (item != null)display_element = item.Element(el.Name);
                        //check if element is in input
                        XElement input_element =null;
                        if (input_item != null ) input_element = input_item.Element(el.Name);

                        var element = new XElement(el.Name);
                        element.Add(new XAttribute("hasvalue",false));
                        element.Add(new XAttribute("control_type", el.Attribute("control_type").Value));
                        //element.Attribute("control_type").SetValue(el.Attribute("control_type").Value);

                        if (el.Value != null)
                        {
                            //element.Add(new XAttribute("hasvalue", true));
                            element.Attribute("hasvalue").SetValue(true);
                        }

                        if (input_element != null || (display_element != null && input_element != null))
                        {

                            element.Add(new XAttribute("readonly", false));
                            element.SetValue(el.Value);
                            item_element.Add(element);
                        }
                        else if (display_element != null)
                        {

                            element.Add(new XAttribute("readonly", true));
                            element.SetValue(el.Value);
                            item_element.Add(element);
                        }

                        else if (input_element != null)
                        {

                            element.Add(new XAttribute("readonly", false));
                            element.SetValue(el.Value);
                            item_element.Add(element);
                        }


                    }
                    form_definition.Add(item_element);
                }

            }

            foreach(var el in form.Elements().Where(e=>e.Name != "item" && e.Name != "signatures" && e.Name != "attachments"))
            {

                //check if the element is in display
                XElement display_element = null;
                if (display != null) display_element = display.Element(el.Name);
                //check if element is in input
                XElement input_element = null;
                if (input != null) input_element = input.Element(el.Name);

                var element = new XElement(el.Name);
                element.Add(new XAttribute("hasvalue", false));
                element.Add(new XAttribute("control_type", el.Attribute("control_type").Value));
                //element.Attribute("control_type").SetValue(el.Attribute("control_type").Value);

                if (el.Value != null)
                {
                    element.Attribute("hasvalue").SetValue(true);
                }

                if (input_element != null || (display_element != null && input_element != null))
                {

                    element.Add(new XAttribute("readonly", false));
                    element.SetValue(el.Value);
                    form_definition.Add(element);
                }
                else if (display_element != null)
                {

                    element.Add(new XAttribute("readonly", true));
                    element.SetValue(el.Value);
                    form_definition.Add(element);
                }

                    
            }


            return form_definition;
        }

        public static string GenerateForm(XElement definition)
        {
            var form_data = "";
            //get the distinct item_ids for the forms
            var item_ids = definition.Elements("item").Select(e => e.Attribute("id").Value).Distinct();
            foreach(var item_id in item_ids)
            {
                //get the first item with this id
                var item = definition.Elements("item").FirstOrDefault(e => e.Attribute("id").Value == item_id);
                //using the first item, draw the header of the table
                form_data += "<table border='1' class='table table-bordered'>";
                form_data += GenerateTableHeader(item);
                form_data += "<tbody>";
                foreach(var itm in definition.Elements("item").Where(e => e.Attribute("id").Value == item_id))
                {
                    form_data += GenerateTableRow(itm);
                }
                form_data += "</tbody>";
                form_data += "</table><br/>";
            }

            foreach (var el in definition.Elements().Where(e => e.Name != "item"))
            {
                
                form_data += string.Format("<div class='form-group'><label>{0}</label>", el.Name.ToString().Replace('_', ' ').ToUpper());
                var readony = Convert.ToBoolean(el.Attribute("readonly").Value);
                var readonly_value = "";
                if (readony) readonly_value = "readonly";

                if (el.Attribute("control_type").Value == "comment" && Convert.ToBoolean(el.Attribute("readonly").Value))
                {
                    form_data += string.Format("<textarea class='form-control' {2}>{1}</textarea>",el.Name,GetXMLValue(el),readonly_value);
                }
                else if (el.Attribute("control_type").Value == "comment" && !Convert.ToBoolean(el.Attribute("readonly").Value))
                {
                    form_data += string.Format("<textarea class='form-control' name='{0}' {2}>{1}</textarea>", el.Name, GetXMLValue(el), readonly_value);
                }
                else if (el.Attribute("control_type").Value == "text" && Convert.ToBoolean(el.Attribute("readonly").Value))
                {
                    form_data += string.Format("<input type='text' class='form-control' value='{1}' {2}/>", el.Name, GetXMLValue(el), readonly_value);
                }
                else if (el.Attribute("control_type").Value == "text" && !Convert.ToBoolean(el.Attribute("readonly").Value))
                {
                    form_data += string.Format("<input type='text' class='form-control' name='{0}' value='{1}' {2}/>", el.Name, GetXMLValue(el), readonly_value);
                }



                form_data += "</div>";
            }

            return form_data;
        }

        private static string GenerateTableHeader(XElement table_element)
        {
            var table_header = "<thead><tr>";
            foreach(var element in table_element.Elements())
            {
                table_header += "<th>" + element.Name.ToString().Replace('_',' ').ToUpper() + "</th>";
            }
            table_header += "</tr></thead>";
            return table_header;
        }

        private static string GenerateTableRow(XElement row_element)
        {
            var row = "<tr>";

            foreach(var element in row_element.Elements())
            {
                row += "<td>";

                if (!Convert.ToBoolean(element.Attribute("readonly").Value))
                {
                    if (element.Attribute("control_type").Value == "text")
                        row += string.Format("<input type='text' class='form-control' name='itm-{0}' value='{1}'/>", element.Name, GetXMLValue(element));
                    else
                        row += string.Format("<textarea class='form-control' name='itm-{0}'>{1}</textarea>", element.Name, GetXMLValue(element));
                }
                else
                {
                    row += GetXMLValue(element);
                }


                row += "</td>";
            }

            return row + "</tr>";
        }

        public static string GetXMLValue(XElement element)
        {
            return element != null || element.Value != null ? element.Value.ToString() : "";
        }
    }
}
