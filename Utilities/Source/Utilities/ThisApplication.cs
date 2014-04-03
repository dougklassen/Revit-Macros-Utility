/*
 * Created by SharpDevelop.
 * User: dklassen
 * Date: 2/12/2014
 * Time: 8:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("7DDD5D8D-869B-4F8C-A991-38934DBD15C4")]
	public partial class ThisApplication
	{
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
		public void eqOverride()
		{
			UIDocument uiDoc = this.ActiveUIDocument;
			Document dbDoc = this.ActiveUIDocument.Document;
			
			ICollection<ElementId> sel = uiDoc.Selection.GetElementIds();
			
			string overrideText = Prompt.ShowDialog("Override", "Value");
			
			using(Transaction t = new Transaction(dbDoc, "EQ"))
			{
		      	t.Start();
				foreach (var id in sel)
				{
					Dimension dim = dbDoc.GetElement(id) as Dimension;
					
					if (dim != null)
					{
						dim.ValueOverride = overrideText;
					}
				}
		      	t.Commit();
			}
		}
		public void resetHand()
		{
			UIDocument uiDoc = this.ActiveUIDocument;
			Document dbDoc = this.ActiveUIDocument.Document;
			
			ICollection<ElementId> sel = uiDoc.Selection.GetElementIds();
			
			using(Transaction t = new Transaction(dbDoc, "Reset Hand of Elements"))
	      	{
		      	t.Start();
				foreach (var id in sel) {
					FamilyInstance inst = dbDoc.GetElement(id) as FamilyInstance;
					
					//some FamilyInstances are flipped even though CanFlipHand is false. CanFlip hand can't be ignored to modify these instances
					if(inst.CanFlipHand) {
					   	//Facing should be maintained, but this requires that the hand also be flipped for the FamilyInstance.
					   	//A window hosted in a flipped wall must have it's facing flipped to locate correctly.
					   	//However, this means that a window with unflipped hand will look like a flipped window.
					   	//Keeping FacingFlipped and HandFlipped synced ensures that the window is oriented correctly.
					   	if ((inst.HandFlipped && !inst.FacingFlipped) || (!inst.HandFlipped && inst.FacingFlipped)) {
							inst.flipHand();							
						}
				    }
				}
		      	t.Commit();
	      	}
		}
		public void createIndividualGroups()
		{
		}
		public void prependViewTemplateNames()
		{			
			UIDocument uiDoc = this.ActiveUIDocument;
			Document dbDoc = this.ActiveUIDocument.Document;
			
			String prependString = Prompt.ShowDialog("String to prepend: ", "Prepend View Template Names");
			
			ElementClassFilter viewFilter = new ElementClassFilter(typeof(Autodesk.Revit.DB.View));
			FilteredElementCollector collector = new FilteredElementCollector(dbDoc);
			IEnumerable<Autodesk.Revit.DB.View> allViews = collector.WherePasses(viewFilter).Cast<Autodesk.Revit.DB.View>().AsEnumerable();
			
			var viewTemplates = allViews.Where(t => t.IsTemplate);
			
			using(Transaction t = new Transaction(dbDoc, "Prepend View Template Names"))
		    {
		      	t.Start();
		      	
				Boolean noParam = false;
		      	foreach (var vt in viewTemplates)
				{
		      		//vt.Name = (prependString + vt.Name);
					Parameter vName = vt.get_Parameter("View Name");
					foreach (Parameter p in vt.Parameters) {
						if ("View Name" == p.Definition.Name) {
							vName = p;
						}
					}
					
					if (null != vName)
					{
						vName.Set(prependString + vName.AsString());
					}
					else
					{
						noParam = true;
					}
				}
		      	
				if (noParam) {
				     TaskDialog.Show("message","couldn't get param");						
				}
				
				t.Commit();
	        }
		}
		
	}
	
	public static class Prompt
	{
	    public static string ShowDialog(string text, string caption)
	    {
	        System.Windows.Forms.Form prompt = new System.Windows.Forms.Form();
	        prompt.Width = 500;
	        prompt.Height = 150;
	        prompt.Text = caption;
	        Label textLabel = new Label() { Left = 50, Top=20, Text=text };
	        System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 50, Top=50, Width=400 };
	        Button confirmation = new Button() { Text = "Ok", Left=350, Width=100, Top=70 };
	        confirmation.Click += (sender, e) => { prompt.Close(); };
	        prompt.Controls.Add(confirmation);
	        prompt.Controls.Add(textLabel);
	        prompt.Controls.Add(textBox);
	        prompt.ShowDialog();
	        return textBox.Text;
	    }
	}
}