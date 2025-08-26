# Script Migrator Tool

The **Script Migrator Tool** is a WPF-based desktop application that allows users to migrate steps from one XML format (`StepModel`) to a new XML format (`NewStepModel`).  
It provides an easy-to-use UI with checkboxes to select which steps to migrate, and generates a new XML file containing only the converted steps.

---

## Features

- Load steps from existing XML files.
- Display steps in a **DataGrid** with checkbox selection.
- Selectively migrate steps by checking the boxes.
- Convert selected steps from `StepModel` to `NewStepModel`.
- Save migrated steps to a new XML file with the suffix **`_migrated`**.
- Support for migrating entire directories at once.

---

## Architecture

- **Models**
  - `StepModel`: Represents the input format of steps (original script).
  - `NewStepModel`: Represents the output format of steps (migrated script).
  - `Parameter`: Represents step parameters.

**Service**
  - `XMLHelper`: Loading and Saving XML Scripts.
- **View**
  - `MainWindow`:
  - Buttons to `Browse` the directory, `Load XML Scripts`,`Convert` steps from the step model to new step model.
  - A **WPF DataGrid** to show steps.
  - Checkboxes bound to `IsSelected` property for migration selection.

- **ViewModel**
  - Handles the binding between UI and logic.
  - Provides commands for:
    - Browsing directory.
    - Loading XML scripts.
    - Migrating selected steps and saving the results to the `Migrated` folder.

## Snapshots
- Browse your script directory.
  
   ![Directory Browsing](Images/Directory%20Browsing.png)
- Scan your XML Scripts

   ![Script Scanning](Images/Script%20Scanning.png)
- Select the steps you wish to convert.

   ![Step Selection](Images/Step%20Selection.png)
- The migrated steps will be saved to the `Migrated` directory.
---


