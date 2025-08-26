using ScriptMigrationUtility.Models;
using System.IO;
using System.Xml.Serialization;

namespace ScriptMigrationUtility.Services
{
    public static class XmlHelper
    {
        public static List<StepModel> LoadStepsFromXml(string filePath)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<StepModel>));

                using var reader = new StreamReader(filePath);
                return (List<StepModel>)serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ex)
            {
                string expectedFormat = "Expected format:\n<ArrayOfStepModel>\n   <StepModel>...</StepModel>\n</ArrayOfStepModel>";

                var result = MessageBox.Show(
                    $"File '{Path.GetFileName(filePath)}' is not in the expected format.\n\n{expectedFormat}\n\n" +
                    $"Do you want to skip this file and continue with the rest?\n\nError details: {ex.Message}",
                    "Migration Error",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);

                if (result == DialogResult.Yes)
                {
                    return new List<StepModel>(); // Skip file
                }
                else
                {
                    MessageBox.Show("Migration cancelled by user.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    throw; // Stop everything
                }
            }
        }

        public static void SaveStepsToXml(List<BaseStep> steps, string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<BaseStep>), new Type[] { typeof(StepModel), typeof(NewStepModel) });
            using var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, steps);
        }

    }
}
