using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ScriptMigrationUtility.Models
{
    public class NewStepModel : BaseStep
    {
        public string NewId { get; set; }
        public string ActionName { get; set; }

        [XmlArray("Config")]
        [XmlArrayItem("Parameter")]
        public List<Parameter> Config { get; set; } = new();

        public string MigratedAt { get; set; } = DateTime.Now.ToString("s");
        public string MigratedBy { get; set; } = Environment.UserName;

        public NewStepModel() { }
        public NewStepModel(StepModel step)
        {
            NewId = step.Id;
            ActionName = step.Name;
            Config = step.Parameters != null ? new List<Parameter>(step.Parameters) : new List<Parameter>();
            MigratedAt = DateTime.Now.ToString("s");
            MigratedBy = Environment.UserName;
        }
    }

}
