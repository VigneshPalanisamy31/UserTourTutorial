using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ScriptMigrationUtility.Models
{
    [XmlInclude(typeof(StepModel))]
    [XmlInclude(typeof(NewStepModel))]
    public abstract class BaseStep
    {
        [XmlIgnore]
        public bool IsSelected { get; set; }

        [XmlIgnore]
        public string SourceFile { get; set; }
    }
}
