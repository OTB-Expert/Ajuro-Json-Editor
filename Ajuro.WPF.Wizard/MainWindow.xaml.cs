using Ajuro.WPF.Wizard.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ajuro.WPF.Wizard
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainModel MainViewModel { get; set; }
		public static int IdSequence = 0;
		JObject SampleJson { get; set; }
		public MainWindow()
		{
			InitializeComponent();
			MainViewModel = MainModel.Instance;
			MainModel.DataJsonPath = "C:\\OTB\\templates\\profile.json";
			if(!File.Exists(MainModel.DataJsonPath))
			{
				MainModel.DataJsonPath = "Resources\\data.json";
			}


			if (File.Exists(MainModel.DataJsonPath))
			{
				MainViewModel.SampleJson = File.ReadAllText(MainModel.DataJsonPath);
				SampleJson = JObject.Parse(MainViewModel.SampleJson);
				MainViewModel.SampleTree = UniversalTreeNode.CreateTree(SampleJson);
				MainViewModel.SampleJson = JsonConvert.SerializeObject(SampleJson, Formatting.Indented);
			}

			if (!File.Exists("Resources\\meta2.json"))
			{
				var stream = File.Create("Resources\\meta2.json");
				stream.Close();
				File.WriteAllText("Resources\\meta2.json", "{}");
			}

			if (File.Exists("Resources\\meta2.json"))
			{
				MainViewModel.MetaJson = File.ReadAllText("Resources\\meta2.json");
				var metaJson = JObject.Parse(MainViewModel.MetaJson);
				MainViewModel.MetaJson = JsonConvert.SerializeObject(metaJson, Formatting.Indented);
			}
			var steps = Newtonsoft.Json.JsonConvert.DeserializeObject<WizardStep>(MainViewModel.MetaJson).Children;
			if (steps == null)
			{
				steps = new ObservableCollection<WizardStep>();
			}
			foreach (var step in steps)
			{
				MainModel.Instance.WizardSteps.Add(step);
			}
			this.DataContext = MainViewModel;
		}

		private void ExpandAll(ItemsControl items, bool expand)
		{
			foreach (object obj in items.Items)
			{
				ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
				if (childControl != null)
				{
					ExpandAll(childControl, expand);
				}
				TreeViewItem item = childControl as TreeViewItem;
				if (item != null)
					item.IsExpanded = true;
			}
		}

		private void StepsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if(StepsTree.SelectedItem != null)
			{
				MainModel.Instance.SelectedStep = (WizardStep)StepsTree.SelectedItem;
				if (MainModel.Instance.SelectedStep.WizardSections.Count == 0)
				{
					/*
					MainModel.Instance.SelectedStep.WizardSections.Add(new WizardSection()
					{
						Id = (MainWindow.IdSequence++),
						Name = "Section 1",
						Info = "FB title",
						Type = WizardSection.DataType.Text32,
						XPath= "$.Collections[*].Placeholder"
					});
					*/
				}
				foreach (var step in MainModel.Instance.SelectedStep.WizardSections)
				{
					try
					{
						if (step.CurrentValues == null)
						{
							step.CurrentValues = new ObservableCollection<EditorMeta>();
						}
						step.CurrentValues.Clear();
						var ducts = SampleJson.SelectTokens(step.XPath);
						IEnumerable<JToken> pricyProducts = SampleJson.SelectTokens(step.XPath);
						int i = 0;
						foreach (JToken item in pricyProducts)
						{
							if (item.Type == JTokenType.Object)
							{
								foreach(var property in item)
								{
									// if (property.Children() > 0 && property.Children[0].Type == JTokenType.Array)
									if (true)
									{
										var editor = new EditorMeta()
										{
											IsObject = false,
											ShowLabel = true,
											CurrentLabel = ((JProperty)property).Name.ToString(),
											CurrentValue = ((JProperty)property).Value.ToString(),
											Index = i
										};
										step.CurrentValues.Add(editor);
										if(editor.CurrentValue.Contains("[") && editor.CurrentValue.Contains("{") && editor.CurrentValue.Length > 200)
										{
											editor.IsObject = true;
										}
									}
									else
									{ 
										step.CurrentValues.Add(new EditorMeta()
										{
											IsObject = false,
											ShowLabel = true,
											CurrentLabel = ((JProperty)property).Name.ToString(),
											CurrentValue = ((JProperty)property).Value.ToString(),
											Index = i
										});
									}
									i++;
								}
							}
							else
							{
								step.CurrentValues.Add(new EditorMeta()
								{
									CurrentLabel = step.XPath.Substring(step.XPath.LastIndexOf('.')),
									CurrentValue = item.ToString(),
									Index = i
								});
								i++;
							}
						}
					}
					catch (Exception ee)
					{
					}
					try
					{
						JToken acme = SampleJson.SelectToken(step.XPath);
					}
					catch (Exception ee)
					{
					}
				}
				/*
				

				CurrentValue = val;
				*/
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			File.WriteAllText("Resources\\meta2.json", Newtonsoft.Json.JsonConvert.SerializeObject(new WizardStep() { Children = MainModel.Instance.WizardSteps }));
		}

		private void SearchNode_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				string searchText = SearchNode.Text.Trim();

				WizardStep node = null;
				ObservableCollection<WizardStep> items = new ObservableCollection<WizardStep>();

				if (MainModel.Instance.SelectedStep == null)
				{
					items = MainModel.Instance.WizardSteps;
				}
				else
				{
					if(MainModel.Instance.SelectedStep.Children == null)
					{
						MainModel.Instance.SelectedStep.Children = new ObservableCollection<WizardStep>();
					}
					items = MainModel.Instance.SelectedStep.Children;
				}

				node = SearchingFor(searchText.ToLower(), items);

				if(node != null)
				{

				}
				else
				{
					var newItem = new WizardStep() {
						Name = searchText
					};
					items.Add(newItem);
				}
			}
		}

		private WizardStep SearchingFor(string searchText, ObservableCollection<WizardStep> items)
		{
			foreach (var step in items)
			{
				if (step.Name.ToLower().Contains(searchText))
				{
					return step;
				}
			}
			return null;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void EntryTextBox_MouseDown(object sender, MouseButtonEventArgs e)
		{

		}

		private void TextBox_MouseEnter(object sender, MouseEventArgs e)
		{
			var control = (TextBox)sender;
			var name = control.Text;
			var parent = (StackPanel)control.Parent;
			parent = (StackPanel)parent.Parent;
			var labelTextBox = (TextBlock)parent.Children[2];
			var label = labelTextBox.Text;
			var pathTextBox = (TextBox)parent.Children[0];
			var id = -1;
			var idTextBox = (TextBox)parent.Children[1];
			int.TryParse(idTextBox.Text, out id);
			var path = pathTextBox.Text;
			for (int i = 0; i < MainViewModel.SelectedStep.WizardSections.Count; i++)
			{
				if (MainViewModel.SelectedStep.WizardSections[i].XPath == path)
				{
					MainViewModel.SelectedSectionIndex = i;
					MainViewModel.SelectedSection = MainViewModel.SelectedStep.WizardSections[i];
					break;
				}
			}
			for (int i = 0; i < MainViewModel.SelectedSection.CurrentValues.Count; i++)
			{
				if (MainViewModel.SelectedSection.CurrentValues[i].Id == id)
				{
					MainViewModel.SelectedEntryIndex = i;
					MainViewModel.SelectedEntry = MainViewModel.SelectedSection.CurrentValues[i];
					break;
				}
			}
			var node = FindTreeViewNode(MainViewModel.SampleTree, path.Split('.'), MainViewModel.SelectedEntryIndex, label, name);
		}

		private List<UniversalTreeNode> FindTreeViewNode(UniversalTreeNode node, string[] path, int index, string label, string name, int i = 1)
		{
			List<UniversalTreeNode> selectedItem = new List<UniversalTreeNode>();
			foreach(var item in node.Children)
			{
				var pathFragment = path[i].Split('[');
				if (pathFragment.Length > 1)
				{
					pathFragment[1] = pathFragment[1].Substring(0, pathFragment[1].Length - 1);
				}
				var uitem = (UniversalTreeNode)item;
				if (uitem.Name == pathFragment[0])
				{
					selectedItem.Add(uitem);
					uitem.IsExpanded = true;
					if (pathFragment.Length > 1 && pathFragment[1]!= "*")
					{
						int idx = -1;
						if(int.TryParse(pathFragment[1], out idx))
						{
							for (var t = 0; t < uitem.Children.Count; t++)
							{
								if (t == idx)
								{
									uitem.Children[t].IsExpanded = true;
									uitem.Children[t].IsSelected = true;
									MainViewModel.SelectedNode = uitem.Children[t];

									if (label != null)
									{
										for (int ii = 0; ii < uitem.Children[t].Children.Count; ii++)
										{
											if(uitem.Children[t].Children[ii].Name == label)
											{
												uitem.Children[t].Children[ii].IsSelected = true;
												MainViewModel.SelectedNode = uitem.Children[t].Children[ii];
											}
										}
									}
								}
								else
								{
									uitem.Children[t].IsExpanded = false;
								}
							}
						}
					}

					if(i == path.Length-2)
					{
						for (var t = 0; t < uitem.Children.Count; t++)
						{
							if (t == index)
							{
								uitem.Children[t].IsExpanded = true;

								for (var p = 0; p < uitem.Children[t].Children.Count; p++)
								{
									if (uitem.Children[t].Children[p].Name == path[path.Length-1])
									{
										uitem.Children[t].Children[p].IsSelected = true;
										MainViewModel.SelectedNode = uitem.Children[t].Children[p];
									}
								}
							}
							else
							{
								uitem.Children[t].IsExpanded = false;
							}
						}
					}
					else if(path.Length == 2 && label.StartsWith("."))
					{
						uitem.IsSelected = true;
						MainViewModel.SelectedNode = uitem;
					}
				}
				else
				{
					uitem.IsExpanded = false;
				}
			}
			return selectedItem;
		}

		private void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			MainViewModel.SelectedNode.Value = ((TextBox)sender).Text;
		}
	}
}
