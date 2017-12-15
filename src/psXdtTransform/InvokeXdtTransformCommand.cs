using Microsoft.Web.XmlTransform;
using System.IO;
using System.Management.Automation;
using System.Xml;

namespace psXdtTransform
{
    [Cmdlet(verbName: "Invoke", nounName: "XdtTransform")]
    public class InvokeXdtTransformCommand : Cmdlet
    {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Source { get; set; }

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true)]
        public string[] Transform { get; set; }

        [Parameter(Position = 2, ValueFromPipelineByPropertyName = true)]
        public string Destination { get; set; }

        protected override void ProcessRecord()
        {
            using (var document = new XmlTransformableDocument())
            {
                if (File.Exists(this.Source))
                {
                    // TODO:
                    //WriteError($"The source file, '{this.Source}', could not be found.");
                }
                if (File.Exists(this.Destination))
                {
                    //WriteWarning($"The destination, '{this.Destination}', already exists. Do you want to overwrite it?");
                    ShouldContinue($"The destination, '{this.Destination}', already exists. Do you want to overwrite it?", "caption");
                }

                using (XmlTextReader reader = new XmlTextReader(this.Source))
                {
                    reader.DtdProcessing = DtdProcessing.Ignore;

                    document.PreserveWhitespace = true;
                    document.Load(reader);
                }

                foreach (var t in this.Transform)
                {
                    using (var transformation = new XmlTransformation(t))
                    {
                        WriteVerbose($"Applicating '{t}' transform...");
                        
                        var success = transformation.Apply(document);
                        
                        if (success)
                        {
                            document.Save(this.Destination);
                        }
                    }
                }
            }
        }
    }
}
