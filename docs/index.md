## Ajuro WPF Json Wizard

Json format is frequently used to serialize settings. Settings files can often grow and become difficult to maintain mainly because the file does not contain informations on how the json objects and properties should be used. With time the meaning of a complex json structure become is lost if no metainformation is added. So we need instructions to work on a json structure. And we want the instructions to be stored in a separate json just to keep the settings file small. 

## The Data Json
This is the json structure you whnat to edit. It does not contain instructuins.
## The Meta Json
This contains all information needed by the wizard to help you edit your Data Json. It contains the sequence of steps you need to complete when updating your Data Json.

## Why using this wizard?

- Organize your editing journey into steps.
- Hide from user the entire complexity of the Data Json.
- Have a flat step-by-step easy to follow, easy to traverse and complete process instead of a branched process. 
- Add human readable instructions.
- Edit wizard steps, or just use the steps to edit the Data Json.
- Use of XPath selectors to identify and edit your Data Jason properties.
- Locate the data you are editing in the Data Json structure viewed as TreeView.

## Applications

Step-by-Step structured data entry.

## How it works?

### Wizard steps
You plan your editing journey into steps. Each step has it's own page. You can edit unrelated fragments of your Data Json in the same step if this makes more sense for your users. The title of the step will appear as the title of the page.

### Wizard sections

Each step has multiple sections. A section contains a instructions paragraph and a XPath selector.
Based on the XPath selector one or multiple editors can be generated in the same section. For example $.Items[*].Name will generate one editor per item while $.Items[0].Name will generate one single editor.

### Wizard editors

Editors are generated based on a wizard section XPath. Editors are TextBox type. On mouse enter event the coresponding property is selected in the preview TreeView.