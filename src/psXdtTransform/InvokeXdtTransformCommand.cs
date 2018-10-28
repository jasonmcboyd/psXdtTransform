using Microsoft.Web.XmlTransform;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml;

namespace psXdtTransform
{
    [Cmdlet(verbName: "Invoke", nounName: "XdtTransform")]
    public class InvokeXdtTransformCommand : PSCmdlet, IDisposable
    {
        #region Command Parameters

        [Parameter(Position = 0)]
        public string SourcePath { get; set; }
        
        [Parameter(Position = 1, ValueFromPipeline = true)]
        public string[] TransformPath { get; set; }

        [Parameter(Position = 2)]
        public string Destination { get; set; }

        [Parameter(Position = 3)]
        public SwitchParameter Overwrite { get; set; }
        
        #endregion

        private XmlTransformableDocument _Document;

        protected override void BeginProcessing()
        {
            // Validate parameters.
            if (this.SourcePath == null)
            {
                throw new ArgumentNullException(nameof(this.SourcePath));
            }

            if (this.Destination == null)
            {
                throw new ArgumentNullException(nameof(this.Destination));
            }

            if (this.TransformPath == null)
            {
                throw new ArgumentNullException(nameof(this.TransformPath));
            }

            // Normalize paths.
            this.SourcePath = this.GetUnresolvedProviderPathFromPSPath(this.SourcePath);
            this.Destination = this.GetUnresolvedProviderPathFromPSPath(this.Destination);

            if (!File.Exists(this.SourcePath))
            {
                throw new FileNotFoundException($"The source file, '{this.SourcePath}', could not be found.");
            }
            
            var destinationExists = File.Exists(this.Destination);
            if (destinationExists 
                && !this.Overwrite 
                && !this.ShouldContinue($"The destination, '{this.Destination}', already exists. Do you want to overwrite it?", "Destination exists."))
            {
                throw new OperationCanceledException("User canceled the command.");
            }
            
            this._Document = new XmlTransformableDocument();

            using (XmlTextReader reader = new XmlTextReader(this.SourcePath))
            {
                reader.DtdProcessing = DtdProcessing.Ignore;

                this._Document.PreserveWhitespace = true;
                this._Document.Load(reader);
            }
        }

        protected override void ProcessRecord()
        {
            this.TransformPath = this.TransformPath.Select(this.GetUnresolvedProviderPathFromPSPath).ToArray();

            foreach (var t in this.TransformPath)
            {
                if (!File.Exists(t))
                {
                    throw new FileNotFoundException($"A transform, '{t}', could not be found.");
                }
            }

            foreach (var t in this.TransformPath)
            {
                using (var transformation = new XmlTransformation(t))
                {
                    this.WriteVerbose($"Applying '{t}' transform...");

                    if (!transformation.Apply(this._Document))
                    {
                        throw new Microsoft.Web.XmlTransform.XmlTransformationException($"Failed to apply transform '{t}'.");
                    }
                }
            }
        }

        protected override void EndProcessing()
        {
            this._Document.Save(this.Destination);
        }

        #region IDisposable Implementation

        private bool _Disposed;

        ~InvokeXdtTransformCommand()
        {
            this.Dispose(false);
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._Disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this._Document?.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                
                // Note disposing has been done.
                this._Disposed = true;
            }
        }

        #endregion
    }
}
